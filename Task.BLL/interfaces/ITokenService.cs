using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.DAL.Entity;

namespace Task.BLL.interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(ApplicationUser user, UserManager<ApplicationUser> userManager);
    }
}
