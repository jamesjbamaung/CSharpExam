using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

public class User
{
    [Key]
    public int UserId {get;set;}
    [Required]
    public string FirstName {get;set;}
    [Required]
    public string LastName {get;set;}
    [EmailAddress]
    [Required]
    public string Email {get;set;}
    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$", ErrorMessage = "Password must contain 1 letter, 1 number and 1 special character and MUST be 8 characters long")]
    

    public string Password {get;set;}
    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string Confirm {get;set;}
    public List<RSVP> listOfReservations { get; set; }
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    // Will not be mapped to your users table!

}