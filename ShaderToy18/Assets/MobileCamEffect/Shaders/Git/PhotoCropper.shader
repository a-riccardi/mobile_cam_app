Shader "ShaderToy/MobileCamEffect/PhotoCropper"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float scale_factor = _MainTex_TexelSize.z / _MainTex_TexelSize.w;
				o.uv.x /= scale_factor;

				float uv_max_extent = 1.0 / scale_factor;
				float margin = (1.0 - uv_max_extent) * 0.5;

				o.uv.x = remap(o.uv.x, 0.0, uv_max_extent, margin, uv_max_extent + margin);

				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}

			ENDCG
		}
	}
}
