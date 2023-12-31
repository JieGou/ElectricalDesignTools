﻿using EDTLibrary.Mappers;
using EDTLibrary.Models.Cables;
using System;
using System.Collections.ObjectModel;

namespace EDTLibrary.Settings
{
    /// <summary>
    /// Project Settings for automating sizing and selections
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// 

    // To change / add settings you must change the name in 3 places:
    //   1 - the name below
    //   2 - the name in the EdtSettingsViewModel
    //   3 - the name of the setting in the Database ProjectSettings Table
    //     - the table name itself in the Database if the setting is a DataTable type Setting

    public class EdtProjectSettings
    {

        //General
        public static string ProjectName {
            get { return _projectName; }
            set { _projectName = value;
                OnProjectNameUpdated();
            }
        }

        public static event EventHandler ProjectNameUpdated;
        public static void OnProjectNameUpdated()
        {
            if (ProjectNameUpdated != null) {
                ProjectNameUpdated(null, EventArgs.Empty);
            }
        }
        public static string ProjectName_Model { get; set; }



        public static string ProjectNumber { get; set; }
        public static string ProjectTitleLine1 { get; set; }
        public static string ProjectTitleLine2 { get; set; }
        public static string ProjectTitleLine3 { get; set; }

        public static string ClientNameLine1 { get; set; }
        public static string ClientNameLine2 { get; set; }
        public static string ClientNameLine3 { get; set; }


        public static string AcadBlockFolder { get; set; }
        public static string AcadSaveFolder { get; set; }





        public static string Code { get; set; }

        //Cable
        #region Cable Settings 
        //TODO - Populate CableSizesUsedInProject list from Library (copy Database table to Library and create when building new projects)
        public static ObservableCollection<CableSizeModel> CableSizesUsedInProject { get; set; }

        public static string CableType3C1kVPower { get; set; }
        public static string CableInsulation1kVPower { get; set; }
        public static string CableInsulation5kVPower { get; set; }
        public static string CableInsulation15kVPower { get; set; }
        public static string CableInsulation35kVPower { get; set; }

        public static string CableInstallationType { get; set; }

        //Load
        public static string DefaultCableTypeLoad_3phLt300V { get; set; } = "Incomplete";
        public static string DefaultCableTypeLoad_2wire { get; set; }
        public static string DefaultCableTypeLoad_3ph300to1kV { get; set; }
        public static string DefaultCableTypeLoad_3ph5kV { get; set; }

        //Dteq
        public static string DefaultCableTypeLoad_4wire { get; set; }
        public static string DefaultCableTypeDteq_3ph1kVLt1200A { get; set; }
        public static string DefaultCableTypeDteq_3ph1kVGt1200A { get; set; }
        public static string DefaultCableType_3ph5kV { get; set; }
        public static string DefaultCableType_3ph15kV { get; set; }


        private static string _cableSpacingMaxAmps_3C1kV;
        private static string _projectName;

        public static string CableSpacingMaxAmps_3C1kV
        {
            get { return _cableSpacingMaxAmps_3C1kV; }
            set
            {
                var oldValue = _cableSpacingMaxAmps_3C1kV;
                double dblOut;
                _cableSpacingMaxAmps_3C1kV = value;
                if (_cableSpacingMaxAmps_3C1kV == "") {
                    _cableSpacingMaxAmps_3C1kV = "0";
                }
                else if (Double.TryParse(_cableSpacingMaxAmps_3C1kV, out dblOut) == false) {
                    _cableSpacingMaxAmps_3C1kV = oldValue;
                }

            }
        }


        public static string LcsControlCableType { get; set; }
        public static string LcsControlCableSize { get; set; }
        #endregion  

        //Cable Lengths
        #region Cable Lengths 
        public static string CableLengthDteq { get; set; }
        public static string CableLengthLoad { get; set; }
        public static string CableLengthDrive { get; set; }
        public static string CableLengthLocalDisconnect { get; set; }
        public static string CableLengthLocalControlStation { get; set; }

        #endregion  


        //Dteq
        #region Dteq
        public static string DteqMaxPercentLoaded { get; set; }
        public static string DteqDefaultPdTypeLV { get; set; }

        
        public static string XfrImpedance { get; set; }
        public static string XfrSubType { get; set; }
        public static string XfrGrounding_Primary { get; set; }
        public static string XfrGrounding_Secondary { get; set; }
        public static string DteqLoadCableDerating { get; set; }
        #endregion  


     
        //Voltage
        public static string VoltageDefault1kV { get; set; }

        public static string LoadDefaultPdTypeLV_NonMotor { get; set; }
        public static string LoadDefaultPdTypeLV_Motor { get; set; }


        //Load
        public static string DemandFactorDefault { get; set; } = "0.8";

        //Demand Factor
        public static string DemandFactorDefault_Heater { get; set; }
        public static string DemandFactorDefault_Panel { get; set; }
        public static string DemandFactorDefault_Other { get; set; }
        public static string DemandFactorDefault_Welding { get; set; }

        //Efficiency
        public static string LoadDefaultEfficiency_Heater { get; set; }
        public static string LoadDefaultEfficiency_Panel { get; set; }
        public static string LoadDefaultEfficiency_Other { get; set; }

        //Power Factor
        public static string LoadDefaultPowerFactor_Heater { get; set; }
        public static string LoadDefaultPowerFactor_Panel { get; set; }
        public static string LoadDefaultPowerFactor_Other { get; set; }

        //Components
        public static string LcsTypeDolLoad { get; set; }
        public static string LcsTypeVsdLoad { get; set; }
        public static string LocalDisconnectType { get; set; }

        

        public static ObservableCollection<ExportMappingModel> ExportMappings { get; set; }
        public static string AreaColumnVisible { get; set; }









        //AutoCad

        private string AutocadTitleBlock { get; set; }


    }
}

