#version 330

in vec3 position;
in vec3 normal;

uniform mat4 model;
uniform mat4 camera;

out vec3 f_normal;

void
main()
{
	mat4 cameraModel = camera * model;
	gl_Position = cameraModel * vec4(position, 1.0);

	f_normal = normalize(mat3(cameraModel) * normal);
	f_normal = normal;
}