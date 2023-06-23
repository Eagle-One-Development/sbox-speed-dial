// sandbox
global using Sandbox;
global using Sandbox.Component;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using Editor;
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

public partial class SDGame : GameManager
{

	public static new SDGame Current { get; protected set; }
	[ConVar.Server( "sd_default_gamemode", Help = "Sets the default gamemode. Uses the library name of the gamemode type." )]
	public static string DefaultGamemode { get; set; } = "Classic";
	public static string LastGamemode { get; private set; }
	public static GameLoop GameLoop { get; private set; }

	[ConVar.Server( "sd_min_players", Help = "The minimum players required to start the game." )]
	public static int MinPlayers { get; set; } = 2;

	public SDGame()
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
		//if ( Game.IsServer && !string.IsNullOrWhiteSpace( LastGamemode ) )
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
		Log.Debug( "PostLevelLoaded" );
		if ( Game.IsServer )
		{
			InitGamemode();
		}
	}

	private void InitGamemode()
	{
		Game.AssertServer();
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
	// IClient States
	//

	public override void ClientJoined( IClient cl )
	{
		Log.Info( $"\"{cl.Name}\" has joined the game" );

		ActiveGamemode.ClientJoined( cl );

		// TODO: Make a menu for this
		ClientReady( cl );
	}

	public override void ClientDisconnect( IClient cl, NetworkDisconnectionReason reason )
	{
		Log.Info( $"\"{cl.Name}\" has left the game ({reason})" );
		ActiveGamemode?.ClientDisconnected( cl, reason );

		if ( cl.Pawn.IsValid() && cl.Pawn is BasePlayer player )
		{
			player.OnClientDisconnected();
			player.Delete();
		}
	}

	public virtual void ClientReady( IClient cl )
	{
		ActiveGamemode?.ClientReady( cl );
	}

	//
	// Pawn States
	//

	public virtual void MoveToSpawnpoint( BasePlayer pawn )
	{
		Game.AssertServer();

		ActiveGamemode?.MoveToSpawnpoint( pawn );

		if ( ActiveGamemode is null )
		{
			pawn.Transform = Transform.Zero;
		}
	}

	public virtual void PawnRespawned( BasePlayer pawn )
	{
		Game.AssertServer();

		ActiveGamemode?.PawnRespawned( pawn );
	}

	public virtual bool PawnDamaged( BasePlayer pawn, ref DamageInfo info )
	{
		Game.AssertServer();

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
		Game.AssertServer();

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


	public virtual void PawnNoClip( IClient client )
	{
		//if ( !client.HasPermission( "noclip" ) ) // TODO: HasPermission does not have a new implementation to be replaced with.
		//	return;

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

	public virtual void PawnDevCam( IClient client )
	{
		Game.AssertServer();

		//if ( !client.HasPermission( "devcam" ) ) // TODO: HasPermission does not have a new implementation to be replaced with.
		//	return;

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

	public virtual void PawnSuicide( IClient client )
	{
		if ( client.Pawn is not BasePlayer player )
			return;
		if ( ActiveGamemode is not null )
		{
			if ( ActiveGamemode.OnClientSuicide( client ) )
				player.Kill();
		}
		else
		{
			player.Kill();
		}
	}

	//
	// Simulate
	//

	public override void Simulate( IClient cl )
	{
		if ( !cl.Pawn.IsValid() )
			return;

		// Block Simulate from running clientside
		// if we're not predictable.
		if ( !cl.Pawn.IsAuthority )
			return;

		(cl.Pawn as Entity)?.Simulate( cl );
	}

	public override void FrameSimulate( IClient cl )
	{
		Game.AssertClient();

		if ( !cl.Pawn.IsValid() )
			return;

		(cl.Pawn as Entity)?.FrameSimulate( cl );
	}

	//
	// Camera & Input
	//

	public virtual CameraMode FindActiveCamera()
	{
		var devCam = Game.LocalClient.Components.Get<DevCamera>();
		if ( devCam != null ) return null;

		var clientCam = Game.LocalClient.Components.Get<CameraMode>();
		if ( clientCam != null ) return clientCam;

		var pawnCam = Game.LocalPawn?.Components.Get<CameraMode>();
		return pawnCam ?? null;
	}

	[Predicted]
	protected CameraMode LastCamera { get; set; }

	[GameEvent.Client.PostCamera]
	public void BuildCamera()
	{
		var cam = FindActiveCamera();

		if ( LastCamera != cam )
		{
			LastCamera?.Deactivated();
			LastCamera = cam;
			LastCamera?.Activated();
		}

		cam?.Build();

		PostCameraSetup();

	}

	public override void BuildInput()
	{
		Event.Run( "buildinput" );

		// the camera is the primary method here
		LastCamera?.BuildInput();
		Game.LocalPawn?.BuildInput();
	}

	public void PostCameraSetup()
	{

		if ( Game.LocalPawn != null )
		{
			// VR anchor default is at the pawn's location
			VR.Anchor = Game.LocalPawn.Transform;
		}
	}

	//
	// Voice
	//

	public override bool CanHearPlayerVoice( IClient source, IClient dest )
	{
		Game.AssertServer();

		var sp = source.Pawn;
		var dp = dest.Pawn;

		return sp != null && dp != null && sp.Position.Distance( dp.Position ) <= 1000;
	}

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
		Game.AssertServer();
		Current.ActiveGamemode?.Finish();
		Current.ActiveGamemode = null;
		Game.ResetMap( CleanupFilter() );
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
		Game.AssertServer();

		var clients = Game.Clients;

		gamemode.Parent = this;

		// we try to explicitly finish a gamemode before we start a new one to separate them by voting
		// this is mostly for if a gamemode is being cut short for whatever reason (debug commands)
		ActiveGamemode?.Finish();
		ActiveGamemode = gamemode;
		LastGamemode = gamemode.ClassName;

		// just to be sure, might save us some headaches
		Game.ResetMap( CleanupFilter() );

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

	private static Entity[] CleanupFilter()
	{
		// Gamemode related stuff, game entity, HUD, etc
		return Entity.All.Where( ent => (ent.ClassName is "player" or "worldent" or "worldspawn" or "soundent" or "player_manager") || (ent is GameManager || ent.Parent is GameManager || ent is Hud || ent is VoteEntity || ent is Gamemode) ).ToArray();
	}
}

public class GamemodeLobbyCookie
{
	public ulong LobbyId { get; set; }
	public string Gamemode { get; set; }
}
