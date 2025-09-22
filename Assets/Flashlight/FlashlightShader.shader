Shader "Custom/lightShadedr"
{
    Properties
    {
        _Color ("Beam Color", Color) = (1,1,1,0.5)
        _Falloff ("Falloff", Float) = 1.5
        _BeamLength ("Beam Length", Float) = 20.0
        _BeamAngle ("Beam Half-Angle", Float) = 30.0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Float) = 0.2
        _NoiseSpeed ("Noise Speed", Float) = 1.0
        _EdgeSoftness ("Edge Softness", Range(0.01,1)) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend One One
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            float4 _Color;
            float _Falloff;
            float _BeamLength;
            float _BeamAngle; // in degrees
            sampler2D _NoiseTex;
            float _NoiseStrength;
            float _NoiseSpeed;
            float _EdgeSoftness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // --- Distance fade (length fade) ---
                float3 origin = float3(0,0,0); // assumes cone pivot at origin
                float dist = length(i.worldPos - origin);
                float lengthFade = saturate(1.0 - dist / _BeamLength);

                // --- Radial fade with soft base ---
                float3 localPos = mul(unity_WorldToObject, float4(i.worldPos,1)).xyz;
                float coneRadius = tan(radians(_BeamAngle));
                float radial = length(localPos.xy) / (localPos.z + 0.0001);

                // Fade along Z (soft at base, full tip)
                float fadeAlongZ = saturate(localPos.z / _BeamLength);

                // Smooth radial fade multiplied by fadeAlongZ
                float edgeFade = smoothstep(1.0, 1.0 - _EdgeSoftness, 1.0 - radial / coneRadius) * fadeAlongZ;

                // --- Noise modulation ---
                float2 uv = i.worldPos.xz * 0.1 + _Time.y * _NoiseSpeed;
                float noise = tex2D(_NoiseTex, uv).r;
                float noiseMod = lerp(1.0, noise, _NoiseStrength);

                // --- Combine fades ---
                float atten = lengthFade * edgeFade * noiseMod;
                fixed4 col = _Color * atten;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
