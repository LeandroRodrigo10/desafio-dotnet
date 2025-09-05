using AutoMapper;

// Aliases para evitar ambiguidade entre WebApi e Application
using WebReq = Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature.AuthenticateUserRequest;
using WebRes = Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature.AuthenticateUserResponse;
using AppCmd = Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser.AuthenticateUserCommand;
using AppRes = Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser.AuthenticateUserResult;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<WebReq, AppCmd>();

        CreateMap<AppRes, WebRes>();
    }
}
