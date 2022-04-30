Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _NoiseTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Primary Scroll Speed", Vector) = (0, 0, 0, 0)
        _ScrollSpeed2 ("Secondary Scroll Speed", Vector) = (0, 0, 0, 0)
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

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float4 _ScrollSpeed;
            float4 _ScrollSpeed2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed t = _Time.x;
                float2 scroll = _ScrollSpeed.xy * t;
                fixed4 mainCol = tex2D(_NoiseTex, i.uv + scroll.xy);
                scroll = _ScrollSpeed2.xy * t;
                fixed4 mask = tex2D(_NoiseTex, i.uv + scroll);

                fixed4 col = mainCol * mask;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
