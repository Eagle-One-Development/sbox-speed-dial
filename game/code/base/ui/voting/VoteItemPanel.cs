namespace SpeedDial;

public partial class VoteItemPanel
{
	private readonly int index;
	private VoteItem Item => VoteEntity.Current?.GetVoteItem( index );

	public string Title { get; set; }
	public string Description { get; set; }
	public string Votes { get; set; }

	public VoteItemPanel()
	{
		index = -2;
		Title = "Abstain";
		Description = "(Skip Vote)";
	}

	public VoteItemPanel( int index )
	{
		this.index = index;
		Title = $"[{Item.Title.Replace( "Gamemode", "" )}]";
		Description = Item.Description;
	}

	public override void Tick()
	{
		SetClass( "skip", index == -2 );
		SetClass( "voted", VoteEntity.Current?.GetClientVotedIndex( Game.LocalClient.SteamId ) == index );
		SetClass( "winner", VoteEntity.Current?.WinnerIndex == index );

		var old = Votes;
		Votes = $"{VoteEntity.Current?.GetVotes( index )}";

		if ( Votes != old )
			StateHasChanged();
	}

	protected override void OnMouseDown( MousePanelEvent e )
	{
		VoteEntity.Vote( index );
	}
}
