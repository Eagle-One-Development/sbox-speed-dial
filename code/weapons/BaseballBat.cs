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
	}
}
