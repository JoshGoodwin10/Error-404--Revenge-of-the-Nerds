Shader "Custom/RayTracing"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			static const float PI = 3.1415;

			// Raytracing Settings
			int MaxBounceCount;
			int NumRaysPerPixel;
			int Frame;

			// Camera Settings
			float3 ViewParams;
			float4x4 CamLocalToWorldMatrix;

			// Environment Settings
			int EnvironmentEnabled;
			float4 GroundColour;
			float4 SkyColourHorizon;
			float4 SkyColourZenith;
			float SunFocus;
			float SunIntensity;

			// structures
			struct Ray
			{
				float3 origin;
				float3 dir;
			};
			
			struct RayTracingMaterial
			{
				float4 colour;
				float4 emissionColour;
				float4 specularColour;
				float emissionStrength;
				float smoothness;
				float specularProbability;
			};

			struct Sphere
			{
				float3 position;
				float radius;
				RayTracingMaterial material;
			};

			struct Triangle
			{
				float3 posA, posB, posC;
				float3 normalA, normalB, normalC;
			};

			struct MeshInfo
			{
				uint firstTriangleIndex;
				uint numTriangles;
				RayTracingMaterial material;
				float3 boundsMin;
				float3 boundsMax;
			};

			struct HitInfo
			{
				bool didHit;
				float dst;
				float3 hitPoint;
				float3 normal;
				RayTracingMaterial material;
			};

			// buffers
			StructuredBuffer<Sphere> Spheres;
			int NumSpheres;

			StructuredBuffer<Triangle> Triangles;
			StructuredBuffer<MeshInfo> AllMeshInfo;
			int NumMeshes;

			// ray intersections
		
			// ray with a sphere
			HitInfo RaySphere(Ray ray, float3 sphereCentre, float sphereRadius)
			{
				HitInfo hitInfo = (HitInfo)0;
				float3 offsetRayOrigin = ray.origin - sphereCentre;
				float a = dot(ray.dir, ray.dir); 
				float b = 2 * dot(offsetRayOrigin, ray.dir);
				float c = dot(offsetRayOrigin, offsetRayOrigin) - sphereRadius * sphereRadius;
				float discriminant = b * b - 4 * a * c;

				if (discriminant >= 0) 
				{
					float dst = (-b - sqrt(discriminant)) / (2 * a);

					if (dst >= 0) 
					{
						hitInfo.didHit = true;
						hitInfo.dst = dst;
						hitInfo.hitPoint = ray.origin + ray.dir * dst;
						hitInfo.normal = normalize(hitInfo.hitPoint - sphereCentre);
					}
				}
				return hitInfo;
			}

			// ray with a triangle
			HitInfo RayTriangle(Ray ray, Triangle tri)
			{
				float3 edgeAB = tri.posB - tri.posA;
				float3 edgeAC = tri.posC - tri.posA;
				float3 normalVector = cross(edgeAB, edgeAC);
				float3 ao = ray.origin - tri.posA;
				float3 dao = cross(ao, ray.dir);

				float determinant = -dot(ray.dir, normalVector);
				float invDet = 1 / determinant;
				
				float dst = dot(ao, normalVector) * invDet;
				float u = dot(edgeAC, dao) * invDet;
				float v = -dot(edgeAB, dao) * invDet;
				float w = 1 - u - v;

				HitInfo hitInfo;
				hitInfo.didHit = determinant >= 1E-6 && dst >= 0 && u >= 0 && v >= 0;
				hitInfo.hitPoint = ray.origin + ray.dir * dst;
				hitInfo.normal = normalize(tri.normalA * w + tri.normalB * u + tri.normalC * v);
				hitInfo.dst = dst;
				return hitInfo;
			}

			bool RayBoundingBox(Ray ray, float3 boxMin, float3 boxMax)
			{
				float3 invDir = 1 / ray.dir;
				float3 tMin = (boxMin - ray.origin) * invDir;
				float3 tMax = (boxMax - ray.origin) * invDir;
				float3 t1 = min(tMin, tMax);
				float3 t2 = max(tMin, tMax);
				float tNear = max(max(t1.x, t1.y), t1.z);
				float tFar = min(min(t2.x, t2.y), t2.z);
				return tNear <= tFar;
			}

			// random no. generation
			uint NextRandom(inout uint state)
			{
				state = state * 747796405 + 2891336453;
				uint result = ((state >> ((state >> 28) + 4)) ^ state) * 277803737;
				result = (result >> 22) ^ result;
				return result;
			}

			float RandomValue(inout uint state)
			{
				return NextRandom(state) / 4294967295.0;
			}

			// background light
			float3 GetEnvironmentLight(Ray ray)
			{
				if (!EnvironmentEnabled) 
				{
					return 0;
				}
				
				float skyGradientT = pow(smoothstep(0, 0.4, ray.dir.y), 0.35);
				float groundToSkyT = smoothstep(-0.01, 0, ray.dir.y);
				float3 skyGradient = lerp(SkyColourHorizon, SkyColourZenith, skyGradientT);
				float sun = pow(max(0, dot(ray.dir, _WorldSpaceLightPos0.xyz)), SunFocus) * SunIntensity;
				float3 composite = lerp(GroundColour, skyGradient, groundToSkyT) + sun * (groundToSkyT>=1);
				return composite;
			}

			// the fun stuff
			HitInfo CalculateRayCollision(Ray ray)
			{
				HitInfo closestHit = (HitInfo)0;
				closestHit.dst = 1e10;

				for (int i=0; i<NumSpheres; i++)
				{
					Sphere sphere = Spheres[i];
					HitInfo newHit = RaySphere(ray, sphere.position, sphere.radius);

					if (newHit.didHit && newHit.dst < closestHit.dst)
					{
						closestHit = newHit;
						closestHit.material = sphere.material;
					}
				}

				for (int i=0; i<NumMeshes; i++)
				{
					MeshInfo mesh = AllMeshInfo[i];

					if (!RayBoundingBox(ray, mesh.boundsMin, mesh.boundsMax))
					{
						continue;
					}

					for (int j=mesh.firstTriangleIndex; j<mesh.firstTriangleIndex+mesh.numTriangles; j++)
					{
						HitInfo newHit = RayTriangle(ray, Triangles[j]);
						if (newHit.didHit && newHit.dst < closestHit.dst)
						{
							closestHit = newHit;
							closestHit.material = mesh.material;
						}
					}
				}

				return closestHit;
			}

			// Get a ray that originates at the camera and goes through the given uv coordinate
			Ray GetCameraRay(float2 uv)
			{
				float2 ndc = 2 * uv - 1;
				float3 rayDir = normalize(mul(CamLocalToWorldMatrix, float4(ndc.x * ViewParams.x, ndc.y * ViewParams.y, 1, 0)).xyz);

				return Ray(_WorldSpaceCameraPos, rayDir);
			}
			
			// Compute the color at the given UV by tracing rays
			float4 frag(v2f i) : SV_Target
			{
				Ray ray = GetCameraRay(i.uv);
				HitInfo hit = CalculateRayCollision(ray);
				return hit.material.colour;
			}

			ENDCG
		}
	}
}
