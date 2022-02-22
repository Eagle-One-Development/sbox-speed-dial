using SpeedDial.Classic.Bots;

//CREDIT: Modified from Espionage.Engine by Jake Wooshito
namespace SpeedDial;

// IF YOU ADD TO THIS, MAKE SURE IT LINES UP LIKE A BITFLAG WOULD AND THAT YOU UPDATE 
// THE ENUM IN GAMEMODEENTITY.CS AS WELL
public enum GamemodeIdentity {
	Base = int.MinValue,
	Classic = 1,
	Koth = 2,
	Dodgeball = 4,
	OneChamber = 8
}

public enum GamemodeState {
	Waiting,
	Preparing,
	Running,
	Ending,
	Paused,
	Voting
}

/// <summary> [Server, Client] Gamemode base </summary>
[Library(Spawnable = false), Skip]
public abstract partial class Gamemode : Entity {

	public static Gamemode Instance;

	[Net]
	public GamemodeState State { get; private set; }
	public bool Waiting => State == GamemodeState.Waiting;
	public bool Preparing => State == GamemodeState.Preparing;
	public bool Running => State == GamemodeState.Running;
	public bool Ending => State == GamemodeState.Ending;
	public bool Paused => State == GamemodeState.Paused;
	public bool Voting => State == GamemodeState.Voting;

	public virtual GamemodeIdentity Identity => GamemodeIdentity.Base;

	public Gamemode() {
		Transmit = TransmitType.Always;

		Instance = this;
	}

	public override void Spawn() {
		Name = ClassInfo.Name;
	}

	public virtual void SetState(GamemodeState state) {
		Log.Debug($"Gamemode state set to {state}");
		State = state;
	}

	/// <summary>
	/// Called from the sd_bot server command. Use this to apply your bot behaviour per gamemode
	/// </summary>
	public virtual void OnBotAdded(ClassicBot bot) { }

	[SpeedDialEvent.Gamemode.Start]
	public static void BotReset() {
		// apply gamemode behaviour to all bots
		foreach(var bot in Bot.All) {
			Instance.OnBotAdded(bot as ClassicBot);
		}
	}

	[Event.Tick.Server]
	protected virtual void Tick() { }

	public void Start() {
		Log.Debug("gamemode started");
		OnStart();
		// the gamemode has technically also reset when it starts
		CallStartEvent();
		CallResetEvent();
	}

	public void CallResetEvent() {
		Event.Run("sd.gamemode.reset", Identity);
		Event.Run("sd.gamemode.reset");
	}

	public void CallStartEvent() {
		Event.Run("sd.gamemode.start", Identity);
		Event.Run("sd.gamemode.start");
	}

	public void CallEndEvent() {
		Event.Run("sd.gamemode.end", Identity);
		Event.Run("sd.gamemode.end");
	}

	// override for gamemode specific rules for ents
	public virtual void EnableEntity(GamemodeEntity ent) {
		ent.Enable();
	}

	// override for gamemode specific rules for ents
	public virtual void DisableEntity(GamemodeEntity ent) {
		ent.Disable();
	}

	// override for gamemode specific rules for ents
	public virtual void HandleGamemodeEntity(GamemodeEntity ent) { }

	protected virtual void OnStart() { }

	public void Finish() {
		Log.Debug("gamemode finished");
		OnFinish();
		KillRound();
		CallEndEvent();

		DestroyGamemodeUIClient();
	}

	protected virtual void OnFinish() { }

	//
	// Round
	//

	[Net] public Round ActiveRound { get; private set; }

	/// <summary> [Assert Server] Forcefully change the active round </summary>
	public void ChangeRound(Round round) {
		Host.AssertServer();
		Log.Debug("round changed");
		Assert.NotNull(round);

		ActiveRound?.Finish();

		ActiveRound = round;

		ActiveRound?.Start();
	}

	public void KillRound() {
		Host.AssertServer();
		Log.Debug("round killed");
		ActiveRound?.Kill();
		ActiveRound = null;
	}

	[Event.Frame]
	protected virtual void RoundDebug() {
		// Do this for now. To lazy to implement UI
		if(ActiveRound is null)
			return;

		//var r = ActiveRound;
		//DebugOverlay.ScreenText(new Vector2(Screen.Width / 2, 32), r.Finished ? "Finished Round" : ActiveRound.ClassInfo.Name);
		//DebugOverlay.ScreenText(new Vector2(Screen.Width / 2, 48), r.Finished ? "Finished Round" : ActiveRound.TimeElapsedFormatted);
	}

	//
	// Gamemode UI
	//

	[ClientRpc]
	internal static void CreateGamemodeUIClient() {
		Instance?.CreateGamemodeUI();
	}

	[ClientRpc]
	internal static void DestroyGamemodeUIClient() {
		Hud.ClearGamemodeUI();
	}

	public virtual void CreateGamemodeUI() { }

	protected override void OnDestroy() {
		base.OnDestroy();

		Log.Debug("gamemode destroyed");

		DestroyGamemodeUIClient();
	}

	//
	// Map Logic
	//

	/// <summary> [Assert Server] Use this to move pawn to position when it has respawned </summary>
	public virtual void MoveToSpawnpoint(BasePlayer pawn) { }

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
		// create ui for this client
		CreateGamemodeUIClient(To.Single(client));
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
