// IT21215988
// Waseem M.I.M

using System.ComponentModel.DataAnnotations;

namespace EcomWave.DTO
{
    public class CustomerUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }

    public class VendorRatingDTO
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
    }

    public class UpdateCommentDTO
    {
        [StringLength(500)]
        public string NewComment { get; set; }
    }
}
