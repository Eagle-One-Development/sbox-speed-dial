using Sandbox;
using System.Collections.Generic;

namespace SpeedDial.GameSound {
	public partial class LayeredSoundTrack {

		// TODO
		// This shit is halted since we can't just "make" sounds and stop/resume them right now
		//
		// ugh

		private List<SoundTrack> layers = new();

		public LayeredSoundTrack(List<SoundTrack> layers) {
			this.layers = layers;
		}
	}
}
