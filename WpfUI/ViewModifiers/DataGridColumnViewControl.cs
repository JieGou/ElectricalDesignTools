using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModifiers
{
    [AddINotifyPropertyChangedInterface]
    public class DataGridColumnViewControl
    {
        public bool Loading { get; set; } = true;
        public bool Ocpd { get; set; } = false;
        public bool Cable { get; set; } = true;

        public void ToggleLoading()
        {
            if (Loading == true) {
                Loading = false;
            }
            else if (Loading == false) {
                Loading = true;
            }
        }

        public void ToggleOcpd()
        {
            if (Ocpd == true) {
                Ocpd = false;
            }
            else if (Ocpd == false) {
                Ocpd = true;
            }
        }
        public void ToggleCable()
        {
            if (Cable == true) {
                Cable = false;
            }
            else if (Cable == false) {
                Cable = true;
            }
        }
    }
}
