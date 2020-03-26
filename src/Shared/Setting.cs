using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class Setting
    {
        public Setting() { }
        public Setting(string settingId, string settingValue, string help)
        {
            SettingId = settingId;
            SettingValue = settingValue;
            Help = help;
        }

        public string SettingId { get; set; }
        public string Help { get; set; }
        public string SettingValue { get; set; }
    }
}
