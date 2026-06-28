# Order Management API

Backend de gestão de pedidos para e-commerce, com foco em **qualidade arquitetural**.

## Stack

- .NET 9 · Clean Architecture · CQRS + MediatR
- Entity Framework Core 9 + SQLite
- JWT Bearer · FluentValidation · xUnit
- Serilog · OpenTelemetry · SonarQube · Docker

## Rodar Localmente

```bash
dotnet restore
cd OrderManagementAPI.API
dotnet run
```

API: `http://localhost:5276` · Swagger: `http://localhost:5276/swagger`

## Rodar com Docker

```bash
docker-compose up --build
```

API: `http://localhost:5000` · SonarQube: `http://localhost:9000`

## Endpoints

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/auth/login` | — | Retorna JWT |
| POST | `/api/orders` | ✅ | Cria pedido |
| GET | `/api/orders?page=1&pageSize=10` | — | Lista paginada |
| GET | `/api/orders/{id}` | ✅ | Busca por ID |
| PATCH | `/api/orders/{id}/cancel` | ✅ | Cancela pedido |

**Login:** `dev@martech.com` / `Senha@123`

## Regras de Negócio

- Pedido deve ter ≥ 1 item
- `Quantity` e `UnitPrice` > 0
- Apenas pedidos com status `Pending` podem ser cancelados
- `TotalAmount` calculado no Domain

## Testes

```bash
dotnet test OrderManagementAPI.Tests
```

13 testes passando — unitários (handlers) + integração (WebApplicationFactory).

## Análise SonarQube

```bash
# Instalar (uma vez)
dotnet tool install --global dotnet-sonarscanner

# Rodar análise
dotnet sonarscanner begin /k:OrderManagementAPI /d:sonar.host.url=http://localhost:9000 /d:sonar.token=SEU_TOKEN
dotnet build
dotnet sonarscanner end /d:sonar.token=SEU_TOKEN
```

Gere o token em `http://localhost:9000` → My Account → Security.

## Arquitetura

**Controllers** em vez de Minimal APIs — escolha justificada pela melhor organização de atributos, middleware e manutenção em APIs maiores.

**CQRS**: Commands mutam estado, Queries retornam dados. MediatR behaviors (`ValidationBehavior`, `LoggingBehavior`) executam como cross-cutting concerns antes de qualquer handler.

**Domain-first**: Regras de negócio (validações, `TotalAmount`) ficam nas entidades — sem lógica em Controllers ou Infrastructure.
