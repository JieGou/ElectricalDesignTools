using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.ProjectSettings;
public class TagManager
{
    public static ObservableCollection<TagModel> TagSettingList { get; set; } = new ObservableCollection<TagModel>();
    public static void LoadTagSettings()
    {
        TagSettingList = DaManager.prjDb.GetRecords<TagModel>("TagSettings");

        var tagSettingsClass = typeof(TagSettings);
        foreach (var tagSetting in TagSettingList) {

            foreach (var property in tagSettingsClass.GetProperties()) {
                if (tagSetting.Name == property.Name) {
                    property.SetValue(string.Empty, tagSetting.Value);
                }

            }
        }
    }

    public static void SaveSettingToDb(string settingName, string settingValue)
    {
        TagModel setting = new TagModel();
        setting = TagSettingList.FirstOrDefault(s => s.Name == settingName);
        setting.Value = settingValue;
        DaManager.prjDb.UpdateRecord<TagModel>(setting, "TagSettings");
    }
}
