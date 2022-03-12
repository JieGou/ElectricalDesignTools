using PropertyChanged;

namespace EDTLibrary.Models
{
    [AddINotifyPropertyChangedInterface]
    public class AreaModel
    {
        public int Id { get; set; }
        private string _tag;

        public string Tag
        {
            get { return _tag; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) == false ) {
                    _tag = value;
                }
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
        public AreaModel Area { get; set; }

        public string AreaCategory { get; set; }
        public string AreaClassification { get; set; }

        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }

        public string NemaType { get; set; }

    }
}
