Shader "Custom/FlickeringEmission"
{
    Properties
    {
        _Color ("Color Base", Color) = (1,1,1,1)
        [HDR] _EmissionColor ("Color Emisión", Color) = (1,1,1,1)
        _MainTex ("Textura Base", 2D) = "white" {}
        _SlowSpeed ("Velocidad Lenta", Float) = 1.5
        _FastSpeed ("Velocidad Rápida", Float) = 25.0
        _Threshold ("Umbral de Apagado (0-1)", Range(0, 1)) = 0.4
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
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _EmissionColor;
            float _SlowSpeed;
            float _FastSpeed;
            float _Threshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // --- MATEMÁTICA DEL PATRÓN DE TITILEO ---
                float time = _Time.y;
                
                // Mezcla de onda senoidal lenta y rápida
                float slowWave = sin(time * _SlowSpeed);
                float fastWave = sin(time * _FastSpeed);
                
                // Combinación: la onda rápida modula el patrón y el step la vuelve binaria (0 o 1)
                float pattern = step(_Threshold, (slowWave + fastWave * 0.5) * 0.5 + 0.5);

                // Sumamos la emisión cuando el patrón está activo
                float3 emission = _EmissionColor.rgb * pattern;

                return fixed4(col.rgb + emission, col.a);
            }
            ENDCG
        }
    }
}