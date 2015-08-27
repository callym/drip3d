#version 330
precision mediump float;

in vec3 f_vert;
in vec3 f_normal;

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
uniform vec3 materialSpecular;
uniform float materialSpecularExponent;

out vec4 o_color;
 
vec3 ApplyLight(Light light, vec3 normal, vec3 position, vec3 surfaceToCamera)
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

	vec3 lightColorIntensity = light.Intensity * light.Color;

	// AMBIENT
	vec3 ambient = light.Ambient * materialDiffuse;

	// DIFFUSE
	// cos of angle of incidence
	float diffuseCoefficient = max(0.0, dot(normal, surfaceToLight));
	vec3 diffuse = diffuseCoefficient * materialDiffuse * lightColorIntensity;

	// SPECULAR
	float specularCoefficient = 0.0;
	if (diffuseCoefficient > 0.0) // if not backfacing
	{
		vec3 half_v = normalize(surfaceToCamera + surfaceToLight);
		float cosAngle = max(0.0, dot(normal, half_v));
		specularCoefficient = pow(cosAngle, materialSpecularExponent);
	}
	vec3 specular = specularCoefficient * materialSpecular * lightColorIntensity;

	// LINEAR
	return ambient + (attenuation * (diffuse + specular));
}

void main()
{
	// normal in world coordinates
	vec3 normal = normalize(transpose(inverse(mat3(model))) * f_normal);

	// location of this fragment in world coordinates
	vec3 position = vec3(model * vec4(f_vert, 1));

	vec3 surfaceToCamera = normalize(cameraPosition - position);

	vec3 linearColor = vec3(0);

	for (int i = 0; i < numberOfLights; ++i)
	{
		linearColor += ApplyLight(allLights[i], normal, position, surfaceToCamera);
	}

	// GAMMA CORRECTION
	//vec3 gamma = vec3(1.0/2.2);
	vec3 gamma = vec3(0.454545455);
	vec3 finalColor = pow(linearColor, gamma);

	o_color = vec4(finalColor, 1.0);
}