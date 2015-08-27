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
using drip3d.Textures;

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
			cc1.Material.DiffuseTexture = new Texture("opentksquare.png", TextureType.DIFFUSE);
			cc1.Material.SpecularExponent = 10.0f;
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
			ObjectManager.Instance.Objects.Add(cc1);

			ExpandedCube cc2 = new ExpandedCube();
			cc2.Material.DiffuseColor = Utils.Colors.ToVector(Color.LightPink);
			cc2.Start();
			cc2.Position = new Vector3(0f, -1.25f, 0f);
			cc2.Scale = new Vector3(5f, 0.1f, 5f);
			ObjectManager.Instance.Objects.Add(cc2);

			AssimpVolume teapot = AssimpVolume.LoadFromFile("teapot.obj");
			teapot.Material.DiffuseColor = Utils.Colors.ToVector(Color.Turquoise);
			teapot.Material.SpecularExponent = 30.0f;
			teapot.Scale = new Vector3(0.3f);
			teapot.Position += new Vector3(0f, -1f, 0f);
			teapot.Start();
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
			GL.Enable(EnableCap.Texture2D);
			GL.Color3(Color.Transparent);
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
				if (File.Exists(Path.Combine("Assets", "Images", m.DiffuseTexture.Name)) && !ObjectManager.Instance.Textures.ContainsKey(m.DiffuseTexture.Name))
				{
					Texture.LoadImage(m.DiffuseTexture);
				}

				if (File.Exists(Path.Combine("Assets", "Images", m.SpecularTexture.Name)) && !ObjectManager.Instance.Textures.ContainsKey(m.SpecularTexture.Name))
				{
					Texture.LoadImage(m.SpecularTexture);
				}

				if (File.Exists(Path.Combine("Assets", "Images", m.NormalTexture.Name)) && !ObjectManager.Instance.Textures.ContainsKey(m.NormalTexture.Name))
				{
					Texture.LoadImage(m.NormalTexture);
				}

				if (File.Exists(Path.Combine("Assets", "Images", m.OpacityTexture.Name)) && !ObjectManager.Instance.Textures.ContainsKey(m.OpacityTexture.Name))
				{
					Texture.LoadImage(m.OpacityTexture);
				}
			}
		}
	}
}
