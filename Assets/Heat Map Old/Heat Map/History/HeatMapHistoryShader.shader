Shader "Custom/HeatMapHistoryShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_HighlightTex ("Highlight Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_HeatMapRadius ("Heat Map Radius", Range(0, 2)) = 0.5
		_HeatmapPower ("Heat Map Power", Range(0, 1)) = 0.2
		_HeatmapIntensity ("Heat Map Intensity", Range(1, 20)) = 1.0
		
		//_GazePosition ("Gaze Pos", Vector) = (1, 1, 1, 1)
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
		sampler2D _HighlightTex;
		float _HeatMapRadius;
		float _HeatmapPower;
		float _HeatmapIntensity;
		int _Gaze_Length;
		float4 _GazePositions [1000];

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// This is the normal color for this fragment
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			// Ignore all this
			//fixed4 highlightInterval = highlightColor / _Gaze_Length;
			//o.Albedo = c.rgb;
			//o.Albedo = float4(_HeatMapRadius, IN.worldPos.y / 10, IN.worldPos.z / 10, 1);

			int numPings = 0; // How many gaze points this fragment is intersecting with
			o.Albedo = c.rgb;
			
			for (int i = 0; i < _Gaze_Length; i++) {
				float dist = distance(IN.worldPos, _GazePositions[i]);
				if (dist < _HeatMapRadius) { // Radius is set in script, determines how wide the circle is
					numPings += 1;
				} 
			}

			// Determines what the highlight color is based on a (effectively) 1D texture. Should just be a gradient from one color to another.
			fixed4 highlightColor = tex2D (_HighlightTex, float2((float) saturate(numPings * _HeatmapIntensity / _Gaze_Length), .5));

			// Lerp between texture color and highlight color. t is set so that the longer you look at a point, the closer to 1 t should be.
			// The power function ramps up the rate at which it becomes the highlight color. Decrease to make it turn faster.
			float t = pow(((float)numPings / (float)_Gaze_Length), (float)_HeatmapPower);
			o.Albedo = lerp (c, highlightColor, t);

			//float dist = distance(IN.worldPos, _GazePositions[0]);
			/*if (dist < _HeatMapRadius) {
				o.Albedo = lerp (highlightColor, c, dist / _HeatMapRadius);
			} else {
				o.Albedo = c.rgb;
			}*/
			//o.Albedo = _GazePositions[0];

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
