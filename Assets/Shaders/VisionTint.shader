Shader "Custom/VisionTint" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Tint("Color", Color) = (1, 1, 1, 1)
		_Amount("Tint Amount", Range(0, 1)) = 1
	}
		SubShader{
		Pass{
		CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag

#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform float4 _Tint;
	uniform float _Amount;

	float4 frag(v2f_img i) : COLOR{
		float4 c = tex2D(_MainTex, i.uv);

		float4 result = c;
		float4 resultTint = result * _Tint;
		result = lerp(result, resultTint, _Amount);
		return result;
	}
		ENDCG
	}
	}
}
