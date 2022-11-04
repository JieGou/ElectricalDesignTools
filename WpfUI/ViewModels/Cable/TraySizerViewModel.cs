using EDTLibrary;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Raceways;
using PropertyChanged;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfUI.Helpers;
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
                _start + _trayHeight - _cableDiameter * _scaleFactor - _trayThickness, new CableModel())); // ypos
        }

    }

    public TraySizerViewModel(ListManager listManager, RacewayModel raceway, List<ICable> cableList)
    {
        ListManager = listManager;
        RacewayGraphics.Clear();

        _scaleFactor = 12;
        _cableDiameter = 0.88;
        _trayHeight = _trayThickness + _scaleFactor * raceway.Height;

        _trayWidth = _trayThickness * 2 + _scaleFactor * raceway.Width;

        //Left
        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Width} x {raceway.Height}", _trayThickness, _trayHeight, _start, _start));

        //Right
        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Width} x {raceway.Height}", _trayThickness, _trayHeight, _start + _trayWidth - _trayThickness, _start));

        //Bottom
        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Width} x {raceway.Height}", _trayWidth, _trayThickness, _start, _start + _trayHeight - _trayThickness));

        //Cables

        cableList = cableList.OrderByDescending(c => c.VoltageRating).ThenByDescending(c => c.Spacing).ThenByDescending(c => c.Diameter).ToList();


        int cableCount = 0;
        double diameter = 0;
        double previousDiameter=0;
        double previousSpacing = 0;
        double x = 0;
        double y = 0;
        double traySeparation = 0;
        double spaceBetweenTrays = 50;

       
        foreach (var cable in cableList) {

            var multiplier = cable.UsageType == CableUsageTypes.Power.ToString() && cable.ConductorQty == 1 ? 3 : 1;
            var qtyToDraw = cable.QtyParallel * multiplier;

            for (int i = 0; i < qtyToDraw; i++) {


                diameter = _cableDiameter * _scaleFactor;

                if (cable.Diameter != null && cable.Diameter != 0) {
                    diameter = cable.Diameter * _scaleFactor;
                }

                //first cable
                if (cableCount == 0) {
                    x += _start + _trayThickness;
                    y = _start + _trayHeight - diameter - _trayThickness;
                }

                else {
                    x += (previousDiameter + (previousSpacing / 100) * previousDiameter);
                    y = _start + _trayHeight - diameter - _trayThickness + traySeparation;


                    var nextCable = x + diameter;
                    var trayEnd = _start + _trayWidth - _trayThickness;

                    //new tray
                    if (nextCable > trayEnd) {
                        traySeparation += _trayHeight + spaceBetweenTrays;

                        x = _start + _trayThickness;
                        y += _trayHeight + spaceBetweenTrays;

                        //Add Tray
                        //Left
                        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Width} x {raceway.Height}",
                            _trayThickness, _trayHeight,
                            _start, _start + traySeparation));

                        //Right
                        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Width} x {raceway.Height}",
                            _trayThickness, _trayHeight,
                            _start + _trayWidth - _trayThickness, _start + traySeparation));

                        //Bottom
                        RacewayGraphics.Add(new TrayGraphicViewModel($"{raceway.Tag} - {raceway.Width} x {raceway.Height}",
                            _trayWidth, _trayThickness,
                            _start, _start + _trayHeight - _trayThickness + traySeparation));

                    }
                }

                RacewayGraphics.Add(new CableGraphicViewModel(cable.Tag, diameter, x, y, cable));
                previousDiameter = diameter;
                previousSpacing = cable.Spacing;
                cableCount += 1;//needed for first cable 

            }

        }
    }
    public ObservableCollection<IRacewaySizerGraphic> RacewayGraphics { get; set; } = new ObservableCollection<IRacewaySizerGraphic>();

   
}
