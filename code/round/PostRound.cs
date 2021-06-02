using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Player;

namespace SpeedDial {
	public class PostRound : BaseRound {
		public override string RoundName => "Post Round";
		public override int RoundDuration => 10;

		protected override void OnFinish() {
			Log.Info("Finished Game Round");

		}

		protected override void OnStart()
		{
			
			var players = Client.All;
			foreach ( var p in players.ToArray() )
			{
				
				if ( p.Pawn is SpeedDialPlayer jp )
				{
					Log.Info( "ROUND STARTED" );
					jp.Freeze();
				}
			}

		}

		protected override void OnTimeUp() {
			Log.Info("Game Round Round Up!");

			SpeedDialGame.Instance.ChangeRound( new WarmUpRound() );

			base.OnTimeUp();
		}
	}
}
