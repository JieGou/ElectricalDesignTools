using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Services;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
                load.CircuitNumber = _nextAvailableCctNumberLeft;

            }
            //Right
            else if ((_rightCctsAvailable != -1 && _rightCctsAvailable <= _leftCctsAvailable) ||
                    (_rightCctsAvailable != -1 && _leftCctsAvailable == -1)) {
                load.PanelSide = DpnSide.Right.ToString();
                load.CircuitNumber = _nextAvailableCctNumberRight;
            }

            dpn.AssignedLoads.Add(load);
            dpn.SetCircuits();
            return true;
        }
        return false;
        
    }
    private static int _leftCctsAvailable = 0;
    private static int _rightCctsAvailable = 0;
    private static int _nextAvailableCctNumberLeft = 0;
    private static int _nextAvailableCctNumberRight = 0;

    public static bool CanAdd(IDpn dpn, IPowerConsumer load)
    {
     
        _leftCctsAvailable = GetAvailableCircuit(dpn, load, DpnSide.Left).Item1;
        _rightCctsAvailable = GetAvailableCircuit(dpn, load, DpnSide.Right).Item1;

        _nextAvailableCctNumberLeft = GetAvailableCircuit(dpn, load, DpnSide.Left).Item2;
        _nextAvailableCctNumberRight = GetAvailableCircuit(dpn, load, DpnSide.Right).Item2;

        if (_leftCctsAvailable < 0 && _rightCctsAvailable < 0) return false;

        if (dpn.AssignedLoads.FirstOrDefault(l => l.Id == load.Id) != null) return false;

        return true;
    }

    public static void DeleteLoad(IDpn dpn, IPowerConsumer load, ListManager listManager)
    {
        if (load == null || load is LoadCircuit) return;
        dpn.RemoveAssignedLoad(load);
        LoadManager.DeleteLoad(load, listManager);
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
    public static Tuple<int, int> GetAvailableCircuit(IDpn dpn, IPowerConsumer load, DpnSide dpnSide)
    {
        var circuit = new Tuple<int, int>(0,0);
        int seqNum = 0;
        int cctNum = 0;

        for (int i = 0; i < dpn.CircuitCount / 2; i++) {
            if (IsCircuitAvailable(dpn, load, seqNum, dpnSide)) {

                cctNum = GetCircuitNumber(dpn, load, seqNum);
                circuit = new Tuple<int, int>(seqNum, cctNum);
                return circuit;
            }
            else {
                seqNum += 1;
            }
        }
        circuit = new Tuple<int, int>(-1,-1);
        return circuit;
    }

    private static int GetCircuitNumber(IDpn dpn, IPowerConsumer load, int sequenceNumber)
    {
        int cct = 0;
        var sideCircuitList = load.PanelSide == DpnSide.Left.ToString() ? dpn.LeftCircuits : dpn.RightCircuits;

        for (int i = 0; i <= sequenceNumber; i++) {
            if (i==0) {
                cct = load.PanelSide == DpnSide.Left.ToString() ? 1 : 2;
            }
            else {
                cct += 2 * sideCircuitList[i - 1].VoltageType.Poles;
            }
        }
        
        return cct;
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

        for (int pole = 0; pole < loadPoles; pole++) {

            //check if there are enough circuits in the panel
            if (cct+pole < cctList.Count) {

                //check if there are enough available circuits for the load
                if (cctList[cct + pole].Tag == DpnCircuitConfig.UnassignedCircuitTag) {
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
        AssignSequenceNumbers(sideCircuitList);
        AssignCircuitNumbers(sideCircuitList);
        RetagCircuits(sideCircuitList);
        sideCircuitList.OrderBy(c => c.CircuitNumber);
        dpn.CalculateLoading(); //to calculate the phase loading
    }

    internal static void RetagCircuits(ObservableCollection<IPowerConsumer> sideCircuitList)
    {
        foreach (var item in sideCircuitList) {
            if (item.Tag.Contains(item.FedFrom.Tag)) {
                item.Tag = $"{item.FedFrom.Tag}-{item.CircuitNumber}";
            }
        }
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

        AssignSequenceNumbers(sideCircuitList);
        AssignCircuitNumbers(sideCircuitList);

        RetagCircuits(sideCircuitList);

        sideCircuitList.OrderBy(c => c.CircuitNumber);
        dpn.CalculateLoading();
    }

    public static void MoveCircuitLeft(IDpn dpn, IPowerConsumer load)
    {
        if (load == null) return;
        DpnSide dpnSide;
        dpnSide = dpn.LeftCircuits.FirstOrDefault(ld => ld == load) != null ? DpnSide.Left : DpnSide.Right;
        if (dpnSide == DpnSide.Left) return;

        if (dpnSide == DpnSide.Right && GetAvailableCircuit(dpn, load, DpnSide.Left).Item1 != -1) {
            load.SequenceNumber = GetAvailableCircuit(dpn, load, DpnSide.Left).Item1;
            load.CircuitNumber = GetAvailableCircuit(dpn, load, DpnSide.Left).Item2;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
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

        if (dpnSide == DpnSide.Left && GetAvailableCircuit(dpn, load, DpnSide.Right).Item1 != -1) {
            load.SequenceNumber = GetAvailableCircuit(dpn, load, DpnSide.Right).Item1;
            load.CircuitNumber = GetAvailableCircuit(dpn, load, DpnSide.Right).Item2;
            dpn.LeftCircuits.Remove(load);
            dpn.RightCircuits.Add(load);
            load.PanelSide = DpnSide.Right.ToString();
        }
        dpn.SetCircuits();
        dpn.CalculateLoading();
    }

    public static void AssignSequenceNumbers(ObservableCollection<IPowerConsumer> sideCircuitList)
    {
        for (int i = 0; i < sideCircuitList.Count; i++) {
            sideCircuitList[i].SequenceNumber = i;
        }
    }
    public static void AssignCircuitNumbers(ObservableCollection<IPowerConsumer> sideCircuitList)
    {
        int cctNumber = 0;

        if (DaManager.GettingRecords == true) return;
        if (sideCircuitList[1].PanelSide == DpnSide.Left.ToString()) {
            for (int i = 0; i < sideCircuitList.Count; i++) {
                if (i == 0) {
                    cctNumber = 1;
                    sideCircuitList[i].CircuitNumber = cctNumber;
                }
                else {
                    cctNumber += sideCircuitList[i - 1].VoltageType.Poles * 2;
                    sideCircuitList[i].CircuitNumber = cctNumber;
                }
            }
        }
        else if(sideCircuitList[1].PanelSide == DpnSide.Right.ToString()) {
            for (int i = 0; i < sideCircuitList.Count; i++) {
                if (i == 0) {
                    cctNumber = 2;
                    sideCircuitList[i].CircuitNumber = cctNumber;
                }
                else {
                    cctNumber += sideCircuitList[i - 1].VoltageType.Poles * 2;
                    sideCircuitList[i].CircuitNumber = cctNumber;
                }
            }
        }

    }

    public static async Task ConvertToLoad(LoadCircuit loadCircuit)
    {
        
        var listManager = ScenarioManager.ListManager;
        var dpn = listManager.DpnList.FirstOrDefault(dpn => dpn.Id == loadCircuit.FedFromId);

        AssignSequenceNumbers(dpn.LeftCircuits);
        AssignSequenceNumbers(dpn.RightCircuits);


        var dpnSide = loadCircuit.PanelSide == DpnSide.Left.ToString() ? DpnSide.Left : DpnSide.Right;
        var circuitList = loadCircuit.PanelSide == DpnSide.Left.ToString() ? dpn.LeftCircuits : dpn.RightCircuits;

        int cctNumber = 0;
        int availableCircuitPosition = GetAvailableCircuit(dpn, loadCircuit, dpnSide).Item1;


        for (int i = 0; i < circuitList.Count; i++) {
            if (circuitList[i].VoltageType == null) {
                cctNumber += 2;
            }
            else {
                cctNumber += 2 * circuitList[i].VoltageType.Poles;
            }
            if (loadCircuit == circuitList[i]) {
                break;
            }
        }

        cctNumber = dpnSide == DpnSide.Left ? cctNumber -= 1 : cctNumber;


        var loadToAdd = new LoadToAddValidator(listManager) {
            
            Type = LoadTypes.OTHER.ToString(),
            Description = "Converted - " + loadCircuit.Description,
            AreaTag = dpn.Area.Tag,
            FedFromTag = dpn.Tag,

            VoltageType = loadCircuit.VoltageType,
            Size = 5.ToString(),
            Unit = Units.A.ToString(),
            LoadFactor = EdtSettings.LoadFactorDefault,
            PanelSide = loadCircuit.PanelSide,
            CircuitNumber = cctNumber,
            SequenceNumber = loadCircuit.SequenceNumber,

        };
        loadToAdd.Tag = $"{dpn.Tag}-{cctNumber}";

        loadCircuit = null;

        try {
            LoadModel newLoad = await LoadManager.AddLoad(loadToAdd, listManager, false);
            dpn.InsertLoad(newLoad);
        }
        catch (Exception ex) {
            EdtNotificationService.SendError(loadCircuit, "", "ConvertToLoad", ex);
        }

    }

}



