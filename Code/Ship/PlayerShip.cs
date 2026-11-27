using Godot;
using GA.Common;
namespace GA.Ships
{
	public partial class PlayerShip : ShipBase, ITarget
	{
		public Vector3 TargetPosition => GlobalPosition;
		public Inventory Inventory { get; private set; }

		public override void _Ready()
		{
			base._Ready();

			Level.Current.RegisterPlayer(this);
			Inventory = this.GetNode<Inventory>();
		}

		public override void _PhysicsProcess(double delta)
		{
			float deltaTime = (float)delta;
			Vector3 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity += GetGravity() * deltaTime;
			}

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			float movementInput = Input.GetAxis("MoveBack", "MoveForward");
			float rotationInput = Input.GetAxis("TurnRight", "TurnLeft");

			RotateY(rotationInput * Mathf.DegToRad(ShipData.RotationSpeed) * deltaTime);

			Vector3 forward = GlobalTransform.Basis.Z;
			Vector3 horizontalVelocity = new Vector3(velocity.X, 0f, velocity.Z);
			Vector3 targetVelocity = forward * (movementInput * ShipData.MovementSpeed);

			float blendSpeed = Mathf.Abs(movementInput) > 0.01f
				? ShipData.Acceleration
				: ShipData.Deceleration;

			horizontalVelocity = horizontalVelocity.Lerp(targetVelocity, blendSpeed * deltaTime);
			velocity.X = horizontalVelocity.X;
			velocity.Z = horizontalVelocity.Z;

			Velocity = velocity;
			MoveAndSlide();
		}

		public void CollectLoot(TreasureChest.Loot loot)
		{
			Inventory.AddLoot(loot);
			GD.Print(Inventory.ToString());
		}
	}
}