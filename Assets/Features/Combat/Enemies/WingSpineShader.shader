Shader "Unlit/WingSpineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _radius ("Radius", Float) = 1.0
        _threshold("Threshold", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #define MAX_NODES 100

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 world_pos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _neighbors[MAX_NODES];
            int _numberOfNodes;
            float _radius;
            float _threshold;
            
            float metaballdensity(float2 uv)
            {
                float sum = 0;
                for (int i = 0; i < _numberOfNodes; ++i)
                {
                    float dist = length(uv - _neighbors[i].xy);
                    sum += _radius/max(dist,0.001);
                }
                return sum;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.world_pos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (metaballdensity(i.world_pos) < _threshold){discard;}
                return float4(0.0,0.0,0.0,1.0);
            }
            ENDCG
        }
    }
}
