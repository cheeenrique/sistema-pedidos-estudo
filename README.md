# Sistema de Pedidos — Backend Profissional (.NET 8)

Backend para gestão de pedidos, clientes, produtos e autenticação, desenvolvido em C# e ASP.NET Core com Clean Architecture e DDD.

## Sobre o projeto

Sistema de pedidos com API REST que oferece:

- **Autenticação**: registro, login, refresh token e revogação de tokens
- **Clientes**: CRUD com paginação e ordenação
- **Produtos**: catálogo com paginação e filtros
- **Pedidos**: criação, listagem, detalhes e cancelamento

O projeto segue padrões de Clean Architecture, DDD tático e CQRS com MediatR, buscando código organizado, testável e manutenível.

## Arquitetura

```
src/
├── Ordering.Domain/      # Regras de negócio puras (entidades, agregados)
├── Ordering.Application/  # Casos de uso, contratos e CQRS (MediatR)
├── Ordering.Infrastructure/  # Persistência (EF Core + PostgreSQL) e integrações
└── Ordering.Api/         # Camada HTTP (controllers, middleware, DI)

tests/
├── Ordering.UnitTests/       # Testes unitários
└── Ordering.IntegrationTests/  # Testes de integração
```

- **Domain**: sem dependências externas; contém entidades e regras de negócio
- **Application**: orquestra casos de uso via CQRS e depende apenas do Domain
- **Infrastructure**: implementa repositórios e acesso a dados
- **Api**: expõe endpoints HTTP e configura o pipeline de requisições

## Tecnologias

- **.NET 8** — ASP.NET Core Web API
- **Entity Framework Core** — ORM com PostgreSQL
- **MediatR** — CQRS
- **ASP.NET Identity** — autenticação e roles
- **JWT** — tokens de acesso e refresh
- **Serilog** — logging estruturado
- **Docker** — ambiente de desenvolvimento e produção

## Pré-requisitos

- **.NET 8 SDK** — [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker** (opcional) — para rodar via Docker Compose
- **PostgreSQL** — para executar localmente sem Docker

### Verificar instalação do .NET

```bash
dotnet --version
```

## Como rodar

### Opção 1: Docker (recomendado)

1. **Configurar variáveis de ambiente**

```powershell
copy .env.example .env
```

2. **Modo desenvolvimento (hot reload)**

```powershell
docker compose up --build db api-dev
```

- API: http://localhost:8080  
- Swagger: http://localhost:8080/swagger  
- Health check: http://localhost:8080/health  

3. **Modo produção**

```powershell
docker compose --profile prod up --build db api
```

- API: http://localhost:8081  

### Opção 2: Local (sem Docker)

1. **Subir PostgreSQL** (ex.: porta 5432, usuário `postgres`, senha `postgres`, banco `ordering_db`)

2. **Configurar connection string** em `src/Ordering.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ordering_db;Username=postgres;Password=postgres"
}
```

3. **Executar**

```powershell
# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Rodar API
dotnet run --project src/Ordering.Api
```

4. API em http://localhost:5000 (ou porta conforme configurado)

## Comandos úteis

### Docker

```powershell
# Build apenas da API
docker compose build api

# Ver logs em tempo real
docker compose logs -f api-dev

# Parar containers
docker compose down

# Parar e remover volumes (reset do banco)
docker compose down -v

# Criar nova migration
docker compose exec api-dev dotnet ef migrations add NomeDaMigration --project src/Ordering.Infrastructure --startup-project src/Ordering.Api
```

### Testes

```powershell
# Testes unitários
dotnet test tests/Ordering.UnitTests

# Testes de integração
dotnet test tests/Ordering.IntegrationTests
```

## Endpoints principais

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/api/auth/register` | POST | Registrar usuário |
| `/api/auth/login` | POST | Login (retorna access + refresh token) |
| `/api/auth/refresh` | POST | Renovar tokens |
| `/api/auth/revoke` | POST | Revogar refresh token |
| `/api/customers` | GET, POST | Listar, criar clientes |
| `/api/customers/{id}` | GET | Detalhes do cliente |
| `/api/products` | GET, POST | Listar, criar produtos |
| `/api/products/{id}` | GET | Detalhes do produto |
| `/api/orders` | GET, POST | Listar, criar pedidos |
| `/api/orders/{id}` | GET | Detalhes do pedido |
| `/api/orders/{id}/cancel` | POST | Cancelar pedido |
| `/health` | GET | Health check |

## Variáveis de ambiente

| Variável | Descrição |
|----------|-----------|
| `CONNECTION_STRING` | Connection string do PostgreSQL |
| `JWT_SIGNING_KEY` | Chave secreta para JWT (mín. 32 caracteres) |
| `JWT_ISSUER` | Emissor do token |
| `JWT_AUDIENCE` | Audiência do token |
| `JWT_ACCESS_TOKEN_MINUTES` | Tempo de vida do access token |
| `JWT_REFRESH_TOKEN_DAYS` | Tempo de vida do refresh token |
| `DATABASE_RUN_MIGRATIONS_ON_STARTUP` | Executar migrations ao iniciar (true/false) |

## Banco de dados

- Com Docker: migrations são aplicadas automaticamente se `DATABASE_RUN_MIGRATIONS_ON_STARTUP=true` no `.env`
- O banco é criado e atualizado automaticamente na inicialização da API

## Licença

Uso pessoal/educacional.
