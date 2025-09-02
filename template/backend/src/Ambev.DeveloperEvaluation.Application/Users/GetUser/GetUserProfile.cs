using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUser
{
    /// <summary>
    /// Profile for mapping Domain.User -> GetUserResult (Application)
    /// </summary>
    public class GetUserProfile : Profile
    {
        public GetUserProfile()
        {
            CreateMap<User, GetUserResult>()
                // Nome do domínio é Username; o DTO expõe Name
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status));
            // Id é mapeado por convenção
        }
    }
}
