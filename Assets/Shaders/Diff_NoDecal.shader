Shader "AdventureTime/DiffuseNoDecal"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (1, 1, 1, 1) // color
		_AddColor("Add Color", Color) = (0, 0, 0, 0) // color
		_RampTex("Ramp", 2D) = "white"{}
		_RampLighten("Ramp Lighten", Range(0.0, 1.0)) = 0.7
		_MainTex("Main Texture", 2D) = "white"{}
		_FaceTex("Face Texture", 2D) = "white"{}
		//_SpecColour("Spec Color", Color) = (1, 1, 1, 1) // color
		//_SpecSmooth("Spec Smooth", Range(0.0, 1.0)) = 0.0
		//_SpecFalloff("Spec power", Range(0.01, 1.0)) = 0.0
		//_Desat("Desaturation Amount", Range(0.0, 1.0)) = 0.0
		//_Darken("Darken", Range(0.0, 1.0)) = 0.0
		_OccDarken("Occlusion colour multiplier", Range(0.0, 1.0)) = 1.0
		_OccStrength("Additonal Occlusion strength", Range(0.0, 1.0)) = 0.5
		_OccPower("Additonal Occlusion Power", Range(0.0, 10.0)) = 2.0
	}

	SubShader
	{
		

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" "RenderType" = "Opaque" }
			Cull Off
			CGPROGRAM

			//#define KETTLE_SPEC
			//#define KETTLE_DESAT
			#define KETTLE_NODECAL
			//#define KETTLE_LUT

			#include "kettleShader.cginc"
			#pragma vertex vert
			#pragma fragment frag
			// compile shader into multiple variants, with and without shadows
			// (we don't care about any lightmaps yet, so skip these variants)
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			//Needed for fog variation to be compiled.
			#pragma multi_compile_fog
			#pragma target 3.0
			ENDCG
		}

		// shadow caster rendering pass, implemented manually
		// using macros from UnityCG.cginc
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" "RenderType" = "Opaque" }
			Cull Off
			CGPROGRAM
			#include "kettleShadow.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			ENDCG
		}
	}
}