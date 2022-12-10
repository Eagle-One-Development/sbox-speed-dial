//CREDIT: Modified from Espionage.Engine by Jake Wooshito
namespace SpeedDial;

/// <summary> Round </summary>
public abstract partial class Round : BaseNetworkable
{
	/// <summary> how long are think ticks in seconds? </summary>
	protected virtual float ThinkTime => 0.1f;

	/// <summary>
	/// Call Finish() to finish a round
	/// </summary>
	[Net] public bool Finished { get; private set; }
	/// <summary>
	/// Call Start() to finish a round
	/// </summary>
	[Net] public bool Started { get; private set; }

	[Net] public float StartTime { get; private set; }
	public virtual string RoundText => "Round";

	/// <summary>
	/// Formatted version of the time elapsed in the round in seconds
	/// </summary>
	[Net]
	public string TimeElapsedFormatted { get; set; } = "";

	/// <summary> [Server Assert] Start the round </summary>
	public virtual void Start()
	{
		Game.AssertServer();

		if ( Started )
			return;

		Log.Debug( $"Round start {ClassName}" );

		Started = true;
		StartTime = Time.Now;

		_ = ThinkTimer();

		OnStart();
	}

	public TimeSpan GetElapsedTime()
	{
		return !Finished ? TimeSpan.FromSeconds( Time.Now - StartTime ) : TimeSpan.Zero;
	}

	public virtual TimeSpan GetTime()
	{
		return GetElapsedTime();
	}

	/// <summary> [Server Assert] Finish the round </summary>
	public virtual void Finish()
	{
		Game.AssertServer();

		if ( Finished || !Started )
			return;

		Log.Debug( $"Round finish {ClassName}" );

		Finished = true;
		Started = false;
		OnFinish();
	}

	public void Kill()
	{
		Game.AssertServer();
		Finished = true;
	}

	private async Task ThinkTimer()
	{
		while ( !Finished && Started )
		{
			OnThink();
			await GameTask.DelaySeconds( ThinkTime );
		}
	}

	/// <summary> [Server] Will invoke when the round has started </summary>
	protected virtual void OnStart() { Log.Debug( $"Round on start {ClassName}" ); }

	/// <summary> [Server] Will invoke every think tick, which can be defined by overriding "ThinkTime" </summary>
	protected virtual void OnThink()
	{
		TimeElapsedFormatted = GetElapsedTime().ToString( @"mm\:ss" );
	}

	/// <summary> [Server] Will invoke when the round has finished </summary>
	protected virtual void OnFinish() { Log.Debug( $"Round on finish {ClassName}" ); }
	/// <summary> A pawn joined for the first time during this round. </summary>
	public virtual void OnPawnJoined( BasePlayer pawn ) { }
	/// <summary> A pawn respawned during this round. </summary>
	public virtual void OnPawnRespawned( BasePlayer pawn ) { }
}
