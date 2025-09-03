using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.IntegrationTests.Authentication
{
    public sealed class AuthenticateTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthenticateTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact(DisplayName = "Deve criar usuário via API, autenticar e acessar endpoint protegido")]
        public async Task Should_CreateUser_Login_And_Access_Protected_Endpoint()
        {
            var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Dados do usuário de teste
            var userId = Guid.NewGuid();
            const string email = "test@example.com";
            const string username = "testuser";
            const string password = "P@ssw0rd";
            const string phone = "11999999999";

            // 1) Criar usuário via API (POST /api/Users) — está [AllowAnonymous] no seu controller
            //    Observação: Ajuste Role/Status conforme seus enums (inteiros). 1 costuma ser "Active".
            var createUserPayload = JsonSerializer.Serialize(new
            {
                username,
                email,
                phone,
                password,
                role = 0,   // ajuste se necessário (ex.: 0=User, 1=Admin). Não impacta o login.
                status = 1  // costuma ser "Active"
            });

            using (var createContent = new StringContent(createUserPayload, Encoding.UTF8, "application/json"))
            {
                var createResp = await client.PostAsync("/api/Users", createContent);

                // Deve retornar 201 Created
                createResp.StatusCode.Should().Be(HttpStatusCode.Created, await createResp.Content.ReadAsStringAsync());
            }

            // 2) Autenticar (POST /api/Auth)
            var authPayload = JsonSerializer.Serialize(new { email, password });
            using var loginContent = new StringContent(authPayload, Encoding.UTF8, "application/json");

            var authResp = await client.PostAsync("/api/Auth", loginContent);
            authResp.StatusCode.Should().Be(HttpStatusCode.OK, await authResp.Content.ReadAsStringAsync());

            var authJson = await authResp.Content.ReadAsStringAsync();
            using var authDoc = JsonDocument.Parse(authJson);

            // Corpo padrão: ApiResponseWithData<AuthenticateUserResponse>
            authDoc.RootElement.TryGetProperty("data", out var dataProp).Should().BeTrue("corpo deve conter 'data'");
            dataProp.TryGetProperty("token", out var tokenProp).Should().BeTrue("data deve conter 'token'");
            var token = tokenProp.GetString();
            token.Should().NotBeNullOrWhiteSpace();

            // 3) Acessar endpoint protegido com o token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var usersResp = await client.GetAsync("/api/Users");
            usersResp.StatusCode.Should().Be(HttpStatusCode.OK, await usersResp.Content.ReadAsStringAsync());
        }
    }
}
