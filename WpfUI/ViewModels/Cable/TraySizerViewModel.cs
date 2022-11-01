using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
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
        CableTray.Add(new TrayGraphicViewModel("Tray Left", _trayThickness, _trayHeight, _start , _start));

        //Right
        CableTray.Add(new TrayGraphicViewModel("Tray Right", _trayThickness, _trayHeight, _start + _trayWidth - _trayThickness, _start));
        
        //Bottom
        CableTray.Add(new TrayGraphicViewModel("Tray Bottom", _trayWidth, _trayThickness, _start, _start + _trayHeight - _trayThickness));

        //Cables

        for (int i = 0; i < 10; i++) {
            CableTray.Add(new CableGraphicViewModel("CABLE-" + i, 
                _cableDiameter * _scaleFactor, // diameter
                _start + _trayThickness + (_cableDiameter + _cableSpacing * _cableDiameter) * i * _scaleFactor, // xpos
                _start + _trayHeight - _cableDiameter * _scaleFactor - _trayThickness)); // ypos
        }

    }
    public ObservableCollection<ITraySizerGraphic> CableTray { get; set; } = new ObservableCollection<ITraySizerGraphic>();

   
    }
