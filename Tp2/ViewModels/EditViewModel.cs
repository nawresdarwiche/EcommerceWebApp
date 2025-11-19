using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Tp2.ViewModels
{
    public class EditViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Le prix est obligatoire")]
        [Range(0.01, 100000, ErrorMessage = "Le prix doit être entre 0.01 et 100000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La quantité est obligatoire")]
        [Range(0, 100000, ErrorMessage = "La quantité doit être entre 0 et 100000")]
        public int QteStock { get; set; }

        [Required(ErrorMessage = "La catégorie est obligatoire")]
        public int CategoryId { get; set; }

        public IFormFile? ImagePath { get; set; }
        public string? ExistingImagePath { get; set; }
    }
}