Shader "Custom/ProximityFadeURP"
{
    Properties
    {
        _BaseColor("Kolor modelu", Color) = (1, 1, 1, 1)
        _BaseMap("Tekstura (Opcjonalna)", 2D) = "white" {}
        _MinFadeDistance("Pełna niewidoczność (m)", Float) = 2.0
        _MaxFadeDistance("Pełna widoczność (m)", Float) = 5.0
    }

    SubShader
    {
        // Tagujemy jako przezroczysty obiekt w potoku URP
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            // Konfiguracja przezroczystości
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Import bibliotek URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 positionWS   : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // Zmienne powiązane z panelem Inspector
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _MinFadeDistance;
                float _MaxFadeDistance;
                float _GlobalFadeRadius; // To jest nasza zmienna z suwaka C#
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Przekształcamy pozycję lokalną na pozycję w świecie (do obliczenia odległości)
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Jeśli nie masz tekstury, _BaseMap zwróci po prostu biel i pomnoży przez Twój kolor z Inspectora
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                
                // 1. Dystans między danym punktem mapy a kamerą (Twoim telefonem)
                float dist = distance(IN.positionWS, GetCameraPositionWS());
                
                // 2. Dodajemy ewentualną modyfikację z naszego suwaka AR Debuggera
                float finalMin = _MinFadeDistance + _GlobalFadeRadius;
                float finalMax = _MaxFadeDistance + _GlobalFadeRadius;
                
                // 3. Wygaszanie przezroczystości
                float fadeAlpha = smoothstep(finalMin, finalMax, dist);
                
                color.a *= fadeAlpha;
                return color;
            }
            ENDHLSL
        }
    }
}