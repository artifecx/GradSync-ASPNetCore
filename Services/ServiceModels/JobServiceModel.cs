using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class JobServiceModel
    {
        [Required]
        public string JobId { get; set; }

        [Required]
        public string PostedById { get; set; }

        [Required(ErrorMessage = "Available slots must be set before the job can be unarchived.")]
        [Display(Name = "Available Slots")]
        public int? AvailableSlots { get; set; }
    }
}
