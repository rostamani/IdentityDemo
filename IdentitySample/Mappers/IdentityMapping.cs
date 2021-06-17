using AutoMapper;
using IdentitySample.ViewModels.Account;
using IdentitySample.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitySample.Mappers
{
    public class IdentityMapping:Profile
    {
        public IdentityMapping()
        {
            CreateMap<RegisterViewModel, IdentityUser>().ReverseMap();
            CreateMap<IndexViewModel, IdentityUser>().ReverseMap();
        }
    }
}
