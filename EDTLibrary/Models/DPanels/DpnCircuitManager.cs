using EDTLibrary.Managers;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EDTLibrary.Models.DistributionEquipment.DPanels;
public class DpnCircuitManager
{

    public static void AddLoad(DpnModel dpn, LoadModel load, ListManager listManager)
    {
        int leftCct = 0;
        int rightCct = 0;

        leftCct = GetAvailableCircuit(dpn, load, DpnSide.Left);
        
        rightCct = GetAvailableCircuit(dpn, load, DpnSide.Right);

        if (leftCct < 0 && rightCct < 0) {
            return;
        }

        if (dpn.AssignedLoads.FirstOrDefault(l => l.Id == load.Id) == null) {

           
            if (leftCct < rightCct || rightCct== -1) {
                if (leftCct != -1) {
                    dpn.LeftCircuits[leftCct] = load;
                    //dpn.SetLeftCircuits();
                }
            }

            else if(rightCct < leftCct || leftCct == -1) {
                 if (rightCct != -1) {
                    dpn.RightCircuits[rightCct] = load;
                    //dpn.SetRightCircuits();
                }
            }

            dpn.AssignedLoads.Add(load);
        }
    }


    /// <summary>
    /// Return the number of the next available circuit(s) for a load
    /// </summary>
    /// <param name="dpn"></param>
    /// <param name="load"></param>
    /// <param name="dpnSide"></param>
    /// <returns></returns>
    private static int GetAvailableCircuit(DpnModel dpn, LoadModel load, DpnSide dpnSide)
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
    private static bool IsCircuitAvailable(DpnModel dpn, LoadModel load, int cct, DpnSide dpnSide)
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
            if (item.Type == "MOTOR") {
                poleCount += 2;
            }
            else if (item.Type == "PANEL") {
                poleCount += 3;
            }
            else { poleCount += 1; }
        }

        int loadPoles = 1;
        if (load.Type == "MOTOR") {
            loadPoles = 2;
        }
        else if (load.Type == "PANEL") {
            loadPoles = 3;
        }
    

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

    
}

