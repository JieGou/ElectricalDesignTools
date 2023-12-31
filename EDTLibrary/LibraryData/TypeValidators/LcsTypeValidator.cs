﻿using EdtLibrary.LibraryData.TypeModels;
using EDTLibrary;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace EdtLibrary.LibraryData.TypeValidators;

[AddINotifyPropertyChangedInterface]
public class LcsTypeValidator : TypeValidatorBase
{
    private ListManager _listManager;
    private IArea _areaModel;
    private IDteq _feedingDteq;

    public LcsTypeValidator()
    {

    }
    public LcsTypeValidator(LcsTypeModel lcsToAdd)
    {

        Type = lcsToAdd.Type;
        Description = lcsToAdd.Description;
        DigitalConductorQty = lcsToAdd.DigitalConductorQty.ToString();
        AnalogConductorQty = lcsToAdd.AnalogConductorQty.ToString();

    }

    private string _type;
    public string Type
    {
        get { return _type; }
        set
        {
            _type = value;
            _isValid = true;

            ClearErrors(nameof(Type));
            if (GlobalConfig.SelectingNew == true) { return; }

            if (string.IsNullOrWhiteSpace(_type) || _type == "") {
                AddError(nameof(Type), "Invalid Type. Type cannot be empty.");
                _isValid = false;

            }
            else if (TypeManager.LcsTypes.FirstOrDefault(lt => lt.Type == _type) != null) {
                AddError(nameof(Type), "Type identifier already exists");
                _isValid = false;

            }

        }
    }

    private string _description;
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }
    private string _areaTag;

    private string _digitalConductorQty;

    public string DigitalConductorQty
    {
        get { return _digitalConductorQty; }
        set
        {

            _digitalConductorQty = value;

            double qty;
            ClearErrors(nameof(DigitalConductorQty));

            if (Double.TryParse(_digitalConductorQty, out qty) == false ||
                string.IsNullOrWhiteSpace(_digitalConductorQty) || _digitalConductorQty == "") {
                AddError(nameof(DigitalConductorQty), "Invalid value");
                _isValid = false;
            }

            else if (double.Parse(_digitalConductorQty) < 2) {
                AddError(nameof(DigitalConductorQty), "Min. qty = 2");
                _isValid = false;
            }

            _isValid = true;
        }
    }

    private string _analogConductorQty;
    public string AnalogConductorQty
    {
        get { return _analogConductorQty; }
        set
        {
            _analogConductorQty = value;

            int qty;
            ClearErrors(nameof(AnalogConductorQty));

            if (int.TryParse(_analogConductorQty, out qty) == false ||
                string.IsNullOrWhiteSpace(_analogConductorQty) || _analogConductorQty == "") {
                AddError(nameof(AnalogConductorQty), "Invalid value");
                _isValid = false;
                return;
            }

            _isValid = true;
        }
    }



    private bool _isValid;
    public override bool IsValid()
    {

        string temp;
        string fake = "fake";



        temp = Type;
        Type = fake;
        Type = temp;

        temp = Description;
        Description = fake;
        Description = temp;

        temp = DigitalConductorQty;
        DigitalConductorQty = fake;
        DigitalConductorQty = temp;

        temp = AnalogConductorQty;
        AnalogConductorQty = fake;
        AnalogConductorQty = temp;

        if (_isValid && HasErrors == false) {
            return true;
        }
        return false;
    }


    
}

