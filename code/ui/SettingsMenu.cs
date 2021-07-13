using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.Internal;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;
using SpeedDial.Player;
using SpeedDial.Settings;

namespace SpeedDial.UI {
	public class SettingsMenu : Panel {

		public SettingsMenu() {

			StyleSheet.Load("/ui/SettingsMenu.scss");
			ReloadSettings();
		}
		public override void OnHotloaded() {
			base.OnHotloaded();
			ReloadSettings();
		}
		public void ReloadSettings() {
			DeleteChildren(true);
			var label = AddChild<Label>("SettingsText");
			label.Text = "Settings";
			if(SettingsManager.Settings is null) SettingsManager.ReloadSettings();
			foreach(var item in SettingsManager.Settings.SettingsItems) {
				if(Client.All[0].NetworkIdent != Local.Client.NetworkIdent && item.Value.ServerOnly) continue;
				SettingsPanel sp = new();
				if(item.Value.ServerOnly) sp.Add.Label("Host only", "serveronly");
				sp.init(item.Value);
				sp.AddClass("SettingItem");
				AddChild(sp);

			}
		}

	}
	public class SettingsPanel : Panel {

		public enum PanelType {
			Float,
			Int,
			Bool,
			String
		}
		public delegate PanelType GetPanelType();

		public static Dictionary<Type, GetPanelType> TypeDictionary = new() {
			{
				typeof(float),
				() => {
					return PanelType.Float;
				}
			},
			{
				typeof(int),
				() => {
					return PanelType.Int;
				}
			},
			{
				typeof(bool),
				() => {
					return PanelType.Bool;
				}
			},
			{
				typeof(string),
				() => {
					return PanelType.String;
				}
			}
		};

		SettingsItem SettingsItem;
		PanelType ownPanelType;

		Label settingLabel;

		SliderEntry sliderEntry;
		Checkbox checkbox;
		TextEntry textEntry;

		public void init(SettingsItem item) {
			SettingsItem = item;
			Type t = Type.GetType(item.TypeofValue);
			//Log.Info(t.ToString());
			ownPanelType = TypeDictionary[t]();
			settingLabel = AddChild<Label>("SettingItemLabel");
			settingLabel.SetText(item.PropertyName);
			switch(ownPanelType) {
				case PanelType.Int:
					sliderEntry = AddChild<SliderEntry>("Int");
					break;
				case PanelType.Bool:
					checkbox = AddChild<Checkbox>("bool");
					break;
				case PanelType.String:
					textEntry = AddChild<TextEntry>("string");
					break;
				default:
					break;
			}

			if(sliderEntry is not null) {
				sliderEntry.MinValue = 0f;
				sliderEntry.MaxValue = 100f;
				item.TryGetInt(out int? res);
				sliderEntry.Value = res.Value;
			}
			if(checkbox is not null) {
				item.TryGetBool(out bool? res);
				checkbox.Checked = res.Value;
				AddEventListener("onchange", (v) => onChanged(v));
			}
		}
		public override void Tick() {
			base.Tick();
			if(sliderEntry is not null) SettingsItem.SettingValue = sliderEntry.Value;
		}

		private void onChanged(PanelEvent value) {
			if(value.Value != null) SettingsItem.SettingValue = value.Value;
		}
	}

}
