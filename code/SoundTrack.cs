using Sandbox;

namespace SpeedDial {
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
	}
}
