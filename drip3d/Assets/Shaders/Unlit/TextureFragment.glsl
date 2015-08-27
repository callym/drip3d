#version 330

in vec2 f_textureCoord;

uniform sampler2D materialDiffuseTexture;

out vec4 o_color;

void main()
{
	vec2 flippedTexCoord = vec2(f_textureCoord.x, 1.0 - f_textureCoord.y);
	
	o_color = texture(materialDiffuseTexture, f_textureCoord);
}