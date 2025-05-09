using Admin_Dashboard.ViewModels;
using AutoMapper;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Admin_Dashboard.Mapping_Profiles
{
    public class UserRolesResolver(UserManager<AppUser> userManager) : IValueResolver<AppUser, UserViewModel, IEnumerable<string>>
    {
        public IEnumerable<string> Resolve(AppUser source, UserViewModel destination, IEnumerable<string> destMember, ResolutionContext context)
        {
            return userManager.GetRolesAsync(source).Result;
        }
    }
}
