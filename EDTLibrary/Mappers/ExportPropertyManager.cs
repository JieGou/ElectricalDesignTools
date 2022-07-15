using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Mappers;
public class ExportPropertyManager
{
    public List<string> GetPropertyList(string listName)
    {
        var propertyList = new List<string>();
        var mapperList = new List<ExportMappingModel>();

        mapperList = DaManager.prjDb.GetRecords<ExportMappingModel>("ExportMapping").ToList();
        propertyList = mapperList.Where(m => m.Type == listName && m.IncludeInReport == true).Select(m => m.PropertyName).ToList();

        return propertyList;
    }
}
