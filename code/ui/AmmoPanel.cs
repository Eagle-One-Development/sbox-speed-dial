using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;

namespace SpeedDial.UI{
    public class AmmoPanel : Panel
    {	
		public Panel ammoCounter;
		public Label ammoLabel;
		public Label clipLabel;

		private TimeSince aTime;
        public AmmoPanel(){
			StyleSheet.Load("/ui/AmmoPanel.scss");
			ammoCounter = Add.Panel("counter");
			clipLabel = ammoCounter.Add.Label("000","ammoLabel");
			//ammoLabel = ammoCounter.Add.Label("/000", "ammoLabel");
			//ammoLabel.SetClass("ammoLeft",true);
			//
			
			//ammoLabel.Style.TextShadow = 
		}

		public override void Tick()
		{
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

			float anim = (MathF.Sin(aTime * 2f) + 1)/2;
			float anim2 = (MathF.Sin(aTime * 1f));
			var transform = new PanelTransform();
			transform.AddScale(  0.8f + anim * 0.2f   );
			transform.AddRotation(0f,0f,anim2 * 5f);

			clipLabel.Style.TextShadow = shadows;
			clipLabel.Style.Transform = transform;
			clipLabel.Style.Dirty();

			var player = Local.Pawn;
			if ( player == null ) return;
			var weapon = player.ActiveChild as BaseSpeedDialWeapon;

			if ( weapon == null ) return;

			clipLabel.Text = $"{weapon.AmmoClip}";

		}
    }

}
