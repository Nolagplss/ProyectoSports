﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace SportsCenterApi.Models.DTO
{
    public class UserDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
        
        public string? Phone { get; set; }

        [Required]
        public int RoleId { get; set; }

    }
}
