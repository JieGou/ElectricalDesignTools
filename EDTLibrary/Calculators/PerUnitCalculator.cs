using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Calculators;
public class PerUnitCalculator
{

    // pu qty = actual qty / base qty

    public enum VaBaseType
    {
        kVA,
        MVA,
    }
    public double BaseKva
    {
        get { return _baseKva; }
        set { _baseKva = value; }
    }
    private double _baseKva;

    public double BaseMva
    {
        get { return _baseMva; }
        set { _baseMva = value; }
    }
    private double _baseMva;
    public double BaseVoltage
    {
        get { return _baseVoltage; }
        set { _baseVoltage = value; }
    }
    private double _baseVoltage;
    public double BaseCurrent
    {
        get { return _baseCurrent; }
        set { _baseCurrent = value; }
    }
    private double _baseCurrent;

    public double Zpu
    {
        get { return _zPu; }
        set { _zPu = value; }
    }
    private double _zPu;
    //public double PuMva
    //{
    //    get { return _puMva; }
    //    set { _puMva = value; }
    //}
    //private double _puMva;
    //public double PuVoltage
    //{
    //    get { return _puVoltage; }
    //    set { _puVoltage = value; }
    //}
    //private double _puVoltage;
    //public double PuCurrent
    //{
    //    get { return _puCurrent; }
    //    set { _puCurrent = value; }
    //}
    //private double _puCurrent;

    public double getZpu_Mva(double actualImpedance, double baseMva, double baseKv)
    {
        return actualImpedance * baseMva / Math.Sqrt(baseKv);
    }

    public double getZpu_Kva(double actualImpedance, double baseKva, double baseKv)
    {
        return actualImpedance * baseKva / Math.Sqrt(baseKv) * 1000;
    }

    public double GetBaseCurrent(double baseKva, double baseVoltage, VaBaseType vaBaseType = VaBaseType.kVA)
    {
        double baseCurrent = 0;

        if (vaBaseType == VaBaseType.kVA) {
            baseCurrent = baseKva * 1000 / (Math.Sqrt(3) * baseVoltage); 
        }
        else {
            baseCurrent = baseKva * 1000000 / (Math.Sqrt(3) * baseVoltage);
        }

        return baseCurrent;
    }



}
