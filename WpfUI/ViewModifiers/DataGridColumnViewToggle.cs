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
        public bool Ocpd { get; set; } = true;
        public bool Cable { get; set; } = true;
        public bool Comp { get; set; } = true;

        public void HideAll()
        {
            Power = false;
            Ocpd = false;
            Cable = false;
            Comp = false;
        }
        public void TogglePower()
        {
            var temp = Power;
            HideAll();
            Power = !temp;
        }

        public void ToggleOcpd()
        {
            var temp = Ocpd;
            HideAll();
            Ocpd = !temp;
        }
        public void ToggleCable()
        {
            var temp = Cable;
            HideAll();
            Cable = !temp;
        }

        public void ToggleComp()
        {
            var temp = Comp;
            HideAll();
            Comp = !temp;
        }
    }
}
