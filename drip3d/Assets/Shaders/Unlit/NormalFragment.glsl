#version 330

in vec3 f_normal;

out vec4 o_color;

void main()
{
	vec3 n = normalize(f_normal);

	o_color = vec4(0.5 + (0.5 * n), 1.0);
}