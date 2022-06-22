﻿using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Cables;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.ProjectSettings
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

    public class EdtSettings
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
        //TODO - Populate CableSizesUsedInProject list from Library (copy Database table to Library and create when building new projects)
        public static ObservableCollection<CableSizeModel> CableSizesUsedInProject { get; set; }

        public static string CableType3C1kVPower { get; set; }
        public static string CableInsulation1kVPower { get; set; }
        public static string CableInsulation5kVPower { get; set; }
        public static string CableInsulation15kVPower { get; set; }
        public static string CableInsulation35kVPower { get; set; }

        public static string DefaultCableInstallationType { get; set; }
        public static string DefaultCableTypeLoad_3phLt300V { get; set; } = "Incomplete";
        public static string DefaultCableTypeLoad_3ph300to1kV { get; set; }
        public static string DefaultCableTypeLoad_3ph5kV { get; set; }
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


        public static string DefaultLcsControlCableType { get; set; }
        public static string DefaultLcsControlCableSize { get; set; }



        //Dteq
        public static string DteqMaxPercentLoaded { get; set; }
        public static string DteqDefaultPdTypeLV { get; set; }

        public static string LoadDefaultPdTypeLV_NonMotor { get; set; }
        public static string LoadDefaultPdTypeLV_Motor { get; set; }
        public static string LoadFactorDefault { get; set; }
        public static string DefaultXfrImpedance { get; set; }

        //Voltage
        public static string VoltageDefault1kV { get; set; }


        //Loads
        public static string LoadDefaultEfficiency_Other { get; set; }
        public static string LoadDefaultPowerFactor_Other { get; set; }
        public static string LoadDefaultEfficiency_Panel { get; set; }
        public static string LoadDefaultPowerFactor_Panel { get; set; }

        //Components
        public static string DefaultLcsTypeDolLoad { get; set; }
        public static string DefaultLcsTypeVsdLoad { get; set; }



        public static ObservableCollection<ExportMappingModel> ExportMappings { get; set; }
        public static string AreaColumnVisible { get; set; }
    }
}

