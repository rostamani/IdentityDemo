using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentitySample.ViewModels.Role
{
    public class CreateRoleViewModel
    {
        [Required]
        [Remote("DoesRoleExist","ManageRoles",HttpMethod ="POST")]
        public string Name { get; set; }

    }
}
