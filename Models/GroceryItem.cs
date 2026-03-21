using CommunityToolkit.Datasync.Server.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class GroceryItem : EntityTableData
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? AddedBy { get; set; }

        public bool IsChecked { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }
    }
}
