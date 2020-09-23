Shader "Unlit/Residual"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Output ("The output", 2D) = "white" {}
        _Threshhold ("Threshhold"  , float) = 1.0
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
            sampler2D _Output;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                return max(0.0, tex2D(_MainTex, i.uv) - 1.0); //dont get caught up rn, but later try to take the original color just if its bright enough.
            }
            ENDCG
        }
    }
}
