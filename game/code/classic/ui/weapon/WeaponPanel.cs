﻿using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.UI;

public partial class WeaponPanel
{
	public static WeaponPanel Current { get; private set; }

	public Label AmmoLabel { get; set; }
	public Label WeaponLabel { get; set; }
	public Panel Panel { get; set; }

	protected float AmmoScale = 1;

	public WeaponPanel()
	{
		Current = this;
	}

	[ClientRpc]
	public static void Fire( float scale )
	{
		if ( Current is null ) return;
		Current.AmmoScale += 0.2f * scale;
	}

	[ClientRpc]
	public static void Fire()
	{
		if ( Current is null ) return;
		Current.AmmoScale += 0.2f;
	}

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
				AmmoScale = AmmoScale.LerpTo( 0, Time.Delta * 7f );
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
