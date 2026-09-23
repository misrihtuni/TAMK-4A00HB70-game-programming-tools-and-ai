using System.Collections.Generic;
using Godot;

namespace GA.Common
{
	public static class NodeExtensions
	{
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