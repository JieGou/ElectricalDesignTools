using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  
}

