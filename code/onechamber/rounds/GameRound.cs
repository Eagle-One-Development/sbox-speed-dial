using SpeedDial.OneChamber.Player;
using SpeedDial.Classic.Player;
using SpeedDial.OneChamber.UI;

namespace SpeedDial.OneChamber.Rounds;
public partial class OneChamberGameRound : TimedRound {
	public override TimeSpan RoundDuration => TimeSpan.FromMinutes(5);
	private OneChamberGamemode onechamber => Game.Current.ActiveGamemode as OneChamberGamemode;
	public override string RoundText => "";

	protected override void OnStart() {
		base.OnStart();

		onechamber.SetState(GamemodeState.Running);

		foreach(var client in Client.All.Where(x => x.Pawn is OneChamberPlayer)) {
			var pawn = client.Pawn as OneChamberPlayer;

			pawn.Frozen = false;
		}
	}

	protected override void OnThink() {
		base.OnThink();
		
		// only one player left
		if(Entity.All.OfType<OneChamberPlayer>().Count(x => x.CanRespawn()) == 1) {
			Finish();
		}
	}

	protected override void OnFinish() {
		base.OnFinish();
		Game.Current.ActiveGamemode?.ChangeRound(new OneChamberPostRound());

		ClassicPlayer.StopSoundtrack(To.Everyone, true);

		OneChamberWinScreen.UpdatePanels(To.Everyone);
	}

	private async Task PlayClimaxMusic(int delay) {
		await GameTask.DelaySeconds(delay);
		ClassicPlayer.PlayRoundendClimax(To.Everyone);
	}

	public override void OnPawnJoined(BasePlayer pawn) {
		base.OnPawnJoined(pawn);
		if(pawn is OneChamberPlayer player) {
			ClassicPlayer.PlaySoundtrack(To.Single(player.Client));
		}
	}
}
