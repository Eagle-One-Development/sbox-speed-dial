using Sandbox;

using SpeedDial.Classic.Player;

//CREDIT: Taken from Espionage.Engine by Jake Wooshito
namespace SpeedDial {
	public partial class Game : GameBase {

		public static Game Current { get; protected set; }
		public static string GamemodeName { get; } = "classic";

		public Game() {
			Transmit = TransmitType.Always;
			Current = this;
			PrecacheAssets();
		}

		public override void Shutdown() {
			if(Current == this)
				Current = null;
		}

		public override void PostLevelLoaded() {
			if(IsServer) {
				SetGamemode(Library.Create<Gamemode>(GamemodeName));
				if(ActiveGamemode is null) {
					Log.Error("GAMEMODES", $"COULDN'T INITIALIZE GAMEMODE {GamemodeName}");
					Log.Info("GAMEMODES", $"COULDN'T INITIALIZE GAMEMODE {GamemodeName}");
				}
			}
			if(MapSettings.Current is null) {
				Log.Error("This map was not made for speed dial or is missing an 'sd_map_settings' entity! Gameplay might be affected!");
			}
		}

		//
		// Client States
		//

		public override void ClientJoined(Client cl) {
			Log.Info($"\"{cl.Name}\" has joined the game");
			ActiveGamemode?.ClientJoined(cl);
			MapSettings.Current?.OnClientJoined.Fire(null, cl.Name);

			// TODO: Make a menu for this
			ClientReady(cl);
		}

		public override void ClientDisconnect(Client cl, NetworkDisconnectionReason reason) {
			Log.Info($"\"{cl.Name}\" has left the game ({reason})");
			ActiveGamemode?.ClientDisconnected(cl, reason);
			MapSettings.Current?.OnClientDisconnected.Fire(null, cl.Name);

			if(cl.Pawn.IsValid()) {
				cl.Pawn.Delete();
				cl.Pawn = null;
			}
		}

		public virtual void ClientReady(Client cl) {
			ActiveGamemode?.ClientReady(cl);
			MapSettings.Current?.OnClientReady.Fire(null, cl.Name);
		}

		//
		// Pawn States
		//

		public virtual void MoveToSpawnpoint(BasePlayer pawn) {
			Host.AssertServer();

			ActiveGamemode?.MoveToSpawnpoint(pawn);

			if(ActiveGamemode is null) {
				Log.Info("No gamemode - Can't move pawn to a spawn point");
				pawn.Transform = Transform.Zero;
			}
		}

		public virtual void PawnRespawned(BasePlayer pawn) {
			Host.AssertServer();

			ActiveGamemode?.PawnRespawned(pawn);
			MapSettings.Current?.OnPawnRespawned.Fire(pawn, pawn.Client.Name);
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

			MapSettings.Current?.OnPawnKilled.Fire(pawn, pawn.Client.Name);
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

			// Block Simulate from running clientside
			// if we're not predictable.
			if(!cl.Pawn.IsAuthority)
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
			//camSetup.FieldOfView = Settings.DefaultFOV;

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

		public override void OnVoicePlayed(long steamId, float level) { }

		//
		// Gamemode
		//

		[Net]
		private Gamemode ActiveGamemode { get; set; }

		/// <summary> [Server Assert] Change the gamemode </summary>
		/// <param name="gamemode"> Gamemode to change to </param>
		public void SetGamemode(Gamemode gamemode) {
			Host.AssertServer();

			ActiveGamemode?.Finish();
			ActiveGamemode = gamemode;
			ActiveGamemode?.Start();
		}

		/// <summary> Returns the active gamemode </summary>
		public Gamemode GetGamemode() {
			return ActiveGamemode;
		}
	}
}
