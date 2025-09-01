using System;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser
{
    /// <summary>
    /// AutoMapper profile for UpdateUser mappings
    /// </summary>
    public class UpdateUserProfile : Profile
    {
        public UpdateUserProfile()
        {
            // Request -> Command
            CreateMap<UpdateUserRequest, UpdateUserCommand>()
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email,    opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone,    opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Password, opt => opt.MapFrom(s => s.Password))
                .ForMember(d => d.Role,     opt => opt.MapFrom(s => (UserRole)s.Role))
                .ForMember(d => d.Status,   opt => opt.MapFrom(s => (UserStatus)s.Status));

            // Result -> Response
            CreateMap<UpdateUserResult, UpdateUserResponse>()
                .ForMember(d => d.Name,   opt => opt.MapFrom(s => s.Username)) // Username -> Name
                .ForMember(d => d.Email,  opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone,  opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Role,   opt => opt.MapFrom(s => Enum.Parse<UserRole>(s.Role, true)))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<UserStatus>(s.Status, true)));
        }
    }
}
