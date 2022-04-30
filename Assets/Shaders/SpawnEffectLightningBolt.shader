Shader "Custom/SpawnEffectLightningBolt"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}

        _Completeness ("Completeness", Range(0, 1)) = 0.5

        _SwirlSpeed ("Swirl Speed", Float) = 1
        _SwirlSpeedRamp ("Swirl Speed Ramp", Range(-1, 1)) = 0
        _Spin ("Spin Direction (-1 left, 1 right)", Float) = 0
        _WaveSpeed ("Wave Speed", Float) = 1
        _WaveFrequency ("Wave Frequency", Float) = 1
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 1
        _WaveStartOffset ("Wave Starting Offset", Range(0, 1)) = 0
        _Dip ("Dip", Range(-1, 1)) = 0

        [ShowAsVector2] _NoiseScrollSpeed ("Noise Scroll Speed", Vector) = (0.5, 0.5, 0, 0)
        _NoiseBandPosition ("Noise Band Position", Float) = 0.4
        _NoiseBandSize ("Noise Band Size", Float) = 0.2
        _NoiseDensity ("Noise Density", Float) = 0

        _Resolution ("Resolution", int) = 64
        _Width ("Width", Float) = 32
        _PrimaryColor ("Primary Color", Color) = (0, 0, 0, 0)
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

            int _Resolution;
            int _Width;
            float4 _PrimaryColor;

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

                // Apply texture, color, and all masks
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * _PrimaryColor;
                col.a *= lineMask;
                //col.a = noiseBandMask;
                //fixed4 col = tex2D(_MainTex, i.uv) * i.color;
 
                return col;
            }
            ENDCG
        }
    }
}
