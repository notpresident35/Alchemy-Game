Shader "Custom/SpawnEffectLightningBolt"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}

        // -1 = Invisible
        // 0 = Solid Color
        // 1 = Solid White
        _RenderFlash("Render Flash", Int) = 1

        _Resolution ("Resolution", Int) = 64
        _Width ("Width", Float) = 32
        _PrimaryColor ("Primary Color", Color) = (0, 0, 0, 0)
        _FlashColor ("Flash Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
        }
        LOD 100

        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha

            #include "UnityCG.cginc"
            #include "Interpolation.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            uint _Resolution;
            float _Width;
            uint _RenderFlash;
            float4 _PrimaryColor;
            float4 _FlashColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Crush position to desired resolution (aka pixellate object)
                float2 crunchedPos = floor((i.uv + 1 / _Resolution) * _Resolution) / _Resolution;

                // ** Line Mask **
                // Distance from the middle of the line to the current pixel
                fixed dist = length(crunchedPos.y - 0.5) * 2;
                // Creates a hard line based on width
                fixed lineMask = saturate(_Width - dist * _Resolution);       

                // Apply texture, color (including flashing), and all masks
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * lerp(_PrimaryColor, _FlashColor, _RenderFlash);
                col.a *= lineMask * (_RenderFlash + 1);
                //col.a = noiseBandMask;
                //fixed4 col = tex2D(_MainTex, i.uv) * i.color;
 
                return col;
            }
            ENDCG
        }
    }
}
