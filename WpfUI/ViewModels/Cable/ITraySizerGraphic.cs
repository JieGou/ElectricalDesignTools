namespace WpfUI.ViewModels.Cable;

public interface ITraySizerGraphic
{
    string Tag { get; set; }
    double X { get; set; }
    double Y { get; set; }

    //Cable
    double Diameter { get; set; }

    //Tray
    double Width { get; set; }
    double Height { get; set; }

    
}