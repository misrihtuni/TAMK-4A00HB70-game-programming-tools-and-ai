using Godot;
using System;
using GA.Common;

namespace GA.Ships
{
	public partial class TreasureChest : Area3D
	{
		[Flags]
		public enum Loot : uint
		{
			None = 0,
			Rum = 1 << 0,         // 0b00000001 eli 1
			Gold = 1 << 1,        // 0b00000010 eli 2
			Cannonball = 1 << 2,  // 0b00000100 eli 4
			WoodenLeg = 1 << 3,   // 0b00001000 eli 8
			Mold = 1 << 4,        // 0b00010000 eli 16
			Skull = 1 << 5,       // 0b00100000 eli 32
			Bones = 1 << 6,       // 0b01000000 eli 64
			SkullAndBones = Skull | Bones,
			All = ~None
		}

		[Export] private Loot _containedLoot = Loot.None;

		private AnimationPlayer _animationPlayer = null;
		private CollisionShape3D _collisionShape = null;

		public override void _Ready()
		{
			_animationPlayer = this.GetNode<AnimationPlayer>(true);
			_collisionShape = this.GetNode<CollisionShape3D>(true);
		}

		public override void _EnterTree()
		{
			BodyEntered += OnBodyEntered;
		}

		public override void _ExitTree()
		{
			BodyEntered -= OnBodyEntered;
		}

		private void OnBodyEntered(Node3D body)
		{
			if (body is PlayerShip playerShip)
			{
				CollectLoot(playerShip);
			}
		}

		private async void CollectLoot(PlayerShip playerShip)
		{
			if (_collisionShape != null)
			{
				_collisionShape.Disabled = true;
			}

			playerShip.CollectLoot(_containedLoot);

			if (_animationPlayer != null)
			{
				_animationPlayer.Play("open-close");
				await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
			}

			// Destroy the treasure chest after the animation has finished playing
			// or immediately if animation player is not available.
			QueueFree();
		}
	}
}