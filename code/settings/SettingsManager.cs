using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sandbox;
using SpeedDial.Player;

namespace SpeedDial.Settings {
	public static class SettingsManager {

		private const string SettingsPath = "speed-dial";
		// the Setting Version. Should be changed when adding new Settings. so it updates on downloaded versions too.
		private const string Version = "3";

		public static SettingInstance Settings { get; set; }
		public static Action SettingsChanged;

		public static void ReloadSettings() {
			if(Host.IsServer) return;
			if(!FileSystem.OrganizationData.DirectoryExists(SettingsPath)) {
				FileSystem.OrganizationData.CreateDirectory(SettingsPath);
			}
			if(!FileSystem.OrganizationData.FileExists(SettingsPath + "/setting.json")) {
				SetDefaultSettings();
			}
			Settings = JsonSerializer.Deserialize<SettingInstance>(FileSystem.OrganizationData.ReadAllText(SettingsPath + "/setting.json"));
			if(Settings.Version is null || !Settings.Version.Equals(Version)) SetDefaultSettings();
			//Log.Info("Settings Loaded");
			Event.Run("SDEvents.Settings.Changed");
		}
		public static void SetDefaultSettings() {
			if(Settings == null) Settings = new();
			Settings.Version = Version;
			Settings.SettingsItems = new();
			SetSetting("Bot Difficulty", BotDifficulties.Medium, true);
			SetSetting("Sniper Wallbang", true, true);
			SetSetting("Kill Feed", true);
			SetSetting("Music On", true);
			SetSetting("Viewshift Toggle", false);
			//SetSetting("Music Volume", 100);
			//SetSetting("SFX On", true);
			//SetSetting("SFX Volume", 100);
			//Log.Info("Setting Default Settings");
			SaveSettings();
		}
		public static bool SaveSettings() {
			if(Host.IsServer) return false;
			if(!FileSystem.OrganizationData.DirectoryExists(SettingsPath)) {
				FileSystem.OrganizationData.CreateDirectory(SettingsPath);
			}
			if(Settings is null) {
				ReloadSettings();
			}
			FileSystem.OrganizationData.WriteAllText(SettingsPath + "/setting.json", JsonSerializer.Serialize(Settings));
			ReloadSettings();
			return true;
		}
		public static SettingsItem GetSetting(string PropertyName) {
			if(Settings is null) ReloadSettings();
			if(Settings.SettingsItems.TryGetValue(PropertyName, out SettingsItem it)) return it;
			return null;
		}
		public static void SetSetting(string PropertyName, object SettingValue, bool ServerOnly = false) {
			if(Settings is null) Settings = new();
			Settings.Version = Version;
			SettingsItem it = new(PropertyName, SettingValue, ServerOnly);
			if(Settings.SettingsItems is null)
				Settings.SettingsItems = new() {
					{ it.PropertyName, it }
				};
			else {
				if(Settings.SettingsItems.ContainsKey(PropertyName)) {
					Settings.SettingsItems[PropertyName] = it;
					return;
				}
				Settings.SettingsItems.Add(PropertyName, it);
			}
		}

	}
	public class SettingInstance {
		public string Version { get; set; }
		public Dictionary<string, SettingsItem> SettingsItems { get; set; }
	}

	public class SettingsItem {
		public string PropertyName { get; set; }
		public bool ServerOnly { get; set; }
		public string TypeofValue { get; set; }
		public object SettingValue { get; set; }
		public SettingsItem(string PropertyName, object SettingValue, bool ServerOnly = false, string TypeofValue = null) {
			this.PropertyName = PropertyName;
			this.SettingValue = SettingValue;
			this.ServerOnly = ServerOnly;
			this.TypeofValue = SettingValue.GetType().ToString();
			if(TypeofValue != null) this.TypeofValue = TypeofValue;
		}

		public bool TryGetBool(out bool? result) {
			result = null;
			if(SettingValue is JsonElement je) {
				result = je.GetBoolean();
				return true;
			}
			if(bool.TryParse(SettingValue.ToString(), out bool res)) {
				result = res;
				return true;
			}
			return false;
		}
		public bool TryGetInt(out int? result) {
			result = null;
			if(SettingValue is JsonElement je) {
				result = je.GetInt32();
				return true;
			}
			if(int.TryParse(SettingValue.ToString(), out int res)) {
				result = res;
				return true;
			}
			return false;
		}
		public bool TryGetFloat(out float? result) {
			result = null;
			if(SettingValue is JsonElement je) {
				result = je.GetSingle();
				return true;
			}
			if(float.TryParse(SettingValue.ToString(), out float res)) {
				result = res;
				return true;
			}
			return false;
		}
		public override string ToString() {
			return $"{PropertyName}:: Type {TypeofValue} :: Is Server Setting? {ServerOnly} :: {SettingValue}";
		}
	}
}
