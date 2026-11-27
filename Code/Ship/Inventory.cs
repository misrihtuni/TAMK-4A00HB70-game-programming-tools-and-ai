using System;
using System.Collections.Generic;
using Godot;

namespace GA.Ships
{
	public partial class Inventory : Node
	{
		private Dictionary<TreasureChest.Loot, uint> _amounts
			= new Dictionary<TreasureChest.Loot, uint>();

		public void AddLoot(TreasureChest.Loot loot)
		{
			TreasureChest.Loot knownLoot = TreasureChest.Loot.None;
			TreasureChest.Loot[] allLootTypes = Enum.GetValues<TreasureChest.Loot>();

			foreach (var lootType in allLootTypes)
			{
				if (IsSingleLootType(lootType))
				{
					// Sets the bit of the lootType (a single loot type) on to the knownLoot variable.
					knownLoot |= lootType;
				}
			}

			// The operator ~ is the bitwise NOT operator, which flips all bits in the value.
			// So ~knownLoot will have bits set for all loot types that are not known.
			// AND between the provided loot and ~knownLoot will yield a non-zero value if there are
			// any unknown loot types in the provided loot.
			if ((loot & ~knownLoot) != TreasureChest.Loot.None)
			{
				throw new ArgumentOutOfRangeException(nameof(loot), loot, "Loot contains unknown flags.");
			}

			foreach (var type in allLootTypes)
			{
				// The check (loot & type) != TreasureChest.Loot.None checks if the specific loot type is present in the provided loot.
				// Remember, since loot is a bitmask, multiple loot types can be represented in a single value.
				// The bitwise AND operation will yield a non-zero value if the specific loot type is present.
				if (IsSingleLootType(type) && (loot & type) != TreasureChest.Loot.None)
				{
					_amounts.TryGetValue(type, out uint amount);
					_amounts[type] = amount + 1;
				}
			}
		}

		public uint GetAmount(TreasureChest.Loot type)
		{
			if (!IsSingleLootType(type) || !Enum.IsDefined(type))
			{
				throw new ArgumentOutOfRangeException(nameof(type), type, "Expected a single loot type.");
			}

			return _amounts.TryGetValue(type, out uint amount) ? amount : 0;
		}

		public override string ToString()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder("Inventory:");
			foreach (TreasureChest.Loot type in Enum.GetValues<TreasureChest.Loot>())
			{
				if (IsSingleLootType(type))
				{
					builder.Append($"\n  {type}: {GetAmount(type)}");
				}
			}

			return builder.ToString();
		}

		private static bool IsSingleLootType(TreasureChest.Loot lootType)
		{
			// If the type represents a single loot type, it will be a power of two (1, 2, 4, 8, ...).
			// A power of two has exactly one bit set, so if we subtract 1 from it, all the bits to the right of that bit will be set,
			// and the bit itself will be cleared. Think for example of 8 (1000 in binary). 8 - 1 = 7 (0111 in binary).
			// If we perform a bitwise AND between 8 and 7, we get 0 (1000 & 0111 = 0000).
			// However, if the type was not of a single loot type (e.g., 3, which is 0011 in binary),
			// subtracting 1 would give us 2 (0010 in binary), and the bitwise AND would not be zero (0011 & 0010 = 0010).
			uint value = (uint)lootType;
			return value != 0 && (value & (value - 1)) == 0;
		}
	}
}