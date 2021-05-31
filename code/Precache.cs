using Sandbox;

namespace SpeedDial {
	public partial class SpeedDialGame {
		private static void PrecacheModels() {
			Log.Info("Precaching models");
			Precache.Add("models/biped_standard/biped_standard.vmdl");
			Precache.Add("weapons/rust_pistol/rust_pistol.vmdl");
			Precache.Add("particles/blood_splash.vpcf");
			Precache.Add("particles/blood_drops.vpcf");
			Precache.Add("particles/blood_plip.vpcf");
		}
	}
}
