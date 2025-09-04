# Ambev Developer Evaluation — API

## Como rodar
```bash
dotnet restore
dotnet build -c Release
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```
**Swagger:** abra `https://localhost:7181/swagger` (ou a porta exibida no console).

---

## Autenticação (JWT)

1) **Criar usuário** (`POST /api/Users`)
```json
{
  "username": "Admin",
  "email": "admin@example.com",
  "phone": "55999999999",
  "role": "Admin",
  "status": "Active",
  "password": "Pass@123"
}
```

2) **Login** (`POST /api/Auth`)
```json
{
  "email": "admin@example.com",
  "password": "Pass@123"
}
```

3) **Autorizar no Swagger**  
Clique em **Authorize** (cadeado) e cole: `Bearer {seu_token}`.

---

## Domínio de Vendas

**Regras por quantidade (por item):**
- 1–3: **0%**
- 4–9: **10%**
- 10–20: **20%**
- **Máximo 20** unidades por item → **400 BadRequest**

**Cálculo:**
- `discount` é aplicado **por unidade** (0/10/20% de `unitPrice`).
- `totalAmount` = soma dos itens **não cancelados**.

**Exemplo — Criar venda** (`POST /api/Sales`)
```json
{
  "number": "S-0002",
  "date": "2025-09-04T19:45:00Z",
  "customer": "ACME Ltda",
  "branch": "Loja Centro",
  "items": [
    { "sku": "P001", "name": "Caneta",  "quantity": 5,  "unitPrice": 2.50, "discount": 0 },
    { "sku": "P002", "name": "Caderno", "quantity": 12, "unitPrice": 10.00, "discount": 0 }
  ]
}
```

**Resposta (esperado):**
- `discount` ≈ **0.25** (10%) e **2.00** (20%)
- `totalAmount` ≈ **107.25**

**Cancelar venda** (`POST /api/Sales/{id}/cancel`) → `status: "Cancelled"`.

---

## Users (CRUD)

- `POST /api/Users` — criar  
- `GET /api/Users/{id}` — obter  
- `PUT /api/Users/{id}` — atualizar
```json
{
  "id": "GUID_DO_USUARIO",
  "username": "Nome Atualizado",
  "phone": "61999998888",
  "role": "Admin",
  "status": "Active"
}
```
- `DELETE /api/Users/{id}` — excluir

---

## Testes e Cobertura
```bash
dotnet test Ambev.DeveloperEvaluation.sln -c Release --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/TestResults/*/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```
Abra `coverage-report/index.html`.

---

## Health Check
`GET /health` — verificação simples da aplicação.

---

## Observações Técnicas
- Arquitetura: DDD + CQRS + MediatR + AutoMapper + EF Core
- Swagger com **JWT Bearer** habilitado
- Middleware traduz exceções de domínio (ex.: quantidade > 20) para **400 BadRequest**
- Diferencial: eventos podem ser logados em criação/alteração/cancelamento
