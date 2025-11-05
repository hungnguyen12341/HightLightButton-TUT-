Shader "UI/StencilRoundedWriter"
{
    Properties
    {
        _MainTex ("Texture (RGBA)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.01
        _Feather ("Feather (softness)", Range(0,0.5)) = 0.01
        _ShowTexture("Show Texture In Hole (0/1)", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

  
        Pass
        {
            Name "STENCIL_WRITE"
            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appv { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;
            float _Feather;

            v2f vert(appv v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Smooth ko hiệu quả lắm đâu nhưng vẫn tạm
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float alpha = tex.a;
                float t = smoothstep(_Cutoff - _Feather, _Cutoff + _Feather, alpha);
                if (t <= 0.001) discard;
        
                return 0;
            }
            ENDCG


            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
                Fail Keep
                ZFail Keep
            }
        }

        // PASS 2: chỉ draw texture khi stencil = 1
        Pass
        {
            Name "DRAW_IN_HOLE"
          
            Stencil
            {
                Ref 1
                Comp Equal
                Pass Keep
                Fail Keep
                ZFail Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag2
            #include "UnityCG.cginc"

            struct appv { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _ShowTexture;

            v2f vert(appv v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag2(v2f i) : SV_Target
            {
                if (_ShowTexture < 0.5)
                {
                    // vẽ solid color cho hole (_Color)
                    return _Color;
                }
                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                // nhớ phải giữ alpha
                return tex;
            }
            ENDCG
        }
    }
}
