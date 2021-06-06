using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Weapons {
	[Library("sd_bat", Title = "Baseball Bat")]
	partial class BaseballBat : BaseSpeedDialWeapon {
		public override float PrimaryRate => 2.0f;
		public override int HoldType => 5; // need melee holdtype
		public override int ClipSize => -1; // no ammo hud
		public override string WorldModel => "models/weapons/bat/bat.vmdl";
		public override string AttachementName => "melee_swing_attach";

		public override void AttackPrimary(bool _ = false, bool __ = false) {
			(Owner as AnimEntity).SetAnimBool("b_attack", true);
			using(Prediction.Off()) {
				if(IsServer) {
					PlaySound("woosh");
				}
			}
			// TODO (gurke) | no touchy josh, I wanna do it
			// 
			// multiple traces for a sweeping attack
			// I wanna be able to BONK multiple people
		}

		public override void Spawn() {

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs(CollisionLayer.Debris); // so player movement doesn't walk into it
			SetModel(WorldModel);
			AmmoClip = ClipSize;
			PickupTrigger = new();
			PickupTrigger.Parent = this;
			PickupTrigger.Position = Position;
			PickupTrigger.EnableTouchPersists = true;

			MoveType = MoveType.Physics;
			//CollisionGroup = CollisionGroup.;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public override void Touch(Entity other) {
			base.Touch(other);
			//Log.Info($"BASEBALL BAT TOUCHED {other}");
		}
	}
}
