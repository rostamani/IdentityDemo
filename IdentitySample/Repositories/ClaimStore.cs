using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Repositories
{
    public static class ClaimStore
    {
        public static List<Claim> Claims = new List<Claim>() { 
            new Claim(ClaimType.EmployeeIndex,true.ToString()),
            new Claim(ClaimType.EmployeeDelete,true.ToString()),
            new Claim(ClaimType.EmployeeEdit,true.ToString()),
            new Claim(ClaimType.EmployeeDetails,true.ToString())
        };
    }

    public static class ClaimType
    {
        public const string EmployeeIndex = "EmployeeIndex";
        public const string EmployeeEdit = "EmployeeEdit";
        public const string EmployeeDelete = "EmployeeDelete";
        public const string EmployeeDetails = "EmployeeDetails";        
    }
}
