using System;
using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser
{
    /// <summary>
    /// Profiles para o fluxo de GetUser:
    /// - Guid -> GetUserCommand
    /// - GetUserResult -> GetUserResponse
    /// </summary>
    public class GetUserProfile : Profile
    {
        public GetUserProfile()
        {
            CreateMap<Guid, GetUserCommand>()
                .ConstructUsing(id => new GetUserCommand(id));

            CreateMap<GetUserResult, GetUserResponse>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role))     
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status));  
        }
    }
}
