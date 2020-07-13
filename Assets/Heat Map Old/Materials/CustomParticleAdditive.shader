//This is a shader Adil wrote and I am dissecting to learn from it

//explanation websites:
	//Particle Shaders: https://docs.unity3d.com/Manual/PartSysVertexStreams.html
	//"Blend SrcAlpha One": https://docs.unity3d.com/Manual/SL-Blend.html
	//"Cull Off": https://docs.unity3d.com/Manual/SL-CullAndDepth.html
	//Ztest and ZWrite: https://docs.unity3d.com/Manual/SL-CullAndDepth.html
	//pragma target: https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html
	//appdata: https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html
	//UV definition: http://wiki.winamp.com/wiki/Pixel_Shader_Basics
	//UNITY_VERTEX_INPUT_INSTANCE_ID: https://docs.unity3d.com/Manual/GPUInstancing.html
	//SV_InstanceID: https://docs.unity3d.com/2018.2/Documentation/Manual/SL-ShaderCompileTargets.html
	//UNITY_FOG_COORDS(1): https://answers.unity.com/questions/981538/shaders-what-is-the-use-of-unity-fog-coords-unity.html
	//#ifdef: https://forum.unity.com/threads/shaders-and-the-mystery-of-multitude-of-different-conditional-define-checks.319112/
	//UNITY_VERTEX_OUTPUT_STEREO: https://docs.unity3d.com/Manual/SinglePassInstancing.html
	//UNITY_DECLARE_DEPTH_TEXTURE: https://docs.unity3d.com/Manual/SL-DepthTextures.html
	//_CameraDeptTexture: https://docs.unity3d.com/Manual/SL-CameraDepthTexture.html
Shader "Custom/Particles/Additive" {
Properties {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)	//creates a color variable for the shader
		//color created is grey with some transparency
    _MainTex ("Particle Texture", 2D) = "white" {}		//Main texture is white
    _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0	//scaler/float variable
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha One	//controls how the textures and colors are written to the screen after all the graphics are rendered
		//"SrcAlpha:" "The value of this stage is multiplied by the source alpha value."
		//"One:" blend factor value of one --> means the source or destination of colors come through fully <--(almost a direct quote from the above site)
		//format: Blend (tells us to blend the factors) SrcFactor (original factor source the color(SrcAlpha) is mult by), DestinationFactor ("SrcAlpha" is mult. by the factor and the result is added to the value of SrcFactor mult. "SrcAlpha")
    ColorMask RGB	//uses RGB color controls
    Cull Off	//"Cull" - prevents polygons from being rendered facing away from the viewer (meaning we will always see the side of a cube regardless of the angle?)
		//"Off" - disables culling and all faces are drawn
    ZWrite Off	//ZWrite - controls whether pixels from the object are written to a depth buffer(something that holds the z-depth value of each pixel in an image); turn off when dealing with transparent or transcluscent objects
    ZTest Always    //ZTest - says how depth testing should be performed

    SubShader {
        Pass {
			//a lot of the data/variables use/are macros

            CGPROGRAM
            #pragma vertex vert		//allows us to make a vertex shader (std unlit shader like I was taught)
            #pragma fragment frag	//allows us to make a frag shader (std unlit shader like I was taught)
            #pragma target 2.0		//compilation target (tells the shader models the shader will support); this shader supports all platforms by Unity
				//tells us what our functions can and cannot do?
				//target 2.0: "Limited amount of arithmetic & texture instructions; 8 interpolators; no vertex texture sampling; no derivatives in fragment shaders; no explicit LOD (level of detail) texture sampling."
				//no special functions like derivatives or geometry are used
            #pragma multi_compile_particles		//multi_compile: allows the shader to be run mult. times with different preprocessor directives --> creates a "'mega shader'" or "'uber shader'" (different shader variants are produced)
				//multi_compile_particles: accesses/uses the shader variant/component for particles
            #pragma multi_compile_fog		//uses the fog variant/component for the shader

            #include "UnityCG.cginc"	//imports files that can be used in a shader program (this one is premade)
				//automatically imported/used
            sampler2D _MainTex;		//creates/instantiates the texture type to be used in methods below
            fixed4 _TintColor;		//creates/instantiates the color type to be used in methods below

			//"appdata": "uses the vertex position and the first texture coordinate as the vertex shader inputs;" "This shader (one from webpage) is very useful for debugging the UV coordinates of the mesh"
            struct appdata_t {
                float4 vertex : POSITION;		//gets "vertex position" from the drawn image/gameObject's position --> default method
                fixed4 color : COLOR;		//gets the "per-vertex color" from the image/gameObject --> default method
                float2 texcoord : TEXCOORD0;	//gets the first UV coordinate (UV coord - coordinate of a pixel being altered on-screen) from the image/gameObject
                UNITY_VERTEX_INPUT_INSTANCE_ID		//used to get the "instance ID" in an input/output vertex shader
				//SV_InstanceID: legacy system, instance system?
				//part of SinglePassInstancing (see site under https://docs.unity3d.com/Manual/SinglePassInstancing.html)
            };

			//takes variables from "appdata" and places them into new variables
            struct v2f {
                float4 vertex : SV_POSITION;	//vertex positions are the SV positions
                fixed4 color : COLOR;	//color is more usable (or does it make a color var)
                float2 texcoord : TEXCOORD0;	//UV texture coordinates
                UNITY_FOG_COORDS(1)		//fog coordinates are at position 1?
                #ifdef SOFTPARTICLES_ON		//if defined, turn on the soft particles
                float4 projPos : TEXCOORD2;		//sets the 2nd coordinate to projPos
                #endif	//ends the #ifdef statement
                UNITY_VERTEX_OUTPUT_STEREO	//necessary for Single Pass Instanced rendering (rendering that replaces each draw call with an instanced draw call)
					//designed to greatly reduce CPU use, marginally reduce GPU use, and "significantly" reduces power consumption
					//used for VR
					//performs only 1 render pass
					//must be used in fragment shaders
            };

            float4 _MainTex_ST;		//the main texture's value is instantiated

			//vertex shader
            v2f vert (appdata_t v)
            {
                v2f o;	//v2f var --> used later in fragment shader
                UNITY_SETUP_INSTANCE_ID(v);		//"calculates and sets the built-in unity_StereoEyeIndex and unity_InstanceID Unity shader variables to the correct values based on which eye the GPU is currently rendering" 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);		//" tells the GPU which eye in the texture array it should render to, based on the value of unity_StereoEyeIndex;" "transfers the value of unity_StereoEyeIndex 
					//from the vertex shader so that it will be accessible in the fragment shader only if UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX is called in the fragment shader"
                o.vertex = UnityObjectToClipPos(v.vertex);		//the vertex values are set to the appdata vertices' values; necessary for using SinglePassInstancing

                o.color = v.color * _TintColor;		//vertex color is the appdata color mult by the tint color
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);	//moves the texture coordinates around the main texture/creates the texture coordinates based on the main Texture 
                UNITY_TRANSFER_FOG(o,o.vertex);		//the fog is made from the vertices created/used in v2f
                return o;	//return the v2f type variable
            }

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);	//used for soft particles, Image effects, and adding effects to a scene's depth
				//_CameraDepthTexture: allows us to sample the main depth texture for the Camera (ZWrite must be Off)

            float _InvFade;		//new float made

			//fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                // #ifdef SOFTPARTICLES_ON
                // float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                // float partZ = i.projPos.z;
                // float fade = saturate (_InvFade * (sceneZ-partZ));
                // i.color.a *= fade;
                // #endif

                fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);		//creates the output color
                UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
                return col;
            }
            ENDCG
        }
    }
}
}