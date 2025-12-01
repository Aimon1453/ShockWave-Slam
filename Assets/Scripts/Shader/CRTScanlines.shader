Shader "Custom/CRTScanlines"
{
    Properties
    {
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
        _ScanlineCount ("Scanline Count", Float) = 800
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "CRTScanlines"
            
            // 使用 HLSLPROGRAM 替代 CGPROGRAM
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            // 引入 URP 核心库和 Blit 工具库
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // 这个库包含了标准的 Vert 函数，自动处理全屏三角形坐标
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _ScanlineIntensity;
            float _ScanlineCount;
            float _NoiseIntensity;

            // 注意：Blit.hlsl 会自动定义 _BlitTexture，不需要我们手动声明

            half4 Frag (Varyings input) : SV_Target
            {
                // 1. 使用 SAMPLE_TEXTURE2D 采样 _BlitTexture
                // input.texcoord 是由 Blit.hlsl 自动计算好的正确 UV
                float2 uv = input.texcoord;
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                // 2. 扫描线逻辑 (保持不变)
                float scanline = sin(uv.y * _ScanlineCount * 3.14159);
                float scanlineEffect = (scanline + 1.0) * 0.5; 
                col.rgb -= _ScanlineIntensity * (1.0 - scanlineEffect);

                // 3. 噪点逻辑 (保持不变)
                float noise = frac(sin(dot(uv + _Time.y, float2(12.9898, 78.233))) * 43758.5453);
                col.rgb += (noise - 0.5) * _NoiseIntensity;

                return col;
            }
            ENDHLSL
        }
    }
}