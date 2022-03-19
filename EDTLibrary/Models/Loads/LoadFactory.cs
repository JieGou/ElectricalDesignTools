using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads
{
    public class LoadFactory
    {

        private ListManager _listManager;

        public LoadFactory(ListManager listManager)
        {
            _listManager = listManager;
        }

        public LoadModel CreateLoad(LoadToAddValidator loadToAddValidator)
        {
            LoadModel newLoad = new LoadModel();

            newLoad.FedFrom = _listManager.DteqList.FirstOrDefault(d => d.Tag == loadToAddValidator.FedFromTag);
            newLoad.Tag = loadToAddValidator.Tag;
            newLoad.Category = Categories.LOAD3P.ToString();
            newLoad.Type = loadToAddValidator.Type;
            newLoad.Size = Double.Parse(loadToAddValidator.Size);
            newLoad.Description = loadToAddValidator.Description;
            newLoad.FedFromTag = loadToAddValidator.FedFromTag;
            newLoad.Voltage = Double.Parse(loadToAddValidator.Voltage);
            newLoad.Unit = loadToAddValidator.Unit;
            newLoad.LoadFactor = Double.Parse(loadToAddValidator.LoadFactor);

            return newLoad;
        }
    }
}
