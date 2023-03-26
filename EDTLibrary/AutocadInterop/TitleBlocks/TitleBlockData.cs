using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.AutocadInterop.TitleBlocks;
public class TitleBlock
{
    public int Id { get; set; }

	public string DisplayName
	{
		get { return _displayName; }
		set { _displayName = value; }
	}
    private string _displayName;

	private string _fullPath;

	public string FullPath
	{
		get { return _fullPath; }
		set { _fullPath = value; }
	}


	public ObservableCollection<CadAttribute> TitleBlockAttributes
    {
		get { return _titleBlockAttributes; }
		set { _titleBlockAttributes = value; }
	}
    private ObservableCollection<CadAttribute> _titleBlockAttributes;

}
