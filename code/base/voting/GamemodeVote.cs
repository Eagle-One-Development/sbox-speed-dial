﻿namespace SpeedDial;

public partial class GamemodeVote : VoteEntity
{

	[ConCmd.Admin( "sd_vote_gamemode_start" )]
	public static void Start()
	{
		new GamemodeVote().OnStart();
	}

	private void OnStart()
	{
		PopulateVoteItems();
	}

	public override string VoteTitle => "Gamemode Voting";
	public override string VoteDescription => "Vote for the next Gamemode.";

	// TEMP TEMP TEMP TEMP // TEMP TEMP TEMP TEMP // TEMP TEMP TEMP TEMP
	private readonly string[] gamemodes = {
		"ClassicGamemode",
		"OneChamberGamemode",
		"KothGamemode"
	};

	// TEMP TEMP TEMP TEMP // TEMP TEMP TEMP TEMP // TEMP TEMP TEMP TEMP
	protected override void PopulateVoteItems()
	{
		foreach ( var gm in gamemodes.Where( x => x != SDGame.LastGamemode ) )
		{
			Current.AddVoteItem( $"{gm}", $"Vote now!", $"image\\path" );
		}
	}

	public override void OnVoteConcluded( VoteItem item )
	{
		base.OnVoteConcluded( item );
		SDGame.ChangeGamemode( item.Title );
	}
}
