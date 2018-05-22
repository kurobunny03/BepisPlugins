﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ConfigurationManager
{
    public partial class ConfigurationManager
    {
        private class SettingEntry
        {
            private bool? browsable;
            private bool? readOnly;

            private string category;
            private object defaultValue;
            private string description;
            private string dispName;

            public object Instance { get; }
            public PropertyInfo Property { get; }

            public SettingEntry(string dispName, string category, string description, object defaultValue, bool? readOnly, bool? browsable, object instance, PropertyInfo settingProp)
            {
                Property = settingProp;
                Instance = instance;
                this.dispName = dispName;
                this.category = category;
                this.description = description;
                this.defaultValue = defaultValue;
                this.readOnly = readOnly;
                this.browsable = browsable;
            }

            public object Get()
            {
                if (customGet != null) return customGet();
                return Property.GetValue(Instance, null);
            }

            public void Set(object newVal)
            {
                if (customSet != null) customSet(newVal);
                else Property.SetValue(Instance, newVal, null);
            }

            private Func<object> customGet;
            private Action<object> customSet;

            /// <summary>
            /// Name of the setting
            /// </summary>
            public string DispName => dispName;
            /// <summary>
            /// Category the setting is under. Null to be directly under the plugin.
            /// </summary>
            public string Category => category;
            /// <summary>
            /// Optional description shown when hovering over the setting
            /// </summary>
            public string Description => description;
            /// <summary>
            /// If set, a "Default" button will be shown next to the setting to allow resetting to default.
            /// </summary>
            public object DefaultValue => defaultValue;
            /// <summary>
            /// Only allow showing of the value. False whenever possible by default.
            /// </summary>
            public bool ReadOnly => readOnly ?? false;
            /// <summary>
            /// Show this setting in the settings screen at all? If false, don't show.
            /// </summary>
            public bool Browsable => browsable ?? true;

            public static SettingEntry FromConfigWrapper(object instance, PropertyInfo settingProp)
            {
                SettingEntry entry = FromAttributes(instance, settingProp);

                dynamic wrapper = entry.Get();

                if (string.IsNullOrEmpty(entry.dispName))
                    entry.dispName = wrapper.Key;
                if (string.IsNullOrEmpty(entry.category))
                    entry.category = wrapper.Section;

                entry.customGet = () => wrapper.Value;
                entry.customSet = (val) => wrapper.Value = val;

                return entry;
            }

            /// <summary>
            /// todo from property that checks canread canwrite
            /// from method that shows a button?
            /// change to inheritance? or isbutton and ignore set argument
            /// </summary>
            private static SettingEntry FromAttributes(object instance, PropertyInfo settingProp)
            {
                var attribs = settingProp.GetCustomAttributes(false);

                var dispName = attribs.OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName;
                var category = attribs.OfType<CategoryAttribute>().FirstOrDefault()?.Category;
                var description = attribs.OfType<DescriptionAttribute>().FirstOrDefault()?.Description;
                var defaultValue = attribs.OfType<DefaultValueAttribute>().FirstOrDefault()?.Value;

                var readOnly = attribs.OfType<ReadOnlyAttribute>().FirstOrDefault()?.IsReadOnly;
                var browsable = settingProp.CanRead ? attribs.OfType<BrowsableAttribute>().FirstOrDefault()?.Browsable : false;

                return new SettingEntry(dispName, category, description, defaultValue, readOnly, browsable, instance, settingProp);
            }
        }
    }
}