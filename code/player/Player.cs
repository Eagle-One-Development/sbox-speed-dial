using System.Numerics;
using System.Diagnostics;
using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;


namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		private TimeSince TimeSinceDied { get; set; } = 0;

		[Net]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 PlayerColor { get; set; }

		[Net]
		public bool pickup { get; set; }
		private Entity pickUpEntity;

		TimeSince timeSinceDropped;

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public BaseSpeedDialCharacter character;

		public void InitialSpawn() {

			if(GetClientOwner().SteamId == 76561198000823482) {
				PlayerColor = new Color32(250, 176, 3); // bak
			} else if(GetClientOwner().SteamId == 76561198203314521) { // gurke
				PlayerColor = new Color32(70, 0, 70);
			} else if(GetClientOwner().SteamId == 76561198095231052) { // generic
				PlayerColor = new Color32(27, 49, 63);
			} else {
				PlayerColor = Color.Random;
			}

			//Set a default character
			character = SpeedDialGame.Instance.characters[0];

			Respawn();
		}

		public override void StartTouch(Entity other) {
			if(timeSinceDropped < 1) return;

			if(IsClient) return;

			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep1) {
					StartTouch(other.Parent);

					float magnitude = wep1.PhysicsBody.Velocity.Length;
					Log.Info($"Velocity: {magnitude}");
					if(magnitude > 450f) {

						wep1.PhysicsBody.EnableAutoSleeping = false;
						
						KillMyself(wep1.previousOwner);
						wep1.Velocity *= -0.5f;
					}
				}
				return;
			}
		}

		[ServerCmd]
		public void KillMyself(Entity attacker) {
			DamageInfo info = new DamageInfo();
			info.Damage = 200f;
			info.Attacker = attacker;
			info.Position = Position;
			TakeDamage(info);
			PlaySound( "weaponhit" );
		}

		public override void Touch(Entity other) {

			if(timeSinceDropped < 1f) return;

			if(IsClient) return;

			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep1) {
					Touch(other.Parent);
					pickup = true;
				}
				return;
			}
			pickUpEntity = other;
		}

		public override void EndTouch(Entity other) {
			base.EndTouch(other);
			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep1) {
					Touch(other.Parent);
					pickUpEntity = null;
					pickup = false;
				}
				return;
			}
		}

		public override void Respawn() {
			SetModel("models/biped_standard/biped_standard.vmdl");

			RenderColor = PlayerColor;

			Camera = new SpeedDialCamera();
			Controller = new SpeedDialController();
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Host.AssertServer();

			KillCombo = 0;

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
		public void IncreaseWeaponClip() {
			if(ActiveChild is BaseSpeedDialWeapon weapon) {
				if(IsClient)
					Log.Info("Updated clip on client in rpc");
				if(IsServer)
					Log.Info("Updated clip on server in rpc");
				weapon.AwardAmmo();
			}
		}

		[ClientRpc]
		public void BloodSplatter(Vector3 dir) {
			Vector3 pos = EyePos + Vector3.Down * 20;

			DebugOverlay.Line(Position, pos + dir, Color.Cyan, 10, false);

			// splatters around and behind the target, mostly from impact
			for(int i = 0; i < 10; i++) {

				// var forward = Owner.EyeRot.Forward;
				// forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
				// forward = forward.Normal;

				// TODO
				// proper distribution of blood behind and around the target
				var trDir = pos + (dir.Normal + (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 0.85f * 0.25f) * 100 + Vector3.Down * i;
				var trSplatter = Sandbox.Trace.Ray(pos, trDir)
						.UseHitboxes()
						.Ignore(this)
						.Size(1)
						.Run();

				DebugOverlay.Line(pos, trDir, Color.Green, 10, false);

				// FIXME
				// oops stupid path
				var decalPathSplatter = "materials/decals/blood/blood_splatter.decal";
				if(decalPathSplatter != null) {
					if(DecalDefinition.ByPath.TryGetValue(decalPathSplatter, out var decal)) {
						decal.PlaceUsingTrace(trSplatter);
					}
				}
			}

			//For blood splatter on the ground, pool of blood essentially

			// UPCOMING
			// Better and more decals for ground splatter
			var tr = Sandbox.Trace.Ray(pos, pos + Vector3.Down * 85f + Vector3.Random * 0.2f)
					.UseHitboxes()
					.Ignore(this)
					.Size(1)
					.Run();

			//DebugOverlay.Line(pos, tr.EndPos, Color.Red, 3f ,false);
			var decalPath = "decals/blood_test.decal";
			//var decalPath = Rand.FromArray(tr.Surface.ImpactEffects.BulletDecal);
			if(decalPath != null) {
				if(DecalDefinition.ByPath.TryGetValue(decalPath, out var decal)) {

					decal.PlaceUsingTrace(tr);
				}
			}

			// TODO
			// particles
			// particles
			// (water particle dyed red? blood impact particle looks lame)
		}

		public override void OnKilled() {
			Game.Current?.OnKilled(this);
			TimeSinceDied = 0;
			LifeState = LifeState.Dead;
			StopUsing();

			Inventory.DeleteContents();

			//Create the combo score on the client
			if(LastDamage.Attacker is SpeedDialPlayer attacker && attacker != this) {
				//attacker.ComboEvents(EyePos,(SpeedDialGame.ScoreBase * attacker.KillCombo));
				BloodSplatter(EyePos + Vector3.Down * 20 - (attacker.EyePos + Vector3.Down * 20));
			}

			BecomeRagdollOnClient(new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0));

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
		}

		[ClientRpc]
		public void GiveLoadout() {

		}

		public override void Simulate(Client cl) {
			if(LifeState == LifeState.Dead) {
				if(TimeSinceDied > RespawnTime && IsServer) {

					Respawn();
				}
				return;
			}

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			if(Input.Pressed(InputButton.Attack2)) {
				var dropped = Inventory.DropActive();
				if(dropped != null) {
					if(dropped.PhysicsGroup != null) {
						//dropped.PhysicsGroup.Velocity = Velocity + (EyeRot.Forward) * 500f;
						//dropped.PhysicsGroup.AngularVelocity = new Vector3( 0, 0, 100f );
						(dropped as BaseSpeedDialWeapon).ApplyThrowVelocity(EyeRot.Forward);
					}

					timeSinceDropped = 0;
				}
			}

			if(IsClient && pickUpEntity != null) {
				Log.Info(pickUpEntity.ToString());
			}
			if(Input.Pressed(InputButton.Attack2) && pickup && pickUpEntity != null && Input.ActiveChild == null) {
				Inventory?.Add(pickUpEntity, Inventory.Active == null);
				pickup = false;
				pickUpEntity = null;
				PlaySound( "weaponspin" );
			}

			if(IsClient) {
				AmmoPanel.Current.pickedup = 0f;
			}


			if(IsClient && pickup && Input.ActiveChild == null) {
				AmmoPanel.Current.pickedup = 1f;
			}

			SimulateActiveChild(cl, ActiveChild);
		}

		DamageInfo LastDamage;

		public override void TakeDamage(DamageInfo info) {
			LastDamage = info;

			base.TakeDamage(info);

			if(info.Attacker is SpeedDialPlayer attacker && attacker != this) {
				// Note - sending this only to the attacker!
				attacker.DidDamage(To.Single(attacker), info.Position, info.Damage, Health);

				TookDamage(To.Single(this), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position);
			}
		}

		/// <summary>
		/// A client side function for client side effects when the player has done damage
		/// </summary>
		[ClientRpc]
		public void DidDamage(Vector3 pos, float amount, float healthinv) {
			//Sound.FromScreen( "dm.ui_attacker" )
			//	.SetPitch( 1 + healthinv * 1 );
			//	
			//HitIndicator.Current?.OnHit( pos, amount );
			if(healthinv <= 0) {
				Log.Info("AYYY");
				int ScoreBase = SpeedDialGame.ScoreBase;
				ComboEvents(pos, ScoreBase * KillCombo);
			}
		}

		[ClientRpc]
		public void TookDamage(Vector3 pos) {
			//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );

			//DamageIndicator.Current?.OnHit( pos );
		}
	}
}
