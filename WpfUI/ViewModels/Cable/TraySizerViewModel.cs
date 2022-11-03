using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Raceways;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfUI.Stores;
using WpfUI.ViewModels.Cable;

namespace WpfUI.ViewModels.Cables;

[AddINotifyPropertyChangedInterface]
public class TraySizerViewModel : ViewModelBase
{
    private ListManager _listManager;

    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }

    static double _start = 10;

    static double _scaleFactor = 10;
    static double _trayThickness = 5;

    double _trayHeight = _trayThickness + _scaleFactor * 6;

    double _trayWidth = _trayThickness*2 + _scaleFactor * 24;

    double _cableDiameter = 0.88;
    private double _cableSpacing = 1;

    public TraySizerViewModel(ListManager listManager)
    {

        //Left
        RacewayGraphics.Add(new TrayGraphicViewModel("Tray Left", _trayThickness, _trayHeight, _start , _start));

        //Right
        RacewayGraphics.Add(new TrayGraphicViewModel("Tray Right", _trayThickness, _trayHeight, _start + _trayWidth - _trayThickness, _start));
        
        //Bottom
        RacewayGraphics.Add(new TrayGraphicViewModel("Tray Bottom", _trayWidth, _trayThickness, _start, _start + _trayHeight - _trayThickness));

        //Cables

        for (int i = 0; i < 10; i++) {
            RacewayGraphics.Add(new CableGraphicViewModel("CABLE-" + i, 
                _cableDiameter * _scaleFactor, // diameter
                _start + _trayThickness + (_cableDiameter + _cableSpacing * _cableDiameter) * i * _scaleFactor, // xpos
                _start + _trayHeight - _cableDiameter * _scaleFactor - _trayThickness)); // ypos
        }

    }

    public TraySizerViewModel(ListManager listManager, RacewayModel raceway, List<CableModel> cableList)
    {
        ListManager = listManager;
        RacewayGraphics.Clear();

        _scaleFactor = 15;
        _trayHeight = _trayThickness + _scaleFactor * raceway.Height;

        _trayWidth = _trayThickness * 2 + _scaleFactor * raceway.Width;

        //Left
        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Height} x {raceway.Width}", _trayThickness, _trayHeight, _start, _start));

        //Right
        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Height} x {raceway.Width}", _trayThickness, _trayHeight, _start + _trayWidth - _trayThickness, _start));

        //Bottom
        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Height} x {raceway.Width}", _trayWidth, _trayThickness, _start, _start + _trayHeight - _trayThickness));

        //Cables

        int cableCount = 0;
        double diameter = 0;
        double x = _start;
        double y = 0;
        foreach (var cable in cableList) {

            diameter = _cableDiameter * _scaleFactor;
            if (cableCount == 0) {
                x += _trayThickness;
            }
            else {
                x += (_cableDiameter + (cable.Spacing / 100) * _cableDiameter) * _scaleFactor;
            }
            y = _start + _trayHeight - _cableDiameter * _scaleFactor - _trayThickness;


            RacewayGraphics.Add(new CableGraphicViewModel(cable.Tag, diameter, x, y));
            
            cableCount += 1;
        }
  
    }
    public ObservableCollection<IRacewaySizerGraphic> RacewayGraphics { get; set; } = new ObservableCollection<IRacewaySizerGraphic>();

   
}
