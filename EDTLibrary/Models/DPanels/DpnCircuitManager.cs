using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Selectors;
using EDTLibrary.Services;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EDTLibrary.Models.DistributionEquipment.DPanels;
public class DpnCircuitManager
{

    public static bool AddNewLoad(IDpn dpn, IPowerConsumer load)
    {
        if (CanAdd(dpn, load)) {

            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            //Left
            if ((_leftAvailableSeq != -1 && _leftAvailableSeq <= _rightAvailaleSeq) ||
                            (_leftAvailableSeq != -1 && _rightAvailaleSeq == -1)) {
                load.PanelSide = PnlSide.Left.ToString();
                load.CircuitNumber = _leftAvailableCct;
                sideCircuitList = dpn.LeftCircuits;
            }
            //Right
            else if ((_rightAvailaleSeq != -1 && _rightAvailaleSeq <= _leftAvailableSeq) ||
                    (_rightAvailaleSeq != -1 && _leftAvailableSeq == -1)) {
                load.PanelSide = PnlSide.Right.ToString();
                load.CircuitNumber = _rightAvailableCct;
                sideCircuitList = dpn.RightCircuits;

            }

            

            var loadCircuitToRemove = new LoadCircuit();
            int cctNum = 0;
            dpn.OrderCircuitsByCircuitNumber(sideCircuitList);
            AssignCircuitNumbers(sideCircuitList);
            for (int i = 0; i < load.VoltageType.Poles; i++) {
                cctNum = i * 2 + load.CircuitNumber;
                loadCircuitToRemove = dpn.AssignedCircuits.FirstOrDefault(lc => lc.CircuitNumber == cctNum);
                dpn.AssignedCircuits.Remove(loadCircuitToRemove);
                sideCircuitList.Remove(loadCircuitToRemove);
                if (loadCircuitToRemove!= null) {
                    DaManager.prjDb.DeleteRecord(GlobalConfig.LoadCircuitTable, loadCircuitToRemove.Id);
                }
            }

            dpn.AssignedLoads.Add(load);
            sideCircuitList.Add(load);
            return true;
        }
        return false;
        
    }

    private static int _leftAvailableSeq = 0;
    private static int _rightAvailaleSeq = 0;
    private static int _leftAvailableCct = 0;
    private static int _rightAvailableCct = 0;

    public static bool CanAdd(IDpn dpn, IPowerConsumer load)
    {
     
        _leftAvailableSeq = GetAvailableCircuit(dpn, load, PnlSide.Left).Item1;
        _rightAvailaleSeq = GetAvailableCircuit(dpn, load, PnlSide.Right).Item1;

        _leftAvailableCct = GetAvailableCircuit(dpn, load, PnlSide.Left).Item2;
        _rightAvailableCct = GetAvailableCircuit(dpn, load, PnlSide.Right).Item2;

        if (_leftAvailableSeq < 0 && _rightAvailaleSeq < 0) return false;

        if (dpn.AssignedLoads.FirstOrDefault(l => l.Id == load.Id) != null) return false;

        return true;
    }

    public static void DeleteLoad(IDpn dpn, IPowerConsumer load, ListManager listManager)
    {
        if (load == null || load is LoadCircuit) return;
        dpn.RemoveAssignedLoad(load);
        LoadManager.DeleteLoadAsync(load, listManager);
        dpn.CalculateLoading();
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
    public static Tuple<int, int> GetAvailableCircuit(IDpn dpn, IPowerConsumer load, PnlSide dpnSide)
    {
        var circuit = new Tuple<int, int>(0,0);
        int seqNum = 0;
        int cctNum = 0;

        for (int i = 0; i < dpn.CircuitCount / 2; i++) {
            if (IsCircuitAvailable(dpn, load, seqNum, dpnSide)) {

                cctNum = GetCircuitNumber(dpn, dpnSide, seqNum);
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

    private static int GetCircuitNumber(IDpn dpn, PnlSide panelSide, int sequenceNumber)
    {
        int cct = 0;
        var sideCircuitList = panelSide == PnlSide.Left ? dpn.LeftCircuits : dpn.RightCircuits;

        for (int i = 0; i <= sequenceNumber; i++) {
            if (i==0) {
                cct = panelSide == PnlSide.Left ? 1 : 2;
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
    private static bool IsCircuitAvailable(IDpn dpn, IPowerConsumer load, int cct, PnlSide dpnSide)
    {
        bool isAvailable = false;
        List<IPowerConsumer> cctList = new List<IPowerConsumer>();
        int poleCount = 0;

        if (dpnSide == PnlSide.Left) {
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
                if (cctList[cct + pole].GetType() == typeof(LoadCircuit)) {
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

        sideCircuitList.OrderBy(c => c.CircuitNumber);
        dpn.CalculateLoading(); //to calculate the phase loading
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


        sideCircuitList.OrderBy(c => c.CircuitNumber);
        dpn.CalculateLoading();
    }

    public static void MoveCircuitLeft(IDpn dpn, IPowerConsumer load)
    {
        if (load == null) return;
        PnlSide dpnSide;
        dpnSide = dpn.LeftCircuits.FirstOrDefault(ld => ld == load) != null ? PnlSide.Left : PnlSide.Right;
        if (dpnSide == PnlSide.Left) return;

        if (dpnSide == PnlSide.Right && GetAvailableCircuit(dpn, load, PnlSide.Left).Item1 != -1) {
            load.SequenceNumber = GetAvailableCircuit(dpn, load, PnlSide.Left).Item1;
            load.CircuitNumber = GetAvailableCircuit(dpn, load, PnlSide.Left).Item2;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
            dpn.LeftCircuits.Add(load);
            dpn.RightCircuits.Remove(load);
            load.PanelSide = PnlSide.Left.ToString();
        }
        dpn.SetCircuits();
        dpn.CalculateLoading();
    }

    public static void MoveCircuitRight(IDpn dpn, IPowerConsumer load)
    {

        if (load == null) return;
        PnlSide dpnSide; 
        dpnSide = dpn.LeftCircuits.FirstOrDefault(ld => ld == load) != null ? PnlSide.Left : PnlSide.Right;
        if (dpnSide == PnlSide.Right) return;

        if (dpnSide == PnlSide.Left && GetAvailableCircuit(dpn, load, PnlSide.Right).Item1 != -1) {
            load.SequenceNumber = GetAvailableCircuit(dpn, load, PnlSide.Right).Item1;
            load.CircuitNumber = GetAvailableCircuit(dpn, load, PnlSide.Right).Item2;
            dpn.LeftCircuits.Remove(load);
            dpn.RightCircuits.Add(load);
            load.PanelSide = PnlSide.Right.ToString();
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

        //left
        if (sideCircuitList[1].PanelSide == PnlSide.Left.ToString()) {
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

        //right
        else if(sideCircuitList[1].PanelSide == PnlSide.Right.ToString()) {
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

    public static bool IsConverting { get; set; }
    public static async Task ConvertToLoad(LoadCircuit loadCircuit)
    {
        IsConverting = true;

        var listManager = ScenarioManager.ListManager;
        var dpn = listManager.DpnList.FirstOrDefault(dpn => dpn.Id == loadCircuit.FedFromId);

        var dpnSide = loadCircuit.PanelSide == PnlSide.Left.ToString() ? PnlSide.Left : PnlSide.Right;
        var circuitList = loadCircuit.PanelSide == PnlSide.Left.ToString() ? dpn.LeftCircuits : dpn.RightCircuits;

        var loadToAdd = new LoadToAddValidator(listManager) {

            Type = LoadTypes.OTHER.ToString(),
            Description = loadCircuit.Description,
            AreaTag = dpn.Area.Tag,
            FedFromTag = dpn.Tag,

            VoltageType = loadCircuit.VoltageType,
            Size = 5.ToString(),
            Unit = Units.A.ToString(),
            DemandFactor = DemandFactorSelector.GetDemandFactor(LoadTypes.OTHER.ToString()).ToString(),
            PanelSide = loadCircuit.PanelSide,
            CircuitNumber = loadCircuit.CircuitNumber,
            SequenceNumber = loadCircuit.SequenceNumber,

        };
        //loadToAdd.Tag = $"{dpn.Tag}-{loadCircuit.CircuitNumber}";
        loadToAdd.Tag = TagManager.AssignEqTag(loadToAdd.Type, listManager);
        loadCircuit = null;

        try {
            LoadModel newLoad = await LoadManager.AddLoad(loadToAdd, listManager, false);
            //newLoad.Tag= TagManager.AssignEqTag(newLoad,listManager);
            dpn.InsertLoad(newLoad);
            IsConverting = false;
            AssignCircuitNumbers(circuitList);

        }
        catch (Exception ex) {
            EdtNotificationService.SendError(loadCircuit, "", "ConvertToLoad", ex);
        }
        finally { 
            IsConverting = false; 
        }

    }

}



