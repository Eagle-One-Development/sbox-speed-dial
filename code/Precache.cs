using Sandbox;

namespace SpeedDial {
	public partial class SpeedDialGame {
		private static void PrecacheModels() {
			Log.Info("Precaching models");

			Precache.Add("models/playermodels/playermodel_base.vmdl");

			Precache.Add("models/playermodels/weapons/prop_pistol.vmdl");
			Precache.Add("models/playermodels/weapons/prop_rifle.vmdl");
			Precache.Add("models/playermodels/weapons/prop_shotgun.vmdl");
			Precache.Add("models/playermodels/weapons/prop_smg.vmdl");

			Precache.Add("particles/blood/blood_splash.vpcf");
			Precache.Add("particles/blood/blood_drops.vpcf");
			Precache.Add("particles/blood/blood_plip.vpcf");

			Precache.Add("particles/pistol_muzzleflash.vpcf");
			Precache.Add("particles/impact.generic.vpcf");
			Precache.Add("particles/weapon_fx/bullet_trail.vpcf");
			Precache.Add("particles/pistol_ejectbrass.vpcf");
		}
	}
}
