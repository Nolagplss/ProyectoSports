using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsCenterApi.Models;

[Table("reservations")]
[Index("ReservationDate", Name = "idx_reservations_date")]
[Index("FacilityId", Name = "idx_reservations_facility")]
[Index("UserId", Name = "idx_reservations_user")]
public partial class Reservation
{
    [Key]
    [Column("reservation_id")]
    public int ReservationId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("facility_id")]
    public int FacilityId { get; set; }

    [Column("reservation_date")]
    public DateOnly ReservationDate { get; set; }

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }

    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [Column("payment_completed")]
    public bool? PaymentCompleted { get; set; }

    [Column("no_show")]
    public bool? NoShow { get; set; }

    [ForeignKey("FacilityId")]
    [InverseProperty("Reservations")]
    public virtual Facility Facility { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Reservations")]
    public virtual User User { get; set; } = null!;
}
