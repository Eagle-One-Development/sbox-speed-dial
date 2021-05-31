using Sandbox;

namespace SpeedDial {
	public partial class SpeedDialGame {
		private static void PrecacheModels() {
			Log.Info("Precaching models");
			Precache.Add("models/biped_standard/biped_standard.vmdl");
			Precache.Add("weapons/rust_pistol/rust_pistol.vmdl");
			Precache.Add("particles/blood/blood_splash.vpcf");
			Precache.Add("particles/blood/blood_drops.vpcf");
			Precache.Add("particles/blood/blood_plip.vpcf");
			Precache.Add("particles/pistol_muzzleflash.vpcf");
			Precache.Add("particles/impact.generic.vpcf");
			Precache.Add("particles/weapon_fx/bullet_trail.vpcf");
		}
	}
}
