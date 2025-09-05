using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    /// <summary>
    /// AutoMapper profile (Application) for mapping Domain.User -> UpdateUserResult
    /// </summary>
    public class UpdateUserApplicationProfile : Profile
    {
        public UpdateUserApplicationProfile()
        {
            CreateMap<User, UpdateUserResult>()
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email,    opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone,    opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Role,     opt => opt.MapFrom(s => s.Role.ToString()))
                .ForMember(d => d.Status,   opt => opt.MapFrom(s => s.Status.ToString()));
        }
    }
}
