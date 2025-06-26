using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsCenterApi.Models;

[Table("members")]
[Index("UserId", Name = "idx_members_user")]
public partial class Member
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("registration_date")]
    public DateOnly RegistrationDate { get; set; }

    [Column("deactivation_date")]
    public DateOnly? DeactivationDate { get; set; }

    [Column("penalized")]
    public bool? Penalized { get; set; }

    [Column("penalty_end_date")]
    public DateOnly? PenaltyEndDate { get; set; }

    [Column("bank_details")]
    [StringLength(34)]
    public string? BankDetails { get; set; }

    [Column("observations")]
    public string? Observations { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Member")]
    public virtual User User { get; set; } = null!;
}
