using Godot;
using System.Collections.Generic;

namespace GA.Ships.Navigation
{
	public partial class PathDebugDrawer : Node3D
	{
		[Export] private Color _pathColor = new Color(1f, 0f, 0f);
		[Export] private float _segmentWidth = 0.1f;
		[Export] private float _segmentHeight = 0.18f;

		private MeshInstance3D _debugMeshInstance;
		private StandardMaterial3D _pathMaterial;
		private Node3D _pointMeshParent;

		public override void _Ready()
		{
			_pathMaterial = new StandardMaterial3D
			{
				AlbedoColor = _pathColor,
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				VertexColorUseAsAlbedo = true,
				DisableFog = true,
				CullMode = BaseMaterial3D.CullModeEnum.Back,
			};

			_debugMeshInstance = new MeshInstance3D
			{
				Mesh = new ArrayMesh(),
				MaterialOverride = _pathMaterial,
				Visible = true
			};
			AddChild(_debugMeshInstance);
			_pointMeshParent = new Node3D();
			_pointMeshParent.Name = "PointMeshParent";
			AddChild(_pointMeshParent);
		}

		public void DrawPath(IList<Vector3> path)
		{
			var tool = new SurfaceTool();
			tool.Begin(Mesh.PrimitiveType.Triangles);
			tool.SetColor(_pathColor);

			foreach (var child in _pointMeshParent.GetChildren())
			{
				child.Free();
			}

			if (path != null)
			{
				for (int i = 1; i < path.Count; i++)
				{
					var pointMesh = new MeshInstance3D()
					{
						Mesh = new BoxMesh()
						{
							Size = new Vector3(0.5f, 0.5f, 0.5f),
							Material = _pathMaterial
						},
					};
					_pointMeshParent.AddChild(pointMesh);
					pointMesh.GlobalPosition = path[i];

					Vector3 previous = ToLocal(path[i - 1]);
					Vector3 current = ToLocal(path[i]);
					AddPathSegment(tool, previous, current);
				}
			}

			_debugMeshInstance.Mesh = tool.Commit();
		}

		public void ClearPath()
		{
			_debugMeshInstance.Mesh = new ArrayMesh();
		}

		private void AddPathSegment(SurfaceTool tool, Vector3 start, Vector3 end)
		{
			Vector3 direction = end - start;
			float length = direction.Length();
			if (length <= 0.0001f)
			{
				return;
			}

			direction /= length;
			Vector3 side = Vector3.Up.Cross(direction);
			if (side.LengthSquared() < 0.0001f)
			{
				side = Vector3.Right.Cross(direction);
			}

			side = side.Normalized() * _segmentWidth;
			Vector3 normal = direction.Cross(side).Normalized() * _segmentHeight;

			Vector3 a = start + side + normal;
			Vector3 b = start - side + normal;
			Vector3 c = start - side - normal;
			Vector3 d = start + side - normal;
			Vector3 e = end + side + normal;
			Vector3 f = end - side + normal;
			Vector3 g = end - side - normal;
			Vector3 h = end + side - normal;

			AddQuad(tool, a, b, f, e);
			AddQuad(tool, b, c, g, f);
			AddQuad(tool, c, d, h, g);
			AddQuad(tool, d, a, e, h);
			AddQuad(tool, a, d, c, b);
			AddQuad(tool, e, f, g, h);
		}

		private void AddQuad(SurfaceTool tool, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			tool.AddVertex(a);
			tool.AddVertex(b);
			tool.AddVertex(c);
			tool.AddVertex(a);
			tool.AddVertex(c);
			tool.AddVertex(d);
		}
	}
}