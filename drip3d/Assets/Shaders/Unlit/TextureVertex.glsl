#version 330

in vec3 position;
in vec2 textureCoord;

uniform mat4 model;
uniform mat4 camera;

out vec2 f_textureCoord;

void main()
{
	gl_Position = camera * model * vec4(position, 1.0);

	f_textureCoord = textureCoord;
}