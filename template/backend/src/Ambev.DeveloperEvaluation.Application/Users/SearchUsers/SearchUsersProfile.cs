using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Users.SearchUsers
{
    public class SearchUsersProfile : Profile
    {
        public SearchUsersProfile()
        {
            CreateMap<User, SearchUsersResult.UserSummary>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));
        }
    }
}
