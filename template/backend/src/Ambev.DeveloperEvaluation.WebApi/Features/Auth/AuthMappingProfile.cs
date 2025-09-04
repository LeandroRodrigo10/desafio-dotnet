using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>();
        CreateMap<AuthenticateUserResult, AuthenticateUserResponse>();

    }
}
