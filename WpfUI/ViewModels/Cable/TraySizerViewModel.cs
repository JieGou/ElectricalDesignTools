using EDTLibrary;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Raceways;
using PropertyChanged;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfUI.Extension_Methods;
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

    static double _trayHeight = _trayThickness + _scaleFactor * 6;

    static double _trayWidth = _trayThickness*2 + _scaleFactor * 24;

    static double _cableDiameter = 0.88;
    static private double _cableSpacing = 1;

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

    public TraySizerViewModel(ListManager listManager, RacewayModel raceway, List<ICable> cableList, bool isRandomFill = false)
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
        var trayBottom = RacewayGraphics[RacewayGraphics.Count - 1];
        //Cables



        if (isRandomFill == true) {
            cableList.Shuffle();
        }
        else {
            cableList = cableList.OrderByDescending(c => c.VoltageRating).ThenByDescending(c => c.Spacing).ThenByDescending(c => c.Diameter).ToList();

        }


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
                var cableGraphic = new CableGraphicViewModel(cable.Tag, diameter, x, y, cable);
                cableGraphic.QtyOfTotal = i + 1;
                RacewayGraphics.Add(cableGraphic);
                previousDiameter = diameter;
                previousSpacing = cable.Spacing;
                cableCount += 1;//needed for first cable 




                if (isRandomFill) {
                    FillRandomly(trayBottom);
                }

            }

        }
    }

    double _increment = 0.05; 
    double _cableContactTolerance = 0.5;
    double _trayEnd = _start + _trayWidth - _trayThickness;

    private void FillRandomly(IRacewaySizerGraphic trayBottom)
    {
        //CollisionDetection for Falling Cables

        _increment = 0.075;
        _cableContactTolerance = -0.5;

        double xDistance = 0;
        double yDistance = 0;
        bool cableIsPlaced = false;
        var placedCables = new List<CableGraphicViewModel>();
        string firstCableContacted ="";
        CableGraphicViewModel firstCable = null;
        string secondCableContacted = "";
        CableGraphicViewModel secondCable = null;

        foreach (var activeCable in RacewayGraphics) {
            cableIsPlaced = false;
            firstCableContacted = "";
            if (activeCable.GetType() == typeof(CableGraphicViewModel)) {
                activeCable.Y = 0;
                while (cableIsPlaced == false) {

                    double cableSeparation = 0;
                    foreach (var placedCable in placedCables) {

                        //skip cables that aren't near cable being added
                        if (activeCable.X + activeCable.Diameter + _cableContactTolerance < placedCable.X || activeCable.X - _cableContactTolerance > placedCable.X + placedCable.Diameter) {
                            continue;
                        }

                        xDistance = (activeCable.X + activeCable.Diameter / 2) - (placedCable.X + placedCable.Diameter / 2);
                        yDistance = (activeCable.Y + activeCable.Diameter / 2) - (placedCable.Y + placedCable.Diameter / 2);
                        cableSeparation = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

                        if (activeCable.Y + activeCable.Diameter > trayBottom.Y) {
                            cableIsPlaced = true;
                            continue;
                        }

                        //first Cable Contacted
                        if (CablesAreTouching((CableGraphicViewModel)activeCable, placedCable)) {

                            if (firstCableContacted != "") {
                                firstCableContacted = placedCable.Tag;
                                firstCable = placedCables.FirstOrDefault(c => c.Tag == firstCableContacted);
                            }
                            if (activeCable.X + activeCable.Diameter + _increment > _trayEnd) {
                                activeCable.X = _start + _trayThickness;

                            }
                            else {
                                activeCable.X += _increment;
                                activeCable.Y -= _increment;
                            }

                            //second cable contact
                            if (CablesAreTouching((CableGraphicViewModel)activeCable, placedCable)) { //} && firstCableContacted != placedCable.Tag) {
                                secondCableContacted = placedCable.Tag;
                                secondCable = placedCables.FirstOrDefault(c => c.Tag == secondCableContacted);

                                //Debug helper to BreakPoint at this cable.
                                if (activeCable.Tag.Contains("MTR333.LCS")) {
                                    bool targetCable = true;
                                }
                                cableIsPlaced = true;
                                continue;
                            }
                        }
                    }
                    activeCable.Y += _increment;
                    if (activeCable.Y + activeCable.Diameter > trayBottom.Y) cableIsPlaced = true;
                }
                placedCables.Add((CableGraphicViewModel)activeCable);
            }
        }
    }

    private bool CablesAreTouching(CableGraphicViewModel cable1, CableGraphicViewModel cable2)
    {
        if (cable1 == null || cable2 == null) return false;

        var xDistance = (cable1.X + cable1.Diameter / 2) - (cable2.X + cable2.Diameter / 2);
        var yDistance = (cable1.Y + cable1.Diameter / 2) - (cable2.Y + cable2.Diameter / 2);
        var cableSeparation = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

        if (cableSeparation - _cableContactTolerance < cable1.Diameter / 2 + cable2.Diameter / 2) {
            return true;
        }
        return false;
    }

    private bool IsCable1LeftOfCable2(CableGraphicViewModel cable1, CableGraphicViewModel cable2)
    {
        if (cable1.X < cable2.X)  return true;
        return false;
    }

    public ObservableCollection<IRacewaySizerGraphic> RacewayGraphics { get; set; } = new ObservableCollection<IRacewaySizerGraphic>();

   
}
