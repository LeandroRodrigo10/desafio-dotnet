# Ambev Developer Evaluation — API

## Como rodar
```bash
dotnet restore
dotnet build -c Release
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

Swagger: abra https://localhost:7181/swagger
 (ou a porta mostrada no console).

Autenticação (JWT)
Criar usuário (POST /api/Users)
{
  "username":"Admin",
  "email":"admin@example.com",
  "phone":"55999999999",
  "role":"Admin",
  "status":"Active",
  "password":"Pass@123"
}

Login (POST /api/Auth)
{
  "email":"admin@example.com",
  "password":"Pass@123"
}


No Swagger, clique em Authorize e cole: Bearer {seu_token}.

Domínio de Vendas

Regras por quantidade (por item):

1–3 itens: 0%

4–9 itens: 10%

10–20 itens: 20%

Validações:

Máximo 20 unidades por item → retorna 400 BadRequest.

totalAmount = soma dos itens não cancelados.

Cancelamento preserva histórico.

Exemplo — Criar venda (POST /api/Sales)
{
  "number": "S-0002",
  "date": "2025-09-04T19:45:00Z",
  "customer": "ACME Ltda",
  "branch": "Loja Centro",
  "items": [
    { "sku":"P001","name":"Caneta","quantity":5,"unitPrice":2.50,"discount":0 },
    { "sku":"P002","name":"Caderno","quantity":12,"unitPrice":10.00,"discount":0 }
  ]
}


Resposta esperada (exemplo):

discount por item ≈ 0.25 (10%) e 2.00 (20%)

totalAmount ≈ 107.25

Cancelar venda

POST /api/Sales/{id}/cancel → status = "Cancelled".

Users (CRUD)

POST /api/Users — criar

GET /api/Users/{id} — obter

PUT /api/Users/{id} — atualizar

{
  "id": "GUID_DO_USUARIO",
  "username": "Nome Atualizado",
  "phone": "61999998888",
  "role": "Admin",
  "status": "Active"
}


DELETE /api/Users/{id} — excluir

Testes e Cobertura
```bash
dotnet test Ambev.DeveloperEvaluation.sln -c Release --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```


Abra coverage-report/index.html.

Health Check

GET /health — verificação simples.

Observações Técnicas

Arquitetura: DDD + CQRS + MediatR + AutoMapper + EF Core

Swagger com JWT Bearer habilitado

Middleware mapeia erros de domínio (ex.: quantidade > 20) para 400 BadRequest

Eventos de domínio (diferencial): logs em criação/alteração/cancelamento

Como contribuir / avaliar

Rodar testes (todos verdes).

Validar login + endpoints protegidos.

Criar venda, verificar descontos e total.

Cancelar venda e tentar editar (deve bloquear).
