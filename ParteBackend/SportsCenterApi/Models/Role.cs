using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsCenterApi.Models;

[Table("roles")]
[Index("RoleName", Name = "roles_role_name_key", IsUnique = true)]
public partial class Role
{
    [Key]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("role_name")]
    [StringLength(30)]
    public string RoleName { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
