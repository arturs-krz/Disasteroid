Shader "Custom/VertexColorsStandard" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
 
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
 
        sampler2D _MainTex;
        float4 _Color;
		float _CO2;
 
        struct Input {
            float2 uv_MainTex;
            float4 color : COLOR;
        };
 
        half _Glossiness;
        half _Metallic;
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = (0.4* IN.color.rgb) + (0.6* lerp(IN.color.rgb, _Color, _CO2));
            
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            //o.Alpha = IN.color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}