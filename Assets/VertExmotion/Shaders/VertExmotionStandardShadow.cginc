#include "./VertExmotion.cginc"

struct VertexInputVM
{
	float4 vertex	: POSITION;
	float4 color	: COLOR;
	float2 uv0		: TEXCOORD0;
	float3 normal	: NORMAL;	
};


VertexInput ApplyVertExmotion ( VertexInputVM v )
{
	VertexInput v2;
	UNITY_INITIALIZE_OUTPUT(VertexInput, v2);
	//v2.vertex = v.vertex;
	v2.vertex = VertExmotion( v.vertex, v.color );
	v2.uv0 = v.uv0;		
	v2.normal = v.normal;
	return v2;
}



#if UNITY_VERSION >= 201720

	void vertShadowCasterVM(VertexInputVM v
		, out float4 opos : SV_POSITION
	#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
		, out VertexOutputShadowCaster o
	#endif
	#ifdef UNITY_STANDARD_USE_STEREO_SHADOW_OUTPUT_STRUCT
		, out VertexOutputStereoShadowCaster os
	#endif
		)
	{
		vertShadowCaster(ApplyVertExmotion(v)
			, opos
	#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
			, o
	#endif	
	#ifdef UNITY_STANDARD_USE_STEREO_SHADOW_OUTPUT_STRUCT
			, os
	#endif
			);
	}

#else

	void vertShadowCasterVM (VertexInputVM v,
		#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
		out VertexOutputShadowCaster o,
		#endif
		out float4 opos : SV_POSITION)
	{
		vertShadowCaster( ApplyVertExmotion(v),
	#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
		o,
	#endif	
		opos	
		);
	}

#endif