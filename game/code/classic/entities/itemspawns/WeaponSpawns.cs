using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Entities;

[Library( "sd_weaponspawn_random" ), Title( "Random Weapon Spawn" ), HammerEntity]
[EditorModel( "models/weapons/rifle/prop_rifle.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public partial class ClassicRandomWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => WeaponBlueprint.GetRandomSpawnable().WeaponClass;
}

[Library( "sd_weaponspawn_pistol" ), HammerEntity]
[EditorModel( "models/weapons/pistol/prop_pistol.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public class PistolWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => "sd_pistol";
}

[Library( "sd_weaponspawn_rifle" ), HammerEntity]
[EditorModel( "models/weapons/rifle/prop_rifle.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public class RifleWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => "sd_rifle";
}

[Library( "sd_weaponspawn_smg" ), HammerEntity]
[EditorModel( "models/weapons/smg/prop_smg.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public class SmgWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => "sd_smg";
}

[Library( "sd_weaponspawn_shotgun" ), HammerEntity]
[EditorModel( "models/weapons/shotgun/prop_shotgun.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public class ShotgunWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => "sd_shotgun";
}

[Library( "sd_weaponspawn_sniper" ), HammerEntity]
[EditorModel( "models/weapons/rifle/prop_rifle.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public class SniperWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => "sd_sniper";
}

[Library( "sd_weaponspawn_roomclearer" ), HammerEntity]
[EditorModel( "models/weapons/shotgun/prop_roomclearer.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public class RoomClearerWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => "sd_roomclearer";
}

[Library( "sd_weaponspawn_bat" ), HammerEntity]
[EditorModel( "models/weapons/melee/melee.vmdl" )]
[Category( "Speed-Dial Weaponspawns" )]
public class BaseballBatWeaponSpawn : ClassicWeaponSpawn
{
	public override string WeaponClass => "sd_bat";
}
