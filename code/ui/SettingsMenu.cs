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


		}
		public override void OnHotloaded() {
			base.OnHotloaded();
			ReloadSettings();
		}
		public void ReloadSettings() {
			DeleteChildren(true);
			FindRootPanel().StyleSheet.Load("/ui/SettingsMenu.scss");
			var label = AddChild<Label>("SettingsText");
			label.Text = "Settings";
			if(SettingsManager.Settings is null) SettingsManager.ReloadSettings();
			if(Global.IsListenServer) {
				Panel sp = new();
				sp.AddClass("SettingItem");
				sp.Add.Label("Host only", "serveronly");
				sp.Add.Label("Add Bot", "SettingItemLabel");

				sp.Add.Button("Add New Bot", "botbutton", () => {
					SpeedDialGame.AddBot();
				});

				AddChild(sp);
			}
			foreach(var item in SettingsManager.Settings.SettingsItems) {
				if(!Global.IsListenServer && item.Value.ServerOnly) continue;
				SettingsPanel sp = new();
				if(item.Value.ServerOnly) sp.Add.Label("Host only", "serveronly");
				sp.init(item.Value);
				sp.AddClass("SettingItem");
				AddChild(sp);

			}
		}
		[Event("SDEvents.Settings.Changed")]
		public void OnSettingChanged() {
			if(Local.Client.IsListenServerHost && SettingsManager.GetSetting("Bot Difficulty").TryGetInt(out int? val)) {
				SpeedDialGame.SetBotDifficulty((BotDifficulties)val.Value);
			}
		}


	}
	public class SettingsPanel : Panel {

		public enum PanelType {
			Float,
			Int,
			Bool,
			String,
			Enum
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
			},
			{
				typeof(Enum),
				() => {
					return PanelType.Enum;
				}
			}
		};

		SettingsItem SettingsItem;
		PanelType ownPanelType;

		Label settingLabel;

		SliderEntry sliderEntry;
		Checkbox checkbox;
		TextEntry textEntry;

		DropDown dropDown;

		public void init(SettingsItem item) {
			SettingsItem = item;
			Type t = Type.GetType(item.TypeofValue);
			//Log.Info(t.ToString());
			if(t.IsEnum)
				ownPanelType = PanelType.Enum;
			else
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
				case PanelType.Enum:
					dropDown = AddChild<DropDown>("dropdown");
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
				checkbox.AddEventListener("onchange", (v) => onChanged(v));
			}
			if(dropDown is not null && item.TryGetInt(out int? val)) {

				string[] array = t.GetEnumNames();
				for(int i = 0; i < array.Length; i++) {
					string titles = array[i];
					dropDown.Options.Add(new(titles, i));
				}
				dropDown.Selected = dropDown.Options[val.Value];

				dropDown.AddEventListener("value.changed", (v) => onChanged(v));
			}
		}
		public override void Tick() {
			base.Tick();
			if(sliderEntry is not null) SettingsItem.SettingValue = sliderEntry.Value;
		}

		private void onChanged(PanelEvent value) {
			//Log.Error(value.Value);
			if(value.Value != null) SettingsItem.SettingValue = value.Value;
		}
	}

}
