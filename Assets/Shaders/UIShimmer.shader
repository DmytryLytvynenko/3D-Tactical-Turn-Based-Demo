Shader "UI/Shimmer"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _ShimmerSpeed ("Shimmer Speed", Range(0.1, 5.0)) = 1.0
        _ShimmerWidth ("Shimmer Width", Range(0.1, 2.0)) = 0.3
        _ShimmerIntensity ("Shimmer Intensity", Range(0.0, 2.0)) = 1.0
        _ShimmerAngle ("Shimmer Angle", Range(-180, 180)) = 45
        _RandomSeed ("Random Seed", Range(0.0, 100.0)) = 0.0
        
        _WaveSpeed ("Wave Speed", Range(0.1, 3.0)) = 1.5
        _WaveAmplitude ("Wave Amplitude", Range(0.0, 1.0)) = 0.2
        _WaveFrequency ("Wave Frequency", Range(1.0, 10.0)) = 3.0
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        _ColorMask ("Color Mask", Float) = 15
        
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            Name "Default"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 vertex   : SV_POSITION;
                half4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float3 objectWorldPos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float4 _ClipRect;
                float _ShimmerSpeed;
                float _ShimmerWidth;
                float _ShimmerIntensity;
                float _ShimmerAngle;
                float _RandomSeed;
                float _WaveSpeed;
                float _WaveAmplitude;
                float _WaveFrequency;
            CBUFFER_END
            
            Varyings vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.worldPosition = v.vertex;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.objectWorldPos = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                
                return o;
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord) * i.color;
                
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif
                
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
                
                float2 uv = i.texcoord;
                float time = _Time.y;
                
                // Generate random values based on object position and seed
                float3 worldPos = i.objectWorldPos + _RandomSeed;
                float random1 = frac(sin(dot(worldPos.xy, float2(12.9898, 78.233))) * 43758.5453);
                float random2 = frac(sin(dot(worldPos.xz, float2(93.9898, 67.345))) * 28571.2341);
                float random3 = frac(sin(dot(worldPos.yz, float2(41.2356, 94.674))) * 35674.8901);
                
                // Randomize parameters for each card
                float randomSpeedMult = 0.5 + random1 * 1.0; // 0.5 to 1.5
                float randomAngleOffset = (random2 - 0.5) * 60.0; // -30 to +30 degrees
                float randomTimeOffset = random3 * 6.28318; // 0 to 2π
                float randomIntensityMult = 0.7 + random1 * 0.6; // 0.7 to 1.3
                
                // Apply randomization
                float shimmerSpeed = _ShimmerSpeed * randomSpeedMult;
                float shimmerAngle = _ShimmerAngle + randomAngleOffset;
                float timeWithOffset = time + randomTimeOffset;
                
                // Convert angle to radians
                float angleRad = radians(shimmerAngle);
                float2 shimmerDir = float2(cos(angleRad), sin(angleRad));
                
                // Create moving shimmer effect with randomized timing
                float shimmerPos = dot(uv, shimmerDir) + timeWithOffset * shimmerSpeed;
                float shimmer = sin(shimmerPos * 6.28318) * 0.5 + 0.5;
                
                // Make shimmer more focused
                shimmer = pow(shimmer, 1.0 / _ShimmerWidth);
                shimmer = saturate(shimmer);
                
                // Add wave distortion with randomized parameters
                float waveSpeed1 = _WaveSpeed * (0.8 + random2 * 0.4);
                float waveSpeed2 = _WaveSpeed * (0.6 + random3 * 0.8);
                float wave1 = sin((uv.x + timeWithOffset * waveSpeed1) * _WaveFrequency * (1.5 + random1)) * _WaveAmplitude;
                float wave2 = cos((uv.y + timeWithOffset * waveSpeed2) * _WaveFrequency * (1.2 + random2)) * _WaveAmplitude;
                float waveEffect = (wave1 + wave2) * 0.5;
                
                // Combine shimmer with wave
                shimmer += waveEffect;
                shimmer = saturate(shimmer);
                
                // Create pulsing effect with random phase
                float pulseSpeed = 2.0 + random3 * 1.0;
                float pulse = sin(timeWithOffset * pulseSpeed + random1 * 6.28318) * 0.1 + 0.9;
                shimmer *= pulse;
                
                // Add radial gradient for more dynamic effect
                float2 center = float2(0.5, 0.5);
                float dist = distance(uv, center);
                float radial = 1.0 - smoothstep(0.0, 0.7, dist);
                
                // Multiple shimmer layers with randomized frequencies
                float shimmer2 = sin(shimmerPos * (3.0 + random1 * 2.0) + timeWithOffset) * 0.3 + 0.7;
                float shimmer3 = cos(shimmerPos * (6.0 + random2 * 4.0) - timeWithOffset * (1.5 + random3)) * 0.2 + 0.8;
                
                // Combine all effects
                float finalShimmer = shimmer * shimmer2 * shimmer3 * radial;
                finalShimmer *= _ShimmerIntensity * randomIntensityMult;
                
                // Apply white shimmer overlay
                float3 shimmerColor = float3(1, 1, 1) * finalShimmer;
                
                // Blend with original color
                color.rgb = lerp(color.rgb, color.rgb + shimmerColor, finalShimmer * 0.8);
                
                // Add subtle color shift with randomization
                float colorShift = sin(timeWithOffset * (1.5 + random1) + uv.x * 3.14159 + random2 * 6.28318) * 0.05;
                color.rgb += colorShift * float3(1, 0.9, 0.8) * finalShimmer;
                
                return color;
            }
            ENDHLSL
        }
    }
}