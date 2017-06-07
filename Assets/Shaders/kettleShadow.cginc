#ifndef KETTLESHADOW_INCLUDED
#define KETTLESHADOW_INCLUDED

#include "UnityCG.cginc"

struct v2f 
{
	V2F_SHADOW_CASTER;
};


v2f vert(appdata_full v)
{
	v2f o;
	TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
	return o;
}

float4 frag(v2f i) : SV_Target
{
	SHADOW_CASTER_FRAGMENT(i)
}

#endif
