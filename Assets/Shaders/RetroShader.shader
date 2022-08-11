Shader "Unlit/RetroShader"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _GeoRes("Geometric Resolution", Float) = 40
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass 
            {
                CGPROGRAM
                
                #include "UnityCG.cginc"

                #pragma vertex vert
                #pragma fragment frag

                CBUFFER_START(UnityPerMaterial)
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float _GeoRes;
                CBUFFER_END

                struct VertexInput
                {
                    float4 position : SV_POSITION;
                    float3 texcoord : TEXCOORD;
                };

                struct VertexOutput
                {
                    float4 position : SV_POSITION;
                    float3 texcoord : TEXCOORD;
                };

                VertexInput vert(appdata_base v) 
                {
                    VertexOutput o;

                    float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
                    wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

                    float4 sp = mul(UNITY_MATRIX_P, wp);
                    o.position = sp;

                    float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.texcoord = float3(uv * sp.w, sp.w);

                    return o;
                }

                fixed4 frag(VertexOutput i) : SV_Target
                {
                    float2 uv = i.texcoord.xy / i.texcoord.z;

                    return tex2D(_MainTex, uv) * _Color * 2;
                }

                ENDCG
            }

        /*  
        Pass
        {
            CGPROGRAM
    
            #include "UnityCG.cginc"
    
            #pragma vertex vert
            #pragma fragment frag
    
            struct v2f
            {
                float4 position : SV_POSITION;
                float3 texcoord : TEXCOORD;
            };
    
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GeoRes;
    
            v2f vert(appdata_base v)
            {
                v2f o;
    
                float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
                wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;
    
                float4 sp = mul(UNITY_MATRIX_P, wp);
                o.position = sp;
    
                float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord = float3(uv * sp.w, sp.w);
    
                return o;
            }
    
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord.xy / i.texcoord.z;
                return tex2D(_MainTex, uv) * _Color * 2;
            }
    
            ENDCG
        }
        */
    }
}
