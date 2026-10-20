using System;
using Godot;

namespace GA.Ships.Pathfinding.Testing
{
	public partial class PathfindingDistance : Node3D
	{
		[Signal] public delegate void ConfigurationChangedEventHandler();

		private int _distance = 5;
		private bool _includeDiagonals = false;
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
			var cells = Level.Current.Pathfinder.GetReachableCells(startCell, _distance, _includeDiagonals);
			Level.Current.Grid.SetHighlightedCells(cells);
		}
	}
}