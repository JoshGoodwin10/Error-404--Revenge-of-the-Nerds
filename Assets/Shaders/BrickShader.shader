Shader "Custom/BrickShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BrickSize ("Brick Size", float) = 0.1
        _BrickSpacing ("Brick Spacing", float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
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
                float4 sv_Position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _BrickSize;
            float _BrickSpacing;

            v2f vert (appdata v)
            {
                v2f o;
                o.sv_Position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate brick coordinates
                float brickX = floor(i.uv.x / (_BrickSize + _BrickSpacing));
                float brickY = floor(i.uv.y / (_BrickSize + _BrickSpacing));

                // Calculate brick offset within the brick
                float brickOffsetX = frac(i.uv.x / (_BrickSize + _BrickSpacing)) * _BrickSize;
                float brickOffsetY = frac(i.uv.y / (_BrickSize + _BrickSpacing)) * _BrickSize;

                // Simulate brick color using a simple pattern
                fixed4 brickColor = fixed4(0.568, 0.25, 0.21, 1); // Red brick color
                // if (brickOffsetX < _BrickSize * 0.2 || brickOffsetY < _BrickSize * 0.2)
                // {
                //         brickColor *= 0.8; // Darker color for edges
                // }

                return brickColor;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}