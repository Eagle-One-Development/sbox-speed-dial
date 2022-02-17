global using Sandbox;
global using System;

using System.Linq;

using SpeedDial.Classic.Player;

//CREDIT: Taken from Espionage.Engine by Jake Wooshito
namespace SpeedDial {
	public partial class Game : GameBase {

		public static Game Current { get; protected set; }
		public static string DefaultGamemode { get; } = "classic";

		[ServerVar("sd_min_players", Help = "The minimum players required to start the game.")]
		public static int MinPlayers { get; set; } = 2;

		public Game() {
			Transmit = TransmitType.Always;
			Current = this;
			PrecacheAssets();

			if(IsServer) {
				_ = new Hud();
			}
		}

		public override void Shutdown() {
			if(Current == this)
				Current = null;
		}

		protected override void OnDestroy() {
			if(IsServer && ActiveGamemode is not null) {
				Log.Debug($"Exit with gamemode {ActiveGamemode?.ClassInfo?.Name}");
				var data = new GamemodeLobbyCookie();
				data.LobbyId = Global.Lobby.Id;
				data.Gamemode = ActiveGamemode.ClassInfo.Name;
				FileSystem.Data.WriteJson("sd_lobby_gamemode_cookie.json", data);
			}
		}

		public override void PostLevelLoaded() {
			if(IsServer) {
				InitGamemode();
			}
		}

		private void InitGamemode() {
			Host.AssertServer();
			Log.Debug("gamemode init");
			// do we have a cookie stored?
			if(FileSystem.Data.FileExists("sd_lobby_gamemode_cookie.json")) {
				Log.Debug("gamemode cookie found");
				// read cookie
				var data = FileSystem.Data.ReadJson<GamemodeLobbyCookie>("sd_lobby_gamemode_cookie.json");
				// we have a cookie from this lobby, load the gamemode that's stored there
				if(data.LobbyId == Global.Lobby.Id) {
					Log.Debug($"gamemode cookie from current lobby: {data.Gamemode}");
					ChangeGamemode(data.Gamemode);
					return;
				}
			}
			ChangeGamemode(DefaultGamemode);
		}

		//
		// Client States
		//

		public override void ClientJoined(Client cl) {
			Log.Info($"\"{cl.Name}\" has joined the game");
			ActiveGamemode?.ClientJoined(cl);

			// TODO: Make a menu for this
			ClientReady(cl);
		}

		public override void ClientDisconnect(Client cl, NetworkDisconnectionReason reason) {
			Log.Info($"\"{cl.Name}\" has left the game ({reason})");
			ActiveGamemode?.ClientDisconnected(cl, reason);

			if(cl.Pawn.IsValid() && cl.Pawn is BasePlayer player) {
				player.OnClientDisconnected();
				player.Delete();
			}
		}

		public virtual void ClientReady(Client cl) {
			ActiveGamemode?.ClientReady(cl);
		}

		//
		// Pawn States
		//

		public virtual void MoveToSpawnpoint(BasePlayer pawn) {
			Host.AssertServer();

			ActiveGamemode?.MoveToSpawnpoint(pawn);

			if(ActiveGamemode is null) {
				pawn.Transform = Transform.Zero;
			}
		}

		public virtual void PawnRespawned(BasePlayer pawn) {
			Host.AssertServer();

			ActiveGamemode?.PawnRespawned(pawn);
		}

		public virtual bool PawnDamaged(BasePlayer pawn, ref DamageInfo info) {
			Host.AssertServer();

			if(ActiveGamemode is not null) {
				var should = ActiveGamemode.PawnDamaged(pawn, ref info);

				return should;
			} else {
				return true;
			}
		}

		public virtual void PawnKilled(BasePlayer pawn, DamageInfo lastDamage) {
			Host.AssertServer();

			if(ActiveGamemode is not null) {
				ActiveGamemode.PawnKilled(pawn);
			}
		}

		//
		// Developer Commands
		//

		[ServerCmd("noclip")]
		public static void NoClipCommand() {
			var client = ConsoleSystem.Caller;
			if(client == null) return;

			Current?.PawnNoClip(client);
		}


		public virtual void PawnNoClip(Client client) {
			if(!client.HasPermission("noclip"))
				return;

			if(client.Pawn is BasePlayer pawn) {
				if(pawn.DevController is ClassicNoclipController) {
					Log.Info("Noclip - Off");
					pawn.DevController = null;
				} else {
					Log.Info("Noclip - On");
					pawn.DevController = new ClassicNoclipController();
				}
			}
		}

		[ServerCmd("devcam")]
		public static void DevModeCommand() {
			var client = ConsoleSystem.Caller;
			if(client == null) return;

			Current?.PawnDevCam(client);
		}

		public virtual void PawnDevCam(Client client) {
			Host.AssertServer();

			if(!client.HasPermission("devcam"))
				return;

			client.DevCamera = client.DevCamera == null ? new DevCamera() : null;

			// TODO: figure out a way to do this in a cool way that doesn't intrude on the normal Debug.Enabled use (prints etc)
			// if(client.DevCamera is DevCamera)
			// 	Debug.Enabled = true;
			// else
			// 	Debug.Enabled = false;
		}

		[ServerCmd("kill")]
		public static void KillCommand() {
			var client = ConsoleSystem.Caller;
			if(client == null) return;

			Current?.PawnSuicide(client);
		}

		public virtual void PawnSuicide(Client client) {
			if(ActiveGamemode is not null) {
				if(ActiveGamemode.OnClientSuicide(client))
					client.Pawn.Kill();
			} else {
				client.Pawn.Kill();
			}
		}

		//
		// Simulate
		//

		public override void Simulate(Client cl) {
			if(!cl.Pawn.IsValid())
				return;

			// Block Simulate from running clientside
			// if we're not predictable.
			if(!cl.Pawn.IsAuthority)
				return;

			cl.Pawn.Simulate(cl);
		}

		public override void FrameSimulate(Client cl) {
			Host.AssertClient();

			if(!cl.Pawn.IsValid())
				return;

			cl.Pawn?.FrameSimulate(cl);
		}

		//
		// Camera & Input
		//

		public virtual ICamera FindActiveCamera() {
			return Local.Client.DevCamera ?? Local.Client.Camera ?? Local.Pawn.Camera ?? null;
		}

		[Predicted]
		protected Camera LastCamera { get; set; }

		public override CameraSetup BuildCamera(CameraSetup camSetup) {
			var cam = FindActiveCamera();

			if(LastCamera != cam) {
				LastCamera?.Deactivated();
				LastCamera = cam as Camera;
				LastCamera?.Activated();
			}

			cam?.Build(ref camSetup);

			PostCameraSetup(ref camSetup);

			return camSetup;
		}

		public override void BuildInput(InputBuilder input) {
			Event.Run("buildinput", input);

			// the camera is the primary method here
			LastCamera?.BuildInput(input);
			Local.Pawn?.BuildInput(input);
		}

		public override void PostCameraSetup(ref CameraSetup camSetup) {

			if(Local.Pawn != null) {
				// VR anchor default is at the pawn's location
				VR.Anchor = Local.Pawn.Transform;
				Local.Pawn.PostCameraSetup(ref camSetup);
			}
		}

		//
		// Voice
		//

		public override bool CanHearPlayerVoice(Client source, Client dest) {
			Host.AssertServer();

			var sp = source.Pawn;
			var dp = dest.Pawn;

			if(sp == null || dp == null)
				return false;

			if(sp.Position.Distance(dp.Position) > 1000)
				return false;

			return true;
		}

		// maybe pass this to the gamemode?
		public override void OnVoicePlayed(long steamId, float level) { }

		//
		// Gamemode
		//

		/// <summary>
		/// The currently active gamemode instance.
		/// </summary>
		[Net] public Gamemode ActiveGamemode { get; private set; }

		[ServerCmd("sd_change_gamemode")]
		public static void ChangeGamemode(string name) {
			Log.Debug($"change gamemode {name}");
			var gamemode = Library.Create<Gamemode>(name);
			if(gamemode is null) {
				Log.Error($"COULDN'T INITIALIZE GAMEMODE {name}");
				Log.Info($"COULDN'T INITIALIZE GAMEMODE {name}");
				return;
			}
			Current.SetGamemode(gamemode);
		}

		[ServerCmd("sd_bot")]
		public static void SpawnBot() {
			Current.ActiveGamemode?.OnBotAdded();
		}

		/// <summary> [Server Assert] Change the gamemode </summary>
		/// <param name="gamemode">Gamemode name to change to </param>
		public void SetGamemode(Gamemode gamemode) {
			Host.AssertServer();

			var clients = Client.All;

			ActiveGamemode?.Finish();
			ActiveGamemode = gamemode;
			// call this before we start the gamemode so entities are valid and enabled when we start (or disabled)
			UpdateGamemodeEntities(gamemode.Identity);
			ActiveGamemode?.Start();

			// ready all previously playing clients
			foreach(var client in clients) {
				ActiveGamemode.ClientReady(client);
			}
		}

		/// <summary>
		/// Enable, Disable and Handle all GamemodeEntities according to their set flag and the gamemode identity, assuming the gamemode allows it.
		/// </summary>
		/// <param name="identity">The Identity enum of the gamemode</param>
		protected void UpdateGamemodeEntities(GamemodeIdentity identity) {
			foreach(var entity in All.OfType<GamemodeEntity>()) {
				if(entity.ExcludedGamemodes.HasFlag((GamemodeEntity.Gamemodes)(int)identity)) {
					ActiveGamemode?.DisableEntity(entity);
				} else {
					ActiveGamemode?.EnableEntity(entity);
				}
				// in case the gamemode wants to force some specific shit
				ActiveGamemode?.HandleGamemodeEntity(entity);
			}
		}
	}

	public class GamemodeLobbyCookie {
		public ulong LobbyId { get; set; }
		public string Gamemode { get; set; }
	}
}
