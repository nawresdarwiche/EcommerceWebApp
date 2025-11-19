using System.ComponentModel.DataAnnotations;

namespace Tp2.ViewModels
{
    public class CreateRoleViewModel
 {
 [Required]
    [Display(Name = "Role")]
    public string RoleName { get; set; }
}
}
