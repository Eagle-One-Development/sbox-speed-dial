namespace SpeedDial.Classic.Voting;

public partial class GamemodeVote : VoteEntity {
	public override void OnVoteConcluded(VoteItem item) {
		Global.ChangeLevel($"{item.Title}");
	}
}
