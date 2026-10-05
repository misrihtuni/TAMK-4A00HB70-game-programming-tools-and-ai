using System.Collections.Generic;
using Godot;

namespace GA.Common
{
	public static class NodeExtensions
	{
		/// <summary>
		/// Finds the first '<see cref="Node"/>' of type '<typeparamref name="T"/>' in the node tree of '<paramref name="node"/>'.
		/// </summary>
		/// <typeparam name="T">The type of the node to look for.</typeparam>
		/// <param name="node">The root node of the search.</param>
		/// <param name="recursive">If the search should include children and their children etc.</param>
		/// <returns>The first node of type T. If no such node is found, returns null.</returns>
		public static T GetNode<T>(this Node node, bool recursive = true) where T : Node
		{
			int childCount = node.GetChildCount();

			for (int i = 0; i < childCount; i++)
			{
				Node child = node.GetChild(i);

				if (child is T result)
				{
					return result;
				}

				if (recursive && child.GetChildCount() > 0)
				{
					T recursiveResult = GetNode<T>(child, recursive);
					if (recursiveResult != null)
					{
						return recursiveResult;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Returns a list containing all child nodes (see remarks) of type <typeparamref name="T"/> in the node tree
		/// of the <paramref name="node"/>.
		/// </summary>
		///
		/// <typeparam name="T">Type of the node to search for.</typeparam>
		/// <param name="node">The root node of the search.</param>
		/// <param name="recursive">If the search should look for matches under child nodes and their child nodes.</param>
		///
		/// <remarks>
		/// Recursive search is used by default meaning that all children in the node tree are checked.
		/// To only include the immediate children of <paramref name="node"/> in the search, set
		/// <paramref name="recursive"/> to <c>false</c>.
		/// </remarks>
		public static IList<T> GetChildren<T>(this Node node, bool recursive = true) where T : Node
		{
			List<T> children = new List<T>();

			int childCount = node.GetChildCount();
			for (int i = 0; i < childCount; i++)
			{
				Node child = node.GetChild(i);

				if (child is T result)
				{
					children.Add(result);
				}

				if (recursive && child.GetChildCount() > 0)
				{
					children.AddRange(GetChildren<T>(child, recursive));
				}
			}

			return children;
		}
	}
}