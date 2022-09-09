using EDTLibrary.Managers;
using EDTLibrary.ProjectSettings;
using System;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment
{
    public class DteqFactory
    {
        private ListManager _listManager;

        public DteqFactory(ListManager listManager)
        {
            _listManager = listManager;
        }
        public IDteq CreateDteq(DteqToAddValidator dteqToAddValidator)
        {
            IDteq newDteq;

            if (dteqToAddValidator.Type == DteqTypes.XFR.ToString()) {
                XfrModel model = new XfrModel();
                //XFR properties
                model.Impedance = Double.Parse(EdtSettings.XfrImpedance);
                model.SubType = EdtSettings.XfrSubType;
                model.Grounding = EdtSettings.XfrGrounding;
                //Todo - Add default Type and NGR
                newDteq = model;
            }
            else if (dteqToAddValidator.Type == DteqTypes.SWG.ToString()) {
                SwgModel model = new SwgModel();
                //SWG properties
                newDteq = model;
            }
            else if (dteqToAddValidator.Type == DteqTypes.MCC.ToString()) {
                MccModel model = new MccModel();
                //MCC properties
                newDteq = model;
            }
            else if (dteqToAddValidator.Type == DteqTypes.DPN.ToString()) {
                DpnModel model = new DpnModel();
                //MCC properties
                newDteq = model;
            }
            else {
                newDteq = new DteqModel();
            }

            newDteq.FedFrom = _listManager.DteqList.FirstOrDefault(d => d.Tag == dteqToAddValidator.FedFromTag);

            newDteq.Tag = dteqToAddValidator.Tag;
            newDteq.Category = Categories.DTEQ.ToString();
            newDteq.Type = dteqToAddValidator.Type;
            newDteq.Area = _listManager.AreaList.FirstOrDefault(a => a.Tag == dteqToAddValidator.AreaTag);
            newDteq.Size = Double.Parse(dteqToAddValidator.Size);
            newDteq.Unit = dteqToAddValidator.Unit;
            newDteq.Description = dteqToAddValidator.Description;
            //newDteq.FedFromTag = dteqToAddValidator.FedFromTag;
            newDteq.LineVoltage = Double.Parse(dteqToAddValidator.LineVoltage);
            newDteq.LoadVoltage = Double.Parse(dteqToAddValidator.LoadVoltage);

            return newDteq;
        }

        public static IDteq Recast(object oDteq)
        {
            if (oDteq == null) {
                return null;
            }
            if (oDteq.GetType() == typeof(XfrModel)) {
                return (XfrModel)oDteq;
            }
            else if (oDteq.GetType() == typeof(SwgModel)) {
                return (SwgModel)oDteq;
            }
            else if (oDteq.GetType() == typeof(MccModel)) {
                return (MccModel)oDteq;
            }
            else if (oDteq.GetType() == typeof(DpnModel)) {
                return (DpnModel)oDteq;
            }
            else {
                return (DteqModel)oDteq;
            }
        }
    }
}
