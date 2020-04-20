using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class Setting
    {
        public Setting() { }
        public Setting(string settingId, string settingValue, string help, bool isAdmin = false)
        {
            SettingId = settingId;
            SettingValue = settingValue;
            Help = help;
            IsAdmin = isAdmin;
        }

        public string SettingId { get; set; }
        public string Help { get; set; }
        public string SettingValue { get; set; }
        public bool IsAdmin { get; set; }
    }
}
