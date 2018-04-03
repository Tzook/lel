// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Hidden/VertExmotion_editor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		 //_Weights ("Weights", Range (0, 1)) = 0
		 _SensorId( "SensorId", int ) = 1
		 //_LayerId("LayerId", int) = 1
	}
	SubShader {
		
		Pass{
	CGPROGRAM
		#pragma target 3.0
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile VERTEXMOTION_GRADIENT_OFF VERTEXMOTION_GRADIENT_ON
		#include "UnityCG.cginc"
		#include "./VertExmotion.cginc"

		uniform float4 _SensorPositionEditor[NB_SENSORS];
		uniform float4 _RadiusCentripetalTorqueEditor[NB_SENSORS];

		struct vertexToFragment {
		float4 vertex : POSITION;
		float2 uv_MainTex : TEXCOORD0;
		float4 color : COLOR;		
		};

				
		sampler2D _MainTex;
		int _SensorId;
		int _LayerId;
		float _Power;

		void vert (appdata_full v, out vertexToFragment o) {

			if (_SensorId > -1)
				_LayerId = _SensorPositionEditor[_SensorId].w;

		if (_LayerId == 0)
			o.color.rgb = float3(1,1,1) * max(max(v.color.r, v.color.g), v.color.b);
		if (_LayerId == 1)
			o.color.rgb = v.color.rrr;
		if (_LayerId == 2)
			o.color.rgb = v.color.ggg;
		if (_LayerId == 3)
			o.color.rgb = v.color.bbb;

		o.color.a = 1;
		/*
		if( _SensorId == -1 )
		{			
				
		}
		else
		*/
		if (_SensorId > -1)
		{
			float4 wrldPos = mul( unity_ObjectToWorld, v.vertex  );	
			float dist = distance(wrldPos.xyz, _SensorPositionEditor[_SensorId].xyz);
			
			
#ifdef VERTEXMOTION_GRADIENT_ON			
			if (dist < _RadiusCentripetalTorqueEditor[_SensorId].x)
			{
				//o.color.rgb = o.color.rgb* (1 - dist / (_RadiusCentripetalTorqueEditor[_SensorId].x + .0000001f)) *o.color.g;
				
				float f = 1- dist / (_RadiusCentripetalTorqueEditor[_SensorId].x + .0000001f);
				float p = pow(f, _Power);
				//f = lerp(f, p, 1-p);				
				f = p;
				o.color.rgb = o.color.rgb* f *o.color.g;
			}
			else
				o.color.rgb = float3(0, 0, 0);
#else
			if (dist < _RadiusCentripetalTorqueEditor[_SensorId].x)
				o.color.rgb = lerp(float3(0, 1, 0), o.color.rgb, dist / (_RadiusCentripetalTorqueEditor[_SensorId].x + .0000001f)) *o.color.g;
#endif			
			//o.color = lerp( float4(0,1,0,1), v.color.gggg, dist/(_RadiusCentripetalTorqueEditor[_SensorId].x+.0000001f) ) * v.color.g;
			//else
				//o.color = v.color.gggg;
				
			
		}		
		o.vertex = UnityObjectToClipPos (v.vertex);	
		o.uv_MainTex = float4( v.texcoord.xy, 0, 0 );
			
		}


		fixed4 frag(vertexToFragment IN) : COLOR {
			float4 col;  	
			

#ifdef VERTEXMOTION_GRADIENT_ON
		//gradient
		float4 red = float4(1, 0, 0, 1);
		float4 yellow = float4(1, 1, 0, 1);
		float4 green = float4(0, 1, 0, 1);
		float4 blue = float4(0, 0, 4, 1);

		float f = IN.color.r;		
		col = lerp(yellow, red, (f - .6) * 3.3333);
		col = f < .6 ? lerp(green, yellow, (f - .3) * 3.3333) : col;
		col = f < .3 ? lerp(blue, green, f * 3.3333) : col;

		return col;
#else

			col = tex2D (_MainTex, IN.uv_MainTex);	
			col.x = col.y = col.z = (col.x + col.y + col.z) / 3;

			return lerp( IN.color, col, .4 );
#endif			


		}
		ENDCG

	 }
	} 
	//FallBack "Diffuse"
}
