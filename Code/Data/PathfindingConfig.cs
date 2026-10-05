namespace GA.Ships.Pathfinding
{
	public static class PathfindingConfig
	{
		/// <summary>
		/// The default movement cost for cells in the <see cref="NavigationGrid"/>.
		/// </summary>
		public const int DefaultCellCost = 1;

		/// <summary>
		/// Whether or not diagonal pathfinding should be allowed.
		/// </summary>
		public const bool AllowDiagonalPathfinding = false;
	}
}