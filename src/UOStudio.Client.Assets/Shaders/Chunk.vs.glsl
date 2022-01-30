#version 460 core

layout (location = 0) in vec3 i_position;
layout (location = 1) in vec3 i_color;
layout (location = 2) in vec3 i_normal;
layout (location = 3) in vec3 i_uv;

layout (location = 0) out gl_PerVertex
{
    vec4 gl_Position;
};
layout (location = 1) out vec4 fs_position;
layout (location = 2) out vec3 fs_color;
layout (location = 3) out vec3 fs_normal;
layout (location = 4) out vec2 fs_uv;
layout (location = 5) out int fs_uv_index;

layout (location = 0) uniform mat4 u_projection;
layout (location = 4) uniform mat4 u_view;
layout (location = 8) uniform mat4 u_world;

void main()
{
    fs_position = u_world * vec4(i_position, 1.0);
    gl_Position = u_projection * u_view * fs_position;

    fs_color = i_color;
    fs_normal = (u_world * vec4(i_normal, 1.0)).xyz;
    fs_uv = i_uv.xy;
    fs_uv_index = int(i_uv.z);
}
