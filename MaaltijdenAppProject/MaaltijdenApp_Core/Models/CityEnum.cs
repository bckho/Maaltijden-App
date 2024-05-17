using System.ComponentModel.DataAnnotations;

namespace MaaltijdenApp_Core.Models
{
    public enum CityEnum
    {
        [Display(Name = "Breda")]
        Breda,
        [Display(Name = "Den Bosch")]
        DenBosch,
        [Display(Name = "Tilburg")]
        Tilburg
    }
}
