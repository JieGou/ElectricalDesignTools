using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.DistributionEquipment
{
    public class DteqFactory
    {
        private ListManager _listManager;

        public DteqFactory(ListManager listManager)
        {
            _listManager = listManager;
        }
        public DteqModel CreateDteq(DteqToAddValidator dteqToAddValidator)
        {
            DteqModel newDteq = new DteqModel(); 

            newDteq.FedFrom = _listManager.DteqList.FirstOrDefault(d => d.Tag == dteqToAddValidator.FedFromTag);

            newDteq.Tag = dteqToAddValidator.Tag;
            newDteq.Category = Categories.DTEQ.ToString();
            newDteq.Type = dteqToAddValidator.Type;

            newDteq.Size = Double.Parse(dteqToAddValidator.Size);
            newDteq.Unit = dteqToAddValidator.Unit;
            newDteq.Description = dteqToAddValidator.Description;
            newDteq.FedFromTag = dteqToAddValidator.FedFromTag;
            newDteq.LineVoltage = Double.Parse(dteqToAddValidator.LineVoltage);
            newDteq.LoadVoltage = Double.Parse(dteqToAddValidator.LoadVoltage);

            return newDteq;
        }
    }
}
