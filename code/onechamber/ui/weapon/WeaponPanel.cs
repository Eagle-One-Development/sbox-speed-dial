using SpeedDial.Classic.UI;
using SpeedDial.Classic.Weapons;
using SpeedDial.OneChamber.Player;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberWeaponPanel : WeaponPanel
{
	public override void Tick()
	{
		var weapon = (Game.LocalPawn as BasePlayer).ActiveChild as Weapon;

		// ammo
		{
			// clamp scale for Fire effect
			AmmoScale = AmmoScale.Clamp( 0, 1.5f );

			// text scaling
			PanelTransform transform = new();
			transform.AddScale( AmmoScale );
			AmmoLabel.Style.Transform = transform;

			// update ammo label number or scale down if no weapon
			if ( weapon is null )
			{
				if ( Game.LocalPawn is OneChamberPlayer player )
				{
					var gun = player.StashedGun as Weapon;
					if ( gun is not null )
					{
						AmmoLabel.Text = gun.Blueprint?.ClipSize < 0 ? $"" : Debug.InfiniteAmmo ? $"∞" : $"{gun.AmmoClip}";
					}
				}
			}
			else
			{
				AmmoLabel.Text = weapon.Blueprint?.ClipSize < 0 ? $"" : Debug.InfiniteAmmo ? $"∞" : $"{weapon.AmmoClip}";

				// lerp to normal scale
				AmmoScale = AmmoScale.LerpTo( 1, Time.Delta * 7f );
			}
		}

		// weapon name
		{
			WeaponLabel.Text = weapon is null ? $"FISTS" : $"{weapon.Blueprint.WeaponTitle}";
		}
	}
}
