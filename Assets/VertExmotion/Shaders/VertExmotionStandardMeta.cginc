#include "./VertExmotion.cginc"

struct VertexInputVM
{
	float4 vertex	: POSITION;
	float4 color	: COLOR;
	half3 normal	: NORMAL;
	float2 uv0		: TEXCOORD0;
	float2 uv1		: TEXCOORD1;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
	float2 uv2		: TEXCOORD2;
#endif
#ifdef _TANGENT_TO_WORLD
	half4 tangent	: TANGENT;
#endif
};


VertexInput ApplyVertExmotion ( VertexInputVM v )
{
	VertexInput v2;
	UNITY_INITIALIZE_OUTPUT(VertexInput, v2);
	//v2.vertex = v.vertex;
	v2.vertex = VertExmotion( v.vertex, v.color );
	v2.uv0 = v.uv0;	
	v2.uv1 = v.uv1;		
	v2.normal = v.normal;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
	v2.uv2	= v.uv2;	
#endif
#ifdef _TANGENT_TO_WORLD
	v2.tangent	= v.tangent;
#endif	
	
	
	return v2;
}




v2f_meta vert_metaVM (VertexInputVM v)
{
	return vert_meta( ApplyVertExmotion(v) );
}


