using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace PersianTranslation.Identity
{
    public class PersianIdentityErrorDexcriber:IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"این ایمیل {email} توسط شخص دیگری انتخاب شده است"
            };
        }
    }
}
