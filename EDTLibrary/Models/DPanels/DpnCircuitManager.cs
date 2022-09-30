using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Services;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EDTLibrary.Models.DistributionEquipment.DPanels;
public class DpnCircuitManager
{

    public static bool AddLoad(IDpn dpn, IPowerConsumer load, ListManager listManager)
    {
        if (CanAdd(dpn, load)) {

            //Left
            if ((_leftCctsAvailable != -1 && _leftCctsAvailable <= _rightCctsAvailable) ||
                            (_leftCctsAvailable != -1 && _rightCctsAvailable == -1)) {
                load.PanelSide = DpnSide.Left.ToString();
            }
            //Right
            else if ((_rightCctsAvailable != -1 && _rightCctsAvailable <= _leftCctsAvailable) ||
                    (_rightCctsAvailable != -1 && _leftCctsAvailable == -1)) {
                load.PanelSide = DpnSide.Right.ToString();
            }
            dpn.AssignedLoads.Add(load);
            dpn.SetCircuits();
            return true;
        }
        return false;
        
    }
    private static int _leftCctsAvailable = 0;
    private static int _rightCctsAvailable = 0;
    public static bool CanAdd(IDpn dpn, IPowerConsumer load)
    {
     
        _leftCctsAvailable = GetAvailableCircuit(dpn, load, DpnSide.Left);

        _rightCctsAvailable = GetAvailableCircuit(dpn, load, DpnSide.Right);

        if (_leftCctsAvailable < 0 && _rightCctsAvailable < 0) return false;

        if (dpn.AssignedLoads.FirstOrDefault(l => l.Id == load.Id) != null) return false;

        return true;
    }

    public static void DeleteLoad(IDpn dpn, IPowerConsumer load, ListManager listManager)
    {
        if (load == null) return;
        dpn.AssignedLoads.Remove(load);
        LoadManager.DeleteLoad(load, listManager);
        dpn.SetCircuits();
    }
    internal static void DeleteLoadCircuit(DpnModel dpnModel, IPowerConsumer powerConsumer, ListManager listManager)
    {
        var loadCircuit = (LoadCircuit)powerConsumer;
        listManager.LoadCircuitList.Remove(loadCircuit);
        DaManager.DeleteLoadCircuit(loadCircuit);
    }



    /// <summary>
    /// Returns the next available circuit position(s) for the required load/breaker. 
    /// Returns -1 when there are not enough avaialbe circuits for the required breaker.
    /// </summary>
    /// <param name="dpn"></param>
    /// <param name="load"></param>
    /// <param name="dpnSide"></param>
    /// <returns></returns>
    private static int GetAvailableCircuit(IDpn dpn, IPowerConsumer load, DpnSide dpnSide)
    {
        int cct = 0;
                
        for (int i = 0; i < dpn.CircuitCount / 2; i++) {
            if (IsCircuitAvailable(dpn, load, cct, dpnSide)) {
                return cct;
            }
            else {
                cct += 1;
            }
        }
        
        return -1;
    }

    /// <summary>
    /// Checks if there are enough available circuits in a panel to add a load at a specified circuit position. 
    /// </summary>
    /// <param name="dpn"></param>
    /// <param name="load"></param>
    /// <param name="cct"></param>
    /// <param name="dpnSide"></param>
    /// <returns></returns>
    private static bool IsCircuitAvailable(IDpn dpn, IPowerConsumer load, int cct, DpnSide dpnSide)
    {
        bool isAvailable = false;
        List<IPowerConsumer> cctList = new List<IPowerConsumer>();
        int poleCount = 0;

        if (dpnSide == DpnSide.Left) {
            cctList = dpn.LeftCircuits.ToList();
            
        }
        else {
            cctList = dpn.RightCircuits.ToList();
        }


        //TODO - PoleCount
        foreach (var item in cctList) {
            if (item.VoltageType!=null) {
                poleCount += item.VoltageType.Poles;
            }
        }

        if (load.VoltageType == null) {
            return false;
        }
        int loadPoles = load.VoltageType.Poles;

        for (int i = 0; i < loadPoles; i++) {

            //check if there are enough circuits in the panel
            if (cct+i < cctList.Count) {

                //check if there are enough available circuits for the load
                if (cctList[cct + i].Tag == "-") {
                    isAvailable = true;
                }
                else {
                    return false;
                }
            }

            else {
                return false;
            }
        }

        return isAvailable;
    }

    public static void GetAssignedCircuits(IDpn dpn, ListManager listManager)
    {
        var loadCircuitList = DaManager.prjDb.GetRecords<LoadCircuit>(GlobalConfig.LoadCircuitTable);
        var list = loadCircuitList.Where(cct => cct.FedFromId == dpn.Id && cct.FedFromType == dpn.GetType().ToString()).ToList();
        loadCircuitList = new ObservableCollection<LoadCircuit>(list);
    }


    public static void MoveCircuitUp(IDpn dpn, IPowerConsumer load)
    {

        if (load == null) return;

        ObservableCollection<IPowerConsumer> sideCircuitList;

        sideCircuitList = dpn.LeftCircuits.FirstOrDefault(ld => ld == load) != null ? dpn.LeftCircuits : dpn.RightCircuits;

        int loadIndex;
        for (int i = 0; i < sideCircuitList.Count; i++) {
            if (load == sideCircuitList[i]) {
                loadIndex = Math.Max(0, i - 1);
                sideCircuitList.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < sideCircuitList.Count; i++) {
            sideCircuitList[i].SequenceNumber = i;
        }
        sideCircuitList.OrderBy(c => c.SequenceNumber);
        dpn.CalculateLoading();
    }

    public static void MoveCircuitDown(IDpn dpn, IPowerConsumer load)
    {

        if (load == null) return;

        ObservableCollection<IPowerConsumer> sideCircuitList;

        sideCircuitList = dpn.LeftCircuits.FirstOrDefault(ld => ld == load) != null ? dpn.LeftCircuits : dpn.RightCircuits;

        int loadIndex;
        for (int i = 0; i < sideCircuitList.Count; i++) {
            if (load == sideCircuitList[i]) {
                loadIndex = Math.Min(i + 1, sideCircuitList.Count - 1);
                sideCircuitList.Move(i, loadIndex);
                break;
            }
        }
        for (int i = 0; i < sideCircuitList.Count; i++) {
            sideCircuitList[i].SequenceNumber = i;
        }
        sideCircuitList.OrderBy(c => c.SequenceNumber);
        dpn.CalculateLoading();
    }

    public static void MoveCircuitLeft(IDpn dpn, IPowerConsumer load)
    {
        if (load == null) return;
        DpnSide dpnSide;
        dpnSide = dpn.LeftCircuits.FirstOrDefault(ld => ld == load) != null ? DpnSide.Left : DpnSide.Right;
        if (dpnSide == DpnSide.Left) return;

        ObservableCollection<IPowerConsumer> sideCircuitList;

        

        if (dpnSide == DpnSide.Right && GetAvailableCircuit(dpn, load, DpnSide.Left) != -1) {
            dpn.LeftCircuits.Add(load);
            dpn.RightCircuits.Remove(load);
            load.PanelSide = DpnSide.Left.ToString();
        }
        dpn.SetCircuits();
        dpn.CalculateLoading();
    }

    public static void MoveCircuitRight(IDpn dpn, IPowerConsumer load)
    {

        if (load == null) return;
        DpnSide dpnSide; 
        dpnSide = dpn.LeftCircuits.FirstOrDefault(ld => ld == load) != null ? DpnSide.Left : DpnSide.Right;
        if (dpnSide == DpnSide.Right) return;


        ObservableCollection<IPowerConsumer> sideCircuitList;

        
       


        if (dpnSide == DpnSide.Left && GetAvailableCircuit(dpn, load, DpnSide.Right) != -1) {
            dpn.LeftCircuits.Remove(load);
            dpn.RightCircuits.Add(load);
            load.PanelSide = DpnSide.Right.ToString();
        }
        dpn.SetCircuits();
        dpn.CalculateLoading();
    }

    internal static async Task ConvertToLoad(LoadCircuit loadCircuit)
    {
        var listManager = ScenarioManager.ListManager;
        var dpn = listManager.DpnList.FirstOrDefault(dt => dt.Id == loadCircuit.FedFromId);
        var dpnSide = loadCircuit.PanelSide == DpnSide.Left.ToString() ? DpnSide.Left : DpnSide.Right;
        var circuitList = loadCircuit.PanelSide == DpnSide.Left.ToString() ? dpn.LeftCircuits : dpn.RightCircuits;

        int cctNumber = 0;
        int availableCircuitPosition = GetAvailableCircuit(dpn, loadCircuit, dpnSide);

        loadCircuit.Tag = "adding";
        
        for (int i = 0; i < circuitList.Count; i++) {
            if (circuitList[i].VoltageType == null) {
                cctNumber += 2 * 1;
            }
            else {
                cctNumber += 2 * circuitList[i].VoltageType.Poles;
            }
            if (loadCircuit == circuitList[i]) {
                break;
            }
        }

        cctNumber = dpnSide == DpnSide.Left ? cctNumber += 1: cctNumber += 2;
        

        var loadToAdd = new LoadToAddValidator(listManager) {
            Tag = $"{dpn.Tag}-{cctNumber}",
            Type = LoadTypes.OTHER.ToString(),
            Description = loadCircuit.Description,
            AreaTag = dpn.Area.Tag,
            FedFromTag = dpn.Tag,

            VoltageType = loadCircuit.VoltageType,
            Size = 5.ToString(),
            Unit = Units.A.ToString(),
            LoadFactor = EdtSettings.LoadFactorDefault

        };
        loadCircuit = null;

        try {
            //LoadModel newLoad = await LoadManager.AddLoad(loadToAdd, listManager);
        }
        catch (Exception ex) {
            EdtNotificationService.SendError(loadCircuit, "", "ConvertToLoad", ex);
        }        

    }
}

