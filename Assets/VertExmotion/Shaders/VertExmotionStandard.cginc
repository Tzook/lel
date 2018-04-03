#include "./VertExmotion.cginc"


struct VertexInputVM
{
	float4 vertex	: POSITION;
	float4 color	: COLOR;
	half3 normal	: NORMAL;
	float2 uv0		: TEXCOORD0;
	float2 uv1		: TEXCOORD1;
#ifdef DYNAMICLIGHTMAP_ON
	float2 uv2		: TEXCOORD2;
#endif
//#ifdef _TANGENT_TO_WORLD
	half4 tangent	: TANGENT;
//#endif
};


VertexInput ApplyVertExmotion ( VertexInputVM v )
{
	VertexInput v2;
		v2.vertex = v.vertex;
		//v2.vertex.xyz = VertExmotion( v.vertex, v.color );
		v2.vertex.xyz = VertExmotion(v.vertex, v.color, v.normal, v.tangent );
		float3 dir = v2.vertex.xyz - v.vertex.xyz;
		/*
		if (distance(v2.vertex.xyz, v.vertex.xyz) > 0.)
			v2.normal = normalize(v.normal + v.normal*dot(v.normal, normalize(dir)));
		else*/
			v2.normal = v.normal;
			
		v2.uv0 = v.uv0;
		v2.uv1 = v.uv1;
	#ifdef DYNAMICLIGHTMAP_ON
		v2.uv2 = v.uv2;
	#endif
	#ifdef _TANGENT_TO_WORLD
		//v2.normal = VertExmotionComputeNormal(v.normal, v.tangent, v2.vertex - v.vertex);
		v2.tangent = v.tangent;
	#endif
	return v2;
}

VertexOutputForwardBase vertForwardBaseVM (VertexInputVM v)
{	
	return vertForwardBase ( ApplyVertExmotion(v) );
}


VertexOutputForwardAdd vertForwardAddVM (VertexInputVM v)
{
	return vertForwardAdd( ApplyVertExmotion(v) );
}

VertexOutputDeferred vertDeferredVM (VertexInputVM v)
{
	return vertDeferred( ApplyVertExmotion(v) );
}






