markdown# Order Management API

API backend para sistema de gestão de pedidos de e-commerce, desenvolvida com foco em **qualidade arquitetural**, padrões de design e boas práticas.

## Stack Tecnológico

- **.NET 9** com C#
- **Clean Architecture** (Domain, Application, Infrastructure, API)
- **CQRS + MediatR** (Commands e Queries separados)
- **Entity Framework Core 9** com SQLite
- **JWT Bearer Authentication**
- **FluentValidation** com Pipeline Behavior
- **xUnit** para testes unitários
- **WebApplicationFactory** para testes de integração
- **Serilog** para logging estruturado
- **OpenTelemetry** para observabilidade/tracing
- **SonarQube** para análise de código
- **Docker** para containerização

## Arquitetura

### Por que Controllers em vez de Minimal APIs?

Controllers foram escolhidos por:

- Melhor escalabilidade em APIs grandes
- Suporte nativo a atributos ([Authorize], [HttpGet], etc)
- Organização clara de lógica relacionada
- Debugging e manutenção mais simples
- Compatibilidade com middleware padrão do ASP.NET

### Camadas

Domain/

├─ Entities/ (Order, OrderItem - lógica de negócio)

├─ Enums/ (OrderStatus)

└─ (sem dependências externas)
Application/

├─ Commands/ (CreateOrderCommand, CancelOrderCommand)

├─ Queries/ (GetOrdersQuery, GetOrderByIdQuery)

├─ Handlers/ (implementam lógica CQRS)

├─ Validators/ (FluentValidation)

├─ Behaviors/ (ValidationBehavior, LoggingBehavior)

├─ DTOs/ (Transfer Objects)

└─ Repositories/ (interfaces apenas)
Infrastructure/

├─ Data/ (OrderDbContext, migrations)

├─ Repositories/ (implementações)

└─ Authentication/ (JwtTokenService, AuthService)
API/

├─ Controllers/ (AuthController, OrdersController)

├─ Program.cs (DI, middleware configuration)

└─ appsettings.json

### Decisões Arquiteturais

**CQRS**: Commands mutam estado, Queries retornam dados → separação de responsabilidades

**MediatR Behaviors**: ValidationBehavior e LoggingBehavior executam **antes** de qualquer handler → cross-cutting concerns centralizados

**Entity Validation no Domain**: Rules como "um pedido precisa ter ≥1 item" ficam na entidade `Order`, não na camada de aplicação

**TotalAmount no Domain**: Calculado como propriedade read-only `Sum(items)` → sempre consistente

## Como Rodar Localmente

### Pré-requisitos

- .NET 9 SDK
- Git

### Instalação

```bash
git clone https://github.com/seu-usuario/pedidos-ecommerce.git
cd pedidos-ecommerce

# Restaurar pacotes
dotnet restore

# Criar migrations e banco de dados (automático ao rodar)
dotnet build
```

### Executar a API

```bash
cd OrderManagementAPI.API
dotnet run
```

A API estará disponível em `http://localhost:5276`

### Swagger UI

Abra no navegador:
http://localhost:5276/swagger

## Endpoints

### Autenticação

POST /auth/login

Body: { "email": "dev@martech.com", "password": "Senha@123" }

Response: { "token": "eyJhbGc..." }

**Usuário fixo**: `dev@martech.com` / `Senha@123`

### Pedidos (requer JWT)

POST /api/orders

Body:

{

"customerId": "550e8400-e29b-41d4-a716-446655440000",

"items": [

{

"productName": "Notebook",

"quantity": 1,

"unitPrice": 4500.00

}

]

}

Response: 201 Created

GET /api/orders?page=1&pageSize=10

Response: 200 OK (PaginatedResultDto)

GET /api/orders/{id}

Response: 200 OK (OrderResponseDto)

PATCH /api/orders/{id}/cancel

Response: 200 OK (OrderResponseDto com status "Cancelled")

## Regras de Negócio

- ✅ Pedido deve ter ≥1 item
- ✅ Quantity e UnitPrice > 0
- ✅ Apenas status "Pending" pode ser cancelado
- ✅ TotalAmount calculado no Domain

## Testes

### Testes Unitários (Handlers)

```bash
dotnet test OrderManagementAPI.Tests
```

Cobre:

- CreateOrderCommandHandler
- CancelOrderCommandHandler
- GetOrderByIdQueryHandler
- GetOrdersQueryHandler

### Testes de Integração (WebApplicationFactory)

Mesmo comando acima — testes de integração usam `WebApplicationFactory<Program>` para testar a API completa com banco de dados real.

Testes:

- POST /api/orders com dados válidos → 201
- GET /api/orders → retorna lista paginada
- GET /api/orders/{id} → retorna pedido
- PATCH /api/orders/{id}/cancel → cancela pedido
- POST /api/orders sem authorization → 401

**Total: 13 testes passando ✅**

## Logging e Observabilidade

### Serilog

Todos os requests são logados com:

- Nome do comando/query
- Dados da requisição
- Tempo de execução
- ID de rastreamento

Exemplo:
[21:36:35 INF] Starting request CreateOrderCommand {...} (b18c3fb1-249d...)

[21:36:35 INF] Completed request CreateOrderCommand in 242ms

### OpenTelemetry

Traces HTTP automaticamente coletados:

- TraceId, SpanId
- Duração total
- Status HTTP
- Headers

Exportado para console em desenvolvimento.

## Docker

### Rodar com Docker Compose

```bash
docker-compose up --build
```

Inicia:

- **API** em `http://localhost:5000`
- **SonarQube** em `http://localhost:9000`

### Dockerfile

Multi-stage build (SDK → runtime):

- Stage 1: Build e publish
- Stage 2: Runtime ASP.NET Core apenas

Reduz tamanho da imagem em ~70%.

## SonarQube - Análise de Código

### Rodar com Docker Compose

```bash
docker-compose up -d
```

Espera ~30 segundos para SonarQube iniciar, depois acessa:
http://localhost:9000

Usuário: admin

Senha: admin

### Análise de Código

#### 1. Instalar dotnet-sonarscanner

```bash
dotnet tool install --global dotnet-sonarscanner
```

#### 2. Gerar token no SonarQube

1. Acessa `http://localhost:9000`
2. Clica em avatar → "My Account" → "Security"
3. Gera novo token: `sqa_...`

#### 3. Rodar análise

```bash
cd C:\repos\pedidos-ecommerce

dotnet sonarscanner begin \
  /k:OrderManagementAPI \
  /d:sonar.host.url=http://localhost:9000 \
  /d:sonar.login=SEU_TOKEN_AQUI

dotnet build

dotnet sonarscanner end \
  /d:sonar.login=SEU_TOKEN_AQUI
```

#### 4. Visualizar resultados

Volta em `http://localhost:9000` → projeto **OrderManagementAPI**

Mostra:

- ✅ Code smell
- ✅ Vulnerabilidades
- ✅ Coverage (cobertura de testes)
- ✅ Duplicação de código

### Métricas Esperadas

Com a estrutura atual:

- **Lines of Code**: ~1500
- **Test Coverage**: ~70% (13 testes)
- **Code Smells**: 0-5 (warnings, nullable)
- **Bugs**: 0
- **Vulnerabilities**: 0

## Validação

**FluentValidation** com MediatR Pipeline Behavior:

1. Validator define rules (`CreateOrderCommandValidator`)
2. `ValidationBehavior<TRequest, TResponse>` executa antes do handler
3. Erros lançam `FluentValidation.ValidationException`
4. Controller captura e retorna 400 Bad Request

Exemplo:
POST /api/orders

Body: { "items": [] }

Response: 400 Bad Request

Message: "Order must have at least one item"

## Estrutura de Pastas

C:\repos\pedidos-ecommerce

├── OrderManagementAPI.sln

├── OrderManagementAPI.Domain/

├── OrderManagementAPI.Application/

├── OrderManagementAPI.Infrastructure/

├── OrderManagementAPI.API/

├── OrderManagementAPI.Tests/

├── Dockerfile

├── docker-compose.yml

├── README.md

└── orders.db (gerado automaticamente)

## Performance

- **Paginação** obrigatória em GET /api/orders (max 100 items)
- **Eager loading** com `.Include()` para evitar N+1
- **Índices** no banco (FK OrderId em OrderItems)
- **Logging assíncrono** com Serilog

## Segurança

- ✅ JWT Bearer authentication
- ✅ [Authorize] em endpoints sensíveis
- ✅ Validação de input (FluentValidation)
- ✅ Sem SQL injection (EF Core parametrizado)

## Melhorias Futuras

- [ ] Teste de carga (k6)
- [ ] Cache distribuído (Redis)
- [ ] Event sourcing para auditoria
- [ ] API versioning
- [ ] Rate limiting
- [ ] Swagger auth com schema

## Contato & Suporte

Para dúvidas técnicas, abra uma issue no repositório.

---

**Desenvolvido com foco em Clean Code e arquitetura escalável.**
