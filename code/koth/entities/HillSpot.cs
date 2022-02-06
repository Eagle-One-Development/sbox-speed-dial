﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Classic.UI;

namespace SpeedDial.Koth.Entities {
	public partial class HillSpot : ModelEntity {

		public List<BasePlayer> TouchingPlayers = new();

		[Net, Predicted]
		public TimeSince TimeSinceAlive { get; set; }

		

		[Event.Tick]
		public void Tick() {
			if(TimeSinceAlive > 10f) {
				foreach(Client c in Client.All) {
					ScreenHints.FireEvent(To.Single(c), "HILL MOVED", "Good luck!");
				}

				if(IsValid) {
					Delete();
				}
				return;
			}
				
		}

		public override void Spawn() {
			base.Spawn();
			SetModel("models/koth/ring.vmdl");
			Transmit = TransmitType.Always;
			CollisionGroup = CollisionGroup.Trigger;
			SetupPhysicsFromModel(PhysicsMotionType.Static);
			TimeSinceAlive = 0f;
		}

		public override void StartTouch(Entity other) {

			if(other is BasePlayer player)
				TouchingPlayers.Add(player);
		}

		public override void EndTouch(Entity other) {
			if(other is BasePlayer player)
				TouchingPlayers.Remove(player);
		}
	}
}