namespace SpeedDial;

[UseTemplate]
public class VoteItemPanel : Panel
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
		Title = $"[{Item.Title}]";
		Description = Item.Description;
	}

	public override void Tick()
	{
		SetClass( "skip", index == -2 );
		SetClass( "voted", VoteEntity.Current?.GetClientVotedIndex( Local.Client.SteamId ) == index );
		SetClass( "winner", VoteEntity.Current?.WinnerIndex == index );
		Votes = $"{VoteEntity.Current?.GetVotes( index )}";
	}

	protected override void OnMouseDown( MousePanelEvent e )
	{
		VoteEntity.Vote( index );
	}
}
