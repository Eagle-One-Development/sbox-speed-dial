using Sandbox;
using System;
using System.Linq;
using SpeedDial.Weapons;

namespace SpeedDial.Player
{
    public partial class SpeedDialInventory : BaseInventory
    {
    	public SpeedDialInventory( Sandbox.Player player ) : base ( player )
		{

		}

		public override bool Add( Entity ent, bool makeActive = false )
		{
			var player = Owner as SpeedDialPlayer;
			var weapon = ent as BaseSpeedDialWeapon;

			//
			// We don't want to pick up the same weapon twice
			// But we'll take the ammo from it Winky Face
			//
			if ( weapon != null && IsCarryingType( ent.GetType() ) )
			{
				var ammo = weapon.AmmoClip;
				var ammoType = weapon.AmmoType;

				if ( ammo > 0 )
				{
					player.GiveAmmo( ammoType, ammo );
				}


				// Despawn it
				ent.Delete();
				return false;
			}

			return base.Add( ent, makeActive );
		}

		public bool IsCarryingType( Type t )
		{
			return List.Any( x => x.GetType() == t );
		}
    }
}