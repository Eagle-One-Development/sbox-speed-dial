using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedDial
{
	public class WarmUpRound : BaseRound
	{
		public override string RoundName => "WarmUp";
		public override int RoundDuration => -1;


		public override void OnSecond()
		{
			if ( Host.IsServer )
			{
				//TODO: When each team has a minimum of one player, start the round
			}
		}

		protected override void OnStart() {

			Log.Info( "AAAAAAAAAAAAAAAAAAAAA" );

		}

		private bool _roundStarted;
	}
}
