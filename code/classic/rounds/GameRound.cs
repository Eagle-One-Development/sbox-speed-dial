using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Rounds;

public partial class GameRound : TimedRound {
	public override TimeSpan RoundDuration => TimeSpan.FromMinutes(5);
	private ClassicGamemode classic => Game.Current.ActiveGamemode as ClassicGamemode;
	public override string RoundText => "";

	protected override void OnStart() {
		base.OnStart();

		classic.SetState(GamemodeState.Running);

		foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
			var pawn = client.Pawn as ClassicPlayer;

			pawn.Frozen = false;
		}

		// start climax track 10 seconds before round ends
		_ = PlayClimaxMusic((int)RoundDuration.TotalSeconds - 10);
	}

	protected override void OnFinish() {
		base.OnFinish();
		Game.Current.ActiveGamemode?.ChangeRound(new PostRound());

		foreach(var client in Client.All.Where(x => x.Pawn is ClassicPlayer)) {
			WinScreen.UpdatePanels(To.Single(client));
		}
	}

	private async Task PlayClimaxMusic(int delay) {
		await GameTask.DelaySeconds(delay);
		ClassicPlayer.PlayRoundendClimax(To.Everyone);
	}

	public override void OnPawnJoined(BasePlayer pawn) {
		base.OnPawnJoined(pawn);
		if(pawn is ClassicPlayer player) {
			ClassicPlayer.PlaySoundtrack(To.Single(player.Client));
		}
	}
}
