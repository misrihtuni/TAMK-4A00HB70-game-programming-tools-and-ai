using Godot;
using System;

namespace GA.Ships.Pathfinding
{
	public partial class NavigationObject : Node3D
	{
		/// <summary>
		/// The movement cost of the Node, which is located at the position of this NavigationObject.
		/// Higher movement costs will make the Node less desirable for pathfinding.
		/// A negative movement cost is interpreted as an impassable Node, and will be ignored by the pathfinding algorithm.
		/// </summary>
		[Export] public int MovementCost { get; set; } = 1;

		/// <summary>
		/// The priority of this navigation object. Higher priority objects will be
		/// preferred over lower priority ones when determining the cost of a Node.
		/// </summary>
		[Export] public int Priority { get; set; } = 0;

		/// <summary>
		/// Indicates whether the Node at this NavigationObject's position is walkable
		/// (i.e., has a non-negative movement cost).
		/// </summary>
		public bool IsWalkable => MovementCost >= 0;
	}
}