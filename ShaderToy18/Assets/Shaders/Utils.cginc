#ifndef UTILS
#define UTILS

#define PI (3.14159265)
#define PI_2 (6.2831853)

#define GRAYSCALE_3 float3(0.2126, 0.7152, 0.0722)
#define GRAYSCALE float4(0.2126, 0.7152, 0.0722, 1.0)

#define UV_CENTER (float2(0.5, 0.5))

inline float remap(float value, float oldMin, float oldMax, float newMin, float newMax)
{
	return newMin + (value - oldMin) / (newMax - newMin) * (oldMax - oldMin);
}

inline float remap01(float value, float oldMin, float oldMax)
{
	return (value - oldMin) / (oldMax - oldMin);
}

inline float cheap_contrast(float val, float contrastF)
{
	return (val - 0.5) * contrastF + 0.5;
}

inline float2 cheap_contrast(float2 val, float contrastF)
{
	return (val - 0.5) * contrastF + 0.5;
}

inline float3 cheap_contrast(float3 val, float contrastF)
{
	return (val - 0.5) * contrastF + 0.5;
}

inline float4 cheap_contrast(float4 val, float contrastF)
{
	return (val - 0.5) * contrastF + 0.5;
}

inline float sqr_magnitude(float2 vec)
{
	return dot(vec, vec);
}

inline float sqr_magnitude(float3 vec)
{
	return dot(vec, vec);
}

inline float sqr_magnitude(float4 vec)
{
	return dot(vec, vec);
}

inline float saturated_dot(float2 vec1, float2 vec2)
{
	return saturate(dot(vec1, vec2));
}

inline float saturated_dot(float3 vec1, float3 vec2)
{
	return saturate(dot(vec1, vec2));
}

inline float saturated_dot(float4 vec1, float4 vec2)
{
	return saturate(dot(vec1, vec2));
}

#endif