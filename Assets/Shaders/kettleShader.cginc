// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'defined KETTLE_NODECAL' with 'defined (KETTLE_NODECAL)'

#ifndef KETTLE_INCLUDED
#define KETTLE_INCLUDED

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"


// global identifiers
uniform float4 _GLOBALTINT;
uniform float _GRAMPLIGHTEN;

//consts
const float3 LUMCOEFF = float3(0.299, 0.587, 0.114);

// --------------------------------------------
// LUT stuff
// --------------------------------------------

// global vars
#ifdef KETTLE_LUT
sampler3D _LUT;
sampler3D _LUT2;
float _LUTBLEND;
#endif


// ------------------------------------------
// HSV shifter
// ------------------------------------------

float3 Shift_col(float3 COL, float3 shift) {
	float3 RESULT = float3(COL);
	float VSU = shift.z*shift.y*cos(shift.x*3.14159265 / 180);
	float VSW = shift.z*shift.y*sin(shift.x*3.14159265 / 180);

	RESULT.x = (.299*shift.z + .701*VSU + .168*VSW)*COL.x
		+ (.587*shift.z - .587*VSU + .330*VSW)*COL.y
		+ (.114*shift.z - .114*VSU - .497*VSW)*COL.z;

	RESULT.y = (.299*shift.z - .299*VSU - .328*VSW)*COL.x
		+ (.587*shift.z + .413*VSU + .035*VSW)*COL.y
		+ (.114*shift.z - .114*VSU + .292*VSW)*COL.z;

	RESULT.z = (.299*shift.z - .3*VSU + 1.25*VSW)*COL.x
		+ (.587*shift.z - .588*VSU - 1.05*VSW)*COL.y
		+ (.114*shift.z + .886*VSU - .203*VSW)*COL.z;

	return (RESULT);
}

// ---------------------------------------------
// main shader
// ---------------------------------------------

struct appdata_AT
{
	float4 vertex : POSITION;
	float2 uv1 : TEXCOORD0;
	float2 uv2 : TEXCOORD1;
	float4 normal : NORMAL;
};

struct v2f
{
	// shadow helper functions and macros
	fixed3 diff : COLOR0;
	fixed3 ambient : COLOR1;
	float4 pos : SV_POSITION;
	float2 uv1 : TEXCOORD0;
	SHADOW_COORDS(1)
	float2 uv2 : TEXCOORD2;
	UNITY_FOG_COORDS(3)
#ifdef KETTLE_SPEC
	float3 spec : TEXCOORD4;
#endif
#ifdef KETTLE_RIM
	float fresnel : TEXCOORD5;
#endif
};

sampler2D _MainTex;
sampler2D _FaceTex;
float4 _MainTex_ST;
float4 _FaceTex_ST;

// vertex shader
v2f vert(appdata_AT v)
{

	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	half3 worldNormal = UnityObjectToWorldNormal(v.normal);
	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

#ifdef KETTLE_SPEC
	// specular
	float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
	half3 h = normalize(_WorldSpaceLightPos0.xyz + worldViewDir);
	o.spec = max(0, dot(worldNormal, h));
#endif

#ifdef KETTLE_RIM
#ifndef KETTLE_SPEC
	float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
#endif
	o.fresnel = max(0, dot(worldNormal, worldViewDir));
#endif
	//diffuse & ambient
	half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
	o.diff = nl;
	o.ambient = ShadeSH9(half4(worldNormal, 1));

	//uvs
	o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
	o.uv2 = TRANSFORM_TEX(v.uv2, _FaceTex);

	// compute shadows data
	TRANSFER_SHADOW(o);
	// compute fog info
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

sampler2D _RampTex;
half4 _TintColor;
half4 _AddColor;
half _RampLighten;

#ifdef KETTLE_DESAT
half _Desat;
half _Darken;
#endif

#ifdef KETTLE_SPEC
half _SpecSmooth;
half _SpecFalloff;
half4 _SpecColour;
#endif

half _Invert;
half _OccPower;
half _OccStrength;
half _OccDarken;

#ifdef KETTLE_ALPHA
half _Fade;
#endif

#ifdef KETTLE_RIM
half4 _RimColor;
half _RimSmooth;
#endif


#ifdef KETTLE_SPEC
// spec function
float3 SpecFunction(float3 spec, float occ) {
	spec = pow(spec, _SpecFalloff * 32.0);
	// toon it
	spec = smoothstep(0.5 - _SpecSmooth*0.5, 0.5 + _SpecSmooth*0.5, spec);
	// kill it based on occlusion
	spec = lerp(spec, float3(0, 0, 0), pow(occ, 0.5));
	return spec * _SpecColour * _LightColor0.rgb;
}
#endif

// fragment shader
fixed4 frag(v2f i) : SV_Target
{
#ifndef KETTLE_UNLIT
	// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
	half shadow = SHADOW_ATTENUATION(i);

	//setup diffise to be diffuse plus half ambient
	//half3 diff = ((i.diff) + (i.ambient*0.5f)) * shadow;
	half3 diff = i.diff * shadow;

	// ramp the result
	diff = (tex2D(_RampTex, float2(diff.x, 0.5))  * _LightColor0);// +_RampLighten;
	diff = lerp(diff, float4(1, 1, 1, 1), _GRAMPLIGHTEN);

	// add ambient back on
	half3 lighting = diff + (i.ambient*1.0);
#endif
	// get textures
	half4 baseTex = tex2D(_MainTex, i.uv1);
	half4 col = 0;
#ifndef KETTLE_UNLIT
	half4 face = tex2D(_FaceTex, i.uv2);

	//tint colour
#if defined (KETTLE_NODECAL)
	baseTex.rgb *= _TintColor;
	face.rgb *= _TintColor;

	// lighting and main colour...
	col.rgb = lighting * baseTex.rgb;
	face.rgb *= lighting;
	// multiply by itself based on alpha of main tex
	col.rgb = lerp(col.rgb, (col.rgb*col.rgb*_OccDarken), baseTex.a);
	face.rgb = lerp(face.rgb, (face.rgb*face.rgb*_OccDarken), baseTex.a);
#else
	baseTex.rgb *= _TintColor;

	// lighting and main colour...
	col.rgb = lighting * baseTex.rgb;
	// multiply by itself based on alpha of main tex
	col.rgb = lerp(col.rgb, (col.rgb*col.rgb*_OccDarken), baseTex.a);
#endif
#else
	col.rgb = baseTex;
#endif

#ifdef KETTLE_SPEC
#ifndef KETTLE_SPECONLY	// if spec only a seperate pass will deal with this
	
	col.rgb += SpecFunction(i.spec, baseTex.a);

#endif
#endif

	//Rim shading
#ifdef KETTLE_RIM
	//return ((1-i.fresnel) - i.diff.r).xxxx;
	col.rgb +=	(smoothstep(0.6 - _RimSmooth*0.5, 0.6 + _RimSmooth*0.5, (1 - i.fresnel) - i.diff.r))*_RimColor;

#endif

#ifndef KETTLE_UNLIT
	//straight occlusion
	col.rgb *= lerp(1.0 - pow(baseTex.a, _OccPower), 1.0, 1.0 - _OccStrength);

	//invert
	col.rgb = lerp((float3(1.0, 1.0, 1.0) - col.rgb), col.rgb, 1.0 - _Invert);

	// lerp between face texture and mainTex
	col = lerp(col, face, face.a);

#endif
	//GLOBALS
	col.rgb *= _GLOBALTINT.rgb;

	//additive colour
	col += _AddColor;

#ifdef KETTLE_DESAT
	// desaturate
	float3 shift = float3(0, 1 - _Desat, 1 - _Darken);
	col.rgb = Shift_col(col.rgb, shift);
#endif
#ifdef KETTLE_ALPHA
	col.a = lerp(_Fade, 1, face.a);
#endif
#ifdef KETTLE_UNLIT_ALPHA
	col.a = baseTex.a;
#endif
	// apply fog
	UNITY_APPLY_FOG(i.fogCoord, col);

	// LUT lookup
	//todo: make work in HDR
#ifdef KETTLE_LUT
	col.rgb = lerp(tex3D(_LUT, saturate(col.rgb)*0.99), tex3D(_LUT2, saturate(col.rgb)*0.99), _LUTBLEND);
#endif
	//col.rgb = i.ambient;// diff;// dot(diff, LUMCOEFF).xxx;
	return (col);
}

#ifdef KETTLE_SPECONLY
// fragment shader for spec
fixed4 fragSpec(v2f i) : SV_Target
{
	half4 baseTex = tex2D(_MainTex, i.uv1);

	half4 col = 0;

	col.rgb += SpecFunction(i.spec, baseTex.a);

	// apply fog: or not, as we end up with double brightness
	//UNITY_APPLY_FOG(i.fogCoord, col);

	// LUT lookup
	//todo: make work in HDR
#ifdef KETTLE_LUT
	col.rgb = lerp(tex3D(_LUT, saturate(col.rgb)*0.99), tex3D(_LUT2, saturate(col.rgb)*0.99), _LUTBLEND);
#endif
	return (col);
}
#endif

#endif
