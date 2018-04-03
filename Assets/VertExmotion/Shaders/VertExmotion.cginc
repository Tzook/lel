
#include "UnityCG.cginc"

//#if SHADER_API_MOBILE
//	static const int NB_SENSORS = 4;
//#elif SHADER_TARGET < 30
#if SHADER_TARGET < 30
	static const int NB_SENSORS = 4;
#else
	static const int NB_SENSORS = 20;
#endif

uniform float4 KVM_SensorPosition[NB_SENSORS];
uniform float4 KVM_MotionDirection[NB_SENSORS];
uniform float4 KVM_MotionAxis[NB_SENSORS];
//uniform int KVM_MotionZoneID[NB_SENSORS];
uniform float4 KVM_RadiusCentripetalTorque[NB_SENSORS];
uniform float4 KVM_SquashStretch[NB_SENSORS];
uniform float4 KVM_Speed[NB_SENSORS];
uniform float4 KVM_AxisXScale[NB_SENSORS];
uniform float4 KVM_AxisYScale[NB_SENSORS];
uniform float4 KVM_AxisZScale[NB_SENSORS];
float KVM_NormalCorrection;
float KVM_NormalSmooth;

float3x3 RotAxis(float3 axis, float angle) {
	axis = normalize(axis);
	float s,c;
	sincos(angle, s, c);
	float oc = 1.0 - c;
	float3 as = axis*s;
	float3x3 p = float3x3(axis.x*axis, axis.y*axis, axis.z*axis);
	float3x3 q = float3x3(c, -as.z, as.y, as.z, c, -as.x, -as.y, as.x, c);
	return p*oc + q;
}


float4 VertExmotionBase(float4 wrldPos, float4 col, inout float weight )
{
	float3 torqueDir = float3(0,0,0);
	float4 motionDir = float4(0,0,0,0);
	float3 centripetalDir = float3(0,0,0);
	float dist;
	float3 squash = float3(0,0,0);
	
	for( int i=0; i<NB_SENSORS; ++i )
	{

		float3 sensorDir = wrldPos.xyz - KVM_SensorPosition[i].xyz;
		dist = length(sensorDir);
		
		if( dist < KVM_RadiusCentripetalTorque[i].x )
		{
			float f = 1.0f - dist / (KVM_RadiusCentripetalTorque[i].x + .0000001f);
			float p = pow(f, KVM_MotionDirection[i].w);			
			weight = i==0? f : (weight+f)*.5;
			f = p;
			//torqueDir.xyz = cross( normalize( wrldPos.xyz-_SensorPosition[i].xyz).xyz, _MotionAxis[i].xyz ) ;
			//torqueDir *= _RadiusCentripetalTorque[i].z * dist;	
			if(length(KVM_MotionAxis[i].xyz)>0)
				torqueDir = mul(RotAxis(KVM_MotionAxis[i].xyz, KVM_RadiusCentripetalTorque[i].z * (UNITY_PI / 180.0) * f), (wrldPos - KVM_SensorPosition[i]).xyz)- (wrldPos - KVM_SensorPosition[i]).xyz;

			centripetalDir = normalize( (wrldPos- KVM_SensorPosition[i]).xyz );
			float3 motion = (KVM_MotionDirection[i].xyz + torqueDir + centripetalDir * KVM_RadiusCentripetalTorque[i].y)*f;

			
			//scale
			float3 axisScale;			
			axisScale = dot(normalize(sensorDir), normalize(KVM_AxisXScale[i].xyz)) * dist * KVM_AxisXScale[i].xyz;
			motion += (axisScale * KVM_AxisXScale[i].w - axisScale) * f;
			axisScale = dot(normalize(sensorDir), normalize(KVM_AxisYScale[i].xyz)) * dist * KVM_AxisYScale[i].xyz;
			motion += (axisScale * KVM_AxisYScale[i].w - axisScale) * f;
			axisScale = dot(normalize(sensorDir), normalize(KVM_AxisZScale[i].xyz)) * dist * KVM_AxisZScale[i].xyz;
			motion += (axisScale * KVM_AxisZScale[i].w - axisScale) * f;

//#if SHADER_API_MOBILE
//#elif SHADER_TARGET < 30
//#else
#if SHADER_TARGET >= 30
			if( length(KVM_Speed[i].xyz) > 0 )
			{
				//stretch
				float d = dot(KVM_Speed[i].xyz,  centripetalDir );
				if(d>=0)
					motion += d * d * d * KVM_SquashStretch[i].y * KVM_Speed[i].xyz;
//				else			
//					motion += d * d * d * _SquashStretch[i].y * _Speed[i].xyz * .1f;
			
				//stretch reduce volume
				float3 c1 = cross( normalize(KVM_Speed[i].xyz ), centripetalDir );
				float3 c2 = cross( normalize(KVM_Speed[i].xyz ), c1 );
				float d2 = dot( (wrldPos- KVM_SensorPosition[i]).xyz, c2 );
				
				if( length(c2)>0 )
				{  
					motion -= normalize(c2)* length(KVM_Speed[i].xyz) * d2 * KVM_SquashStretch[i].y * .8f;
					motion += normalize(c2)* length(KVM_Speed[i].xyz)* d2 * KVM_SquashStretch[i].x;
				}
			}
#endif				
			float layerWeight = ((KVM_SensorPosition[i].w == 2) ? col.g : ((KVM_SensorPosition[i].w == 1) ? col.r : ((KVM_SensorPosition[i].w == 3) ? col.b : max(max(col.r, col.g), col.b))));
			motionDir.xyz += motion * layerWeight;			
			weight *= layerWeight;
		}		
	}	 



	return (wrldPos + motionDir);
}


float4 VertExmotion(float4 vpos, float4 col)
{
	float4 wrldPos = mul(unity_ObjectToWorld, vpos); 		
	float w = 0;
	wrldPos = VertExmotionBase(wrldPos, col, w);
	vpos.xyz = mul(unity_WorldToObject, wrldPos);
	return vpos;
}


float4 VertExmotion(float4 vpos, float4 col, inout float weight)
{
	float4 wrldPos = mul(unity_ObjectToWorld, vpos);
	wrldPos = VertExmotionBase(wrldPos, col, weight);
	vpos.xyz = mul(unity_WorldToObject, wrldPos);
	return vpos;
}


float4 VertExmotion(float4 vpos, float4 col, inout float3 n, float4 t)
{
	//_NormalCorrection = .8;
	float w = 0;
	float4 newpos = VertExmotion(vpos, col,w);
#if !VERTEXMOTION_NORMAL_CORRECTION_OFF
	if (length(t.xyz) > 0 && KVM_NormalCorrection>0)
	{		
		float4 biTan = float4(cross(n, t.xyz), t.w);
		float4 posTan = VertExmotion(vpos + normalize(t) * .001, col);
		float4 posBiTan = VertExmotion(vpos + normalize(biTan) * .001, col);
		float3 newNormal = normalize(cross(normalize(posTan - newpos), normalize(posBiTan - newpos)));
		//float dotF = abs(dot(n, newNormal));		
		n = lerp(n, newNormal, KVM_NormalCorrection*(w < KVM_NormalSmooth ? w*(1.0 / KVM_NormalSmooth) : 1));
	}
#endif
	return newpos;
}



float4 VertExmotionUV( float4 vpos, float4 uv )
{	
	half4 wrldPos = mul( unity_ObjectToWorld, vpos  );	
	int sensorId = 0;
	
	//compute torque
	half3 torqueDir = half3(0,0,0);
	half4 motionDir = half4(0,0,0,0);
	half3 centripetalDir = half3(0,0,0);
	half dist;
	
	for( int i=0; i<NB_SENSORS; ++i )
	{
		sensorId = i;

		torqueDir.xyz = cross( normalize( wrldPos.xyz- KVM_SensorPosition[sensorId].xyz).xyz, KVM_MotionAxis[sensorId].xyz ) ;
		torqueDir *= KVM_RadiusCentripetalTorque[sensorId].z;
		
		centripetalDir = normalize( (wrldPos- KVM_SensorPosition[sensorId]).xyz ) * KVM_RadiusCentripetalTorque[sensorId].y;
		motionDir.xyz += KVM_MotionDirection[sensorId].xyz + torqueDir + centripetalDir;
	}
		
	vpos.xyz = mul( unity_WorldToObject, wrldPos + motionDir *  uv.y *  uv.y * uv.y).xyz;
	vpos.w = vpos.w;
	return vpos;
}


void VertExmotion( inout appdata_full v )
{	 
	 //float4 position = VertExmotion(v.vertex, v.color);//without normal fix
	 float4 position = VertExmotion(v.vertex, v.color, v.normal, v.tangent );
	 v.vertex = position;	 
}


void VertExmotionUV( inout appdata_full v )
{			
	v.vertex = VertExmotionUV( v.vertex, v.texcoord );	
}


//------------------------------------------
//Shader Forge function

float3 VertExmotionSF(float3 wrldXYZ, float wrldW, float3 col)
{
	float w = 0;
	//return mul(unity_WorldToObject, VertExmotionBase(float4(wrldXYZ, wrldW), float4(col, 0),w) - wrldXYZ);
	return mul(unity_WorldToObject, VertExmotionBase(float4(wrldXYZ, wrldW), float4(col, 0), w) - wrldXYZ);
}


float3 VertExmotionSF(float3 wrldXYZ, float wrldW, float3 col, inout float3 n, float3 t)
{	
	//return mul(unity_WorldToObject, VertExmotionBase(float4(wrldXYZ, wrldW), float4(col, 0), w) - wrldXYZ);
	float w = 0;
	float4 newpos = VertExmotionBase(float4(wrldXYZ, wrldW), float4(col, 0), w);
#if !VERTEXMOTION_NORMAL_CORRECTION_OFF
	if (length(t.xyz) > 0 && KVM_NormalCorrection>0)
	{
		float w2 = 0;
		float4 biTan = float4(cross(n.xyz, t.xyz), 1);		
		float4 posTan = VertExmotionBase(float4(wrldXYZ + normalize(t) * .001, wrldW) , float4(col, 0), w2);
		float4 posBiTan = VertExmotionBase(float4(wrldXYZ + normalize(biTan) * .001, wrldW), float4(col, 0),w2);
		float3 newNormal = normalize(cross(normalize(posTan - newpos), normalize(posBiTan - newpos)));
		//float dotF = abs(dot(n, newNormal));
		n = lerp(n, newNormal, KVM_NormalCorrection*(w < KVM_NormalSmooth ? w*(1.0 / KVM_NormalSmooth) : 1));		
	}
#endif
	return mul(unity_WorldToObject, newpos - wrldXYZ);
}


//------------------------------------------
//Amplify Shader Editor functions
float3 VertExmotionAdvancedASE(float3 localXYZ, float4 col)
{	
	return VertExmotion(float4(localXYZ,1), col)- localXYZ;
}

float3 VertExmotionAdvancedASE(float3 localXYZ, float4 col, inout float3 n, float3 t)
{
	return VertExmotion(float4(localXYZ, 1), col, n, float4(t,1)) - localXYZ;
}


float4 VertExmotionASE(inout appdata_full v)
{
	float4 oldpos = v.vertex;
	VertExmotion(v);
	return v.vertex - oldpos;
}

//------------------------------------------
// simple vertex pass for surface shader
void VertExmotion_appdata_full_vert(inout appdata_full v)
{
	VertExmotion(v);
}


//------------------------------------------
// simple vertex/fragment pass
struct VertExmotion_v2f
{
	float4 vertex : SV_POSITION;
};

VertExmotion_v2f VertExmotion_vert(appdata_full v)
{
	VertExmotion_v2f o;
	VertExmotion(v);
	o.vertex = UnityObjectToClipPos(v.vertex);
	return o;
}

fixed4 VertExmotion_frag(VertExmotion_v2f i) : SV_Target
{
	return float4(0,0,0,0);
}

//------------------------------------------