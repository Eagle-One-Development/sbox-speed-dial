using SpeedDial.OneChamber.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.OneChamber.Rounds;

public partial class OneChamberPostRound : TimedRound {
	public override TimeSpan RoundDuration => TimeSpan.FromSeconds(11);
	private OneChamberGamemode onechamber => Game.Current.ActiveGamemode as OneChamberGamemode;
	public override string RoundText => "";

	protected override void OnStart() {
		base.OnStart();

		onechamber.SetState(GamemodeState.Ending);

		foreach(var client in Client.All.Where(x => x.Pawn is OneChamberPlayer)) {
			var pawn = client.Pawn as OneChamberPlayer;

			pawn.Frozen = true;
			CharacterSelect.ForceState(To.Single(client), false);
			WinScreen.SetState(To.Single(client), true);
		}
	}

	protected override void OnFinish() {
		base.OnFinish();
		Game.Current.ActiveGamemode?.ChangeRound(new OneChamberPreRound());

		foreach(var client in Client.All) {
			WinScreen.SetState(To.Single(client), false);
		}
	}

	public override void OnPawnJoined(BasePlayer pawn) {
		base.OnPawnJoined(pawn);
		if(pawn is OneChamberPlayer player) {
			player.Frozen = true;
		}
	}
}
