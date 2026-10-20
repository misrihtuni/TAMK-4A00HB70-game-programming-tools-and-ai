using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GA.Collections;
using Cell = GA.Ships.Pathfinding.NavigationGrid.Cell;

namespace GA.Ships.Pathfinding
{
	public class Pathfinder
	{
		private class FrontierEntry : IComparable<FrontierEntry>
		{
			public Cell Cell { get; }
			public int TotalCost { get; }

			public FrontierEntry(Cell cell, int totalCost)
			{
				Cell = cell;
				TotalCost = totalCost;
			}

			public int CompareTo(FrontierEntry other)
			{
				return other == null ? -1 : TotalCost.CompareTo(other.TotalCost);
			}
		}

		private NavigationGrid _grid = null;

		// Cells, which will be inspected.
		private PriorityQueue<FrontierEntry> _frontier = new PriorityQueue<FrontierEntry>();

		// Cells which has been inspected already.
		private HashSet<Cell> _visited = new HashSet<Cell>();

		public Pathfinder(NavigationGrid grid)
		{
			_grid = grid;
		}

		#region Breadth-First Search
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

		[Obsolete("Use GetReachableCellsByMaxDistance")]
		public IList<Cell> GetReachableCells(Cell start, int maxSteps, bool includeDiagonal) =>
			GetReachableCellsByMaxDistance(start, maxSteps, includeDiagonal);

		/// <summary>
		/// Finds all the <see cref="Cell"/>s that are within
		/// <paramref name="maxSteps"/> cells away from the
		/// <paramref name="start"/> cell. This method does not take movement
		/// cost into account.
		/// </summary>
		///
		/// <param name="start">The cell where the search flood-fill starts from.</param>
		/// <param name="maxSteps">The maximum distance (in cells) from the <paramref name="start"/> cell.</param>
		/// <param name="includeDiagonal">Whether diagonal movement should be allowed or not.</param>
		///
		/// <returns>A list containing all the reachable cells.</returns>
		///
		/// <remarks>
		/// - This method is a simple flood-fill algorithm based on breadth-first search.<br/>
		/// - Allowing diagonal movement results in a square-shaped area of accepted cells.
		/// </remarks>
		public IList<Cell> GetReachableCellsByMaxDistance(Cell start, int maxSteps, bool includeDiagonal)
		{
			Queue<Cell> frontier = new Queue<Cell>();
			Dictionary<Cell, (Cell, int)> cameFrom = new Dictionary<Cell, (Cell, int)>();

			frontier.Enqueue(start);
			cameFrom[start] = (null, 0);

			bool isEndReached = false;

			while (frontier.Count > 0)
			{
				Cell current = frontier.Dequeue();
				int distance = cameFrom[current].Item2;

				isEndReached = distance >= maxSteps;
				if (isEndReached)
				{
					// Max. distance is reached. Path is complete.
					break;
				}

				IList<Cell> neighbours = _grid.GetNeighbours(current, includeDiagonal: includeDiagonal);
				foreach (Cell neighbour in neighbours)
				{
					if (neighbour.IsWalkable && !cameFrom.ContainsKey(neighbour))
					{
						frontier.Enqueue(neighbour);
						cameFrom[neighbour] = (current, distance + 1);
					}
				}
			}

			HashSet<Cell> validCells = new HashSet<Cell>();
			foreach (var kvp in cameFrom)
			{
				if (kvp.Key != start)
				{
					validCells.Add(kvp.Key);
				}
			}

			return validCells.ToList();
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

		#endregion

		#region Dijkstra's Algorithm
		/// <summary>
		/// Performs Dijkstra's algorithm to find the shortest path from the start position to the end position.
		/// Link: https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
		/// </summary>
		/// <param name="startPosition">Start position</param>
		/// <param name="endPosition">End position</param>
		/// <returns>List of nodes representing the path, or null if no path is found</returns>
		public IList<Vector3> Dijkstra(Vector3 startPosition, Vector3 endPosition)
		{
			Cell startCell = _grid.GetCell(startPosition);
			Cell endCell = _grid.GetCell(endPosition);

			if (startCell == null || endCell == null || startCell == endCell ||
				!startCell.IsWalkable || !endCell.IsWalkable)
			{
				// Early exit in case there is no valid path possible.
				return null;
			}

			_frontier.Clear();
			_visited.Clear();
			_grid.ResetPathData();

			startCell.Parent = null;
			startCell.GCost = 0; // At the beginning the cost is 0. We haven't travelled anywhere yet.
			startCell.HCost = 0; // Has to be zeroed if A* was used between two Dijkstra calls.

			_frontier.Enqueue(new FrontierEntry(startCell, 0));

			while (_frontier.Count > 0)
			{
				FrontierEntry entry = _frontier.Dequeue();
				Cell current = entry.Cell;

				if (!_visited.Add(current))
				{
					// Outdated entry. The cell was already processed through a cheaper route.
					continue;
				}

				if (current == endCell)
				{
					// Early exit.
					// We have reached the end node. No need to continue.
					break;
				}

				IList<Cell> neighbours = _grid.GetNeighbours(current, PathfindingConfig.AllowDiagonalPathfinding);
				foreach (Cell neighbour in neighbours)
				{
					if (!neighbour.IsWalkable || _visited.Contains(neighbour))
					{
						// Skip this neighbour if it's not walkable or if it has been already visited.
						continue;
					}

					int costToNeighbour = _grid.GetCostToNeighbour(current, neighbour);
					if (costToNeighbour <= 0)
					{
						// The neighbour is not a neighbour of the current node.
						GD.PrintErr("Invalid cost to the neighbour! Did GetNeighbours return a Node " +
											"which is not a neighbour?");
						continue;
					}

					// The total cost of the path so far.
					int costSoFar = current.GCost + costToNeighbour;
					if (costSoFar < neighbour.GCost) // or there is a better path to the neighbour.
					{
						// Update the cost to the neighbour from current node
						neighbour.GCost = costSoFar;
						neighbour.HCost = 0;

						// Add to the frontier in order to process its neighbours.
						_frontier.Enqueue(new FrontierEntry(neighbour, costSoFar));

						// It's cheapest to navigate to this neighbour from the current node.
						neighbour.Parent = current;
					}
				}
			}

			return RetracePath(startCell, endCell);
		}

		/// <summary>
		/// Finds all the <see cref="Cell"/>s that are reachable using no more
		/// than <paramref name="maxMovementPoints"/> points (i.e., the
		/// movement cost is taken into account). This method uses Dijkstra's
		/// algorithm as its base.
		/// </summary>
		///
		/// <param name="start">The cell where the flood-fill starts from.</param>
		/// <param name="maxMovementPoints">The maximum number of points that can be used to reach a specific cell.</param>
		/// <param name="useDiagonal">Whether or not diagonal movement is allowed.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="start"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="maxMovementPoints"/> is less than 1.</exception>
		public IList<Cell> GetReachableCellsByMaxTotalCost(Cell start, int maxMovementPoints, bool useDiagonal)
		{
			if (start == null)
			{
				throw new ArgumentNullException(nameof(start));
			}

			if (maxMovementPoints < 0)
			{
				throw new ArgumentOutOfRangeException($"{nameof(maxMovementPoints)} must be non-negative");
			}

			Dictionary<Cell, float> movementPointsUsed = new Dictionary<Cell, float>();

			_frontier.Clear();
			_visited.Clear();
			_grid.ResetPathData();

			start.Parent = null; // Didn't come to start from anywhere.
			start.GCost = 0; // First cell, no points used yet.
			start.HCost = 0; // Clear in case A* was used earlier.

			_frontier.Enqueue(new FrontierEntry(start, 0));

			while (_frontier.Count > 0)
			{
				FrontierEntry entry = _frontier.Dequeue();
				Cell current = entry.Cell;

				// Skip current if it has already been processed.
				if (!_visited.Add(current))
				{
					continue;
				}

				movementPointsUsed[current] = current.GCost;

				// Check neighbouring cells.
				IList<Cell> neighbours = _grid.GetNeighbours(current, useDiagonal);
				foreach (Cell neighbour in neighbours)
				{
					// Skip non-accessible and visited cells.
					if (!neighbour.IsWalkable || _visited.Contains(neighbour))
					{
						continue;
					}

					int costToNeighbour = _grid.GetCostToNeighbour(current, neighbour);
					if (costToNeighbour <= 0)
					{
						GD.PrintErr("Cost to neighbour was non-positive. Is the cell really a neighbour?");
						continue;
					}

					int costSoFar = current.GCost + costToNeighbour;

					// maxMovementPoints is multiplied by 10 because diagonal
					// distance is approx 1.4 times the axial distance and
					// we're using ints instead of floats.
					if (costSoFar > maxMovementPoints * 10)
					{
						// There are not enough movement points left to move to
						// the neighbour.
						continue;
					}

					if (costSoFar < neighbour.GCost)
					{
						// Moving to the neighbour from the current cell is
						// cheaper than previously discovered route(s).

						neighbour.GCost = costSoFar;
						neighbour.HCost = 0;
						neighbour.Parent = current;

						_frontier.Enqueue(new FrontierEntry(neighbour, costSoFar));
					}
				}
			}

			return movementPointsUsed.Keys.ToList();
		}
		#endregion

		#region A*
		public IList<Vector3> AStar(Vector3 startPosition, Vector3 endPosition)
		{
			Cell startCell = _grid.GetCell(startPosition);
			Cell endCell = _grid.GetCell(endPosition);

			if (startCell == null || endCell == null || startCell == endCell ||
				!startCell.IsWalkable || !endCell.IsWalkable)
			{
				// Early exit in case there is no valid path possible.
				return null;
			}

			_frontier.Clear();
			_visited.Clear();
			_grid.ResetPathData();

			startCell.Parent = null;
			startCell.GCost = 0;
			startCell.HCost = GetEstimatedCost(startCell, endCell);

			_frontier.Enqueue(new FrontierEntry(startCell, startCell.FCost));

			while (_frontier.Count > 0)
			{
				FrontierEntry entry = _frontier.Dequeue();
				Cell current = entry.Cell;

				if (!_visited.Add(current))
				{
					// Outdated entry. The cell was already processed through a cheaper route.
					continue;
				}

				if (current == endCell)
				{
					// Early exit.
					// We have reached the end node. No need to continue.
					break;
				}

				IList<Cell> neighbours = _grid.GetNeighbours(current, PathfindingConfig.AllowDiagonalPathfinding);
				foreach (Cell neighbour in neighbours)
				{
					if (!neighbour.IsWalkable || _visited.Contains(neighbour))
					{
						// Skip this neighbour if it's not walkable or if it has been already visited.
						continue;
					}

					int costToNeighbour = _grid.GetCostToNeighbour(current, neighbour);
					if (costToNeighbour <= 0)
					{
						// GetCostToNeighbour returns a negative value when the cells are not neighbours.
						GD.PrintErr("Invalid cost to the neighbour! Did GetNeighbours return a Node " +
											"which is not a neighbour?");
						continue;
					}

					int costSoFar = current.GCost + costToNeighbour;
					if (costSoFar < neighbour.GCost) // Not reached yet (int.MaxValue) or a cheaper path was found.
					{
						neighbour.GCost = costSoFar;
						neighbour.HCost = GetEstimatedCost(neighbour, endCell);
						neighbour.Parent = current;

						_frontier.Enqueue(new FrontierEntry(neighbour, neighbour.FCost));
					}
				}
			}

			return RetracePath(startCell, endCell);
		}

#pragma warning disable CS0162
		private int GetEstimatedCost(Cell startCell, Cell endCell)
		{
			if (PathfindingConfig.AllowDiagonalPathfinding)
			{
				return GetEstimatedDiagonalDistance(startCell, endCell);
			}
			return GetEstimatedAxialDistance(startCell, endCell);
		}
#pragma warning restore CS0162

		private int GetEstimatedDiagonalDistance(Cell startCell, Cell endCell)
		{
			int xDist = Mathf.Abs(startCell.X - endCell.X);
			int yDist = Mathf.Abs(startCell.Y - endCell.Y);

			int diagonalSteps = Mathf.Min(xDist, yDist);
			int axialSteps = Mathf.Max(xDist, yDist) - diagonalSteps;

			return diagonalSteps * 14 + axialSteps * 10;
		}

		private int GetEstimatedAxialDistance(Cell startCell, Cell endCell)
		{
			// Manhattan distance
			return (Mathf.Abs(startCell.X - endCell.X) + Mathf.Abs(startCell.Y - endCell.Y)) * 10;
		}
		#endregion

		#region Common
		private IList<Vector3> RetracePath(Cell startCell, Cell endCell)
		{
			IList<Vector3> path = new List<Vector3>();

			Cell current = endCell;
			bool isValid = true;

			while (current != startCell && (isValid = current != null))
			{
				path.Add(current.WorldPosition);
				current = current.Parent;
			}

			if (!isValid)
			{
				// The path is invalid. The end node was not reached.
				return null;
			}

			path.Reverse();

			return path;
		}
		#endregion
	}
}