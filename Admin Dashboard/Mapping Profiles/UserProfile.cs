using Admin_Dashboard.ViewModels;
using AutoMapper;
using Domain.Models.Identity;

namespace Admin_Dashboard.Mapping_Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserViewModel>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom<UserRolesResolver>());
        }
    }
}
