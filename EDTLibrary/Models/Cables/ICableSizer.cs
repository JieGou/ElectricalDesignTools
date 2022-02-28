namespace EDTLibrary.Models.Cables
{
    public interface ICableSizer
    {
        string GetAmpacityTable(IPowerCable cable);
    }
}