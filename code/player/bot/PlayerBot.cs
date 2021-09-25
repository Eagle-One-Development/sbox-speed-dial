using System.Numerics;

using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;
using SpeedDial.Meds;
using System.Threading.Tasks;
using SpeedDial.GameSound;
using System.Linq;

namespace SpeedDial.Player {
	public enum BotMoveStates {
		GOTO_PLAYER,
		GOTO_GUN,
		GOTO_MED,
	}

	public partial class SpeedDialBotPlayer : SpeedDialPlayer {
		public bool Debug => SpeedDialGame.BotDebugEnabled;

		BotMoveStates State { get; set; }
		SpeedDialPlayer ClosestPlayer { get; set; }
		BaseMedication ClosestPickup { get; set; }
		BaseSpeedDialWeapon ClosestWeapon { get; set; }

		float ClosePlayerDist { get; set; }
		float ClosePickupDist { get; set; }
		float CloseWeaponDist { get; set; }

		bool HasWeapon { get; set; }
		public bool ShootAtPlayer { get; set; }
		float ShootRange => 300f;

		public TimeSince TimeSinceUpdate;
		public float UpdateInterval => 0.5f;

		NavPath Path = new NavPath();
		public NavSteer Steer;

		Vector3 InputVelocity;
		public Rotation LookRot;

		public override void InitialSpawn() {
			BodyGroup = Rand.Int(0, 9);

			Controller = new SpeedDialBotController();
			Camera = new SpeedDialCamera();

			Steer = new NavSteer();

			//Set a default character
			MedTaken = false;
			character = SpeedDialGame.Instance.characters[0];

			Respawn();
		}

		public override void Respawn() {
			if(character == null) {
				SetModel("models/playermodels/character_fallback.vmdl");
			} else {
				SetModel(character.Model);
			}

			SetBodyGroup(0, BodyGroup);

			RenderColor = Color.White;

			(Camera as SpeedDialCamera).Freeze = false;
			(Controller as SpeedDialBotController).Freeze = false;
			Animator = new PlayerBotAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			CauseOfDeath = COD.Gunshot;

			Host.AssertServer();

			KillCombo = 0;
			MedTaken = false;

			// reset weapon/ammo on spawn
			ResetWeapon();

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);
		}

		public override void Simulate(Client cl) {

			if(SpeedDialGame.Instance.Round is PreRound) {
				screenOpen = true;
			}

			if(Frozen) return;

			if(LifeState == LifeState.Dead) {
				DrugParticles?.Destroy(false);
				if(TimeSinceDied > RespawnTime && IsServer) {
					Respawn();
				}
				return;
			}

			if(!screenOpen && SpeedDialGame.Instance.Round is GameRound) {
				CharacterSelect.Current?.ToggleOpen();

				screenOpen = true;
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

				DrugParticles?.Destroy(false);

				//Basically remove our extra health after the drug duration if we're high on leaf
				if(CurrentDrug == DrugType.Leaf) {
					if(Health > 100) {
						Health = 100;
					}
				}
			}

			SetAnimBool("b_polvo", MedTaken && CurrentDrug == Meds.DrugType.Polvo);

			if(IsClient) {
				AmmoPanel.Current.pickedup = 0f;
			}

			if(IsClient && pickup && Input.ActiveChild == null) {
				AmmoPanel.Current.pickedup = 1f;
			}

			SimulateActiveChild(cl, ActiveChild);
		}

		[Event.Tick.Server]
		public void Tick() {
			InputVelocity = 0;

			if(Steer != null) {
				Steer.Tick(Position);
				Steer.Target = GetTarget();

				if(!Steer.Output.Finished) {
					InputVelocity = Steer.Output.Direction.Normal;
				}
			}

			if(ActiveChild != null) {
				HasWeapon = true;
			} else {
				HasWeapon = false;
			}

			var controller = Controller as SpeedDialBotController;
			//var local = Transform.NormalToLocal(InputVelocity.Normal);
			controller.Forward = InputVelocity.x;
			controller.Left = InputVelocity.y;

			if(ClosestPlayer != null && ClosestPlayer.IsValid) {
				Vector3 dirToClosestPlayer = ClosestPlayer.Position - Position;
				ClosePlayerDist = dirToClosestPlayer.LengthSquared;
				ClosePlayerDist = MathF.Sqrt(ClosePlayerDist);
			}

			if(ClosestWeapon != null && ClosestWeapon.IsValid) {
				Vector3 dirToClosestWeapon = ClosestWeapon.Position - Position;
				CloseWeaponDist = dirToClosestWeapon.LengthSquared;
				CloseWeaponDist = MathF.Sqrt(CloseWeaponDist);
			}

			if(ClosestPickup != null && ClosestPickup.IsValid) {
				Vector3 dirToClosestMed = ClosestPickup.Position - Position;
				ClosePickupDist = dirToClosestMed.LengthSquared;
				ClosePickupDist = MathF.Sqrt(ClosePickupDist);
			}

			if(ClosestPlayer != null && ClosestPlayer.IsValid) {
				if(ClosePlayerDist <= ShootRange && ClosestPlayer.LifeState == LifeState.Alive) {
					ShootAtPlayer = true;
				} else {
					ShootAtPlayer = false;
				}
			}

			if(ShootAtPlayer || State == BotMoveStates.GOTO_PLAYER) {
				if(ClosestPlayer != null && ClosestPlayer.IsValid) {
					var targetRot = Rotation.LookAt(ClosestPlayer.Position - Position);
					LookRot = Rotation.Slerp(LookRot, targetRot, Time.Delta * 5f);
				}
			} else {
				if(State == BotMoveStates.GOTO_GUN) {
					if(ClosestWeapon != null && ClosestWeapon.IsValid) {
						var targetRot = Rotation.LookAt(ClosestWeapon.Position - Position);
						LookRot = Rotation.Slerp(LookRot, targetRot, Time.Delta * 5f);
					}
				} else if(State == BotMoveStates.GOTO_MED) {
					if(ClosestPickup != null && ClosestPickup.IsValid) {
						var targetRot = Rotation.LookAt(ClosestPickup.Position - Position);
						LookRot = Rotation.Slerp(LookRot, targetRot, Time.Delta * 5f);
					}
				}
			}

			if(TimeSinceUpdate >= UpdateInterval) {
				UpdateClosests();
			}

			Scores();
			HandleGunGrabbingThrowing();
		}

		public override void FrameSimulate(Client cl) {
			base.FrameSimulate(cl);
		}

		public void Scores() {
			float playerScore = 0f;
			float pickupScore = 0f;
			float weaponScore = 0f;

			if(true) {
				float distT = MathX.LerpInverse(ClosePlayerDist, 1000, 100);
				float score = 0f;
				score += MathF.Pow(distT, 1);
				if(HasWeapon) {
					score += 0.6f;
				}
				if(MedTaken) {
					score += 0.6f;
				}
				score *= 1.2f;

				playerScore = score;
			}

			if(true) {
				float DistT = MathX.LerpInverse(ClosePickupDist, 1000, 100);
				float score = 0f;
				score += MathF.Pow(DistT, 2);
				if(!MedTaken) {
					score += 1.0f;
				} else {
					score = 0f;
				}
				pickupScore = score;
			}

			if(true) {
				float distT = MathX.LerpInverse(CloseWeaponDist, 1000, 100);
				float score = 0f;
				score += MathF.Pow(distT, 2);
				if(!HasWeapon) {
					score += 1.2f;
				} else {
					score = 0f;
				}
				if(ClosestWeapon.AmmoClip <= 0) {
					score = 0f;
				}
				weaponScore = score;
			}

			float highestScore = new[] { playerScore, pickupScore, weaponScore }.Max();
			if(highestScore == playerScore) {
				State = BotMoveStates.GOTO_PLAYER;
			} else if(highestScore == pickupScore) {
				State = BotMoveStates.GOTO_MED;
			} else if(highestScore == weaponScore) {
				State = BotMoveStates.GOTO_GUN;
			}

			if(Debug) {
				DebugScores(playerScore, pickupScore, weaponScore);
			}
		}

		public void DebugScores(float player, float pickup, float weapon) {
			if(ClosestPlayer != null && ClosestPlayer.IsValid) {
				var draw = Sandbox.Debug.Draw.ForSeconds(0);
				var lift = Vector3.Up * 15;

				draw.WithColor(Color.White.WithAlpha(player)).Circle(lift + ClosestPlayer.Position, Vector3.Up, 20.0f);
				draw.WithColor(Color.Yellow.WithAlpha(player)).Arrow(Position + lift, ClosestPlayer.Position + lift, Vector3.Up, 10.0f);
			}

			if(ClosestPickup != null && ClosestPickup.IsValid) {
				var draw = Sandbox.Debug.Draw.ForSeconds(0);
				var lift = Vector3.Up * 15;

				draw.WithColor(Color.White.WithAlpha(pickup)).Circle(lift + ClosestPickup.Position, Vector3.Up, 20.0f);
				draw.WithColor(Color.Yellow.WithAlpha(pickup)).Arrow(Position + lift, ClosestPickup.Position + lift, Vector3.Up, 10.0f);
			}

			if(ClosestWeapon != null && ClosestWeapon.IsValid) {
				var draw = Sandbox.Debug.Draw.ForSeconds(0);
				var lift = Vector3.Up * 15;

				draw.WithColor(Color.White.WithAlpha(weapon)).Circle(lift + ClosestWeapon.Position, Vector3.Up, 20.0f);
				draw.WithColor(Color.Yellow.WithAlpha(weapon)).Arrow(Position + lift, ClosestWeapon.Position + lift, Vector3.Up, 10.0f);
			}

		}

		public Vector3 GetTarget() {
			var target = Vector3.Zero;
			if(State == BotMoveStates.GOTO_PLAYER && ClosestPlayer != null && ClosestPlayer.IsValid) {
				if(ActiveChild == null || ActiveChild is BaseballBat) {
					target = ClosestPlayer.Position + (Position - ClosestPlayer.Position).Normal * 20f;
				} else {
					target = ClosestPlayer.Position + (Position - ClosestPlayer.Position).Normal * 150f;
				}
			}

			if(State == BotMoveStates.GOTO_MED && ClosestPickup != null && ClosestPickup.IsValid) {
				target = ClosestPickup.Position;
			}

			if(State == BotMoveStates.GOTO_GUN && ClosestWeapon != null && ClosestWeapon.IsValid) {
				target = ClosestWeapon.Position;
			}

			return target;
		}

		/// <summary>
		/// Handles Punching
		/// </summary>
		protected override async Task HandleMelee() {
			if(ShootAtPlayer) {
				if(TimeSinceMelee > 0.5f) {
					ResetTimeSinceMelee = true;
					await GameTask.DelaySeconds(0.1f);
					var forward = EyeRot.Forward;
					Vector3 pos = EyePos + Vector3.Down * 20f;
					var tr = Trace.Ray(pos, pos + forward * 40f)
					.UseHitboxes()
					.Ignore(this)
					.Size(20f)
					.Run();

					using(Prediction.Off()) {
						if(IsServer) {
							PlaySound("woosh");
						}
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
						if(IsServer) {
							PlaySound("smack");
						}
						await GameTask.DelaySeconds(0.2f);
						if(!(LifeState == LifeState.Alive)) return;

						if(tr.Entity is SpeedDialPlayer player) {
							player.CauseOfDeath = COD.Melee;
						}

						tr.Entity.TakeDamage(damage);
					}
				}
			}
		}

		public void HandleGunGrabbingThrowing() {
			TryGrabWeapon();
			if(ActiveChild != null && (ActiveChild as BaseSpeedDialWeapon).AmmoClip <= 0) {
				ThrowWeapon();
			}
		}

		public void TryGrabWeapon() {
			if(pickup && pickUpEntity != null && Input.ActiveChild == null) {
				Inventory?.Add(pickUpEntity, Inventory.Active == null);
				(pickUpEntity as BaseSpeedDialWeapon).GlowState = GlowStates.GlowStateOff;
				(pickUpEntity as BaseSpeedDialWeapon).GlowActive = false;
				pickup = false;
				pickUpEntity = null;
			}
		}

		public void ThrowWeapon() {
			var dropped = Inventory.DropActive();
			if(dropped != null) {
				ResetInterpolation();
				dropped.Position = EyePos;
				//dropped.Rotation = Rotation.Identity;
				if(dropped.PhysicsGroup != null) {
					if(dropped is BaseSpeedDialWeapon wep) {
						wep.ApplyThrowVelocity(EyeRot.Forward);
						wep.DespawnAfterTime = true;
						wep.GlowState = GlowStates.GlowStateOn;
						wep.GlowDistanceStart = 0;
						wep.GlowDistanceEnd = 1000;
						if(wep.AmmoClip > 0)
							wep.GlowColor = new Color(0.2f, 1, 0.2f, 1);
						else {
							if(wep.AmmoClip == -1)
								wep.GlowColor = new Color(1, 1, 1, 1);
							else
								wep.GlowColor = new Color(1, 0.2f, 0.2f, 1);
						}
						wep.GlowActive = true;
						PlaySound("weaponspin");
					}
				}

				timeSinceDropped = 0;
			}
			if(IsClient && ActiveChild != null) {
				PlaySound("weaponspin");
			}
		}

		public void UpdateClosests() {
			TimeSinceUpdate = 0;

			ClosestPlayer = GetClosestPlayer();
			ClosestWeapon = GetClosestWeapon();
			ClosestPickup = GetClosestPickup();
		}

		SpeedDialPlayer GetClosestPlayer() {

			SpeedDialPlayer bestTarget = null;
			float closestDistanceSqr = float.MaxValue;

			foreach(var potentialTarget in Client.All) {
				var potentialPawn = potentialTarget.Pawn as SpeedDialPlayer;
				if(potentialPawn == this)
					continue;

				Vector3 dirToTarget = potentialPawn.Position - Position;
				var dSqrToTarget = dirToTarget.LengthSquared;

				if(dSqrToTarget <= closestDistanceSqr) {
					closestDistanceSqr = dSqrToTarget;
					bestTarget = potentialPawn;
				}
			}

			return bestTarget;
		}

		BaseMedication GetClosestPickup() {

			BaseMedication bestTarget = null;
			float closestDistanceSqr = float.MaxValue;

			foreach(var potentialTarget in Entity.All.OfType<BaseMedication>()) {

				Vector3 dirToTarget = potentialTarget.Position - Position;
				var dSqrToTarget = dirToTarget.LengthSquared;

				if(dSqrToTarget <= closestDistanceSqr) {
					closestDistanceSqr = dSqrToTarget;
					bestTarget = potentialTarget;
				}
			}

			return bestTarget;
		}

		BaseSpeedDialWeapon GetClosestWeapon() {

			BaseSpeedDialWeapon bestTarget = null;
			float closestDistanceSqr = float.MaxValue;

			foreach(var potentialTarget in Entity.All.OfType<BaseSpeedDialWeapon>()) {
				if(potentialTarget.Parent != null)
					continue;
				Vector3 dirToTarget = potentialTarget.Position - Position;
				var dSqrToTarget = dirToTarget.LengthSquared;

				if(dSqrToTarget <= closestDistanceSqr) {
					closestDistanceSqr = dSqrToTarget;
					bestTarget = potentialTarget;
				}
			}

			return bestTarget;
		}
	}
}
