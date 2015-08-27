using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;

using drip3d.Objects;
using drip3d.Objects.Models;
using drip3d.Objects.Lights;
using drip3d.Shaders;
using drip3d.Materials;

namespace drip3d
{
	public class Game : GameWindow
	{
		float time = 0.0f;
		Camera camera = new Camera();
		Vector2 lastMousePosition = new Vector2();

		List<Volume> objects = new List<Volume>();
		List<Light> lights = new List<Light>();

		public Game()
			: base(512, 512, new GraphicsMode(32, 24, 0, 4))
		{

		}

		void InitProgram()
		{
			lastMousePosition = new Vector2(Mouse.X, Mouse.Y);

			ObjectManager.Instance.Init();

			ShaderManager.Instance.Shaders.AddRange(
				new List<ShaderProgram>()
				{
					new ShaderProgram("color", "Unlit\\Color"),
					new ShaderProgram("texcoord", "Unlit\\TexCoord"),
					new ShaderProgram("normal", "Unlit\\Normal"),
					new ShaderProgram("textured", "Unlit\\Texture"),
					new ShaderProgram("lambert", "Lit\\Lambert", "Lit\\Base"),
					new ShaderProgram("phong", "Lit\\Phong", "Lit\\Base"),
					new ShaderProgram("blinnphong", "Lit\\BlinnPhong", "Lit\\Base"),
				});
			ShaderManager.Instance.Init();

			//LoadMaterials("opentk.mtl");

			ExpandedCube cc1 = new ExpandedCube();
			cc1.Material.DiffuseColor = Utils.Colors.ToVector(Color.Teal);
			cc1.Material.DiffuseTextureFile = "opentksquare.png";
			cc1.Material.SpecularExponent = 10.0f;
			Console.WriteLine(cc1.Material.DiffuseTextureFile);
			cc1.OnUpdate = (GameObject o, float t) =>
				{
					Volume v = (Volume)o;
					v.Position = new Vector3((float)Math.Cos(time), (float)Math.Sin(time), 0);
				};
			cc1.OnStart = (GameObject o) =>
				{
					Volume v = (Volume)o;
					v.Scale = new Vector3(0.5f, 0.5f, 0.5f);
				};
			cc1.Start();
			Console.WriteLine(cc1.Material.DiffuseTextureFile);
			ObjectManager.Instance.Objects.Add(cc1);

			ExpandedCube cc2 = new ExpandedCube();
			cc2.Material.DiffuseColor = Utils.Colors.ToVector(Color.LightPink);
			cc2.Start();
			cc2.Position = new Vector3(0f, -1.25f, 0f);
			cc2.Scale = new Vector3(5f, 0.1f, 5f);
			ObjectManager.Instance.Objects.Add(cc2);

			OBJVolume teapot = OBJVolume.LoadFromFile("teapot.obj");
			teapot.Start();
			teapot.Material.DiffuseColor = Utils.Colors.ToVector(Color.Turquoise);
			teapot.Material.SpecularExponent = 30.0f;
			teapot.Scale = new Vector3(0.3f);
			teapot.Position += new Vector3(0f, -1f, 0f);
			ObjectManager.Instance.Objects.Add(teapot);

			/*PointLight pointLight = new PointLight();
			pointLight.Data.Color = Utils.Colors.ToVector(Color.LightGoldenrodYellow);
			pointLight.Data.Intensity = 1f;
			ObjectManager.Instance.Lights.Add(pointLight);*/

			SpotLight sLight = new SpotLight();
			sLight.ChangePosition(new Vector3(3f, 3f, 0f));
			sLight.Data.Color = new Vector3(1f, 0f, 0f);
			sLight.Data.ConeAngle = 5f;
			sLight.OnUpdate = (GameObject o, float t) =>
				{
					SpotLight l = (SpotLight)o;
					l.LookAt(teapot.Position);
				};
			ObjectManager.Instance.Lights.Add(sLight);

			DirectionalLight dirLight = new DirectionalLight();
			dirLight.ChangePosition(new Vector3(0f, 10f, 5f));
			ObjectManager.Instance.Lights.Add(dirLight);

			AmbientLight ambientLight = new AmbientLight(Color.Red, 0.1f);
			ObjectManager.Instance.Lights.Add(ambientLight);

			ExpandedCube ccLight = new ExpandedCube();
			ccLight.Start();
			ccLight.Scale = new Vector3(0.1f);
			ccLight.OnUpdate = (GameObject o, float t) =>
				{
					Volume v = (Volume)o;
					v.Position = ObjectManager.Instance.Lights[0].Data.Position.Xyz;
				};
			ObjectManager.Instance.Objects.Add(ccLight);

			camera.Position += new Vector3(0f, 0f, 3f);

			Utils.CheckError();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Console.WriteLine(GL.GetString(StringName.Version));

			InitProgram();

			Title = "Hello OpenTK!";

			GL.CullFace(CullFaceMode.FrontAndBack);
			GL.ClearColor(Color.CornflowerBlue);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			ShaderManager.Instance.SwitchShaders();
			
			base.OnUpdateFrame(e);
			time += (float)e.Time;

			if (Focused)
			{
				Vector2 delta = lastMousePosition - new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);

				camera.AddRotation(delta.X, delta.Y);
				ResetCursor();
			}

			camera.Update(ClientSize.Width, ClientSize.Height);

			ObjectManager.Instance.Update(ShaderManager.Instance.CurrentShader, camera, time);

			Utils.CheckError();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Viewport(0, 0, Width, Height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.DepthTest);

			ShaderManager.Instance.CurrentShader.EnableVertexAttribArrays();

			ObjectManager.Instance.Render(ShaderManager.Instance.CurrentShader, camera);

			ShaderManager.Instance.CurrentShader.DisableVertexAttribArrays();

			GL.Flush();
			SwapBuffers();

			Utils.CheckError();
		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == Key.Escape)
			{
				Exit();
			}

			switch (e.Key)
			{
				case Key.W:
					camera.Move(0f, 0.1f, 0f);
					break;
				case Key.A:
					camera.Move(-0.1f, 0f, 0f);
					break;
				case Key.S:
					camera.Move(0f, -0.1f, 0f);
					break;
				case Key.D:
					camera.Move(0.1f, 0f, 0f);
					break;
				case Key.Q:
					camera.Move(0f, 0f, -0.1f);
					break;
				case Key.E:
					camera.Move(0f, 0f, 0.1f);
					break;
				case Key.Space:
					ObjectManager.Instance.Lights[0].ChangePosition(camera.Position);
					break;
				case Key.L:
					ShaderManager.Instance.ChangeToNextShader();
					break;
				case Key.P:
					ShaderManager.Instance.ChangeToPreviousShader();
					break;
			}
		}

		void ResetCursor()
		{
			OpenTK.Input.Mouse.SetPosition(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
			lastMousePosition = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
		}

		protected override void OnFocusedChanged(EventArgs e)
		{
			base.OnFocusedChanged(e);

			if (Focused)
			{
				ResetCursor();
			}
		}

		public static int LoadImage(Bitmap image)
		{
			int textureID = GL.GenTexture();

			GL.BindTexture(TextureTarget.Texture2D, textureID);
			BitmapData data = image.LockBits(
				new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
				ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.SrgbAlpha,
				data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
				OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);

			image.UnlockBits(data);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			return textureID;
		}

		public static int LoadImage(string filename)
		{
			if (ObjectManager.Instance.Textures.ContainsKey(filename))
			{
				return ObjectManager.Instance.Textures[filename];
			}
			string path = Path.Combine("Assets", "Images", filename);
			try
			{
				Bitmap file = new Bitmap(path);
				int result = LoadImage(file);
				ObjectManager.Instance.Textures.Add(filename, result);
				return result;
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("!!! ERROR: image not found (" + filename + ") !!!");
				return -1;
			}
		}

		private static void LoadMaterials(string filename)
		{
			foreach (var m in Material.LoadFromFile(filename))
			{
				if (!ObjectManager.Instance.Materials.ContainsKey(m.Key))
				{
					ObjectManager.Instance.Materials.Add(m.Key, m.Value);
				}
			}

			foreach (var m in ObjectManager.Instance.Materials.Values)
			{
				if (File.Exists(Path.Combine("Assets", "Images", m.DiffuseTextureFile)) && !ObjectManager.Instance.Textures.ContainsKey(m.DiffuseTextureFile))
				{
					LoadImage(m.DiffuseTextureFile);
				}

				if (File.Exists(Path.Combine("Assets", "Images", m.SpecularTextureFile)) && !ObjectManager.Instance.Textures.ContainsKey(m.SpecularTextureFile))
				{
					LoadImage(m.SpecularTextureFile);
				}

				if (File.Exists(Path.Combine("Assets", "Images", m.NormalTextureFile)) && !ObjectManager.Instance.Textures.ContainsKey(m.NormalTextureFile))
				{
					LoadImage(m.NormalTextureFile);
				}

				if (File.Exists(Path.Combine("Assets", "Images", m.OpacityTextureFile)) && !ObjectManager.Instance.Textures.ContainsKey(m.OpacityTextureFile))
				{
					LoadImage(m.OpacityTextureFile);
				}
			}
		}
	}
}
