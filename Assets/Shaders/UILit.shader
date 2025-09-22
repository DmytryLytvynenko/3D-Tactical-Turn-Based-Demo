Shader "UI/UIGlare"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        _SpecularStrength ("Specular Strength", Range(0.0, 2.0)) = 1.0
        
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
                float3 normal   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 vertex   : SV_POSITION;
                half4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float3 objectPos : TEXCOORD4;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float4 _ClipRect;
                float _Smoothness;
                float _Metallic;
                float _SpecularStrength;
            CBUFFER_END
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"
            
            Varyings vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.objectPos = v.vertex.xyz;
                
                // Transform normal to world space for lighting calculations
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.viewDir = GetWorldSpaceViewDir(o.worldPosition.xyz);
                
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
                
                // Normalize vectors
                float3 normalWS = normalize(i.worldNormal);
                float3 viewDirWS = normalize(i.viewDir);
                
                // Create a dynamic light direction based on object rotation and time
                float3 lightDir = normalize(float3(
                    sin(_Time.y * 0.5) * 0.3 + normalWS.x * 0.7,
                    cos(_Time.y * 0.3) * 0.2 + 0.8,
                    sin(_Time.y * 0.7) * 0.2 + normalWS.z * 0.5
                ));
                
                // Calculate reflection vector
                float3 reflection = reflect(-lightDir, normalWS);
                
                // Specular highlight using view direction and reflection
                float spec = saturate(dot(viewDirWS, reflection));
                
                // Smoother specular curve
                float specularPower = lerp(2.0, 128.0, _Smoothness);
                spec = pow(spec, specularPower);
                
                // Add position-based variation for more dynamic effect
                float2 uv = i.texcoord;
                float positionVariation = sin(uv.x * 3.14159) * sin(uv.y * 3.14159);
                spec *= (0.7 + 0.3 * positionVariation);
                
                // Fresnel effect
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 2.0);
                fresnel *= _Metallic;
                
                // Combine specular effects
                float3 specular = spec * _SpecularStrength * _Smoothness;
                
                // Add subtle color tinting to specular based on base color
                specular *= lerp(1.0, color.rgb, _Metallic * 0.5);
                
                // Final color combination
                color.rgb += specular + fresnel * color.rgb * 0.3;
                
                return color;
            }
            ENDHLSL
        }
    }
}