using System.Numerics;

using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		public TimeSince TimeSinceDied { get; set; } = 0;

		[Net, Local]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 PlayerColor { get; set; }

		[Net]
		public bool pickup { get; set; }
		private Entity pickUpEntity;

		

		TimeSince timeSinceDropped;

		[Net, Predicted]
		public TimeSince timeSinceMelee { get; set; }

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

			Controller = new SpeedDialController();
			Camera = new SpeedDialCamera();

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
					//Log.Info($"Velocity: {magnitude}");
					if(magnitude > 450f) {
						wep1.PhysicsBody.EnableAutoSleeping = false;
						Sound.FromEntity("weaponhit", this);
						KillMyself(wep1.previousOwner);
						wep1.Velocity *= -0.5f;
						
					}
				}
				return;
			}
		}

		public override void Touch(Entity other) {

			if(timeSinceDropped < 1f) return;

			if(IsClient) return;

			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
					Touch(other.Parent);
					pickup = true;
				}
				return;
			}
			pickUpEntity = other;
		}

		public override void EndTouch(Entity other) {
			base.EndTouch(other);
			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
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

			(Camera as SpeedDialCamera).Freeze = false;
			(Controller as SpeedDialController).Freeze = false;
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
		public void GiveLoadout() {

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
		public void HandleMelee()
		{
			
			if(Input.ActiveChild == null)
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					if(timeSinceMelee > 0.33f )
					{
						timeSinceMelee = 0;
						var forward = EyeRot.Forward;
						Vector3 pos = EyePos + Vector3.Down * 20f;
						var tr = Trace.Ray( pos, pos + forward * 40f )
						.UseHitboxes()
						.Ignore( this )
						.Size( 20f )
						.Run();

						PlaySwoosh();
						

						//DebugOverlay.Line( EyePos + Vector3.Down * 20, tr.EndPos, Color.White, 1, false );

						if ( !IsServer ) return;
						if ( !tr.Entity.IsValid() ) return;

						// We turn predictiuon off for this, so any exploding effects don't get culled etc
						using ( Prediction.Off() )
						{
							var damage = DamageInfo.FromBullet( tr.EndPos, Owner.EyeRot.Forward * 100, 200 )
								.UsingTraceResult( tr )
								.WithAttacker( Owner )
								.WithWeapon( this );

							damage.Attacker = this;
							damage.Position = Position;
							PlayClientSound( "punch_connect_1" );
							PlaySound( "punch_connect_1" );

							tr.Entity.TakeDamage( damage );
						}


					}
				}
			}
		}

		[ClientRpc]
		public void PlaySwoosh()
		{
			float f = Rand.Float( 1 );
			if(f > 0.5f )
			{
				PlaySound( "punch_woosh_1" );
			}
			else
			{
				PlaySound( "punch_woosh_2" );
			}
		}

		
		public void PlayClientSound(string s)
		{
			PlaySound( s );
		}

		public override void Simulate( Client cl ) {
			if ( LifeState == LifeState.Dead ) {
				if ( TimeSinceDied > RespawnTime && IsServer ) {

					Respawn();
				}
				return;
			}

			if ( KillCombo > maxCombo )
			{
				maxCombo = KillCombo;
			}

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			if ( ActiveChild == null )
			{
				HandleMelee();
			}

			if(Input.Pressed(InputButton.Attack2)) {
				var dropped = Inventory.DropActive();
				if(dropped != null) {
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
