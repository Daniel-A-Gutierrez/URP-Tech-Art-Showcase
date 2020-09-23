Shader "Unlit/EdgeHighlight"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color("Main Color", Color) = (1,1,1,1)
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

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                //float4 col = float4(i.uv, 0.0,1.0);
                //float4 col = _Color* step(  min(i.uv.x,i.uv.y) , .1);
                //if x or y is below .1 or above .9 , set true. 
                //what if its just x ? 
                // step ( .9, x) or step(x,.1) 
                //alt abs(x-.5) in the range of .4 to .5 which translates to 
                // abs(x-.5) -.4 >= .1
                //and to combine the terms with an max them
                float4 col = _Color * max(step(.03, abs(i.uv.x - .5) - .4), step(.03, abs(i.uv.y - .5) - .4));
                return col;
            }
            ENDCG
        }
    }
}
