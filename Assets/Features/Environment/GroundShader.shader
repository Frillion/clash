Shader "Unlit/VerticalCut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _threshold ("Threshold", Float) = 0.0
        _dirtColor ("Dirt Color", Color) = (1,1,1,1)
        _grassColor ("Grass Color", Color) = (1,1,1,1)
        _grassThreshold ("Grass Threshold", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _threshold;
            float4 _dirtColor;
            float4 _grassColor;
            float _grassThreshold;
            
            float remap_uv(float value, float min, float max)
            {
                return min + value * (max - min);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float remapped_uv = remap_uv(i.uv.y,1,0);
                float4 grass = _grassColor * step(_grassThreshold,i.uv.y);
                float4 dirt = _dirtColor * step(1 - _grassThreshold,remapped_uv);
                col.a = step(_threshold, remapped_uv);
                return col * grass + col * dirt;
            }
            ENDCG
        }
    }
}
