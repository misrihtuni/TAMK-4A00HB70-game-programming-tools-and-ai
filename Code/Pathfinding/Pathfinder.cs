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

		public IList<Cell> GetReachableCells(Cell start, int maxSteps, bool includeDiagonal)
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