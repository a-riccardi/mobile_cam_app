Shader "ShaderToy/MobileCamEffect/PhotoPostProduction"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}
		_LuminanceMask("Luminace Ramp", 2D) = "white" {}
		_RedOffset ("Red Offset", Range(-20.0, 20.0)) = 5.0
		_BlueOffset("Blue Offset", Range(-20.0, 20.0)) = -5.0
		_FisheyeF("Fisheye Strenght", Range(-1.8, 1.8)) = 1.0
		_LuminanceCorrectionF("Luminance Correction", Range(0.0, 1.0)) = 1.0
		_ColorCorrectionF("Color Correction", Range(0.0, 1.0)) = 1.0
		_FlareTex("Flare Texture", 2D) = "black" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile ______ UNITY_EDITOR
			#pragma multi_compile ______ FISHEYE
			#pragma multi_compile ______ RGB_DISTORTION
			#pragma multi_compile ______ LUMINANCE_FILTER
			#pragma multi_compile ______ COLOR_CORRECTION
			#pragma multi_compile ______ FLARE

			#include "UnityCG.cginc"
			#include "../../../Shaders/Utils.cginc"
			#include "../../../Shaders/BlendModes.cginc"

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
			sampler2D _MaskTex;
			sampler2D _LuminanceMask;
			sampler2D _FlareTex;

			sampler2D _LightMask;
			sampler2D _ShadowMask;
			float _LightStrength;
			float _ShadowStrength;

			float _RedOffset;
			float _BlueOffset;
			float4 _ScreenTextureTexelSize;
			float _FisheyeF;
			float4 _FlipUV;

			float _LuminanceCorrectionF;
			float _ColorCorrectionF;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
#ifdef UNITY_EDITOR
				o.uv = v.uv;
#else
				o.uv = float2(lerp(v.uv.y, 1.0 - v.uv.y, _FlipUV.x), lerp(v.uv.x, 1.0 - v.uv.x, _FlipUV.y));
#endif
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float2 baseUV = i.uv;
				float4 baseCol = tex2D(_MainTex, baseUV);

#ifdef FISHEYE
				//taken from http://www.geeks3d.com/20140213/glsl-shader-library-fish-eye-and-dome-and-barrel-distortion-post-processing-filters/5/
				float2 direction = i.uv - UV_CENTER;
				float2 distance = sqrt(dot(direction, direction));

				float power_sign = sign(_FisheyeF) * 0.5 + 0.5;

				float bind = lerp(0.5, 0.7071068, power_sign);

				float2 normalized_direction = normalize(direction);
				float2 distance_times_power = distance * _FisheyeF;
				float bind_times_power = bind * _FisheyeF;

				float2 uv_modifier = lerp
				(
					float2(normalized_direction.x, normalized_direction.y) * atan(-distance_times_power * 3.0) * bind / atan(-bind_times_power * 3.0),
					normalized_direction * tan(distance_times_power) * bind / tan(bind_times_power),
					power_sign
				);

				baseUV = _FisheyeF != 0 ? UV_CENTER + uv_modifier : i.uv;
				baseCol = tex2D(_MainTex, baseUV);
#endif
				
#ifdef RGB_DISTORTION
				float4 maskCol = tex2D(_MaskTex, i.uv);

				float col_r = tex2D(_MainTex, clamp(baseUV + _RedOffset * _ScreenTextureTexelSize.xy, 0.0, 1.0)).r;
				float col_g = tex2D(_MainTex, baseUV).g;
				float col_b = tex2D(_MainTex, clamp(baseUV + _BlueOffset * _ScreenTextureTexelSize.xy, 0.0, 1.0)).b;

				baseCol.r = lerp(baseCol.r, col_r, maskCol.r);
				baseCol.g = lerp(baseCol.g, col_g, maskCol.r);
				baseCol.b = lerp(baseCol.b, col_b, maskCol.r);
#endif

#ifdef COLOR_CORRECTION
				float grayscale = saturated_dot(baseCol.rgb, GRAYSCALE_3);
				grayscale = cheap_contrast(grayscale + 0.15, 1.3);

				float2 maskUV = float2(grayscale, 0.5);
				
				float3 shadows = lerp(baseCol.rgb, overlay(baseCol.rgb, tex2D(_ShadowMask, maskUV).rgb), _ShadowStrength);
				float3 lights = lerp(baseCol.rgb, overlay(baseCol.rgb, tex2D(_LightMask, maskUV).rgb), _LightStrength);
				float3 colorCorrectedCol = lerp(shadows, lights, grayscale);

				baseCol.rgb = lerp(baseCol.rgb, colorCorrectedCol, _ColorCorrectionF);
#endif

#ifdef LUMINANCE_FILTER

				float3 luminanceCol = float3(0.0, 0.0, 0.0);

				luminanceCol.r = tex2D(_LuminanceMask, float2(baseCol.r, 0.5)).r;
				luminanceCol.g = tex2D(_LuminanceMask, float2(baseCol.g, 0.5)).r;
				luminanceCol.b = tex2D(_LuminanceMask, float2(baseCol.b, 0.5)).r;

				baseCol.rgb = lerp(baseCol.rgb, luminanceCol, _LuminanceCorrectionF);
#endif

//#ifdef FLARE
				float3 flareCol = tex2D(_FlareTex, i.uv) * 0.5;
				//baseCol.rgb += flareCol;
				baseCol.rgb = soft_light(baseCol, flareCol);
//#endif

				return baseCol;
			}
			ENDCG
		}
	}
}
