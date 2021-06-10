using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Weapons {
	[Library("sd_bat", Title = "Baseball Bat")]
	partial class BaseballBat : BaseSpeedDialWeapon {
		public override float PrimaryRate => 2.0f;
		public override int HoldType => 6; // need melee holdtype
		public override int ClipSize => -1; // no ammo hud
		public override string WorldModel => "models/weapons/bat/bat.vmdl";
		public override string AttachementName => "melee_bat_attach";

		[Net, Predicted, Local]
		public bool Hitting { get; set; } = false;

		[Net, Predicted, Local]
		public TimeSince TimeSinceSwing { get; set; }

		public MeleeTrigger MeleeTrigger;

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

			MeleeTrigger = new();
			MeleeTrigger.SetupTrigger(EffectEntity.GetAttachment("melee_start", true).Position, EffectEntity.GetAttachment("melee_end", true).Position);
			MeleeTrigger.Parent = this;
			MeleeTrigger.Position = Position;
			MeleeTrigger.EnableTouchPersists = true;

			MeleeTrigger.EnableTouch = false;

			MoveType = MoveType.Physics;
			//CollisionGroup = CollisionGroup.;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public override void Simulate(Client owner) {
			base.Simulate(owner);
		}

		public override void Touch(Entity other) {
			base.Touch(other);
			if(Owner != null)
				Log.Info($"BASEBALL BAT TOUCHED {other} WITH OWNER");
		}

		public override void OnCarryStart(Entity carrier) {
			if(IsClient) return;

			//spawned via a weaponspawn. Tell the spawn that it's cleared up and can start respawning the weapon
			if(WeaponSpawn != null) {
				WeaponSpawn.ItemTaken = true;
				WeaponSpawn.TimeSinceTaken = 0;
				WeaponSpawn = null;
			}

			SetParent(carrier, AttachementName, Transform.Zero);

			//EffectEntity.EnableAllCollisions = true;

			Owner = carrier;
			MoveType = MoveType.None;
			EnableAllCollisions = false;
			EnableDrawing = true;

			if(PickupTrigger.IsValid()) {
				PickupTrigger.EnableTouch = false;
			}

			if(MeleeTrigger.IsValid()) {
				MeleeTrigger.EnableTouch = true;
			}
		}

		public override void OnCarryDrop(Entity dropper) {
			base.OnCarryDrop(dropper);

			if(PickupTrigger.IsValid()) {
				PickupTrigger.EnableTouch = true;
			}

			if(MeleeTrigger.IsValid()) {
				MeleeTrigger.EnableTouch = false;
			}
		}
	}
}
