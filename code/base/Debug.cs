namespace SpeedDial;

// debug convars
public static partial class Debug
{

	[ConVar.Replicated( "debug" )]
	public static bool Enabled { get; set; }

	[ConVar.Replicated( "sd_debug_ui" )]
	public static bool UI { get; set; }

	[ConVar.Replicated( "sd_debug_camera" )]
	public static bool Camera { get; set; }

	[ConVar.Replicated( "sd_debug_weapons" )]
	public static bool Weapons { get; set; }

	[ConVar.Replicated( "sd_debug_bots" )]
	public static bool Bots { get; set; }

	[ConVar.Replicated( "sd_debug_infinite_ammo" )]
	public static bool InfiniteAmmo { get; set; }

	[ConVar.Replicated( "sd_playtest" )]
	public static bool Playtest { get; set; }
}

// Log.Debug
public static class LoggerExtension
{
	public static void Debug( this Logger log, object obj )
	{
		if ( !SpeedDial.Debug.Enabled ) return;

		log.Info( $"[{(Host.IsClient ? "CL" : "SV")}] {obj}" );
	}
}
