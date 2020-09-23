Shader "Unlit/DownsampleBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Input Width" , int) = 0
        _Height ("Input Height" , int) = 0
        sigma ("Sigma", float ) = 1.0
        blurSize ("Blur Size", float) = 1.0
        blurSamples("Blur samples" , float) = 1.0
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


            // The following are all mutually exclusive macros for various 
            // seperable blurs of varying kernel size
            // #if VERTICAL_BLUR_9 //this didnt work
            // #define numBlurPixelsPerSide 4.0
            // #define  blurMultiplyVecX 0.0
            // #define  blurMultiplyVecY 1.0
            // #elif HORIZONTAL_BLUR_9
            // #define numBlurPixelsPerSide 4.0
            // #define  blurMultiplyVecX 1.0
            // #define  blurMultiplyVecY 0.0            
            // #elif VERTICAL_BLUR_7
            // #define numBlurPixelsPerSide 3.0
            // #define  blurMultiplyVecX 0.0
            // #define  blurMultiplyVecY 1.0            
            // #elif HORIZONTAL_BLUR_7
            // #define numBlurPixelsPerSide 3.0
            // #define  blurMultiplyVecX 1.0
            // #define  blurMultiplyVecY 0.0           
            // #elif VERTICAL_BLUR_5
            // #define numBlurPixelsPerSide 2.0
            // #define  blurMultiplyVecX 0.0
            // #define  blurMultiplyVecY 1.0            
            // #elif HORIZONTAL_BLUR_5
            // #define numBlurPixelsPerSide 2.0
            // #define  blurMultiplyVecX 1.0
            // #define  blurMultiplyVecY 0.0            
            // #else
            // // This only exists to get this shader to compile when no macros are defined
            // #define numBlurPixelsPerSide 5.0
            // #define  blurMultiplyVecX 1.0
            // #define  blurMultiplyVecY 0.0
            // #endif

            #define pi 3.14159265 // this is legal
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
            int _Width;
            int _Height;
            float sigma;
            float blurSize;
            float blurSamples;
            //const float pi =  3.14159265 ;// apparently this is illegal! Pi was not set in all fragments.


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin( dot(co ,float2(12.9898,78.233) )) * 43758.5453);
            }

            fixed4 frag (v2f input) : SV_Target
            {
                float2 pixSize = 1.0/float2(_Width,_Height);
                float angle = rand(input.uv*_Time.x)*pi/blurSamples; //random number to at most have us go around the full circle twice, on average once. 
                float2 blurMultiplyVec = float2 (cos(angle),sin(angle));
                float2 blurMultiplyVec2 = float2 (cos(angle + pi/2), sin(angle + pi/2));
                
                // Incremental Gaussian Coefficent Calculation (See GPU Gems 3 pp. 877 - 889)
                float3 incrementalGaussian;
                incrementalGaussian.x = 1.0 / (sqrt(2.0 * pi) * sigma);
                incrementalGaussian.y = exp(-0.5 / (sigma * sigma));
                incrementalGaussian.z = incrementalGaussian.y * incrementalGaussian.y;
                float4 avgValue = float4(0.0, 0.0, 0.0, 0.0);
                float coefficientSum = 0.0;

                // Take the central sample first...
                avgValue += tex2D(_MainTex, input.uv) * incrementalGaussian.x;
                coefficientSum += incrementalGaussian.x;
                incrementalGaussian.xy *= incrementalGaussian.yz;

                // Go through the remaining 8 vertical samples (4 on each side of the center)
                for (float i = 1.0; i <= (blurSamples-1)/4; i++) 
                { 
                    avgValue += tex2D(_MainTex, input.uv - pixSize * i * blurSize * 
                                        blurMultiplyVec) * incrementalGaussian.x;
                    avgValue += tex2D(_MainTex, input.uv + pixSize * i * blurSize * 
                                        blurMultiplyVec) * incrementalGaussian.x;
                    avgValue += tex2D(_MainTex, input.uv + pixSize * i * blurSize * 
                                        blurMultiplyVec2) * incrementalGaussian.x; 
                    avgValue += tex2D(_MainTex, input.uv - pixSize * i * blurSize * 
                                        blurMultiplyVec2) * incrementalGaussian.x;          
                    blurMultiplyVec = float2(cos(i*angle),sin(i*angle));
                    blurMultiplyVec = float2(cos(i*angle + i*pi/2),sin(i*angle + i*pi/2));
                    coefficientSum += 4.0*incrementalGaussian.x;
                    incrementalGaussian.xy *= incrementalGaussian.yz;
                }

                return  avgValue / coefficientSum;

            }
            ENDCG
        }
    }
}





//ORIGINAL BOX BLUR
// float4 col =tex2D(_MainTex, i.uv + float2(pixSize.x,0) ) +
                //             tex2D(_MainTex, i.uv + float2(-pixSize.x,0) ) +
                //             tex2D(_MainTex, i.uv + float2(0,pixSize.y) ) +
                //             tex2D(_MainTex, i.uv + float2(0,-pixSize.y) );
                // //fixed4 col = tex2D(_MainTex, i.uv + float2(pixSize.x,0) );
                // return col /4.0;


//ORIGINAL LINEAR GAUSSIAN BLUR
                // float2 pixSize = 1.0/float2(_Width,_Height);
                
                // float2 blurMultiplyVec = float2 (blurX,blurY);
                
                // // Incremental Gaussian Coefficent Calculation (See GPU Gems 3 pp. 877 - 889)
                // float3 incrementalGaussian;
                // incrementalGaussian.x = 1.0 / (sqrt(2.0 * pi) * sigma);
                // incrementalGaussian.y = exp(-0.5 / (sigma * sigma));
                // incrementalGaussian.z = incrementalGaussian.y * incrementalGaussian.y;
                // float4 avgValue = float4(0.0, 0.0, 0.0, 0.0);
                // float coefficientSum = 0.0;

                // // Take the central sample first...
                // avgValue += tex2D(_MainTex, input.uv) * incrementalGaussian.x;
                // coefficientSum += incrementalGaussian.x;
                // incrementalGaussian.xy *= incrementalGaussian.yz;

                // // Go through the remaining 8 vertical samples (4 on each side of the center)
                // for (float i = 1.0; i <= (blurSamples-1)/2; i++) 
                // { 
                //     avgValue += tex2D(_MainTex, input.uv - pixSize * i * blurSize * 
                //                         blurMultiplyVec) * incrementalGaussian.x;         
                //     avgValue += tex2D(_MainTex, input.uv + pixSize * i * blurSize * 
                //                         blurMultiplyVec) * incrementalGaussian.x;         
                //     coefficientSum += 2.0 * incrementalGaussian.x;
                //     incrementalGaussian.xy *= incrementalGaussian.yz;
                // }

                // return  avgValue / coefficientSum;