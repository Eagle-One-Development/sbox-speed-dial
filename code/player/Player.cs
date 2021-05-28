using Sandbox;
using SpeedDial.Weapons;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		private TimeSince TimeSinceDied { get; set; } = 0;

		[Net]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 PlayerColor { get; set; }

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public void InitialSpawn() {

			if(GetClientOwner().SteamId == 76561198000823482) {
				PlayerColor = new Color32(250, 176, 3); // bak
			} else if(GetClientOwner().SteamId == 76561198203314521) { // gurke 76561198203314521
				PlayerColor = new Color32(70, 0, 70);
			} else if(GetClientOwner().SteamId == 76561198095231052) { // generic
				PlayerColor = new Color32(27, 49, 63);
			} else {
				PlayerColor = Color.Random;
			}
			Respawn();
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

			Inventory.Add(new Pistol(), true);

			GiveAmmo(AmmoType.Pistol, 1000);

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);
		}

		public override void OnKilled() {
			Game.Current?.OnKilled(this);

			//force and bone, fix later with damage stuff in place

			var tr = Trace.Ray(Position, Position + Vector3.Down * 500)
					.UseHitboxes()
					.Ignore(this)
					.Size(1)
					.Run();

			// fuck the current decal stuff, this doesn't work

			//DebugOverlay.Sphere(tr.EndPos, 3, Color.Green, false, 10);

			//var decalPath = "decals/bullet-metal.decal";
			var decalPath = Rand.FromArray(tr.Surface.ImpactEffects.BulletDecal);
			if(decalPath != null) {
				if(DecalDefinition.ByPath.TryGetValue(decalPath, out var decal)) {
					Log.Info("DECAL");
					decal.PlaceUsingTrace(tr);
				}
			}

			if ( LastDamage.Attacker is SpeedDialPlayer attacker && attacker != this )
			{
				int ScoreBase = SpeedDialGame.ScoreBase;
				attacker.ComboEvents(EyePos,(ScoreBase * attacker.KillCombo));

			}


			//tr.Surface.DoBulletImpact(tr);

			BecomeRagdollOnClient(new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0));

			Inventory.DeleteContents();

			TimeSinceDied = 0;
			LifeState = LifeState.Dead;

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
		}

		public override void Simulate(Client cl) {
			if(LifeState == LifeState.Dead) {
				if(TimeSinceDied > RespawnTime && IsServer) {

					Respawn();
				}
				return;
			}

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			
			SimulateActiveChild(cl, ActiveChild);

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());

			//DebugOverlay.ScreenText(new Vector2(300, 300), 1, Color.Green, $"{KillCombo}x {KillScore} Score {TimeSinceMurdered}\ts");
		}

		DamageInfo LastDamage;

		public override void TakeDamage( DamageInfo info )
		{
			LastDamage = info;


			base.TakeDamage( info );

			if ( info.Attacker is SpeedDialPlayer attacker && attacker != this )
			{
				// Note - sending this only to the attacker!
				attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, ((float)Health).LerpInverse( 100, 0 ) );

				TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
			}
		}


		/// <summary>
		/// A client side function for client side effects when the player has done damage
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="amount"></param>
		/// <param name="healthinv"></param>
		[ClientRpc]
		public void DidDamage( Vector3 pos, float amount, float healthinv )
		{
			//Sound.FromScreen( "dm.ui_attacker" )
			//	.SetPitch( 1 + healthinv * 1 );
	//
			//HitIndicator.Current?.OnHit( pos, amount );
		}


		[ClientRpc]
		public void TookDamage( Vector3 pos )
		{
			//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );

			//DamageIndicator.Current?.OnHit( pos );
		}
	}
}
