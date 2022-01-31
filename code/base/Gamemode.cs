using System;
using System.Linq;

using Sandbox;
using Sandbox.UI;

//CREDIT: Taken from Espionage.Engine by Jake Wooshito
namespace SpeedDial {
	/// <summary> [Server, Client] Gamemode base </summary>
	[Library(Spawnable = false), Hammer.Skip]
	public abstract partial class Gamemode : Entity {

		public static Gamemode Instance;

		public Gamemode() {
			Transmit = TransmitType.Always;

			Instance = this;

			if(IsClient)
				CreateGamemodeUI();
		}

		public override void Spawn() {
			Name = ClassInfo.Name;
		}

		[Event.Tick.Server]
		protected virtual void Tick() { }

		public void Start() {
			MapSettings.Current?.GamemodeStart.Fire(null, ClassInfo.Name);
			OnStart();
			// the gamemode has technically also reset when it starts
			Event.Run("sd.gamemode.start");
			Event.Run("sd.gamemode.reset");
		}

		protected virtual void OnStart() { }

		public void Finish() {
			MapSettings.Current?.GamemodeFinish.Fire(null, ClassInfo.Name);
			OnFinish();
			Event.Run("sd.gamemode.end");
		}

		protected virtual void OnFinish() { }

		//
		// Round
		//

		[Net] public Round ActiveRound { get; private set; }

		/// <summary> [Assert Server] Forcefully change the active round </summary>
		public void SetRound(Round round) {
			Host.AssertServer();
			Assert.NotNull(round);

			ActiveRound?.Finish();

			ActiveRound = round;

			ActiveRound?.Start();
		}

		[Event.Frame]
		protected virtual void RoundDebug() {
			// Do this for now. To lazy to impliemt UI
			if(ActiveRound is null)
				return;

			//var r = ActiveRound;
			//DebugOverlay.ScreenText(new Vector2(Screen.Width / 2, 32), r.Finished ? "Finished Round" : ActiveRound.ClassInfo.Name);
			//DebugOverlay.ScreenText(new Vector2(Screen.Width / 2, 48), r.Finished ? "Finished Round" : ActiveRound.TimeElapsedFormatted);
		}

		//
		// Gamemode UI
		//

		public RootPanel GamemodeUI { get; protected set; }

		public virtual void CreateGamemodeUI() { }

		protected override void OnDestroy() {
			base.OnDestroy();

			Local.Hud = null;

			GamemodeUI?.Delete();
			GamemodeUI = null;
		}

		//
		// Map Logic
		//

		/// <summary> [Assert Server] Use this to move pawn to position when it has respawned </summary>
		public virtual void MoveToSpawnpoint(BasePlayer pawn) {
			Host.AssertServer();
			var spawnpoints = All.Where((s) => s is SpawnPoint);
			Entity optimalSpawn = spawnpoints.ToList()[0];
			float optimalDistance = 0;
			foreach(var spawn in spawnpoints) {
				float smallestDistance = 999999;
				foreach(var player in All.Where((p) => p is BasePlayer)) {
					var distance = Vector3.DistanceBetween(spawn.Position, player.Position);
					if(distance < smallestDistance) {
						smallestDistance = distance;
					}
				}
				if(smallestDistance > optimalDistance) {
					optimalSpawn = spawn;
					optimalDistance = smallestDistance;
				}
			}
			pawn.Transform = optimalSpawn.Transform;
			return;
		}

		/// <summary> [Assert Server] Use this to validate the gamemode for the active map </summary>
		public virtual bool ValidGamemode() { return true; }

		//
		// Pawn States
		//

		/// <summary> [Assert Server] </summary>
		public void PawnKilled(BasePlayer pawn) {
			Host.AssertServer();

			OnPawnKilled(pawn);
			pawn.TimeSinceDied = 0;
		}

		/// <summary> [Server Assert] Should this pawn be damaged ? </summary>
		/// <returns> True if damage should be taken </returns>
		public bool PawnDamaged(BasePlayer pawn, ref DamageInfo info) {
			Host.AssertServer();

			return OnPawnDamaged(pawn, ref info);
		}

		/// <summary> [Assert Server] </summary>
		public void PawnRespawned(BasePlayer pawn) {
			Host.AssertServer();

			OnPawnRespawned(pawn);
		}

		/// <summary> [Server] </summary>
		protected virtual void OnPawnKilled(BasePlayer pawn) { }

		/// <summary> [Server] Should this pawn be damaged ? Default just checks if the teams are the same </summary>
		/// <returns> True if damage should be taken </returns>
		protected virtual bool OnPawnDamaged(BasePlayer pawn, ref DamageInfo info) {
			// if(pawn is null || info.Attacker is null)
			// 	return true;

			// if(pawn.GetTeam() is null || (info.Attacker is BasePlayer coolPawn && coolPawn.GetTeam() is not null))
			// 	return true;

			// return pawn.GetTeam().Name != info.Attacker.Cast<BasePlayer>().GetTeam().Name;
			return true;
		}

		/// <summary> [Server] </summary>
		protected virtual void OnPawnRespawned(BasePlayer newPawn) { }

		public virtual bool OnClientSuicide(Client client) {
			return true;
		}

		//
		// Client States
		//

		/// <summary> [Assert Server] </summary>
		public void ClientJoined(Client client) {
			Host.AssertServer();
			OnClientJoined(client);
		}

		/// <summary> [Assert Server] </summary>
		public void ClientReady(Client client) {
			Host.AssertServer();
			OnClientReady(client);
		}

		/// <summary> [Assert Server] </summary>
		public void ClientDisconnected(Client client, NetworkDisconnectionReason reason) {
			Host.AssertServer();
			OnClientDisconnect(client, reason);
		}

		/// <summary> [Server] Is called when a client joins the server </summary>
		protected virtual void OnClientJoined(Client client) { }

		/// <summary> [Server] Is called when a client has chosen a team and wants to spawn
		/// we should assign a pawn in this callback too </summary>
		protected virtual void OnClientReady(Client client) {
			//client.AssignPawn<Specialist>();
		}

		/// <summary> [Server] Is called when a client has disconnected. Possibly use this for cleanup? </summary>
		protected virtual void OnClientDisconnect(Client client, NetworkDisconnectionReason reason) { }
	}
}
