using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsCenterApi.Models;

[Table("permissions")]
public partial class Permission
{
    [Key]
    [Column("permission_id")]
    public int PermissionId { get; set; }

    [Column("description")]
    [StringLength(100)]
    public string Description { get; set; } = null!;

    [ForeignKey("PermissionId")]
    [InverseProperty("Permissions")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
