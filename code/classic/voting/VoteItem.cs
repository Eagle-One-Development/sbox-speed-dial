namespace SpeedDial.Classic.Voting;

public partial class VoteItem : BaseNetworkable {
	[Net] public string Title { get; set; }
	[Net] public string Description { get; set; }
	[Net] public string ImagePath { get; set; }
}
