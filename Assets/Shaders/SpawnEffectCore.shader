Shader "Custom/SpawnEffectCore"
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
        _Radius ("Radius", Float) = 0.5
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

            fixed _Completeness;

            fixed _SwirlSpeed;
            fixed _SwirlSpeedRamp;
            fixed _Spin;
            fixed _WaveSpeed;
            fixed _WaveFrequency;
            fixed _WaveAmplitude;
            fixed _WaveStartOffset;
            fixed _Dip;

            fixed4 _NoiseScrollSpeed;
            fixed _NoiseBandPosition;
            fixed _NoiseBandSize;
            fixed _NoiseDensity;

            int _Resolution;
            int _Radius;
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

                // ** Circle Mask **
                // Distance from the center of the texture to the current pixel
                fixed dist = length(crunchedPos - 0.5) * 2;
                // Neat
                //fixed dist = floor(length(crunchedPos - 0.5) * _Resolution) / _Resolution;
                // Creates a hard circle based on radius
                fixed circleMask = saturate(_Radius - dist * _Resolution);

                // ** Upper Wave Mask **
                fixed t = _Time.x;
                fixed pi = 3.14159;
                // Add a little dip/bump to the middle, and make waves swirl faster or slower in this dip
                fixed dip = _Dip * sin(crunchedPos.x * pi) * sin(_Completeness * pi); // Ranges from sin(0) to sin(pi) along x axis
                fixed speedRamp = _SwirlSpeedRamp * sin((crunchedPos.x + 0.5) * pi); // Ranges from sin(pi/2) to sin(3pi/2) along x axis
                // Get the position of the wave
                float waveSamplePosition = _WaveFrequency * (crunchedPos.x + _WaveStartOffset) + t * _WaveSpeed * _Spin;
                // Wave movement (aka "swirl")
                fixed waveSample = _Spin * _SwirlSpeed * t + speedRamp;
                // Generate waves
                fixed waveHeight = (sin(waveSample + waveSamplePosition) / 2) * _WaveAmplitude * sin(_Completeness * pi);
                // Make mask out of waves
                fixed waveMask = saturate(floor(crunchedPos.y + waveHeight + dip + _Completeness));

                // ** Gaps **
                // Sample scrolling noise texture
                float2 noiseScroll = _NoiseScrollSpeed.xy;
                noiseScroll.x *= _Spin;
                float noise = tex2D(_NoiseTex, crunchedPos + t * noiseScroll);
                // Generate a band just under the fill line for noise to populate
                float samplePos = crunchedPos.y + dip + _NoiseBandPosition - (1 - _Completeness);
                float topCompression = (1 - _Completeness * _Completeness * _Completeness);
                float band = round(sin(saturate(samplePos / (_NoiseBandSize * topCompression)) * pi));
                // Populate band with noise and convert to a mask
                float noiseSample = floor(noise + samplePos + _NoiseDensity);
                float noiseBandMask = 1 - saturate((1 - noiseSample) * band);
                

                // Apply texture, color, and all masks
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * _PrimaryColor;
                col.a *= circleMask * waveMask * noiseBandMask;
                //col.a = noiseBandMask;

                return col;
            }
            ENDCG
        }
    }
}
