using Sandbox;
using System;

namespace SpeedDial.Weapons{
[Library( "sd_tac", Title = "TAC-7" )]
partial class Tac7 : BaseSpeedDialWeapon
{ 


	public override float PrimaryRate => 2.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 15;
	public override int Bucket => 2;
	

	public int burst = 3;
	private int curBurst = 0;
	private bool isFiring;
	public TimeSince burstTimer;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
		AmmoClip = 15;
	}

	public override void Simulate(Client owner) {

			base.Simulate(owner);

			if ( isFiring )
			{
			if ( burstTimer > 0.05f && curBurst < burst )
			{
				curBurst++;
				if ( !TakeAmmo( 1 ) )
				{
					DryFire();
					burstTimer = 0;
					curBurst = 0;
					isFiring = false;
					return;
				}

				(Owner as AnimEntity).SetAnimParam( "b_attack", true );

				//
				// Tell the clients to play the shoot effects
				//
				ShootEffects();
				PlaySound( "rust_smg.shoot" );
				ShootBullet( 0.025f * (float)((curBurst * 1.5) + 1), 1.5f, 35f, 3.0f );
				burstTimer = 0;
			}

			if ( curBurst >= burst )
			{
				burstTimer = 0;
				curBurst = 0;
				isFiring = false;
			}
		}
		
		}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !isFiring )
		{
			isFiring = true;

		}

	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
		}

		ViewModelEntity?.SetAnimParam( "fire", true );
		CrosshairPanel?.OnEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

}
}
