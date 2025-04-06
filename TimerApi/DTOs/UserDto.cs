﻿using System.ComponentModel.DataAnnotations;

namespace TimerApi.DTOs
{
    public class UserDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
