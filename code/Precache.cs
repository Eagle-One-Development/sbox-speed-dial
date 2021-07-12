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
			Precache.Add("particles/weapon_fx/shotgun_ejectbrass.vpcf");

			Precache.Add("animgraphs/sd_playermodel.vanmgrph");
			Precache.Add("animgraphs/sd_playermodel_test.vanmgrph");

			Precache.Add("sounds/music_onelesshero.vsnd");
			Precache.Add("sounds/punch.vsnd");
			Precache.Add("sounds/punch_2.vsnd");
			Precache.Add("sounds/spin.vsnd");
			Precache.Add("sounds/tape_noise.vsnd");
			Precache.Add("sounds/tape_top.vsnd");
			Precache.Add("sounds/woosh_1.vsnd");
			Precache.Add("sounds/woosh_2.vsnd");
			Precache.Add("sounds/spin.vsnd");
			Precache.Add("sounds/drukqs/leaf.vsnd");
			Precache.Add("sounds/drukqs/ollie_2.vsnd");
			Precache.Add("sounds/drukqs/polvo.vsnd");
			Precache.Add("sounds/drukqs/ritindi.vsnd");
			Precache.Add("sounds/kill/draftsfx_killsecured_long (1).vsnd");
			Precache.Add("sounds/kill/hit01.vsnd");
			Precache.Add("sounds/music/[draft]speed-dial_main_theme_bmanedit.vsnd");
			Precache.Add("sounds/music/[draft]speed-dial_main_theme_climax_10secondsearly_bmanedit.vsnd");
			Precache.Add("sounds/music/music_onelesshero.vsnd");
			Precache.Add("sounds/music/mx_speeddial_combat.vsnd");
			Precache.Add("sounds/punch/punch03.vsnd");
			Precache.Add("sounds/punch/punch04.vsnd");
			Precache.Add("sounds/punch/woosh_2.vsnd");
			Precache.Add("sounds/punch/woosh_1.vsnd");
			Precache.Add("sounds/ui/[draft]sfx_killsecured_long.vsnd");
			Precache.Add("sounds/ui/[draft]sfx_killsecured_short.vsnd");
			Precache.Add("sounds/ui/click.vsnd");
			Precache.Add("sounds/ui/tape_noise.vsnd");
			Precache.Add("sounds/ui/tape_stop.vsnd");
			Precache.Add("sounds/ui/tape_top.vsnd");
			Precache.Add("sounds/weapon_fx/spin.vsnd");
			// bat hit
			Precache.Add("sounds/simpleguns/bat/sd_bat.hit01.vsnd");
			Precache.Add("sounds/simpleguns/bat/sd_bat.hit02.vsnd");
			Precache.Add("sounds/simpleguns/bat/sd_bat.hit03.vsnd");
			// pistol shoot
			Precache.Add("sounds/simpleguns/pistol/sd_pistol.shoot01.vsnd");
			Precache.Add("sounds/simpleguns/pistol/sd_pistol.shoot02.vsnd");
			Precache.Add("sounds/simpleguns/pistol/sd_pistol.shoot03.vsnd");
			// rifle shoot
			Precache.Add("sounds/simpleguns/rifle/sd_rifle.shoot01.vsnd");
			Precache.Add("sounds/simpleguns/rifle/sd_rifle.shoot02.vsnd");
			Precache.Add("sounds/simpleguns/rifle/sd_rifle.shoot03.vsnd");
			// shotgun shoot
			Precache.Add("sounds/simpleguns/shotgun/sd_shotgun.shoot01.vsnd");
			Precache.Add("sounds/simpleguns/shotgun/sd_shotgun.shoot02.vsnd");
			Precache.Add("sounds/simpleguns/shotgun/sd_shotgun.shoot03.vsnd");
			// smg shoot
			Precache.Add("sounds/simpleguns/smg/sd_smg.shoot01.vsnd");
			Precache.Add("sounds/simpleguns/smg/sd_smg.shoot02.vsnd");
			Precache.Add("sounds/simpleguns/smg/sd_smg.shoot03.vsnd");
			Precache.Add("sounds/simpleguns/smg/sd_smg.shoot04.vsnd");
			Precache.Add("sounds/simpleguns/smg/sd_smg.shoot05.vsnd");
			Precache.Add("sounds/simpleguns/smg/sd_smg.shoot06.vsnd");
			// sniper shoot
			Precache.Add("sounds/simpleguns/sniper/sd_sniper.shoot01.vsnd");
			Precache.Add("sounds/simpleguns/sniper/sd_sniper.shoot02.vsnd");
			Precache.Add("sounds/simpleguns/sniper/sd_sniper.shoot03.vsnd");

		}
	}
}
