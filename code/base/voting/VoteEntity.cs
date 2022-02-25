namespace SpeedDial;

public partial class VoteEntity : Entity {
	public override void Spawn() {
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	public VoteEntity() {
		Current = this;
	}

	public static VoteEntity Current;
	[Net] public bool Concluded { get; set; }
	[Net]
	public TimeSince TimeSinceConcluded { get; set; }
	[Net] public int WinnerIndex { get; set; } = -3;

	[Net] private IDictionary<long, int> _votes { get; set; }
	[Net] public IList<VoteItem> VoteItems { get; set; }

	public void AddVoteItem(string title, string description, string imagePath) {
		Log.Debug($"add vote item {title}");
		VoteItems.Add(new VoteItem { Title = title, Description = description, ImagePath = imagePath });
	}

	public VoteItem GetVoteItem(int index) {
		if(VoteItems.Count - 1 >= index)
			return VoteItems[index];
		return null;
	}

	/// <summary>
	/// Get the number of votes per vote index, stored in an array.
	/// </summary>
	/// <returns>An array of ints, representing the vote count at the given item index.</returns>
	public int[] GetVoteCounts() {
		// count amount of votes per index and populate array
		var array = new int[VoteItems.Count];
		for(int i = 0; i < array.Length; i++) {
			array[i] = _votes.Count((kv) => kv.Value == i);
		}
		return array;
	}

	/// <summary>
	/// Get the number of Votes on a given item by its index.
	/// </summary>
	/// <param name="index">The index of the item.</param>
	/// <returns>The number of votes the item has.</returns>
	public int GetVotes(int index) {
		return _votes.Count((kv) => kv.Value == index);
	}

	[ServerCmd("sd_vote")]
	public static void Vote(int index) {
		if(ConsoleSystem.Caller is null) {
			Log.Error("invalid client");
			return;
		}
		var id = ConsoleSystem.Caller.PlayerId;
		Current.HandleVote(id, index);
	}

	public void HandleVote(long playerid, int index) {
		// simple range check to see if the index exists. -2 means skip vote
		if((index >= 0 && index < VoteItems.Count) || index == -2) {
			// player has already voted, change previous vote index
			if(_votes.ContainsKey(playerid)) {
				_votes[playerid] = index;
			} else {
				_votes.Add(playerid, index);
			}
			return;
		}
		Log.Error("Cannot vote for non-existant vote item!");
	}

	[ServerCmd("sd_test_vote_start")]
	public static void StartVote() {
		if(Current is not null && !Current.Concluded) {
			Log.Warning("New vote started while previous vote hasn't concluded!");
		}
		_ = new VoteEntity();
		PopulateTestItems();
	}

	protected virtual void PopulateVoteItems() { }

	private static void PopulateTestItems() {
		for(var i = 0; i < 10; i++) {
			Current.AddVoteItem($"Item{i}", $"Description {i}", $"image\\path");
		}
	}

	[ServerCmd("sd_vote_end")]
	public static void ConcludeVote() {
		Current.HandleVoteEnd();
	}

	private void HandleVoteEnd() {
		Host.AssertServer();
		Log.Debug($"vote concluded {Concluded}");
		Concluded = true;
		TimeSinceConcluded = 0;
		var winner = GetWinnerIndex();

		// -2 is our skip vote index
		if(winner == -2) {
			OnVoteSkipped();
			return;
		}
		WinnerIndex = winner;
		OnVoteConcluded(VoteItems[winner]);
	}

	/// <summary>
	/// Get the item index of the item with the most votes. Introduces some randomness for items with the same amount of votes.
	/// </summary>
	/// <returns>The item index of the item with the most votes.</returns>
	private int GetWinnerIndex() {
		// there is probably a better way for this, but iterating gives more control
		var winner = -1;
		var votes = 0;
		var skips = _votes.Count((kv) => kv.Value == -2);
		var items = GetVoteCounts();
		for(int i = 0; i < items.Length; i++) {
			// has more votes
			if(items[i] > votes) {
				winner = i;
				votes = items[i];
				continue;
			}
			// has the same amount of votes, throw in some random
			if(items[i] == votes && votes != 0) {
				if(Rand.Int(0, 1) == 1) {
					winner = i;
					votes = items[i];
					continue;
				}
			}
		}

		if(skips >= votes) return -2;

		// if all items have 0 votes, pick random
		return winner == -1 ? Rand.Int(0, items.Length - 1) : winner;
	}

	/// <summary>
	/// Returns the index of the item the given client has voted for.
	/// </summary>
	/// <param name="playerid">PlayerID of the client.</param>
	/// <returns>The index of the voted item. -1 if the client has not voted.</returns>
	public int GetClientVotedIndex(long playerid) {
		if(_votes.TryGetValue(playerid, out var votes)) {
			return votes;
		}
		return -1;
	}

	/// <summary>
	/// Called if this vote was skipped by majority vote.
	/// </summary>
	public virtual void OnVoteSkipped() {
		Log.Debug($"Vote skipped");
	}

	/// <summary>
	/// Called when a vote is concluded.
	/// </summary>
	/// <param name="item">The item that won the vote.</param>
	public virtual void OnVoteConcluded(VoteItem item) {
		Log.Debug($"Vote concluded: {item.Title} | {item.Description} | {item.ImagePath}");
	}
}
