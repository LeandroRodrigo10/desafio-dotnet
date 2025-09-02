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
                // Nome do dom�nio � Username; o DTO exp�e Name
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone))
                .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status));
            // Id � mapeado por conven��o
        }
    }
}
