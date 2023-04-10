using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlightSightings.Models;

public enum SortOrder { Ascending = 0, Descending = 1}
public partial class Flight
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Make must be between 1 and 128 character in length.")]
    public string Make { get; set; } = null!;

    [Required]
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Model must be between 1 and 128 character in length.")]
    public string Model { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[\s\S]{1,2}-[\s\S]{1,5}$",
            ErrorMessage = "Registration should be separated by a hyphen prefix 1-2 characters suffix 1-5 characters.")]
    public string Registration { get; set; } = null!;

    [StringLength(50)]
    [DisplayName("Image")]
    public string? ImageName { get; set; }

    public byte[]? ImageData { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Location must be between 1 and 128 character in length.")]
    public string Location { get; set; } = null!;

    [Required]
    [DisplayName("Sighting Date & Time")]
    [MyDate(ErrorMessage = "Cannot be a future datetime.")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime DateTime { get; set; } = DateTime.Now.AddDays(-1);

    [NotMapped]
    [DisplayName("Upload File")]
    public IFormFile? ImageFile { get; set; }

    public class MyDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? SightingTime)
        {
            DateTime d = Convert.ToDateTime(SightingTime);
            return d < DateTime.Now;

        }
    }
}
