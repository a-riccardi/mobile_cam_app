Shader "ShaderToy/MobileCamEffect/PhotoAssembler"
{
	Properties
	{
		_MainTex ("Photo", 2D) = "white" {}
		_FrameTex("Frame", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../../../Shaders/Utils.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _FrameTex;
			float4 _FrameTex_TexelSize;
			float4 _FrameTex_UV;

			float4 frag (v2f i) : SV_Target
			{
				float4 frameCol = tex2D(_FrameTex, i.uv);

				float2 photoUV = float2(0.0, 0.0);

				photoUV.x = remap01(i.uv.x, _FrameTex_UV.x, _FrameTex_UV.z);
				photoUV.y = remap01(i.uv.y, _FrameTex_UV.y, _FrameTex_UV.w);

				float4 photoCol = tex2D(_MainTex, photoUV);
				return float4(lerp(photoCol.rgb, frameCol.rgb, frameCol.a), 1.0);
			}
			ENDCG
		}
	}
}
