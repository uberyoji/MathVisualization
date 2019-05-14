Shader "Unlit/LineScroll"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScrollTex("Scroll", 2D) = "white" {}
		_ScrollSpeed("Scroll Speed", float ) = 1
		_Tint("Tint", Color)= (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend One One
		
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


						
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			sampler2D _ScrollTex;
			float4 _MainTex_ST;
			float _ScrollSpeed;
			float4 _Tint;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);				
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 uv;
				uv.x = i.color.r + _Time.y * _ScrollSpeed;	// TODO move this to vert shader
				uv.y = 0.5;

				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 gra = tex2D(_ScrollTex, uv );
				col *= gra;
				col *= _Tint;
				return col;
			}
			ENDCG
		}
	}
}
