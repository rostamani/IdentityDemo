using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentitySample.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="نام کاربری")]
        [Remote("IsUsernameInUse","Account",HttpMethod ="POST",AdditionalFields = "__RequestVerificationToken")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "ایمیل")]
        [EmailAddress]
        [Remote("IsEmailInUse","Account",HttpMethod ="POST",AdditionalFields = "__RequestVerificationToken")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "کلمه عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "تایید کلمه عبور")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="تایید کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }
    }
}
