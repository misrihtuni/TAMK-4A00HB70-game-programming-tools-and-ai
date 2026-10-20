using System;
using System.Collections.Generic;
using Godot;

namespace GA.Ships.Pathfinding.Testing
{
	public partial class PathfindingDistance : Node3D
	{
		[Signal] public delegate void ConfigurationChangedEventHandler();

		private enum FloodFillType
		{
			BreadthFirstSearch,
			Dijkstra,
		}

		private int _distance = 5;
		private bool _includeDiagonals = false;
		private FloodFillType _algorithm = FloodFillType.BreadthFirstSearch;
		private Vector3 _lastPosition;

		[Export]
		private int Distance
		{
			get => _distance;
			set
			{
				_distance = value;
				EmitSignal(SignalName.ConfigurationChanged);
			}
		}

		[Export]
		private bool IncludeDiagonals
		{
			get => _includeDiagonals;
			set
			{
				_includeDiagonals = value;
				EmitSignal(SignalName.ConfigurationChanged);
			}
		}

		[Export]
		private FloodFillType Algorith
		{
			get => _algorithm;
			set
			{
				_algorithm = value;
				EmitSignal(SignalName.ConfigurationChanged);
			}
		}

		public override void _Ready()
		{
			ConfigurationChanged += PerformDistanceTest;
			CallDeferred(nameof(PerformDistanceTest));
		}

		public override void _PhysicsProcess(double delta)
		{
			if (!_lastPosition.IsEqualApprox(GlobalPosition))
			{
				_lastPosition = GlobalPosition;
				PerformDistanceTest();
			}
		}

		private void PerformDistanceTest()
		{
			NavigationGrid.Cell startCell = Level.Current.Grid.GetCell(GlobalPosition);
			IList<NavigationGrid.Cell> cells = null;
			switch (_algorithm)
			{
				case FloodFillType.BreadthFirstSearch:
					cells = Level.Current.Pathfinder.GetReachableCellsByMaxDistance(startCell, _distance, _includeDiagonals);
					break;
				case FloodFillType.Dijkstra:
					cells = Level.Current.Pathfinder.GetReachableCellsByMaxTotalCost(startCell, _distance, _includeDiagonals);
					break;
				default:
					GD.PushError("Unknown algorith selected for distande test.");
					return;
			}
			Level.Current.Grid.SetHighlightedCells(cells);
		}
	}
}