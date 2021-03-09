struct VSInput
{
    float4 Position : SV_Position;
    float3 Normal   : NORMAL0;
    float2 UV       : TEXCOORD0;
};

struct VSOutput
{
    float3 UVW        : TEXCOORD0;
    float3 Normal     : NORMAL0;
    float3 Position   : POSITION1;
    float4 PositionPS : SV_Position;
};

Texture3D T_Atlas : register(t0);
sampler3D S_Atlas : register(s0);

cbuffer Parameters : register(b0)
{
    float4x4 M_World;
    float3x3 M_WorldInverseTranspose;
};

cbuffer ProjectionMatrix : register(b1)
{
    float4x4 M_WorldViewProj;
};

VSOutput VSMain(VSInput input)
{
    VSOutput output;

    float4 position = float4(input.Position.xy, 0, 1);
    output.Position = input.Position.xyz;
    output.PositionPS = mul(position, M_WorldViewProj);
    output.UVW = float3(input.UV, input.Position.z);
    output.Normal = input.Normal;

    return output;
}

float4 PSMain(VSOutput input) : SV_Target0
{
    float4 lightColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

    float3 lightPosition = float3(1.0f, 1.0f, 1.0f);
    float3 lightDirection = normalize(lightPosition - input.Position);

    float3 diffuse = float3(0.5, 0.5, 0.5) + saturate(dot(lightDirection, input.Normal)) * lightColor;

    return tex3D(S_Atlas, input.UVW);// * float4(diffuse, 1.0f);
}

technique WorldEffect
{
    pass p0
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader  = compile ps_3_0 PSMain();
    }
}
