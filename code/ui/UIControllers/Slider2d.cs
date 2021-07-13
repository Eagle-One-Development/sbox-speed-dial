using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Sandbox.UI
{
	/// <summary>
	/// A horizontal slider. Can be float or whole number.
	/// </summary>
	public class Slider2D : Panel
	{
		public Panel Track { get; protected set; }
		public Panel Thumb { get; protected set; }

		public Slider2D()
		{
			AddClass( "slider2d" );

			Track = Add.Panel( "track" );
			Thumb = Add.Panel( "thumb" );
		}

		protected Vector2 _value;

		/// <summary>
		/// The actual value. Setting the value will snap and clamp it.
		/// </summary>
		[Property]
		public Vector2 Value 
		{
			get => _value;
			set
			{
				var snapped = value;
				//snapped = snapped.Clamp( MinValue, MaxValue );

				if ( _value == snapped ) return;

				_value = snapped;

				CreateValueEvent( "value", _value );

				if ( !HasActive )
				{
					UpdateSliderPositions();
				}
			}
		}

		public override void SetProperty( string name, string value )
		{
			base.SetProperty( name, value );
		}


		/// <summary>
		/// Convert a screen position to a value. The value is clamped, but not snapped.
		/// </summary>
		public virtual Vector2 ScreenPositionToValue( Vector2 pos )
		{
			var localPos = ScreenPositionToPanelDelta( pos );

			var x = MathX.Clamp( localPos.x, 0, 1 );
			var y = MathX.Clamp( localPos.y, 0, 1 );

			return new Vector2( x, 1 - y );
		}

		/// <summary>
		/// If we move the mouse while we're being pressed then set the position,
		/// but skip transitions.
		/// </summary>
		protected override void OnMouseMove( MousePanelEvent e )
		{
			base.OnMouseMove( e );

			if ( !HasActive ) return;

			Value = ScreenPositionToValue( Mouse.Position );
			UpdateSliderPositions();
			SkipTransitions();

			e.StopPropagation();
		}

		/// <summary>
		/// On mouse press jump to that position
		/// </summary>
		protected override void OnMouseDown( MousePanelEvent e )
		{
			base.OnMouseDown( e );

			Value = ScreenPositionToValue( Mouse.Position );
			UpdateSliderPositions();
		}

		int positionHash;

		/// <summary>
		/// Updates the styles for TrackInner and Thumb to position us based on the current value.
		/// Note this purposely uses percentages instead of pixels when setting up, this way we don't
		/// have to worry about parent size, screen scale etc.
		/// </summary>
		void UpdateSliderPositions()
		{
			var hash = HashCode.Combine( Value );
			if ( hash == positionHash ) return;

			positionHash = hash;

			Thumb.Style.Left = Length.Fraction( Value.x );
			Thumb.Style.Top = Length.Fraction( 1 - Value.y );

			Thumb.Style.Dirty();

			Style.PaddingRight = 0;// Thumb.Box.Rect.width * ScaleFromScreen * 0.5f;
			Style.PaddingBottom = 0;// Thumb.Box.Rect.height * ScaleFromScreen * 0.5f;
		}

	}
}
