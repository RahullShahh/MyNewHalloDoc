using DAL.DataModels;

namespace DAL.ViewModels
{
    public class ProvidersOnCallModel
    {
        public List<Physician>? OnDuty { get; set; }
        public List<Physician>? OffDuty { get; set; }
    }
}
