using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial;

namespace SpeedDial.UI{
    public class GameRoundPanel : Panel
    {
		public Label timer;
		public Panel container;
		private TimeSince aTime;
        public GameRoundPanel(){
			StyleSheet.Load("/ui/GameRoundPanel.scss");
			container = Add.Panel("container");
			timer = container.Add.Label("00:00","timer");
		}
		public override void Tick()
		{
			base.Tick();
			Shadow s1 = new Shadow();
			s1.OffsetX = 2f + MathF.Sin(aTime * 2f) * 2f;
			s1.OffsetY = 0f;
			s1.Color = new Color(173f/255f,255f/255f,226f/255f,1.0f);
			s1.Blur = 4f;

			Shadow s2 = new Shadow();
			s2.OffsetX = -2f + MathF.Sin(aTime * 2f) * 2f;
			s2.OffsetY = 0;
			s2.Color = new Color(255f/255f,163f/255f,255f/255f,1.0f);
			s2.Blur = 4f;

			ShadowList shadows = new ShadowList();
			shadows.Add(s1);
			shadows.Add(s2);
			
			timer.Style.TextShadow = shadows;
			timer.Style.Dirty();

			if(SpeedDialGame.Instance.Round is GameRound gr) {
				container.SetClass("active", true);
				//if(gr == null )
				//{
				//	return;
				//}
				if(gr.TimeLeftFormatted != null) {
					timer.Text = gr.TimeLeftFormatted.ToString();
				}
			} else {
				container.SetClass("active", false);
			}
		}
    }
}
