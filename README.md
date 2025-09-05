Ambev Developer Evaluation — API (.NET 8)
📌 Visão Geral

Projeto desenvolvido como parte do desafio Developer Evaluation Project.
API em .NET 8 com arquitetura DDD + CQRS + MediatR + EF Core + AutoMapper + JWT (Bearer) + Swagger + Serilog.

Funcionalidades:

CRUD completo de Sales (com itens).

Regras de desconto por quantidade e limite de 20 unidades.

Cancelamento de venda e cancelamento de item.

Logs de eventos de domínio (SaleCreated, SaleModified, SaleCancelled, ItemCancelled).

Autenticação e autorização com JWT.

⚙️ Como Rodar
Pré-requisitos

.NET 8 SDK

PostgreSQL
 ou Docker

Clonar e restaurar
git clone https://github.com/LeandroRodrigo10/desafio-dotnet.git
cd desafio-dotnet/template/backend
dotnet restore

Banco de Dados

Rodar com Docker:

docker run -d --name ambev-pg -e POSTGRES_PASSWORD=dev -e POSTGRES_DB=ambev -p 5432:5432 postgres:16


Configurar appsettings.Development.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ambev;Username=postgres;Password=dev"
  },
  "Jwt": {
    "Issuer": "Ambev.DeveloperEvaluation",
    "Audience": "Ambev.DeveloperEvaluation.Clients",
    "Key": "dev-super-secret-key-change-me-0123456789"
  }
}


Criar/migrar DB:

dotnet ef database update --project src/Ambev.DeveloperEvaluation.WebApi

Rodar API
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi


Swagger: https://localhost:7181/swagger

🔐 Autenticação

Criar usuário Admin:

POST /api/Users
{
  "username":"Admin",
  "email":"admin@example.com",
  "phone":"55999999999",
  "role":"Admin",
  "status":"Active",
  "password":"Pass@123"
}


Login:

POST /api/Auth
{
  "email":"admin@example.com",
  "password":"Pass@123"
}


No Swagger, clique em Authorize e cole:

Bearer {seu_token}

🛒 Endpoints Principais
Criar venda
POST /api/Sales
{
  "number": "S-0003",
  "date": "2025-09-05T12:00:00Z",
  "customerId": "CUST-123",
  "customer": "Cliente Demo",
  "branchId": "BR-001",
  "branch": "Loja Centro",
  "items": [
    { "sku": "P001", "name": "Caneta Azul", "quantity": 5,  "unitPrice": 2.50 },
    { "sku": "P002", "name": "Caderno",     "quantity": 12, "unitPrice": 10.00 }
  ]
}


Resposta (201 Created):

{
  "id": "uuid",
  "number": "S-0003",
  "date": "2025-09-05T12:00:00Z",
  "customer": "Cliente Demo",
  "branch": "Loja Centro",
  "status": "Active",
  "total": 107.25,
  "items": [
    {
      "id": "uuid",
      "sku": "P001",
      "name": "Caneta Azul",
      "quantity": 5,
      "unitPrice": 2.5,
      "discount": 0.25,
      "total": 11.25
    },
    {
      "id": "uuid",
      "sku": "P002",
      "name": "Caderno",
      "quantity": 12,
      "unitPrice": 10,
      "discount": 2,
      "total": 96
    }
  ]
}

Cancelar venda
POST /api/Sales/{id}/cancel


Retorna a venda com status = "Cancelled".

Cancelar item
POST /api/Sales/{saleId}/items/{itemId}/cancel


Retorna a venda com o item cancelado (isCancelled = true) e total recalculado.

📊 Regras de Negócio

1–3 itens → 0% desconto

4–9 itens → 10% desconto

10–20 itens → 20% desconto

>20 itens → proibido (400 Bad Request)

✅ Testes

Rodar testes:

dotnet test -c Release


⚠️ Se rodar fora de Development, definir variáveis de ambiente:

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:Jwt__Key = "test-secret-key"


Cobertura de testes:

dotnet test --collect:"XPlat Code Coverage"


Abrir relatório em coverage-report/index.html.

🛠️ Stack

.NET 8, C#

EF Core + PostgreSQL

CQRS + MediatR + AutoMapper

Serilog (logs)

Swagger/OpenAPI

xUnit (unit, integration, functional tests)