using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Player;

public partial class ClassicPlayer
{
	private static readonly EntityLimit RagdollLimit = new() { MaxTotal = 20 };
	public ModelEntity Corpse { get; set; }
	[Net] public CauseOfDeath DeathCause { get; set; } = CauseOfDeath.Generic;

	public enum CauseOfDeath
	{
		Generic, // any damage that isn't one of the below
		Bullet, // guns
		Melee, // for melee weapons (bat)
		Punch, // for fists
		Impact, // thrown guns
		Suicide // kill command
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		DrugParticles?.Destroy( true );
	}

	public override void OnKilled()
	{
		Frozen = true;

		EnableAllCollisions = false;
		EnableDrawing = false;

		ActiveDrug = false;

		DrugParticles?.Destroy( true );

		// chuck weapon away in a random direction
		DropWeapon( out var weapon );
		if ( weapon.IsValid() )
		{
			weapon.Velocity += (Vector3.Random.WithZ( 0 ).Normal * 150) + (Vector3.Up * 150);
			weapon.PhysicsBody.AngularVelocity = new Vector3( 0, 0, 60f );
			weapon.CanImpactKill = false;
		}

		// death effects, body + particles/decals
		BecomeRagdollOnClient( To.Everyone, new Vector3( Velocity.x / 2, Velocity.y / 2, 300 ), GetHitboxBone( 0 ) );
		BloodSplatter( To.Everyone );
		SoundFromScreen( To.Single( Client ), "player_death" );

		// give the killer his score etc during gameplay
		if ( LastRecievedDamage.Attacker is ClassicPlayer attacker )
		{
			attacker.TimeSinceMurdered = 0;
			attacker.AwardKill( this );
			// TODO: find better kill confirm sound
			//SoundFromScreen(To.Single(attacker.Client), "kill_confirm");
		}

		if ( LastRecievedDamage.Weapon is Weapon wep )
		{
			// HACK. this could be done better... too bad!
			DeathCause = wep.Blueprint.Special == WeaponSpecial.Melee ? CauseOfDeath.Melee : CauseOfDeath.Bullet;
		}

		// reset combo
		Client.SetValue( "combo", 0 );

		base.OnKilled();
	}

	public override void OnClientDisconnected()
	{
		// chuck weapon away in a random direction
		DropWeapon( out var weapon );
		if ( weapon.IsValid() )
		{
			weapon.Velocity += (Vector3.Random.WithZ( 0 ).Normal * 150) + (Vector3.Up * 150);
			weapon.PhysicsBody.AngularVelocity = new Vector3( 0, 0, 60f );
			weapon.CanImpactKill = false;
		}

		// death effects, body + particles/decals
		BecomeRagdollOnClient( To.Everyone, new Vector3( Velocity.x / 2, Velocity.y / 2, 300 ), GetHitboxBone( 0 ) );
		BloodSplatter( To.Everyone );

		DrugParticles?.Destroy( true );
	}

	[ClientRpc]
	protected void BecomeRagdollOnClient( Vector3 force, int forceBone )
	{

		ModelEntity ent = new();
		ent.Position = Position;
		ent.Rotation = Rotation;

		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		ent.UsePhysicsCollision = true;
		Tags.Add( "debris" );

		ent.Model = Model;

		ent.CopyBonesFrom( this );
		ent.TakeDecalsFrom( this );
		ent.SetRagdollVelocityFrom( this );
		ent.DeleteAsync( 20.0f );

		ent.PhysicsGroup.AddVelocity( force );

		if ( forceBone >= 0 )
		{
			var body = ent.GetBonePhysicsBody( forceBone );
			if ( body != null )
			{
				body.ApplyForce( force * 1000 );
			}
			else
			{
				ent.PhysicsGroup.AddVelocity( force );
			}
		}

		Corpse = ent;

		RagdollLimit.Watch( ent );
	}

	[ClientRpc]
	public void BloodSplatter()
	{
		Host.AssertClient();
		BloodSplatter( Vector3.Down );
	}

	[ClientRpc]
	public void BloodSplatter( Vector3 dir )
	{
		Host.AssertClient();
		Vector3 pos = EyePosition + (Vector3.Down * 20);

		// splatters around and behind the target, mostly from impact
		for ( int i = 0; i < 10; i++ )
		{
			var trDir = pos + ((dir.Normal + ((Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 0.85f * 0.25f)) * 100) + (Vector3.Down * i);
			var tr = Trace.Ray( pos, trDir )
					.UseHitboxes()
					.Ignore( this )
					.Size( 1 )
					.Run();

			_ = CreateDecalAsync( "decals/blood_splatter.decal", tr, i * 0.05f );
		}

		//For blood splatter on the ground, pool of blood essentially
		for ( int i = 0; i < 5; i++ )
		{
			var trDir = pos + ((Vector3.Down + ((Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 3 * 0.25f)) * 100);
			var tr = Trace.Ray( pos, trDir )
					.WorldOnly()
					.Ignore( this )
					.Size( 1 )
					.Run();

			_ = CreateDecalAsync( "decals/blood_splatter_floor.decal", tr, i * 0.05f );
		}

		//For blood detail splatters on the ground
		for ( int i = 0; i < 5; i++ )
		{
			var trDir = pos + ((Vector3.Down + ((Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 3 * 0.25f)) * 100);
			var tr = Trace.Ray( pos, trDir )
					.WorldOnly()
					.Ignore( this )
					.Size( 1 )
					.Run();

			_ = CreateDecalAsync( "decals/blood_splatter.decal", tr, i * 0.1f );
		}

		// three slightly different particle effects, splash will be the most noticeable 
		_ = CreateParticleAsync( "particles/blood/blood_splash.vpcf", Corpse, dir.Normal, 0, "head" );

		_ = CreateParticleAsync( "particles/blood/blood_drops.vpcf", Corpse, Vector3.Down, 0.5f, "head", false, true, 3 );

		_ = CreateParticleAsync( "particles/blood/blood_plip.vpcf", Corpse, Vector3.Down, 0, "head", true );
	}

	private async Task CreateDecalAsync( string decalname, TraceResult tr, float delay = 0 )
	{
		await GameTask.DelaySeconds( delay );

		var decalPath = decalname;
		if ( decalPath != null )
		{
			if ( ResourceLibrary.TryGet<DecalDefinition>( decalPath, out var decal ) )
			{
				DecalSystem.PlaceUsingTrace( decal, tr );
			}
		}
	}

	private async Task CreateParticleAsync( string particle, Entity entity, Vector3 forward, float delay = 0, string bone = "root", bool attach = false, bool bloodpool = false, int pools = 1 )
	{
		await GameTask.DelaySeconds( delay );
		if ( entity is ModelEntity ent )
		{
			var boneBody = ent.GetBonePhysicsBody( ent.GetBoneIndex( bone ) );
			var ps = Particles.Create( particle, boneBody.Position );
			ps.SetForward( 0, forward );
			if ( attach )
				ps.SetEntityAttachment( 0, entity, "head_blood", true );
			if ( bloodpool )
			{
				for ( int i = 0; i < pools; i++ )
				{
					await GameTask.DelaySeconds( i * 0.1f );
					var trDir = boneBody.Position + (Vector3.Down * 1000);
					var tr = Trace.Ray( boneBody.Position, trDir )
							.WorldAndEntities()
							.UseHitboxes()
							.Ignore( this )
							.Size( 1 )
							.Run();

					_ = CreateDecalAsync( "decals/blood_splatter_floor.decal", tr, 0.5f );
				}
			}
		}
	}
}
