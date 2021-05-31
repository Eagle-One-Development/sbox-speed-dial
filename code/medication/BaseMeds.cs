using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Meds
{
	[Library( "base_med", Title = "Drugs" )]
	public class BaseMedication : ModelEntity, IRespawnableEntity
	{

		public virtual string WorldModel    => "models/weapons/sk_prop_pistol_01.vmdl";
		public virtual float  rotationSpeed => 75f;
		public virtual string drugName      => "Over the Counter Medication";
		public virtual float drugDuration   => 5f;

		private Vector3 initialPosition = Vector3.Zero;


		public PickupTrigger  PickupTrigger { get; protected set; }
		private float rotationAngle;

		
		public override void Spawn()
		{
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it

			SetModel( WorldModel );

			PickupTrigger = new();
			PickupTrigger.Parent = this;
			PickupTrigger.Position = Position;
			PickupTrigger.EnableTouchPersists = true;

			
		}

		[Event("server.tick")]
		public void Simulate()
		{
			if(initialPosition == Vector3.Zero)
			{
				initialPosition = Position;
			}

			rotationAngle += rotationSpeed * Time.Delta;
			Rotation = Rotation.FromAxis( Vector3.Up, rotationAngle );

			Position = initialPosition + Vector3.Up * MathF.Sin( Time.Now * 2f) * 7f;
		}

		public virtual void PickUp()
		{
			Delete();
		}
	}
}
