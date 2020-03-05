Shader "Unlit/HoloShader"
{   //written in shaderlab code: https://docs.unity3d.com/Manual/SL-Shader.html
    // surface shaders deal with lighting, this is unlit, simple vertex and fragment shader, used for transparency
    //https://docs.unity3d.com/Manual/SL-ShaderPrograms.html
    Properties
    {
    //a bit like public vars, appear in inspector, can set from code. underscoring is just a convention
        _MainTex ("Texture Name", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1,1,1,1)
        _Transparency("Transparency", Range(0.0,0.5)) = 0.25 //Range instead of Float since can have probs if alpha to high, and Range gives slider
        
        //for glitch effect

        //_CutoutThresh("Cutout Threshold", Range(0.0,1.0)) = 0.2 //discards pixels if not past this threshold for redness
        //_Distance("Distance", Float) = 1
        //_Amplitude("Amplitude", Float) = 1
        //_Speed ("Speed", Float) = 1
        //_Amount("Amount", Float) = 1
        
        //these are updated in cs script. 
        //eg:
        //Renderer holoRenderer = GetComponent<Renderer>();
        //holoRenderer.material.SetFloat("_Amount", 1f);
        //holoRenderer.material.SetFloat("_CutoutThresh", 0.4f);

		//for setting colors of 3-sphere
		//_FourDCoordinatesAtStart("FourDCoords", Vector) = (0,0,0,1)
		_SliceID("SliceID", Float) = 0.0
		_RandColors("RandColors", Vector) = (0,0,0,0)
		_SliceLayer("SliceLayer", Int) = 0

		
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" } //predefined render queues, trans goes after opaque
        LOD 100 //"level of detail"

        ZWrite Off //dont write to depth buffer for trans
        //Cull Off
        Blend SrcAlpha OneMinusSrcAlpha //specifying method to blend colours after rendering everything -see https://docs.unity3d.com/Manual/SL-Blend.html

        Pass
        {
            CGPROGRAM //actual shader code
            #pragma vertex vert //"we have a vertex function called vert" 
            //(takes in appdata, returns v2f, which is passed to frag fn, used in frag shader)
            #pragma fragment frag
            
            
            #include "UnityCG.cginc"

            struct appdata
            {
            //the model with this material attached, has POSITION coordinates from mesh -these we lable here vertex
            //also TEXCOORD0 texture coordinates from the material

                float4 vertex : POSITION; //semantic binding - vertex is a POSITION (local coord space), float4
                //vertices of model are passed in through local coord space
                float2 uv : TEXCOORD0;
                //uv texture coordinates are passed in as float2, these are bound to "TEXCOORD0"
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                
                float4 vertex : SV_POSITION; //"SV_POSITION = screen space positon" screen view
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _TintColor;
            float _Transparency;
            float _CutoutThresh;
            
            float _Distance;
            float _Amplitude;
            float _Speed;
            float _Amount;
            
			uniform float3 _RandColors;

			int _SliceLayer;
			float _SliceID;

            v2f vert (appdata v) //could pass in vertex and uv data directly in () but cleaner as a struct
            //returns v2f, with data structure for frag shader, defined above
            {
                v2f o;

                //glitch effect, update the model x, coordinates according to sin fn:
                //can see this in scene view (not in play mode), if selected "Animated Materials" from picture icon
                //v.vertex.x += sin(_Time.y * _Speed + v.vertex.y * _Amplitude) * _Distance * _Amount; //_Time is special unity vec4, y=time in seconds

                o.vertex = UnityObjectToClipPos(v.vertex); //(unity helper function) takes local coords of vertex data and transforms to world, projects to clip space
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); //this fn includes tiling and offset transformations from editor
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target //outputs(bounds) to render target (screen frame buffer)
            {
                //// sample the texture
                ////fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;
                //fixed4 col = _TintColor;
                //col.a = _Transparency;
                ////clip(col.r - _CutoutThresh); //this is equiv to:
                                              // if (col.r < _CutoutThresh) discard; (if pixel does not have enough red, discard it)
				fixed4 col;
				col.xyz = _RandColors + _SliceLayer*_TintColor;
				col.a = _Transparency * _SliceID;

				return col;
            }
            ENDCG
        }
    }
}
