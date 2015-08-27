using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d
{
	public class Camera
	{
		private Vector3 position = Vector3.Zero;
		public Vector3 Position
		{
			get { return position; }
			set { position = value; }
		}

		private Vector3 orientation = new Vector3((float)Math.PI, 0f, 0f);
		public Vector3 Orientation
		{
			get { return orientation; }
			set { orientation = value; }
		}
		
		private float moveSpeed = 0.2f;
		public float MoveSpeed
		{
			get { return moveSpeed; }
			set { moveSpeed = value; }
		}

		private float mouseSensitivity = 0.01f;
		public float MouseSensitivity
		{
			get { return mouseSensitivity; }
			set { mouseSensitivity = value; }
		}

		public float FieldOfViewY = 1.3f;
		public float zNear = 0.1f;
		public float zFar = 40f;

		public Matrix4 ViewMatrix;
		public Matrix4 PerspectiveMatrix;
		
		public void Update(int width, int height)
		{
			ViewMatrix = CalculateViewMatrix();
			PerspectiveMatrix = CalculatePerspectiveMatrix(width, height);
		}

		public Matrix4 CalculateViewMatrix()
		{
			Vector3 lookat = new Vector3();

			lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
			lookat.Y = (float)Math.Sin((float)Orientation.Y);
			lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));

			return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
		}

		public Matrix4 CalculatePerspectiveMatrix(int width, int height)
		{
			float aspect = width / (float)height;
			Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView
				(
					FieldOfViewY,
					aspect,
					zNear,
					zFar
				);
			return CalculateViewMatrix() * perspective;
		}

		public void Move(float x, float y, float z)
		{
			Vector3 offset = new Vector3();

			Vector3 forward = new Vector3((float)Math.Sin((float)Orientation.X), 0, (float)Math.Cos((float)Orientation.X));
			Vector3 right = new Vector3(-forward.Z, 0, forward.X);

			offset += x * right;
			offset += y * forward;
			offset.Y += z;

			offset.NormalizeFast();
			offset = Vector3.Multiply(offset, MoveSpeed);

			Position += offset;
		}

		public void AddRotation(float x, float y)
		{
			x = x * MouseSensitivity;
			y = y * MouseSensitivity;

			Vector3 o = Orientation;

			o.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
			o.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);

			Orientation = o;
		}
	}
}
