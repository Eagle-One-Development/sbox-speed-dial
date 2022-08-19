using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Player;

public partial class ClassicPlayer
{
	[Net] public Weapon PickupWeapon { get; set; }
	[Net] public TimeSince TimeSincePickup { get; set; }
	[Net] public TimeSince TimeSinceWeaponCarried { get; set; }
	[Net] public bool Pickup { get; set; }

	public virtual void ThrowWeapon()
	{
		if ( IsServer )
		{
			if ( !DropWeapon( out Weapon dropped ) ) return;

			dropped.Position = EyePosition;
			dropped.ResetInterpolation();
			if ( dropped.PhysicsGroup != null && dropped is Weapon weapon )
			{
				weapon.PhysicsBody.Velocity += EyeRotation.Forward * 700;
				weapon.PhysicsBody.AngularVelocity = new Vector3( 0, 0, 100f );
				using ( Prediction.Off() )
				{
					weapon.PlaySound( "weaponspin" );
				}
			}
		}
	}

	/// <summary>
	/// Give the player a weapon
	/// </summary>
	/// <param name="weaponclass">Library class name of the weapon</param>
	/// <param name="drop">Whether to drop the old weapon or simply override it.</param>
	public void GiveWeapon( string weaponclass, bool drop = false )
	{
		if ( !WeaponBlueprint.Exists( weaponclass ) ) return;
		var weapon = WeaponBlueprint.Create( weaponclass );
		if ( weapon is null ) return;

		if ( drop )
		{
			DropWeapon();
		}
		else
		{
			if ( ActiveChild is not null )
			{
				var oldwep = ActiveChild;
				oldwep.Delete();
				ActiveChild = null;
			}
		}

		weapon.Parent = this;
		weapon.OnCarryStart( this );
		ActiveChild = weapon;
	}

	/// <summary>
	/// Drop the current weapon of the player.
	/// </summary>
	/// <param name="weapon">[out] the dropped weapon</param>
	/// <returns>true if we were able to drop a weapon, false otherwise</returns>
	public bool DropWeapon( out Weapon weapon )
	{
		weapon = DropWeapon();
		return weapon is not null && weapon.IsValid();
	}

	/// <summary>
	/// Drop the current weapon of the player
	/// </summary>
	/// <returns></returns>
	public Weapon DropWeapon()
	{
		if ( ActiveChild is null || ActiveChild is not Weapon weapon )
		{
			return null;
		}
		weapon.Parent = null;
		weapon.OnCarryDrop( this );

		if ( IsServer )
		{
			weapon.Position = EyePosition;
			weapon.Velocity = Velocity * 0.75f;
			weapon.ResetInterpolation();
		}
		ActiveChild = null;

		return weapon;
	}
}
