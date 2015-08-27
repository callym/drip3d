using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace drip3d.Textures
{
	public enum TextureType
	{
		DIFFUSE = TextureUnit.Texture0,
		SPECULAR = TextureUnit.Texture1,
		NORMAL = TextureUnit.Texture2,
		OPACITY = TextureUnit.Texture3
	}

	public class Texture
	{
		public TextureUnit Unit = TextureUnit.Texture0;
		public int ID = -1;
		public bool Loaded = false;
		public string Name = "";

		Texture()
		{

		}

		public Texture(string name, TextureType type)
		{
			Name = name;
			Unit = (TextureUnit)type;
		}

		public void Start()
		{
			Texture result = LoadImage(this);
			Unit = result.Unit;
			ID = result.ID;
			Name = result.Name;
			Loaded = true;
		}

		public static Texture LoadImage(Bitmap image)
		{
			BitmapData data = image.LockBits(
				new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
				ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int textureID = GL.GenTexture();

			GL.BindTexture(TextureTarget.Texture2D, textureID);

			GL.TexImage2D(TextureTarget.Texture2D,
						0,
						PixelInternalFormat.Rgba,
						data.Width,
						data.Height,
						0,
						OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
						PixelType.UnsignedByte,
						data.Scan0);

			image.UnlockBits(data);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			Texture result = new Texture();
			result.ID = textureID;

			return result;
		}

		public static Texture LoadImage(string filename)
		{
			if (ObjectManager.Instance.Textures.ContainsKey(filename))
			{
				return ObjectManager.Instance.Textures[filename];
			}
			string path = Path.Combine("Assets", "Images", filename);
			try
			{
				Bitmap file = new Bitmap(path);
				Texture result = LoadImage(file);
				ObjectManager.Instance.Textures.Add(filename, result);
				return result;
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("!!! ERROR: image not found (" + filename + ") !!!");
				return null;
			}
		}

		public static Texture LoadImage(Texture texture)
		{
			Texture result = LoadImage(texture.Name);
			result.Unit = texture.Unit;
			result.Name = texture.Name;
			return result;
		}
	}
}
