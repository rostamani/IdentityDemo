using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace IdentitySample.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required, Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required, Display(Name = "کلمه عبور"),DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
        public IEnumerable<AuthenticationScheme> ExternalLogins { get; set; }

    }
}
