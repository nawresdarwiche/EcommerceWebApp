using Tp2.Models;
using System.ComponentModel.DataAnnotations;
using Tp2.Models;

namespace Tp2.ViewModels
{
    public class CreateViewModel
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public required string Name { get; set; }
        [Required]
        [Display(Name = "Prix en dinar :")]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Quantité en unité :")]
        public int QteStock { get; set; }
        public int CategoryId { get; set; }
        //public Category? Category { get; set; } // Nullable, car non renseigné par le formulaire
        [Required]
        [Display(Name = "Image :")]
        public required IFormFile ImagePath { get; set; }
    }
}
