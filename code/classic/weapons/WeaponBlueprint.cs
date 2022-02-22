namespace SpeedDial.Classic.Weapons;

public enum WeaponHoldType {
	Unarmed = 0,
	Melee = 1,
	Pistol = 2,
	Smg = 3,
	Rifle = 4,
	Shotgun = 5
}

public enum WeaponFireMode {
	Automatic,
	SemiAutomatic,
}

public enum WeaponSpecial {
	None,
	Burst,
	Melee
}

[Library("sdwep"), AutoGenerate]
public partial class WeaponBlueprint : Asset {
	//important
	[Category("Info")] public string WeaponClass { get; set; } = "sd_wep";
	[Category("Info")] public string WeaponTitle { get; set; } = "Speed-Dial Weapon";
	[Category("Info")] public string WeaponDescription { get; set; } = "This is a Speed-Dial Weapon";

	//stats
	[Category("Stats")] public WeaponFireMode FireMode { get; set; } = WeaponFireMode.SemiAutomatic;
	[Category("Stats")] public float FireRate { get; set; } = 5.0f;
	[Category("Stats")] public float BulletDamage { get; set; } = 100.0f;
	[Category("Stats")] public float BulletSize { get; set; } = 1.0f;
	[Category("Stats")] public float BulletForce { get; set; } = 1.0f;
	[Category("Stats")] public float BulletRange { get; set; } = 4096.0f;
	[Category("Stats")] public int BulletCount { get; set; } = 1;
	[Category("Stats")] public float BulletSpread { get; set; } = 0.1f;
	[Category("Stats")] public float VerticalSpreadMultiplier { get; set; } = 1.0f;
	[Category("Stats")] public int AmmoPerShot { get; set; } = 1;
	[Category("Stats")] public int ClipSize { get; set; } = 10;

	//special
	[Category("Special")] public WeaponSpecial Special { get; set; } = WeaponSpecial.None;
	[Category("Special")] public int BurstLength { get; set; } = 3; // used for burst
	[Category("Special")] public float FireDelay { get; set; } = 0.3f; // used for burst or melee
	[Category("Special")] public bool Penetrate { get; set; } = false; // used for wall penetration (ie sniper)
	[Category("Special")] public bool Scoped { get; set; } = false; // used for extended view shift range
	[Category("Special")] public bool RandomSpawnable { get; set; } = false;
	[Category("Special"), BitFlags] public GamemodeEntity.Gamemodes ExcludedGamemodes { get; set; }

	//model
	[Category("Model"), ResourceType("vmdl")] public string WorldModelPath { get; set; } = "models/weapons/pistol/prop_pistol.vmdl";
	[Category("Model")] public WeaponHoldType HoldType { get; set; } = WeaponHoldType.Pistol;
	[Category("Model")] public string HoldAttach { get; set; } = "pistol_attach";

	//vfx
	[Category("VFX"), ResourceType("vpcf")] public string MuzzleParticle { get; set; } = "particles/pistol_muzzleflash.vpcf";
	[Category("VFX")] public string MuzzleAttach { get; set; } = "muzzle";
	[Category("VFX"), ResourceType("vpcf")] public string EjectParticle { get; set; } = "particles/pistol_ejectbrass.vpcf";
	[Category("VFX")] public string EjectAttach { get; set; } = "ejection_point";

	//ui
	[Category("UI")] public float UIEffectsScalar { get; set; } = 1; // used for ui panel bump strength
	[Category("UI"), ResourceType("png")] public string Icon { get; set; } = "materials/ui/weapons/pistol.png";

	//sounds
	[Category("Sounds"), ResourceType("sound")] public string ShootSound { get; set; } = "sounds/simpleguns/pistol/sd_pistol.shoot.sound";
	[Category("Sounds"), ResourceType("sound")] public string DryFireSound { get; set; } = "sounds/simpleguns/misc/sd_dryfrire.sound";
	[Category("Sounds"), ResourceType("sound")] public string EmptyPickupSound { get; set; } = "sounds/weapon_fx/sd_pickup.empty.sound";
	[Category("Sounds"), ResourceType("sound")] public string LoadedPickupSound { get; set; } = "sounds/weapon_fx/sd_pickup.loaded.sound";


	[Skip] public Model WorldModel { get; private set; }
	[Skip] public Texture IconTexture { get; private set; }


	[Skip] public static List<WeaponBlueprint> All = new();

	public static WeaponBlueprint GetBlueprint(string weaponclass) {
		return All.FirstOrDefault(x => x.WeaponClass == weaponclass);
	}

	public static bool Exists(string weaponclass) {
		return All.Any(x => x.WeaponClass == weaponclass);
	}

	public static Weapon Create(string name) {
		var blueprint = GetBlueprint(name);
		return Create(blueprint);
	}

	public static Weapon Create(WeaponBlueprint blueprint) {
		var wep = new Weapon();
		wep.ApplyBlueprint(blueprint);
		return wep;
	}

	public static WeaponBlueprint GetRandomSpawnable() {
		return All.Where(x => x.RandomSpawnable).Random();
	}

	protected override void PostLoad() {
		// WeaponClass needs to be set
		if(string.IsNullOrWhiteSpace(WeaponClass)) {
			Log.Debug($"unable to load weapon \"{Path}\" due to empty weapon class!");
			return;
		}

		// WeaponClass needs to be unique
		if(All.Any(x => x.WeaponClass == WeaponClass)) {
			Log.Debug($"unable to load weapon \"{Path}\". it is already loaded!");
			return;
		}

		// get sound event name from full path
		ShootSound = System.IO.Path.GetFileNameWithoutExtension(ShootSound);
		DryFireSound = System.IO.Path.GetFileNameWithoutExtension(DryFireSound);
		EmptyPickupSound = System.IO.Path.GetFileNameWithoutExtension(EmptyPickupSound);
		LoadedPickupSound = System.IO.Path.GetFileNameWithoutExtension(LoadedPickupSound);

		// precache particles and sounds
		Precache.Add(MuzzleParticle);
		Precache.Add(EjectParticle);

		Precache.Add($"{ShootSound}.sound");
		Precache.Add($"{DryFireSound}.sound");
		Precache.Add($"{EmptyPickupSound}.sound");
		Precache.Add($"{LoadedPickupSound}.sound");

		WorldModel = Model.Load(WorldModelPath);

		if(Host.IsClient) {
			IconTexture = Texture.Load(FileSystem.Mounted, Icon);
		}

		Log.Debug($"loaded weapon {WeaponClass}");

		All.Add(this);
	}
}
