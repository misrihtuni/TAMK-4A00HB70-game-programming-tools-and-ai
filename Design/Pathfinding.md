# Pathfinding

## Different Area Types and Traversal Costs

| Water Type     | Descriprion                                      | Cost |
| -------------- | ------------------------------------------------ | ---- |
| Calm Water     | Most common type, easiest to travel through.     | 1    |
| Light Current  | Makes controlling the ship a little bit harder.  | 5    |
| Strong Current | Makes controlling the ship significantly harder. | 9    |
| Rocky Water    | Poses a risk of physical damage to the ship.     | 20   |
| Islands        | Non-navigable.                                   | -1   |

## Detecting Different Area Types

The game uses a custom `NavigationGrid` for pathfinding purposes. The grid is
made of `Cell` objects that have a record of their world position and grid
position. Since the grid is created dynamically when the level scene is loaded,
the cost must be determined automatically from the objects in the level scene.
To do this, we can use the `CollisionShape3D` node.

When the `_Ready` method of the level is executed, we start by creating a
cuboid-shaped `CollisionShape3D` whose base has the same size as one cell on
the grid, and the height is set to something reasonably high so

- Area3D nodes are placed in the world to represent different water types.
- Each water type has its own collision layer.

```csharp
private void BuildGrid()
{
	// Existing code.

	// Create a seeker object that will check collisions with all the water
	// type layers.
	Area3D seeker = new Area3D()
	{
		CollisionLayer = 0,
		CollisionMask = WATER_TYPE_LAYER_MASKS,
	};
	// Add collision shape to the seeker and set its size based on CellSize.
	seeker.AddChild(
		new CollisionShape3D()
		{
			Shape = new BoxShape3D()
			{
				Size = new Vector3I(CellSize, 10, CellSize)
			}
		}
	);
	// Add the seeker to the node tree.
	AddChild(seeker);

	// Existing code.

	for (int y = 0; y < Height; y++)
	{
		for (int x = 0; x < Width; x++)
		{
			float worldX = origin.X + ((x + 0.5f) * CellSize) - halfWidth;
			float worldY = origin.Z + ((y + 0.5f) * CellSize) - halfHeight;

			seeker.GlobalPosition = GetCellCornerWorldPosition();
			int cost = 1;

			// Check if islands overlap with the seeker.
			foreach (Node3D node in seeker.GetOverlappingBodies())
			{
				if (node is INavigationArea island)
				{
					cost = -1;
				}
			}

			// Iterate through areas that overlap with the seeker and select
			// the highest cost if multiple different areas are found.
			foreach (Area3D area in seeker.GetOverlappingAreas())
			{
				INavigationArea waterArea = (INavigationArea)area;
				if (waterArea.TraversalCost > cost)
				{
					cost = waterArea.TraversalCost;
				}
			}



			_cells[x, y] = new Cell(x, y, new Vector3(worldX, origin.Y, worldY), cost);
		}
	}
}
```

1. Create a cuboid seeker object (Area3D) in BuildGrid.
   - Bottom size is the same as for one cell.
2.
