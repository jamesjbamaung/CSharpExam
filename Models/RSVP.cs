using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

public class RSVP
{
    [Key]
    public int RSVPId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ActivityId { get; set; }
    public Activity Activity { get; set; }

}