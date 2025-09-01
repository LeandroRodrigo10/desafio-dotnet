using System;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser
{
    public class CreateUserProfile : Profile
    {
        public CreateUserProfile()
        {
            // WebApi Request -> Application Command
            CreateMap<CreateUserRequest, CreateUserCommand>()
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Password, opt => opt.MapFrom(s => s.Password))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => (UserRole)s.Role))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => (UserStatus)s.Status));

            // Application Result -> WebApi Response
            CreateMap<CreateUserResult, CreateUserResponse>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => Enum.Parse<UserRole>(s.Role, true)))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => Enum.Parse<UserStatus>(s.Status, true)))
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id));
        }
    }
}
