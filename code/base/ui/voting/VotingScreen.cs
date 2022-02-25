﻿namespace SpeedDial;

[UseTemplate]
public partial class VotingScreen : Panel {
	private string Title => "Voting";
	private Panel Items { get; set; }
	// used to cache the number of vote items
	private int _count;
	private TimeSince _updateDelta;
	private VoteEntity Vote => VoteEntity.Current;
	public override void Tick() {
		SetClass("open", Vote.IsValid() && (!Vote.Concluded || (Vote.Concluded && Vote.TimeSinceConcluded < 1.0f)));
		if(!Vote.IsValid()) {
			return;
		}

		if(_updateDelta > 0.1f) {
			_updateDelta = 0;
			// number of items has changed, re init the ui
			if(_count != Vote.VoteItems.Count) {
				_count = Vote.VoteItems.Count;
				InitializeItems();
			}
		}
	}

	private void InitializeItems() {
		Items.DeleteChildren();

		for(var i = 0; i < Vote.VoteItems.Count; i++) {
			Items.AddChild(new VoteItemPanel(i));
		}
	}
}
