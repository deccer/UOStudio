#version 460 core

layout (location = 1) in vec4 fs_position;
layout (location = 2) in vec3 fs_color;
layout (location = 3) in vec3 fs_normal;
layout (location = 4) in vec2 fs_uv;
layout (location = 5) in flat int fs_uv_index;

layout (location = 0) out vec4 out_color;

layout (location = 0) uniform int u_uv_index;

layout (binding = 0) uniform sampler2DArray t_textures;

void main()
{
    out_color = vec4(texture(t_textures, vec3(fs_uv, u_uv_index)).rgb, 1.0);
}
