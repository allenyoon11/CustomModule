Shader "Custom/CropShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rect ("Crop Rect", Vector) = (0,0,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Rect; // x, y, width, height

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // UV 변환
                o.uv.x = lerp(_Rect.x, _Rect.x + _Rect.z, v.uv.x);
                o.uv.y = lerp(_Rect.y, _Rect.y + _Rect.w, v.uv.y);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
