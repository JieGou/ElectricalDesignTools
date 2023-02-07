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
    public double IBase
   {
        get { return _baseCurrent; }
        set { _baseCurrent = value; }
    }
    private double _baseCurrent;

    public double GetSbase(double vBase, double iBase)
    {
        return vBase * iBase * Math.Sqrt(3);
    }
    public double GetVbase(double sBase, double iBase)
    {
        return sBase / (Math.Sqrt(3) * iBase);
    }
    public double GetIBase(double sBase, double vBase)
    {
        return sBase  / (Math.Sqrt(3) * vBase);
    }

    public double GetZbase(double sBase, double vBase)
    {
        return Math.Pow(vBase, 2) / sBase;
    }
     
    
    

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

    public double getZpu_Mva(double actualZohm, double baseMva, double baseKv)
    {
        return actualZohm * baseMva / Math.Pow(baseKv,2);
    }

    public double getZpu_Kva(double actualZohm, double baseKva, double baseKv)
    {
        return actualZohm * baseKva / Math.Sqrt(baseKv) * 1000;
    }

    

}
