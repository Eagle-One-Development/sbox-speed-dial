// sandbox
global using Sandbox;
global using Sandbox.Component;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using SandboxEditor;
// system
global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Linq;
global using System.Threading.Tasks;
// internal
using SpeedDial.Classic.Bots;
using SpeedDial.Classic.Player;

//CREDIT: Modified from Espionage.Engine by Jake Wooshito
namespace SpeedDial;

public partial class Game : GameBase
{

	public static Game Current { get; protected set; }
	[ConVar.Server( "sd_default_gamemode", Help = "Sets the default gamemode. Uses the library name of the gamemode type." )]
	public static string DefaultGamemode { get; set; } = "Classic";
	public static string LastGamemode { get; private set; }
	public static GameLoop GameLoop { get; private set; }

	[ConVar.Server( "sd_min_players", Help = "The minimum players required to start the game." )]
	public static int MinPlayers { get; set; } = 2;

	public Game()
	{
		Current = this;
	}

	public override void Spawn()
	{
		_ = new Hud();
		PrecacheAssets();
		Transmit = TransmitType.Always;
	}

	public override void Shutdown()
	{
		if ( Current == this )
			Current = null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		// FIXME, Global.Lobby is deprecated, need new way to do per-lobby cookies
		//if ( IsServer && !string.IsNullOrWhiteSpace( LastGamemode ) )
		//{
		//	Log.Debug( $"Exit with gamemode {LastGamemode}" );
		//	var data = new GamemodeLobbyCookie();
		//	data.LobbyId = Global.Lobby.Id;
		//	data.Gamemode = LastGamemode;
		//	FileSystem.Data.WriteJson( "sd_lobby_gamemode_cookie.json", data );
		//}
	}

	public override void PostLevelLoaded()
	{
		if ( IsServer )
		{
			InitGamemode();
		}
	}

	private void InitGamemode()
	{
		Host.AssertServer();
		Log.Debug( "gamemode init" );
		// do we have a cookie stored?

		// FIXME, Global.Lobby is deprecated, need new way to do per-lobby cookies
		//if ( FileSystem.Data.FileExists( "sd_lobby_gamemode_cookie.json" ) )
		//{
		//	Log.Debug( "gamemode cookie found" );
		//	// read cookie
		//	var data = FileSystem.Data.ReadJson<GamemodeLobbyCookie>( "sd_lobby_gamemode_cookie.json" );
		//	// we have a cookie from this lobby, load the gamemode that's stored there
		//	if ( data.LobbyId == Global.Lobby.Id )
		//	{
		//		Log.Debug( $"gamemode cookie from current lobby: {data.Gamemode}" );
		//		ChangeGamemode( data.Gamemode );
		//		return;
		//	}
		//}

		ChangeGamemode( DefaultGamemode );
	}

	//
	// Client States
	//

	public override void ClientJoined( Client cl )
	{
		Log.Info( $"\"{cl.Name}\" has joined the game" );

		ActiveGamemode?.ClientJoined( cl );

		// TODO: Make a menu for this
		ClientReady( cl );
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		Log.Info( $"\"{cl.Name}\" has left the game ({reason})" );
		ActiveGamemode?.ClientDisconnected( cl, reason );

		if ( cl.Pawn.IsValid() && cl.Pawn is BasePlayer player )
		{
			player.OnClientDisconnected();
			player.Delete();
		}
	}

	public virtual void ClientReady( Client cl )
	{
		ActiveGamemode?.ClientReady( cl );
	}

	//
	// Pawn States
	//

	public virtual void MoveToSpawnpoint( BasePlayer pawn )
	{
		Host.AssertServer();

		ActiveGamemode?.MoveToSpawnpoint( pawn );

		if ( ActiveGamemode is null )
		{
			pawn.Transform = Transform.Zero;
		}
	}

	public virtual void PawnRespawned( BasePlayer pawn )
	{
		Host.AssertServer();

		ActiveGamemode?.PawnRespawned( pawn );
	}

	public virtual bool PawnDamaged( BasePlayer pawn, ref DamageInfo info )
	{
		Host.AssertServer();

		if ( ActiveGamemode is not null )
		{
			var should = ActiveGamemode.PawnDamaged( pawn, ref info );

			return should;
		}
		else
		{
			return true;
		}
	}

	public virtual void PawnKilled( BasePlayer pawn, DamageInfo lastDamage )
	{
		Host.AssertServer();

		if ( ActiveGamemode is not null )
		{
			ActiveGamemode.PawnKilled( pawn );
		}
	}

	//
	// Developer Commands
	//

	[ConCmd.Server( "noclip" )]
	public static void NoClipCommand()
	{
		var client = ConsoleSystem.Caller;
		if ( client == null ) return;

		Current?.PawnNoClip( client );
	}


	public virtual void PawnNoClip( Client client )
	{
		if ( !client.HasPermission( "noclip" ) )
			return;

		if ( client.Pawn is BasePlayer pawn )
		{
			if ( pawn.DevController is ClassicNoclipController )
			{
				Log.Info( "Noclip - Off" );
				pawn.DevController = null;
			}
			else
			{
				Log.Info( "Noclip - On" );
				pawn.DevController = new ClassicNoclipController();
			}
		}
	}

	[ConCmd.Server( "devcam" )]
	public static void DevModeCommand()
	{
		var client = ConsoleSystem.Caller;
		if ( client == null ) return;

		Current?.PawnDevCam( client );
	}

	public virtual void PawnDevCam( Client client )
	{
		Host.AssertServer();

		if ( !client.HasPermission( "devcam" ) )
			return;

		var camera = client.Components.Get<DevCamera>( true );

		if ( camera == null )
		{
			camera = new DevCamera();
			client.Components.Add( camera );
			return;
		}

		camera.Enabled = !camera.Enabled;
	}

	[ConCmd.Server( "kill" )]
	public static void KillCommand()
	{
		var client = ConsoleSystem.Caller;
		if ( client == null ) return;

		Current?.PawnSuicide( client );
	}

	public virtual void PawnSuicide( Client client )
	{
		if ( ActiveGamemode is not null )
		{
			if ( ActiveGamemode.OnClientSuicide( client ) )
				client.Pawn.Kill();
		}
		else
		{
			client.Pawn.Kill();
		}
	}

	//
	// Simulate
	//

	public override void Simulate( Client cl )
	{
		if ( !cl.Pawn.IsValid() )
			return;

		// Block Simulate from running clientside
		// if we're not predictable.
		if ( !cl.Pawn.IsAuthority )
			return;

		cl.Pawn.Simulate( cl );
	}

	public override void FrameSimulate( Client cl )
	{
		Host.AssertClient();

		if ( !cl.Pawn.IsValid() )
			return;

		cl.Pawn?.FrameSimulate( cl );
	}

	//
	// Camera & Input
	//

	public virtual CameraMode FindActiveCamera()
	{
		var devCam = Local.Client.Components.Get<DevCamera>();
		if ( devCam != null ) return devCam;

		var clientCam = Local.Client.Components.Get<CameraMode>();
		if ( clientCam != null ) return clientCam;

		var pawnCam = Local.Pawn?.Components.Get<CameraMode>();
		return pawnCam ?? null;
	}

	[Predicted]
	protected CameraMode LastCamera { get; set; }

	public override CameraSetup BuildCamera( CameraSetup camSetup )
	{
		var cam = FindActiveCamera();

		if ( LastCamera != cam )
		{
			LastCamera?.Deactivated();
			LastCamera = cam;
			LastCamera?.Activated();
		}

		cam?.Build( ref camSetup );

		PostCameraSetup( ref camSetup );

		return camSetup;
	}

	public override void BuildInput( InputBuilder input )
	{
		Event.Run( "buildinput", input );

		// the camera is the primary method here
		LastCamera?.BuildInput( input );
		Local.Pawn?.BuildInput( input );
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{

		if ( Local.Pawn != null )
		{
			// VR anchor default is at the pawn's location
			VR.Anchor = Local.Pawn.Transform;
			Local.Pawn.PostCameraSetup( ref camSetup );
		}
	}

	//
	// Voice
	//

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		Host.AssertServer();

		var sp = source.Pawn;
		var dp = dest.Pawn;

		return sp != null && dp != null && sp.Position.Distance( dp.Position ) <= 1000;
	}

	// maybe pass this to the gamemode?
	public override void OnVoicePlayed( long steamId, float level ) { }

	//
	// Gamemode
	//

	/// <summary>
	/// The currently active gamemode instance.
	/// </summary>
	[Net] public Gamemode ActiveGamemode { get; private set; }

	[ConCmd.Server( "sd_change_gamemode" )]
	public static void ChangeGamemode( string name )
	{
		Log.Debug( $"change gamemode {name}" );
		var gamemode = TypeLibrary.Create<Gamemode>( name );
		if ( gamemode is null )
		{
			Log.Error( $"COULDN'T INITIALIZE GAMEMODE {name}" );
			Log.Info( $"COULDN'T INITIALIZE GAMEMODE {name}" );
			return;
		}
		Current.SetGamemode( gamemode );
	}

	[ConCmd.Server( "sd_gamemode_end" )]
	public static void EndGamemode()
	{
		Current.OnEndGamemode();
	}

	private void OnEndGamemode()
	{
		Host.AssertServer();
		Current.ActiveGamemode?.Finish();
		Current.ActiveGamemode = null;
		Map.Reset( CleanupFilter );
		Log.Debug( "gamemode ended" );
		GamemodeVote.Start();
	}

	[ConCmd.Admin( "sd_bot" )]
	public static void SpawnBot()
	{
		Current.ActiveGamemode?.OnBotAdded( new ClassicBot() );
	}

	/// <summary> [Server Assert] Change the gamemode </summary>
	/// <param name="gamemode">Gamemode name to change to </param>
	public void SetGamemode( Gamemode gamemode )
	{
		Host.AssertServer();

		var clients = Client.All;

		gamemode.Parent = this;

		// we try to explicitly finish a gamemode before we start a new one to separate them by voting
		// this is mostly for if a gamemode is being cut short for whatever reason (debug commands)
		ActiveGamemode?.Finish();
		ActiveGamemode = gamemode;
		LastGamemode = gamemode.ClassName;

		// just to be sure, might save us some headaches
		Map.Reset( CleanupFilter );

		// call this before we start the gamemode so entities are valid and enabled when we start (or disabled)
		UpdateGamemodeEntities( gamemode.Identity );
		ActiveGamemode?.Start();

		// ready all previously playing clients
		foreach ( var client in clients )
		{
			ActiveGamemode.ClientReady( client );
		}
	}

	/// <summary>
	/// Enable, Disable and Handle all GamemodeEntities according to their set flag and the gamemode identity, assuming the gamemode allows it.
	/// </summary>
	/// <param name="identity">The Identity enum of the gamemode</param>
	protected void UpdateGamemodeEntities( GamemodeIdentity identity )
	{
		foreach ( var entity in All.OfType<GamemodeEntity>() )
		{
			if ( entity.ExcludedGamemodes.HasFlag( (GamemodeEntity.Gamemodes)(int)identity ) )
			{
				ActiveGamemode?.DisableEntity( entity );
			}
			else
			{
				ActiveGamemode?.EnableEntity( entity );
			}
			// in case the gamemode wants to force some specific shit
			ActiveGamemode?.HandleGamemodeEntity( entity );
		}
	}

	[Net]
	public int CompletedGameloops { get; set; } = 0;

	/// <summary>
	/// This is called when a gamemode's gameloop is finished. We use this to account for voting between gamemodes
	/// </summary>
	public void GameloopCompleted()
	{
		Log.Debug( "gameloop completed" );
		if ( !ActiveGamemode.IsValid )
			return;
		CompletedGameloops++;
		if ( CompletedGameloops >= ActiveGamemode?.GameloopsUntilVote )
		{
			EndGamemode();
		}
	}

	private static bool CleanupFilter( string className, Entity ent )
	{
		// Basic Source engine stuff
		if ( className is "player" or "worldent" or "worldspawn" or "soundent" or "player_manager" )
		{
			return false;
		}

		// When creating entities we only have classNames to work with..
		if ( ent == null || !ent.IsValid ) return true;

		// Gamemode related stuff, game entity, HUD, etc
		return ent is not GameBase && ent.Parent is not GameBase && ent is not Hud && ent is not VoteEntity;
	}
}

public class GamemodeLobbyCookie
{
	public ulong LobbyId { get; set; }
	public string Gamemode { get; set; }
}
