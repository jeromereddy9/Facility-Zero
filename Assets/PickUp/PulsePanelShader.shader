Shader "Custom/PulsingPanel"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0.6, 1.0, 0.6, 1.0)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}

        _VignetteStrength("Vignette Strength", Range(0, 2)) = 1.0
        _PulseSpeed("Pulse Speed", Range(0, 5)) = 1.0
        _PulseAmount("Pulse Amount", Range(0, 1)) = 0.2
        _FlickerStrength("Flicker Strength", Range(0, 0.05)) = 0.02
        _FlickerSpeed("Flicker Speed", Range(0, 5)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _VignetteStrength;
                float _PulseSpeed;
                float _PulseAmount;
                float _FlickerStrength;
                float _FlickerSpeed;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // base color + texture 
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                //  vignette to darken edges 
                float2 centered = IN.uv - 0.5;
                float vignette = 1.0 - dot(centered, centered) * 2.0 * _VignetteStrength;
                color.rgb *= saturate(vignette);

                //  pulsating effect 
                float pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5) * _PulseAmount + 1.0;
                color.rgb *= pulse;

                //  scan line 
                float flicker = sin(IN.uv.y * 30.0 + _Time.y * _FlickerSpeed) * _FlickerStrength;
                color.rgb += flicker;

                return color;
            }

            ENDHLSL
        }
    }
}
