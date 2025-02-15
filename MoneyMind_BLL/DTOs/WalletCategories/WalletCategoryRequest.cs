using MoneyMind_BLL.DTOs.Activities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.WalletCategories
{
    public class WalletCategoryRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "IconPath must be a valid URL.")]
        [MaxLength(500, ErrorMessage = "IconPath cannot exceed 500 characters.")]
        public string IconPath { get; set; }

        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a valid hex color code (e.g., #FFFFFF).")]
        public string Color { get; set; }

        [Required(ErrorMessage = "WalletTypeId is required.")]
        public Guid WalletTypeId { get; set; }

        public List<Attachment_ActivityRequest>? Activities { get; set; } = new List<Attachment_ActivityRequest>();

    }
}
