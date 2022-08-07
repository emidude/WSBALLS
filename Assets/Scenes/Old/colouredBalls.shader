Shader "Custom/colouredBalls"{
	Properties {
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_FourDCoordinatesAtStart ("FourDCoords", Vector) = (0,0,0,1)
		//could also have for 4d position where color of sphere changes depending on updated 4d position in space
		//_BallColorID ("BallColorID" , Float) = 0.0
		_SliceID("SliceID", Float) = 0.0
		_RandColors("RandColors", Vector) = (0,0,0,0)
		
	}
	SubShader {
     Tags {"Queue"="Transparent" "RenderType"="Transparent" }
	 //Tags { "RenderType"="Opaque" }
     CGPROGRAM
     //#pragma surface surf Lambert
	 #pragma surface surf Standard fullforwardshadows alpha:fade
     #pragma target 3.0

    	

	 uniform float4 _FourDCoordinatesAtStart;
	 //uniform float _BallColorID;
	 uniform float _SliceID;
	 uniform float3 _RandColors;

	 //half _Metallic;

     struct Input {
         float4 color : COLOR;
     };
     void surf (Input IN, inout SurfaceOutputStandard o) {
	 

		//o.Albedo.rgb = _FourDCoordinatesAtStart.rgb * 0.5 + 0.5; //fix later for unique colors (currently 2 balls with same color)
		
		o.Albedo = _RandColors;
		
		o.Alpha = 0.5f;
			
     }
     ENDCG
    }
    Fallback "Diffuse"
}
