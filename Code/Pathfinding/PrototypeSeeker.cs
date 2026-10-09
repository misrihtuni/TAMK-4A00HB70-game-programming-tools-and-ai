using Godot;
using System.Collections.Generic;
using GA.Common;
using System.Diagnostics;
using System;
using GA.Ships.Pathfinding;

namespace GA.Ships.Navigation
{
	public enum PathfinderAlgorithm
	{
		None = 0,
		BreadthFirstSearch,
		Dijkstra,
		AStar
	}

	public partial class PrototypeSeeker : Node3D
	{
		[Export] private PathfinderAlgorithm _algorithm = PathfinderAlgorithm.None;
		[Export] Node3D _targetNode = null;
		[Export] private float _pathfinderTimeStep = 1.0f;
		[Export] private PathPostprocessorResource[] _postProcessors = null;

		private IList<Vector3> _currentPath = null;
		private PathDebugDrawer _pathDebugDrawer = null;

		private float _pathfinderTimer = 0.0f;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_pathfinderTimer = _pathfinderTimeStep;
			_pathDebugDrawer = this.GetNode<PathDebugDrawer>();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			_pathfinderTimer -= (float)delta;
			if (_pathfinderTimer <= 0.0f)
			{
				_pathfinderTimer = _pathfinderTimeStep;
				// Perform pathfinding logic here
				PerformPathfinding();

				_pathDebugDrawer.DrawPath(_currentPath);
			}
		}


		private void PerformPathfinding()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			switch (_algorithm)
			{
				case PathfinderAlgorithm.BreadthFirstSearch:
					// Call the Breadth First Search method here
					_currentPath = Level.Current.Pathfinder.BreadthFirstSearch(GlobalPosition, _targetNode.GlobalPosition);
					break;
				case PathfinderAlgorithm.Dijkstra:
					// Call the Dijkstra method here
					_currentPath = Level.Current.Pathfinder.Dijkstra(GlobalPosition, _targetNode.GlobalPosition);
					break;
				case PathfinderAlgorithm.AStar:
					// Call the A* method here
					_currentPath = Level.Current.Pathfinder.AStar(GlobalPosition, _targetNode.GlobalPosition);
					break;
				default:
					GD.Print("No pathfinding algorithm selected.");
					break;
			}

			ProsessPath();

			stopwatch.Stop();
			GD.Print($"Pathfinding took {stopwatch.ElapsedMilliseconds} ms");
		}

		private void ProsessPath()
		{
			foreach (var postProcessor in _postProcessors)
			{
				_currentPath = postProcessor.PostProcess(_currentPath);
			}
		}
	}
}