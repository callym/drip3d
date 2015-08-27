using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using drip3d.Materials;
using drip3d.Objects.Models;
using drip3d.Objects.Lights;
using drip3d.Shaders;
using drip3d.Textures;

namespace drip3d
{
	public sealed class ObjectManager
	{
		public List<Volume> Objects = new List<Volume>();
		public List<Light> Lights = new List<Light>();
		public Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
		public Dictionary<string, Material> Materials = new Dictionary<string, Material>();

		AttributeVector3 attributePositions = new AttributeVector3();
		AttributeVector3 attributeColor = new AttributeVector3();
		AttributeVector2 attributeTextureCoords = new AttributeVector2();
		AttributeVector3 attributeNormals = new AttributeVector3();

		UniformVector3 materialDiffuse =  new UniformVector3();
		UniformTexture2D materialDiffuseTexture = new UniformTexture2D();
		UniformVector3 materialSpecular =  new UniformVector3();
		UniformFloat materialSpecularExponent = new UniformFloat();

		UniformMatrix4 modelMatrix = new UniformMatrix4();
		UniformMatrix4 cameraMatrix = new UniformMatrix4();
		UniformVector3 cameraPosition = new UniformVector3();

		UniformFloat lightIntensity = new UniformFloat();
		UniformVector3 lightColor = new UniformVector3();
		UniformVector4 lightPosition = new UniformVector4();
		UniformFloat lightAttenuation = new UniformFloat();
		UniformVector3 lightAmbient = new UniformVector3();
		UniformFloat lightConeAngle = new UniformFloat();
		UniformVector3 lightConeDirection = new UniformVector3();

		UniformInt numberOfLights = new UniformInt();

		int iboElements;

		ObjectManager() { }

		public void Init()
		{
			GL.GenBuffers(1, out iboElements);

			attributePositions.Name			= "position";
			attributePositions.Normalised	= false;
			attributeColor.Name				= "color";
			attributeNormals.Name			= "normal";
			attributeTextureCoords.Name		= "textureCoord";

			materialDiffuse.Name			= "materialDiffuse";
			materialDiffuseTexture.Name		= "materialDiffuseTexture";
			materialSpecular.Name			= "materialSpecular";
			materialSpecularExponent.Name	= "materialSpecularExponent";

			cameraMatrix.Name				= "camera";
			cameraPosition.Name				= "cameraPosition";
			modelMatrix.Name				= "model";

			lightIntensity.Name				= "allLights[{0}].Intensity";
			lightColor.Name					= "allLights[{0}].Color";
			lightPosition.Name				= "allLights[{0}].Position";
			lightAttenuation.Name			= "allLights[{0}].Attenuation";
			lightAmbient.Name				= "allLights[{0}].Ambient";
			lightConeAngle.Name				= "allLights[{0}].ConeAngle";
			lightConeDirection.Name			= "allLights[{0}].ConeDirection";

			numberOfLights.Name				= "numberOfLights";
		}

		public void Update(ShaderProgram shader, Camera camera, float time)
		{
			foreach (Light l in Lights)
			{
				l.Update(time);
			}

			List<Vector3> vertices = new List<Vector3>();
			List<int> indices = new List<int>();
			List<Vector3> colors = new List<Vector3>();
			List<Vector2> textureCoords = new List<Vector2>();
			List<Vector3> normals = new List<Vector3>();
			int verticesCount = 0;

			foreach (Volume v in Objects)
			{
				v.Update(time);
				v.CalculateModelMatrix();
				v.ViewProjectionMatrix = camera.PerspectiveMatrix;
				v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewProjectionMatrix;

				vertices.AddRange(v.GetVertices().ToList());
				indices.AddRange(v.GetIndices(verticesCount).ToList());
				colors.AddRange(v.GetColorData().ToList());
				textureCoords.AddRange(v.GetTextureCoords().ToList());
				normals.AddRange(v.GetNormals().ToList());
				verticesCount += v.VerticesCount;
			}

			attributePositions.Value = vertices.ToArray();
			attributePositions.Update(shader);

			attributeColor.Value = colors.ToArray();
			attributeColor.Update(shader);

			attributeNormals.Value = normals.ToArray();
			attributeNormals.Update(shader);

			attributeTextureCoords.Value = textureCoords.ToArray();
			attributeTextureCoords.Update(shader);

			int[] indicesArray = indices.ToArray();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboElements);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicesArray.Length * sizeof(int)), indicesArray, BufferUsageHint.StaticDraw);

			GL.UseProgram(shader.ProgramID);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		public void Render(ShaderProgram shader, Camera camera)
		{
			int indiceat = 0;

			Matrix4 cameraPerspective = camera.PerspectiveMatrix;
			cameraMatrix.Value = cameraPerspective;
			cameraMatrix.Update(shader);

			cameraPosition.Value = camera.Position;
			cameraPosition.Update(shader);

			numberOfLights.Value = Lights.Count;
			numberOfLights.Update(shader);

			for (int i = 0; i < Lights.Count; i++)
			{
				lightIntensity.Value = Lights[i].Data.Intensity;
				lightIntensity.Update(shader, i);

				lightColor.Value = Lights[i].Data.Color;
				lightColor.Update(shader, i);

				lightPosition.Value = Lights[i].Data.Position;
				lightPosition.Update(shader, i);

				lightAttenuation.Value = Lights[i].Data.Attenuation;
				lightAttenuation.Update(shader, i);

				lightAmbient.Value = Lights[i].Data.Ambient;
				lightAmbient.Update(shader, i);

				lightConeAngle.Value = Lights[i].Data.ConeAngle;
				lightConeAngle.Update(shader, i);

				lightConeDirection.Value = Lights[i].Data.ConeDirection;
				lightConeDirection.Update(shader, i);
			}

			foreach (Volume v in ObjectManager.Instance.Objects)
			{
				IHasMaterial vMat = v as IHasMaterial;
				if (vMat != null)
				{
					var m = vMat.Material;

					materialDiffuse.Value = m.DiffuseColor;
					materialDiffuse.Update(shader);

					materialDiffuseTexture.Value = m.DiffuseTexture;
					materialDiffuseTexture.Update(shader);

					materialSpecular.Value = m.SpecularColor;
					materialSpecular.Update(shader);

					materialSpecularExponent.Value = m.SpecularExponent;
					materialSpecularExponent.Update(shader);
				}

				modelMatrix.Value = v.ModelMatrix;
				modelMatrix.Update(shader);

				GL.DrawElements(BeginMode.Triangles, v.IndiceCount, DrawElementsType.UnsignedInt, indiceat * sizeof(uint));
				indiceat += v.IndiceCount;
			}
		}

		public static ObjectManager Instance { get { return Nested.instance; } }
		class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested() { }
			internal static readonly ObjectManager instance = new ObjectManager();
		}
	}
}
