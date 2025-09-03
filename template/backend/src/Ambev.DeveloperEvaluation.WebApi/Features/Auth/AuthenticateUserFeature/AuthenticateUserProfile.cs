using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature
{
    /// <summary>
    /// AutoMapper profile for the Authenticate User feature.
    /// Maps WebApi request/response <-> Application command/result.
    /// </summary>
    public class AuthenticateUserProfile : Profile
    {
        public AuthenticateUserProfile()
        {
            // WebApi -> Application
            CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>();

            // Application -> WebApi
            CreateMap<AuthenticateUserResult, AuthenticateUserResponse>();
        }
    }
}
