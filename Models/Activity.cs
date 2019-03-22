using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

public class Activity
{
    [Key]
    public int ActivityId { get; set; }
    [Required]
    [MinLength(2, ErrorMessage="Title must be 2 characters or longer!")]
    public string Title { get; set; }
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Activity Date:")]
    public DateTime ActivityDate { get; set; }
    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Time:")]
    public DateTime Time { get; set; }
    [Required]
    public int Duration { get; set; }
    [Required]
    public string DurationFormat { get; set; }

    [Required]
    public string Description { get; set; }
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }
    public List<RSVP> listOfGuests { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    // Will not be mapped to your users table!

}