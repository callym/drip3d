#version 330

in vec3 position;
in vec3 color;

uniform mat4 model;
uniform mat4 camera;

out vec4 f_color;

void
main()
{
	gl_Position = camera * model * vec4(position, 1.0);

	f_color = vec4(color, 1.0);
}