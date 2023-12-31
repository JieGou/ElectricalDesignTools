﻿using EDTLibrary.Models.Cables;

namespace WpfUI.ViewModels.Cable;

public interface IRacewaySizerGraphic
{
    string Tag { get; set; }
    double X { get; set; }
    double Y { get; set; }

    //Cable or conduit
    double Diameter { get; set; }

    //Tray
    double Width { get; set; }
    double Height { get; set; }

    string ToolTip { get;}
    int QtyOfTotal { get; set; }
    ICable CableModel { get; set; }


}