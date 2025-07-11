using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsCenterApi.Models;

[Table("facility_schedules")]
[Index("FacilityId", Name = "idx_facility_schedules_facility")]
public partial class FacilitySchedule
{
    [Key]
    [Column("schedule_id")]
    public int ScheduleId { get; set; }

    [Column("facility_id")]
    public int FacilityId { get; set; }

    [Column("day_of_week")]
    public string Day_of_Week { get; set; } = null!;

    [Column("opening_time")]
    public TimeOnly OpeningTime { get; set; }

    [Column("closing_time")]
    public TimeOnly ClosingTime { get; set; }

    [ForeignKey("FacilityId")]
    [InverseProperty("FacilitySchedules")]
    public virtual Facility Facility { get; set; } = null!;
}
