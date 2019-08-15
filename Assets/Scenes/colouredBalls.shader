Shader "Custom/colouredBalls"{
	Properties {
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_FourDCoordinatesAtStart ("FourDCoords", Vector) = (0,0,0,1)
		//could also have for 4d position where color of sphere changes depending on updated 4d position in space
		_BallColorID ("BallColorID" , Float) = 0.0
	}
	SubShader {
     Tags {"Queue"="Transparent" "RenderType"="Transparent" }
	 //Tags { "RenderType"="Opaque" }
     CGPROGRAM
     //#pragma surface surf Lambert
	 #pragma surface surf Standard fullforwardshadows alpha:fade
     #pragma target 3.0

	 uniform float4 _FourDCoordinatesAtStart;
	 uniform float _BallColorID;

     struct Input {
         float4 color : COLOR;
     };
     void surf (Input IN, inout SurfaceOutputStandard o) {
	 //void surf (Input IN, inout SurfaceOutput o) {
        // o.Albedo.x = _FourDCoordinatesAtStart.x*_FourDCoordinatesAtStart.y *0.5 + 0.5;
		// o.Albedo.y = _FourDCoordinatesAtStart.z*_FourDCoordinatesAtStart.w *0.4 + 0.5;
		// o.Albedo.z = _FourDCoordinatesAtStart.z*_FourDCoordinatesAtStart.y *0.4 + 0.5;

		o.Albedo.rgb = _FourDCoordinatesAtStart.rgb * 0.5 + 0.5;
		o.Alpha = _FourDCoordinatesAtStart.a * 0.25 + 0.75;

		// o.Alpha = 1;

		 //o.Albedo = _FourDCoordinatesAtStart * 0.5 * + 0.5; //works but gives 2 the same

		//o.Albedo = (_BallColorID, _BallColorID, _BallColorID); //greyscale
		//o.Albedo = _FourDCoordinatesAtStart * 0.2 * + 0.25;
		//if (_FourDCoordinatesAtStart.w < 0 ){
		//o.Albedo *= 2;

		//
		//}

		//o.Albedo = _FourDCoordinatesAtStart  *0.5 + 0.5 ;
		//o.Albedo.r = _BallColorID;
		 
     }
     ENDCG
    }
    Fallback "Diffuse"
}
