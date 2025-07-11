using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsCenterApi.Models;

[Table("facilities")]
public partial class Facility
{
    [Key]
    [Column("facility_id")]
    public int FacilityId { get; set; }

    [Column("name")]
    [StringLength(15)]
    public string Name { get; set; } = null!;

    [Column("type")]
    [StringLength(20)]
    public string Type { get; set; } = null!;


    [Column("max_reservation_hours")]
    public int MaxReservationHours { get; set; }

    [Column("min_reservation_hours")]
    public int MinReservationHours { get; set; }

    [Column("cancellation_hours")]
    public int CancellationHours { get; set; }

    [InverseProperty("Facility")]
    public virtual ICollection<FacilitySchedule> FacilitySchedules { get; set; } = new List<FacilitySchedule>();

    [InverseProperty("Facility")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
