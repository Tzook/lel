Shader "VertExmotion/GUI/Text Shader" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color("Text Color", Color) = (1,1,1,1)
		_VertExmotion("VertExmotion", Range(0,1.0)) = 1.0
		_ShowLimits("ShowLimits", Range(0,1.0)) = 0.0
		_TopLimit("Top Limit", Float) = 10.0
		_BottomLimit("Bottom Limit", Float) = -10.0
			
		
	}

	SubShader {

		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		Lighting Off Cull Off ZTest Always ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ UNITY_SINGLE_PASS_STEREO

			#include "UnityCG.cginc"
			#include "../VertExmotion.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			float _VertExmotion;
			float _TopLimit;
			float _BottomLimit;
			float _ShowLimits;

			v2f vert(appdata_t v)
			{
				float4 limitColor = float4(1, 1, 1, 1);
				float4 v1 = v.vertex;
				float4 v2 = VertExmotion(v.vertex, float4(1, 1, 1, 1));

				if (v.vertex.y < _BottomLimit * 10)
				{
					v2 = v1;
					limitColor = float4(1, 0, 0, 1);;
				}

				if (v.vertex.y > _TopLimit * 10)
				{
					limitColor = float4(0, 0, 1, 1);;
					v2 = v1;
				}

				float4 v3 = lerp(v1, v2, _VertExmotion);


				v2f o;
				o.vertex = UnityObjectToClipPos(v3);
				o.color = lerp( v.color * _Color, limitColor, _ShowLimits);			
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}


			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				col.a *= tex2D(_MainTex, i.texcoord).a;
				return col;
			}
			ENDCG
		}
	}
}
