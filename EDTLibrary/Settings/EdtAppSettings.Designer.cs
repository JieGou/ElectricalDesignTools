﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EDTLibrary.Settings {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    public sealed partial class EdtAppSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static EdtAppSettings defaultInstance = ((EdtAppSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new EdtAppSettings())));
        
        public static EdtAppSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoSize_ProtectionDevice {
            get {
                return ((bool)(this["AutoSize_ProtectionDevice"]));
            }
            set {
                this["AutoSize_ProtectionDevice"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoSize_PowerCable {
            get {
                return ((bool)(this["AutoSize_PowerCable"]));
            }
            set {
                this["AutoSize_PowerCable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Notification_VoltageChange {
            get {
                return ((bool)(this["Notification_VoltageChange"]));
            }
            set {
                this["Notification_VoltageChange"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Notification_CableAlreadyInRaceway {
            get {
                return ((bool)(this["Notification_CableAlreadyInRaceway"]));
            }
            set {
                this["Notification_CableAlreadyInRaceway"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoSize_CircuitComponents {
            get {
                return ((bool)(this["AutoSize_CircuitComponents"]));
            }
            set {
                this["AutoSize_CircuitComponents"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoSize_SCCR {
            get {
                return ((bool)(this["AutoSize_SCCR"]));
            }
            set {
                this["AutoSize_SCCR"] = value;
            }
        }
    }
}
