// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/ColoredPoint" {
	Properties {
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//_FourDCoordinatesAtStart ("FourDCoords", Vector) = (0,0,0,1)
		//could also have for 4d position where color of sphere changes depending on updated 4d position in space
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
	//	Tags { "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		#include "UnityCG.cginc"

		uniform float4 _FourDCoordinatesAtStart;

		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		//void surf (Input IN, inout SurfaceOutputStandard o) {
			//o.Albedo.rg = IN.worldPos.xy * 0.5 + 0.5;
			//o.Aldebo = _FourDCoordinatesAtStart.xyz * 0.5 + 0.5;  
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			//o.Alpha = _FourDCoordinatesAtStart.w * 0.5;
		//}

		//fixed4 frag (v2f i) : SV_Target
        //    {
        //        fixed4 col = _FourDCoordinatesAtStart * 0.4 + 0.5;
        //        return col;
        //    }
		ENDCG
	}
	FallBack "Diffuse"
}
