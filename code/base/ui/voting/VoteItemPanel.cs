namespace SpeedDial;

[UseTemplate]
public class VoteItemPanel : Panel {
	private int index;
	private VoteItem Item => VoteEntity.Current.VoteItems[index];

	private string Title { get; set; }
	private string Description { get; set; }
	private string Votes { get; set; }

	public VoteItemPanel() {
		index = -2;
		Title = "Abstain";
		Description = "(Skip Vote)";
	}

	public VoteItemPanel(int index) {
		this.index = index;
		Title = $"[{Item.Title}]";
		Description = Item.Description;
	}

	public override void Tick() {
		SetClass("skip", index == -2);
		SetClass("voted", VoteEntity.Current?.GetClientVotedIndex(Local.Client.PlayerId) == index);
		Votes = $"{VoteEntity.Current?.GetVotes(index)}";
	}

	protected override void OnMouseDown(MousePanelEvent e) {
		VoteEntity.Vote(index);
	}
}
