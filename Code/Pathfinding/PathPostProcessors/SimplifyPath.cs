using System.Collections.Generic;
using Godot;

namespace GA.Ships.Pathfinding
{
	[GlobalClass]
	public partial class SimplifyPath : PathPostprocessorResource
	{
		public override IList<Vector3> PostProcess(IList<Vector3> path)
		{
			if (path == null || path.Count < 3)
			{
				return path;
			}

			int length = path.Count;

			Vector3 start = path[length - 1];
			Vector3 current = path[length - 2];

			Vector3 direction = (current - start).Normalized();

			for (int i = length - 3; i >= 0; i--)
			{
				current = path[i];
				Vector3 currentDirection = (current - start).Normalized();

				if (Mathf.IsEqualApprox(direction.Dot(currentDirection), 1.0f))
				{
					path.RemoveAt(i + 1);
				}
				else
				{
					start = path[i + 1];
					direction = (current - start).Normalized();
				}
			}

			return path;
		}
	}
}