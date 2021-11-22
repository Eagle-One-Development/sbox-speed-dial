using Sandbox;

namespace SpeedDial.Classic {

	[Library("func_dance_floor", Description = "Dance floor tile entity")]
	[Hammer.Solid]
	public partial class DanceFloor : FuncBrush {

		public static readonly Color[] Colors = new Color[10];

		[Property(Title = "Custom Color Code")]
		public string CustomColorCode { get; set; } = "";

		[Property(Title = "Color Code")]
		public int ColorCode { get; set; } = 0;

		[Property(Title = "Color Code Change Interval in seconds")]
		public int Interval { get; set; } = 1;

		private float NextInterval { get; set; }
		private int CurrentColor = 0;
		private int CurrentCustomIndex = 0;
		private bool UseCustomCode = false;

		public override void Spawn() {

			Colors[0] = Color.Black;
			Colors[1] = Color.Red;
			Colors[2] = new Color(1, 0.3f, 0);
			Colors[3] = Color.Yellow;
			Colors[4] = Color.Green;
			Colors[5] = new Color(86f / 255f, 30f / 255f, 138f / 255f); // purple
			Colors[6] = Color.Magenta;
			Colors[7] = Color.Blue;
			Colors[8] = Color.Cyan;
			Colors[9] = Color.White;

			if(CustomColorCode.Length > 0)
				UseCustomCode = true;

			Transmit = TransmitType.Always;
			NextInterval = RealTime.Now;

			base.Spawn();
		}

		[Event("server.tick")]
		public void Simulate() {
			if(RealTime.Now >= NextInterval) {

				if(UseCustomCode) {
					if(CurrentCustomIndex > CustomColorCode.Length - 1) {
						CurrentCustomIndex = 0;
					}
					var color = CustomColorCode[CurrentCustomIndex];
					RenderColor = Colors[int.Parse($"{color}")];
					CurrentCustomIndex++;
				} else {
					if(CurrentColor > Colors.Length - 1) {
						CurrentColor = 0;
					}
					RenderColor = Colors[CurrentColor];
					CurrentColor++;
				}
				NextInterval = RealTime.Now + Interval;
			}
		}
	}
}
