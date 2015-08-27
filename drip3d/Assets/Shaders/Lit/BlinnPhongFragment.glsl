#version 330
precision mediump float;

in vec3 f_vert;
in vec3 f_normal;
in vec2 f_textureCoord;

uniform mat4 model;
uniform vec3 cameraPosition;

#define MAX_LIGHTS 10
uniform int numberOfLights;
uniform struct Light
{
	float Intensity;
	vec3 Color;
	vec4 Position;
	float Attenuation;
	vec3 Ambient;
	float ConeAngle;
	vec3 ConeDirection;
} allLights[MAX_LIGHTS];

uniform vec3 materialDiffuse;
uniform sampler2D materialDiffuseTexture;
uniform vec3 materialSpecular;
uniform float materialSpecularExponent;

out vec4 o_color;
 
vec4 ApplyLight(Light light, vec4 materialColor, vec3 normal, vec3 position, vec3 surfaceToCamera)
{
	vec3 surfaceToLight;
	float attenuation = 1.0;

	// directional light
	if (light.Position.w == 0.0)
	{
		surfaceToLight = normalize(light.Position.xyz);
		// no attenuation for directional lights
		attenuation = 1.0; 
		// already at 1.0
	}
	else
	{
		// point light
		surfaceToLight = normalize(light.Position.xyz - position);
		float distanceToLight = length(light.Position.xyz - position);
		attenuation = 1.0 / (1.0 + (light.Attenuation * pow(distanceToLight, 2.0)));

		float lightToSurfaceAngle = degrees(acos(dot(-surfaceToLight, normalize(light.ConeDirection))));
		if (lightToSurfaceAngle > light.ConeAngle)
		{
			attenuation = 0.0;
		}
	}

	vec4 lightColorIntensity = vec4(light.Intensity * light.Color, 1.0);

	// AMBIENT
	vec4 ambient = vec4(light.Ambient, 1.0) * materialColor;

	// DIFFUSE
	// cos of angle of incidence
	float diffuseCoefficient = max(0.0, dot(normal, surfaceToLight));
	vec4 diffuse = diffuseCoefficient * materialColor * lightColorIntensity;

	// SPECULAR
	float specularCoefficient = 0.0;
	if (diffuseCoefficient > 0.0) // if not backfacing
	{
		vec3 half_v = normalize(surfaceToCamera + surfaceToLight);
		float cosAngle = max(0.0, dot(normal, half_v));
		specularCoefficient = pow(cosAngle, materialSpecularExponent);
	}
	vec4 specular = specularCoefficient * vec4(materialSpecular, 1.0) * lightColorIntensity;

	// LINEAR
	return ambient + (attenuation * (diffuse + specular));
}

void main()
{
	// normal in world coordinates
	vec3 normal = normalize(transpose(inverse(mat3(model))) * f_normal);

	// location of this fragment in world coordinates
	vec3 position = vec3(model * vec4(f_vert, 1));

	vec2 flipped_coord = vec2(f_textureCoord.x, 1.0 - f_textureCoord.y);
	vec4 materialColor = vec4(materialDiffuse, 1.0) * texture(materialDiffuseTexture, flipped_coord);

	vec3 surfaceToCamera = normalize(cameraPosition - position);

	vec4 linearColor = vec4(0);

	for (int i = 0; i < numberOfLights; ++i)
	{
		linearColor += ApplyLight(allLights[i], materialColor, normal, position, surfaceToCamera);
	}

	// GAMMA CORRECTION
	//vec3 gamma = vec3(1.0/2.2);
	vec4 gamma = vec4(0.454545455);
	vec4 finalColor = pow(linearColor, gamma);

	o_color = finalColor;
}