using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedDial.Classic.Voting;

public partial class GamemodeVote : VoteEntity {
	public override void OnVoteConcluded(VoteItem item) {
		Global.ChangeLevel($"{item.Title}");
	}
}
