using Godot;
using Godot.Collections;
using System;
using GA.Common;

namespace GA.Ships.Pathfinding
{
	public partial class NavigationGrid : Node3D
	{
		/// <summary>
		/// The size of the grid in game world units (meters).
		/// </summary>
		[Export] public Vector2I GridSize { get; set; } = new Vector2I(10, 10);

		/// <summary>
		/// The cell size. Cell is always a square.
		/// </summary>
		[Export] public int CellSize { get; set; } = 1;

		[Export] public bool DrawDebugGrid { get; set; } = false;
		[Export] public bool ShowCostOverlay { get; set; } = true;

		/// <summary>
		/// Cell count horizontally.
		/// </summary>
		public int Width => GridSize.X / CellSize;

		/// <summary>
		/// Cell count vertically.
		/// </summary>
		public int Height => GridSize.Y / CellSize;

		private Cell[,] _cells;

		private MeshInstance3D _debugGridMesh;

		public override void _Ready()
		{
			// Initialize the grid;
			BuildGraph();
			RefreshDebugGrid();
		}

		private void BuildGraph()
		{
			if (CellSize <= 0)
			{
				throw new InvalidOperationException($"{nameof(CellSize)} must be greater than 0.");
			}

			if (GridSize.X <= 0 || GridSize.Y <= 0)
			{
				throw new InvalidOperationException($"{nameof(GridSize)} must be positive.");
			}

			_cells = new Cell[Width, Height];

			// The half width of the whole grid.
			float halfWidth = Width * CellSize * 0.5f;

			// The half height of the whole grid.
			float halfHeight = Height * CellSize * 0.5f;

			Vector3 origin = GlobalPosition;

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					float worldX = origin.X + ((x + 0.5f) * CellSize) - halfWidth;
					float worldZ = origin.Z + ((y + 0.5f) * CellSize) - halfHeight;

					Vector3 cellPosition = new Vector3(worldX, origin.Y, worldZ);
					int movementCost = CalculateMovementCost(cellPosition);

					_cells[x, y] = new Cell(x, y, cellPosition, movementCost);
				}
			}
		}

		private int CalculateMovementCost(Vector3 cellPosition)
		{
			BoxShape3D boxShape = new BoxShape3D
			{
				Size = new Vector3(CellSize, 1.0f, CellSize)
			};

			PhysicsShapeQueryParameters3D queryParameters = new PhysicsShapeQueryParameters3D
			{
				Shape = boxShape,
				Transform = new Transform3D(Basis.Identity, cellPosition),
				CollideWithAreas = true,
				CollideWithBodies = true
			};

			Array<Dictionary> results = GetWorld3D().DirectSpaceState.IntersectShape(queryParameters);

			int priority = int.MinValue;
			int cost = PathfindingConfig.DefaultCellCost;

			foreach (Dictionary hitData in results)
			{
				CollisionObject3D collider = (CollisionObject3D)hitData["collider"];
				if (collider == null)
				{
					continue;
				}

				NavigationObject navigationObject = collider.GetNode<NavigationObject>(recursive: true);
				if (navigationObject != null && navigationObject.Priority > priority)
				{
					priority = navigationObject.Priority;
					cost = navigationObject.MovementCost;
				}
			}

			return cost;
		}

		#region Debug draw
		private void RefreshDebugGrid()
		{
			if (_debugGridMesh == null)
			{
				_debugGridMesh = new MeshInstance3D();
				_debugGridMesh.Name = "DebugGridMesh";
				AddChild(_debugGridMesh);
			}

			if (!DrawDebugGrid || _cells == null)
			{
				_debugGridMesh.Visible = false;
				return;
			}

			_debugGridMesh.Visible = true;
			_debugGridMesh.Mesh = BuildDebugMesh();
			_debugGridMesh.MaterialOverride = CreateGridMaterial();
		}

		private ArrayMesh BuildDebugMesh()
		{
			var tool = new SurfaceTool();
			tool.Begin(Mesh.PrimitiveType.Triangles);

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					Cell cell = _cells[x, y];
					Color color = GetNodeColor(cell.Cost, cell.IsWalkable);
					float half = CellSize * 0.5f;
					Vector3 offset = cell.WorldPosition - GlobalPosition;
					Vector3 a = new Vector3(offset.X - half, 0.02f, offset.Z - half);
					Vector3 b = new Vector3(offset.X + half, 0.02f, offset.Z - half);
					Vector3 c = new Vector3(offset.X + half, 0.02f, offset.Z + half);
					Vector3 d = new Vector3(offset.X - half, 0.02f, offset.Z + half);

					tool.SetColor(color);
					tool.AddVertex(a);
					tool.AddVertex(b);
					tool.AddVertex(c);
					tool.AddVertex(a);
					tool.AddVertex(c);
					tool.AddVertex(d);
				}
			}

			return tool.Commit() as ArrayMesh;
		}

		private StandardMaterial3D CreateGridMaterial()
		{
			var material = new StandardMaterial3D
			{
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				VertexColorUseAsAlbedo = true,
				CullMode = BaseMaterial3D.CullModeEnum.Back,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				AlphaAntialiasingMode = BaseMaterial3D.AlphaAntiAliasing.Off,
				DisableFog = true
			};
			return material;
		}

		private Color GetNodeColor(int cost, bool isWalkable)
		{
			if (!isWalkable)
			{
				return new Color(0.8f, 0.15f, 0.15f, 0.75f);
			}

			if (!ShowCostOverlay)
			{
				return new Color(0.1f, 0.8f, 0.2f, 0.65f);
			}

			float t = Mathf.Clamp(cost / 10f, 0f, 1f);
			return new Color(0, 1.0f - t, t, 0.75f);
		}
		#endregion

		/// <summary>
		/// Represents one cell in the grid graph.
		/// </summary>
		public class Cell : IComparable<Cell>
		{
			public int X { get; }
			public int Y { get; }
			public Vector3 WorldPosition { get; }
			public int Cost { get; set; }
			public bool IsWalkable => Cost >= 0;

			public Cell(int x, int y, Vector3 worldPosition, int cost)
			{
				X = x;
				Y = y;
				WorldPosition = worldPosition;
				Cost = cost;
			}

			public int CompareTo(Cell other)
			{
				throw new NotImplementedException();
			}
		}
	}
}