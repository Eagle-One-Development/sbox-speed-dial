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
		var weapon = (Local.Pawn as BasePlayer).ActiveChild as Weapon;

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
				if ( Local.Pawn is OneChamberPlayer player )
				{
					var gun = player.StashedGun as Weapon;
					if ( gun is not null )
					{
						if ( gun.Blueprint?.ClipSize < 0 )
							AmmoLabel.Text = $"";
						else
						{
							if ( Debug.InfiniteAmmo )
								AmmoLabel.Text = $"∞";
							else
								AmmoLabel.Text = $"{gun.AmmoClip}";
						}
					}
				}
			}
			else
			{
				if ( weapon.Blueprint?.ClipSize < 0 )
					AmmoLabel.Text = $"";
				else
				{
					if ( Debug.InfiniteAmmo )
						AmmoLabel.Text = $"∞";
					else
						AmmoLabel.Text = $"{weapon.AmmoClip}";
				}

				// lerp to normal scale
				AmmoScale = AmmoScale.LerpTo( 1, Time.Delta * 7f );
			}
		}

		// weapon name
		{
			if ( weapon is null )
			{
				WeaponLabel.Text = $"FISTS";
			}
			else
			{
				WeaponLabel.Text = $"{weapon.Blueprint.WeaponTitle}";
			}
		}
	}
}
