namespace EcomWave.ViewModels.Vendor
{
    public class VendorViewModel
    {
        public string VendorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal AverageRating { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
