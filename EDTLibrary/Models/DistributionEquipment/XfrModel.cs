using EDTLibrary.LibraryData;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment
{

    [AddINotifyPropertyChangedInterface]
    public class XfrModel : DistributionEquipment
    {
        public XfrModel()
        {
            Category = Categories.DTEQ.ToString();
            Voltage = LineVoltage;
            PdType = ProjectSettings.EdtSettings.DteqDefaultPdTypeLV;
        }

        private double _impZ;

        public double ImpZ
        {
            get { return _impZ; }
            set
            {
                var oldImpZ = _impZ;
                _impZ = value;
                if (_impZ <= 0) {
                    _impZ = oldImpZ;
                }
                CalculateSCCR();
            }
        }

        

        private double _impX;

        public double ImpX
        {
            get { return _impX; }
            set 
            { 
                var oldImpX = _impX;
                _impX = value; 
                if (_impX <= 0) {
                    _impX = oldImpX;
                }
            }
        }

        private double _impR;

        public double ImpR
        {
            get { return _impR; }
            set { _impR = value; }
        }

        private double _sccr;

        public double SCCR
        {
            get { return _sccr; }
            set 
            { 
                var oldSccr = _sccr;
                _sccr = value; 
                if(_sccr<=0) {
                    _sccr = oldSccr;
                }   
            }
        }

        public ILoad LargestMotor
        {
            get { return FindLargestMotor(this, new LoadModel { ConnectedKva=0}); }
        }


        private void CalculateSCCR()
        {
            SCCR = 100 / (_impZ * 0.9) * Fla;
            SCCR = Math.Round(SCCR, 2);
        }

        public ILoad FindLargestMotor(IDteq dteq, IPowerConsumer largestMotor)
        {
            foreach (var assignedLoad in dteq.AssignedLoads) {
                if (assignedLoad.Type == LoadTypes.MOTOR.ToString()) {
                    if (assignedLoad.ConnectedKva > largestMotor.ConnectedKva) {
                        largestMotor = assignedLoad;
                    }
                }
                else if (assignedLoad.Category == Categories.DTEQ.ToString()) { 
                    largestMotor = FindLargestMotor((IDteq)assignedLoad, largestMotor);
                }
            }
            return (ILoad)largestMotor;
        }

    }

}
