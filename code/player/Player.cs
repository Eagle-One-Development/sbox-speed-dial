using System.Numerics;

using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;
using SpeedDial.Meds;
using System.Threading.Tasks;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		public TimeSince TimeSinceDied { get; set; } = 0;

		[Net, Local]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 PlayerColor { get; set; }

		[Net]
		public int BodyGroup { get; set; }

		[Net]
		public bool pickup { get; set; }
		private Entity pickUpEntity;

		TimeSince timeSinceDropped;

		[Net, Local, Predicted]
		public TimeSince TimeSinceMelee { get; set; }

		[Net, Local]
		public bool ResetTimeSinceMelee { get; set; } = false;

		[Net, Local, Predicted]
		public TimeSince TimeSinceMedTaken { get; set; }

		[Net, Local]
		public bool ResetTimeSinceMedTaken { get; set; }

		[Net]
		public bool MedTaken { get; set; }

		[Net]
		public float MedDuration { get; set; }

		[Net]
		public DrugType CurrentDrug { get; set; }

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public BaseSpeedDialCharacter character;

		public void InitialSpawn() {

			if(GetClientOwner().SteamId == 76561198000823482) { // bak
				PlayerColor = new Color32(250, 176, 3);
			} else if(GetClientOwner().SteamId == 76561198203314521) { // gurke
				PlayerColor = new Color32(70, 0, 70);
			} else if(GetClientOwner().SteamId == 76561198095231052) { // generic
				PlayerColor = new Color32(27, 49, 63);
			} else if(GetClientOwner().SteamId == 76561198257053769) { // whimsy
				PlayerColor = Color.Cyan;
			} else {
				PlayerColor = Color.Random;
			}

			BodyGroup = Rand.Int(0, 8);

			Controller = new SpeedDialController();
			Camera = new SpeedDialCamera();

			//Set a default character
			MedTaken = false;
			character = SpeedDialGame.Instance.characters[0];

			Respawn();
		}

		public override void Respawn() {
			SetModel("models/playermodels/playermodel_base.vmdl");

			SetBodyGroup(1, BodyGroup);

			RenderColor = PlayerColor;

			(Camera as SpeedDialCamera).Freeze = false;
			(Controller as SpeedDialController).Freeze = false;
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Host.AssertServer();

			KillCombo = 0;
			MedTaken = false;

			BaseSpeedDialWeapon weapon = Library.Create<BaseSpeedDialWeapon>(character.Weapon);
			Inventory.Add(weapon, true);

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);

		}

		[ClientRpc]
		public void SetPlayerBodyGroup(int group, int value) {
			Log.Info("Set Bodygroup Client");
			SetBodyGroup(group, value);
		}

		[ClientRpc]
		public void DrugBump(string s, string f) {
			AmmoPanel.Current.DrugBump(s, f);
		}

		public void Freeze() {
			(Controller as SpeedDialController).Freeze = true;
			(Camera as SpeedDialCamera).Freeze = true;
		}

		public void Unfreeze() {
			(Controller as SpeedDialController).Freeze = false;
			(Camera as SpeedDialCamera).Freeze = false;
		}

		public void Freeze(bool freeze) {
			(Controller as SpeedDialController).Freeze = freeze;
			(Camera as SpeedDialCamera).Freeze = freeze;
		}

		/// <summary>
		/// Handles Punching
		/// </summary>
		async Task HandleMelee() {
			if(Input.Pressed(InputButton.Attack1)) {
				if(TimeSinceMelee > 0.33f) {
					ResetTimeSinceMelee = true;
					await GameTask.DelaySeconds(0.1f);
					var forward = EyeRot.Forward;
					Vector3 pos = EyePos + Vector3.Down * 20f;
					var tr = Trace.Ray(pos, pos + forward * 40f)
					.UseHitboxes()
					.Ignore(this)
					.Size(20f)
					.Run();

					if(IsClient) {
						PlaySwoosh();
					}
					SetAnimBool("b_attack", true);

					//DebugOverlay.Line(EyePos + Vector3.Down * 20, tr.EndPos, Color.White, 1, false);

					if(!IsServer) return;
					if(!tr.Entity.IsValid()) return;
					if(!(LifeState == LifeState.Alive)) return;

					// We turn predictiuon off for this, so any exploding effects don't get culled etc
					using(Prediction.Off()) {
						var damage = DamageInfo.FromBullet(tr.EndPos, Owner.EyeRot.Forward * 100, 200)
							.UsingTraceResult(tr)
							.WithAttacker(Owner)
							.WithWeapon(this);

						damage.Attacker = this;
						damage.Position = Position;
						PlayClientSound("punch_connect_1");
						PlaySound("punch_connect_1");
						await GameTask.DelaySeconds(0.2f);
						if(!(LifeState == LifeState.Alive)) return;
						tr.Entity.TakeDamage(damage);
					}
				}
			}
		}

		[ClientRpc]
		public void PlaySwoosh() {
			float f = Rand.Float(1);
			if(f > 0.5f) {
				PlaySound("punch_woosh_1");
			} else {
				PlaySound("punch_woosh_2");
			}
		}

		public void PlayClientSound(string s) {
			PlaySound(s);
		}

		public override void Simulate(Client cl) {
			if(LifeState == LifeState.Dead) {
				if(TimeSinceDied > RespawnTime && IsServer) {

					Respawn();
				}
				return;
			}

			if(ResetTimeSinceMelee) {
				TimeSinceMelee = 0;
				ResetTimeSinceMelee = false;
			}

			if(ResetTimeSinceMedTaken) {
				TimeSinceMedTaken = 0;
				ResetTimeSinceMedTaken = false;
			}

			if(KillCombo > maxCombo) {
				maxCombo = KillCombo;
				cl.SetScore("maxcombo", maxCombo);
			}

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			if(ActiveChild == null) {
				_ = HandleMelee();
			}

			if(TimeSinceMedTaken > MedDuration) {
				MedTaken = false;
				//Basically remove our extra health after the drug duration if we're high on leaf
				if(CurrentDrug == DrugType.Leaf) {
					if(Health > 100) {
						Health = 100;
					}
				}
			}

			SetAnimBool("b_polvo", (MedTaken && CurrentDrug == Meds.DrugType.Polvo) ? true : false);

			if(Input.Pressed(InputButton.Attack2)) {
				var dropped = Inventory.DropActive();
				if(dropped != null) {
					ResetInterpolation();
					dropped.Position = EyePos;
					if(dropped.PhysicsGroup != null) {
						(dropped as BaseSpeedDialWeapon).ApplyThrowVelocity(EyeRot.Forward);
						PlaySound("weaponspin");
					}

					timeSinceDropped = 0;
				}
				if(IsClient && ActiveChild != null) {
					PlaySound("weaponspin");
				}
			}

			if(IsClient && pickUpEntity != null) {
				Log.Info(pickUpEntity.ToString());
			}
			if(Input.Pressed(InputButton.Attack2) && pickup && pickUpEntity != null && Input.ActiveChild == null) {
				Inventory?.Add(pickUpEntity, Inventory.Active == null);
				pickup = false;
				pickUpEntity = null;
			}

			if(IsClient) {
				AmmoPanel.Current.pickedup = 0f;
			}

			if(IsClient && pickup && Input.ActiveChild == null) {
				AmmoPanel.Current.pickedup = 1f;
			}

			SimulateActiveChild(cl, ActiveChild);
		}
	}
}
