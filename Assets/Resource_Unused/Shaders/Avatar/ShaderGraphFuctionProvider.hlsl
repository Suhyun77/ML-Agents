#ifndef MYHLSLINCLUDE_INCLUDE
#define MYHLSLINCLUDE_INCLUDE

void GetAlbedoColor_float(float4 maskingColor, float4 albedoColor, float4 mainColor, float4 clothColor, float4 skinToneColor, out float4 albedo)
{
    if (maskingColor.r > 0) // Red Channel Masking < SkinTone
    {
        albedo = (albedoColor * (skinToneColor * maskingColor.r));
    }
    else if (maskingColor.b > 0) // Blue Channel Masking < Cloth
    {
        albedo = (albedoColor * (clothColor * maskingColor.b));
    }
    else // r <= 0.1 && g <= 0.1 && b <= 0.1
    {
        albedo = (albedoColor * mainColor);
    }
}
#endif