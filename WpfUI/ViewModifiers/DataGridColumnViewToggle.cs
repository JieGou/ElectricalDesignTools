using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModifiers
{
    [AddINotifyPropertyChangedInterface]
    public class DataGridColumnViewToggle
    {
        public bool Power { get; set; } = true;
        public bool PowerInv { get; set; } = false;
        public bool Ocpd { get; set; } = false;
        public bool OcpdInv { get; set; } = true;
        public bool Cable { get; set; } = false;
        public bool CableInv { get; set; } = true;
        public bool Comp { get; set; } = false  ;
        public bool CompInv { get; set; } = true;

        public void HideAll()
        {
            Power = false;
            PowerInv = true;
            Ocpd = false;
            OcpdInv = true;
            Cable = false;
            CableInv = true;
            Comp = false;
            CompInv = true;
        }
        public void TogglePower()
        {
            var temp = Power;
            HideAll();
            Power = true;
            PowerInv = false;
            //Power = !temp;
        }

        public void ToggleOcpd()
        {
            var temp = Ocpd;
            HideAll();
            Ocpd = true;
            OcpdInv = false;

            //Ocpd = !temp;
        }
        public void ToggleCable()
        {
            var temp = Cable;
            HideAll();
            Cable = true;
            CableInv = false;

            //Cable = !temp;
        }

        public void ToggleComp()
        {
            var temp = Comp;
            HideAll();
            Comp = true;
            CompInv = false;
            //Comp = !temp;
        }
    }
}
