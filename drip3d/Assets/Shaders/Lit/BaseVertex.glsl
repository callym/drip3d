#version 330

in vec3 position;
in vec3 normal;
in vec2 textureCoord;

uniform mat4 model;
uniform mat4 camera;

out vec3 f_position;
out vec3 f_normal;
out vec2 f_textureCoord;

void
main()
{
	gl_Position = camera * model * vec4(position, 1.0);

	f_position = (model * vec4(position, 1.0)).xyz;
	f_normal = normalize(transpose(inverse(mat3(model))) * normal);
	f_textureCoord = textureCoord;
}