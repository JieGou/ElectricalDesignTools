using EDTLibrary;
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

namespace WpfUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class CableListViewModel : ViewModelBase
{
    private ListManager _listManager;

    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }

    public CableListViewModel(ListManager listManager)
    {
        _listManager = listManager;
        //_view = CollectionViewSource.GetDefaultView(listManager.CableList);
        //View = CollectionViewSource.GetDefaultView(listManager.CableList);
    }

    private PropertyGroupDescription _usageGroup = new PropertyGroupDescription();
    private PropertyGroupDescription _typeGroup = new PropertyGroupDescription();
    private PropertyGroupDescription _sizeGroup = new PropertyGroupDescription();

    private ICollectionView _view;
    public ICollectionView View
    {
        get
        {
            if (_view == null) {
                //View = CollectionViewSource.GetDefaultView(_listManager.CableList);
                _view = CollectionViewSource.GetDefaultView(_listManager.CableList);
            }
            return _view; }
        set { _view = View; }
    }

    private bool _usageGroupView;
    public bool UsageGroupView
    {
        get { return _usageGroupView; }
        set
        {
            _usageGroupView = value;
            SetSizeGroupNames();
            View.GroupDescriptions.Clear();
            if (value == false) {
                _typeGroupView = value;
                _sizeGroupView = value;
            }
            if (value) {
                View.GroupDescriptions.Add(_usageGroup);
            }
         
            OnPropertyChanged("");

        }
    }
    private bool _typeGroupView;
    public bool TypeGroupView
    {
        get { return _typeGroupView; }
        set
        {
            _typeGroupView = value;

            SetSizeGroupNames();
            View.GroupDescriptions.Remove(_typeGroup);
            View.GroupDescriptions.Remove(_sizeGroup);

            if (_usageGroupView == false) {
                _usageGroupView = true;
                View.GroupDescriptions.Add(_usageGroup);
            }
            if (value == false) {
                _sizeGroupView = value;
            }
            if (value) {
                _usageGroupExpanded = true;
                View.GroupDescriptions.Add(_typeGroup);
            }
       
            OnPropertyChanged("");
        }
    }

    private bool _sizeGroupView;
    public bool SizeGroupView
    {
        get { return _sizeGroupView; }
        set
        {
            _sizeGroupView = value;
            SetSizeGroupNames();
            View.GroupDescriptions.Remove(_sizeGroup);
            if (_usageGroupView == false) {
                _usageGroupView = true;
                View.GroupDescriptions.Add(_usageGroup);
            }
            if (_typeGroupView ==false) {
                _typeGroupView = value;
                View.GroupDescriptions.Add(_typeGroup);
            }
            if (value) {
                _usageGroupExpanded = true;
                _typeGroupExpanded = true;
                View.GroupDescriptions.Add(_sizeGroup);
            }
     
            OnPropertyChanged("");
        }
    }

    private bool _usageGroupExpanded = true;
    public bool UsageGroupExpanded
    {
        get { return _usageGroupExpanded; }
        set
        {
            _usageGroupExpanded = value;
            var temp = UsageGroupView;
            UsageGroupView = false;
            UsageGroupView = temp;
            OnPropertyChanged("");
        }
    }

    private bool _typeGroupExpanded = true;
    public bool TypeGroupExpanded
    {
        get { return _typeGroupExpanded; }
        set
        {
            _typeGroupExpanded = value;
            var temp = TypeGroupView;
            TypeGroupView = false;
            TypeGroupView = temp;
            OnPropertyChanged("");
        }
    }

    private bool _sizeGroupExpanded;
    public bool SizeGroupExpanded
    {
        get { return _sizeGroupExpanded; }
        set 
        {
            _sizeGroupExpanded = value;
            var temp = SizeGroupView;
            SizeGroupView = false;
            SizeGroupView = temp;
            OnPropertyChanged("");
        }
    }

    private string _tagFilter;

    public string TagFilter
    {
        get { return _tagFilter; }
        set 
        { 
            _tagFilter = value;
            View.Filter = new Predicate<object>(Contains);
            OnPropertyChanged();
        }
    }
    private bool Contains(object item)
    {
        PowerCableModel cable = (PowerCableModel)item;

        if (TagFilter != "") {
            return (cable.Tag.ToLower()).Contains(TagFilter.ToLower());
        }
        return true;
    }
    private void SetSizeGroupNames()
    {
        //TODO - This is a method so that names can be changed and tested with HotReload.
        //          Move values to field instantiation once finalized.
        _usageGroup.PropertyName = ("UsageType");
        _typeGroup.PropertyName = ("Type");
        _sizeGroup.PropertyName = ("Size");
    }

    public void PropChanged()
    {
        OnPropertyChanged();
    }
}

