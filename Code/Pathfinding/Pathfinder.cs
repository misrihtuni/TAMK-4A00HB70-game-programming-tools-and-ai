using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Cell = GA.Ships.Pathfinding.NavigationGrid.Cell;

namespace GA.Ships.Pathfinding
{
	public class Pathfinder
	{
		private NavigationGrid _grid = null;

		public Pathfinder(NavigationGrid grid)
		{
			_grid = grid;
		}

		/// <summary>
		/// Performs a breadth-first search to find a path from the start position to the end position.
		/// Link: https://en.wikipedia.org/wiki/Breadth-first_search
		/// </summary>
		/// <param name="startPosition">Start position</param>
		/// <param name="endPosition">End position</param>
		/// <returns>List of positions representing the path, or null if no path is found</returns>
		public IList<Vector3> BreadthFirstSearch(Vector3 startPosition, Vector3 endPosition)
		{
			Queue<Cell> frontier = new Queue<Cell>();
			Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();

			Cell startCell = _grid.GetCell(startPosition);
			Cell endCell = _grid.GetCell(endPosition);

			frontier.Enqueue(startCell);
			cameFrom[startCell] = null;

			bool isEndReached = false; // Early exit flag

			while (frontier.Count > 0)
			{
				Cell current = frontier.Dequeue();

				isEndReached = current == endCell;
				if (isEndReached)
				{
					// The end node is reached. Path is complete.
					break;
				}

				IList<Cell> neighbours = _grid.GetNeighbours(current, includeDiagonal: false);
				foreach (Cell neighbour in neighbours)
				{
					if (neighbour.IsWalkable && !cameFrom.ContainsKey(neighbour))
					{
						frontier.Enqueue(neighbour);
						cameFrom[neighbour] = current;
					}
				}
			}

			// If isEndReached is false here, there is no path to the end cell.
			if (isEndReached)
			{
				// Construct path
				return ConstructPath(startCell, endCell, cameFrom);
			}

			// There is no path between start and end positions.
			return null;
		}

		/// <summary>
		/// Returns a list of cells that are <paramref name="maxStep"/> steps away from the <paramref name="start"/>
		/// cell. The start cell is not counted as a step.
		/// Reference: https://www.redblobgames.com/pathfinding/tower-defense/
		/// </summary>
		///
		/// <param name="start">The cell where the search starts from.</param>
		/// <param name="maxStep">The maximum number of cells that can be traveled (start not included).</param>
		///
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="start"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxStep"/> is negative.</exception>
		public IList<Cell> GetReachableCells(Cell start, int maxStep)
		{
			if (start == null)
			{
				throw new ArgumentNullException(nameof(start));
			}

			if (maxStep < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(maxStep), "Must be greater than 0!");
			}

			Queue<Cell> frontier = new Queue<Cell>();
			Dictionary<Cell, int> distanceToStart = new Dictionary<Cell, int>();

			frontier.Enqueue(start);
			distanceToStart[start] = 0;

			while (frontier.Count > 0)
			{
				Cell current = frontier.Dequeue();
				int currentDistance = distanceToStart[current];

				if (currentDistance > maxStep)
				{
					// Cell is too far away.
					continue;
				}

				IList<Cell> neighbours = _grid.GetNeighbours(current, PathfindingConfig.AllowDiagonalPathfinding);
				foreach (Cell neighbour in neighbours)
				{
					if (neighbour.IsWalkable && !distanceToStart.ContainsKey(neighbour))
					{
						// Neighbour is walkable and has not been visited yet.
						frontier.Enqueue(neighbour);
						distanceToStart[neighbour] = currentDistance + 1;
					}
				}
			}

			return distanceToStart.Keys.ToList();
		}

		private IList<Vector3> ConstructPath(Cell startCell, Cell endCell, Dictionary<Cell, Cell> cameFrom)
		{
			IList<Vector3> path = new List<Vector3>();
			Cell current = endCell;

			while (current != startCell)
			{
				path.Add(current.WorldPosition);
				current = cameFrom[current];
			}

			path.Reverse();

			return path;
		}
	}
}