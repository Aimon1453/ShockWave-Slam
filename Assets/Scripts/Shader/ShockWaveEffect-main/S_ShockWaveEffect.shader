Shader "Custom/Shockwave"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _CaptureTex ("Capture Texture", 2D) = "white" {}
        _CullingTex ("Culling Texture", 2D) = "white" {}
        _UpdateTime ("Update Time", Range(0, 1)) = 0
        _Width ("Ring Width", Range(0.001, 1)) = 0.1
        _DistortionStrength ("Distortion Strength", Range(0,1)) = 0.05
        _LightColor ("Shockwave Light Color", Color) = (1,1,1,1)
        _Smooth ("Smooth Ring",float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            sampler2D _CaptureTex;
            sampler2D _CullingTex;
            float _UpdateTime;
            float _Smooth;
            float _Width;
            float _DistortionStrength;
            float4 _LightColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv      : TEXCOORD0;
                float4 vertex  : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // ��ú�Sprite���ĵľ���
                float2 centerUV = (uv - 0.5) * 2.0;
                float radius = length(centerUV);

                // Բ����Ȧ�뾶����Ȧ�뾶
                float outer = _UpdateTime;
                float inner = max(0.0, outer - _Width*(1-_UpdateTime));
                // ��ʵ_UpdateTime������Ȧ�뾶����Ϊ�ű����ǿ������������������Ч���ȣ���������Ϊ�����
                // *��1 - _UpdateTime)��Ϊ������Ч�ӽ�����ʱԲ���Ŀ����𽥱�Ϊ0�������Ƕ�������ֱ����ʧ���Ƚ�ͻأ��


                // Բ������Ȧ������λ��
                float halfWidth = (outer - inner) * 0.5;
                float ringCenter = (outer + inner) * 0.5;

                // ��һ���뾶�[-1, 1] ����
                // �����Բ������Ȧ���ĵľ������ƽ������
                float norm = abs(radius - ringCenter) / halfWidth;

                // ����Ȩ�أ��� norm=0 ʱΪ1��norm=1ʱΪ0���м�ƽ������
                // ���ߣ�ƽ�����ɣ�
                float soft = smoothstep(1.0, 0.0, norm);

                // Ӳ�ߣ�ȫ 1 �� 0��
                float hard = step(norm, 1.0);

                // ����_Smooth���
                float ringWeight = lerp(hard, soft, _Smooth);

                // ��������Ť����UVƫ�Ʒ���
                float2 dir = centerUV == 0 ? float2(0, 0) : normalize(centerUV);

                // ����ǿ�ȵȲ�������uvƫ����
                float2 offsetUV = uv + dir * (ringWeight * _DistortionStrength);

                // ��ƫ�ƺ��uv����
                fixed4 captureCol = tex2D(_CaptureTex, offsetUV);

                // ��������ҪӦ�ó�����Ĳ��ͼ��
                fixed4 cullCol = tex2D(_CullingTex, uv);
                float cullAlpha = cullCol.a;
                // �����culling���ͼ������ʾ����ͼ��û������Ť�����ͼ��
                fixed4 baseCol = lerp(captureCol, cullCol, cullAlpha);

                // ���ӹ�����ɫ
                fixed4 lightColor = _LightColor * (_LightColor.a * _DistortionStrength * ringWeight);
                fixed4 finalCol = baseCol + lightColor;

                // ʹ�ó����Բ�����������Ĳ�͸����Ϊ0
                // ��������Ļ�����������ᱻSprite�赲
                finalCol.a *= ringWeight ;
                return finalCol;
            }
            ENDCG
        }
    }
}
