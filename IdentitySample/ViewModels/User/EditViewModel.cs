using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentitySample.ViewModels.User
{
    public class EditViewModel
    {
        public EditViewModel()
        {
            Roles = new List<RoleViewModel>();
            Claims = new List<ClaimViewModel>();
        }
        public string UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        public List<RoleViewModel> Roles{ get; set; }
        public List<ClaimViewModel> Claims{ get; set; }
    }

    public class RoleViewModel
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }

    public class ClaimViewModel
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}
