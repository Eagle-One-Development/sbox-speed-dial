namespace SpeedDial.Classic.GameSound;

public partial class SoundTrack {

	private Sound track;
	private float Volume = 1;

	public static SoundTrack FromScreen(string track) {
		return new(Sound.FromScreen(track));
	}

	private SoundTrack(Sound track) {
		this.track = track;
	}

	public Sound SetVolume(float volume) {
		Volume = volume;
		return track.SetVolume(volume);
	}

	public float GetVolume() {
		return Volume;
	}

	public Sound Stop() {
		return track.Stop();
	}

	public async Task Stop(float seconds, int steps = 100) {
		for(int i = 0; i < steps; i++) {
			SetVolume(1 - (i * 1 / (float)steps));
			await GameTask.DelaySeconds(seconds / steps);
		}
		Stop();
	}

	public async Task FadeVolumeTo(float volume, float seconds = 1, int steps = 100) {
		var initialVolume = GetVolume();
		var volumeMod = initialVolume - volume;
		for(int i = 0; i < steps; i++) {
			SetVolume(initialVolume - (i * volumeMod / steps));
			await GameTask.DelaySeconds(seconds / steps);
		}
	}
}
