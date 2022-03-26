namespace SpeedDial;

/// <summary> Timed Round </summary>
public abstract partial class TimedRound : Round
{

	/// <summary> How long does this round go for? </summary>
	public virtual TimeSpan RoundDuration { get => TimeSpan.FromSeconds( 60 ); }

	/// <summary>
	/// Formatted version of the time left in the round in seconds
	/// </summary>
	[Net]
	public string TimeLeftFormatted { get; set; } = "";

	[Net, Predicted]
	public float RoundEndTime { get; private set; }

	public TimeSpan GetTimeLeft()
	{
		if ( !Finished )
			return TimeSpan.FromSeconds( RoundDuration.TotalSeconds - GetElapsedTime().TotalSeconds );
		else
			return TimeSpan.Zero;
	}

	public override TimeSpan GetTime()
	{
		return GetTimeLeft();
	}

	public override void Start()
	{
		if ( RoundDuration.TotalSeconds > 0 )
			RoundEndTime = Time.Now + (float)RoundDuration.TotalSeconds;

		base.Start();
	}

	public override void Finish()
	{
		if ( Host.IsServer )
			RoundEndTime = 0f;

		base.Finish();
	}

	protected override void OnThink()
	{
		if ( RoundEndTime > 0 && Time.Now >= RoundEndTime )
		{
			RoundEndTime = 0f;
			OnTimeUp();
		}
		else
		{
			TimeLeftFormatted = GetTimeLeft().ToString( @"mm\:ss" );
		}
	}

	/// <summary> [Server] Gets called when the round ends </summary>
	protected virtual void OnTimeUp() { Finish(); }
}
