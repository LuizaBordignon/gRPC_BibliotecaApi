# BibliotecaApi

Sistema de biblioteca com API expondo os mesmos casos de uso via **REST** e **gRPC**, ambos chamando o mesmo Domínio, em arquitetura de três camadas (Apresentação → Domínio → Repositório).

## Sobre o projeto

Trabalho em grupo — API que expõe os mesmos casos de uso por REST e por gRPC, sem duplicar regra de negócio entre os dois protocolos.

- **Domínio escolhido:** Biblioteca
- **Entidades relacionadas:** `Livro` e `Emprestimo`
- **Regra de negócio:** um livro só pode ser emprestado se tiver exemplar disponível **e** o usuário não tiver 3 empréstimos ativos ao mesmo tempo. A regra consulta os dois agregados (`Livro` e `Emprestimo`), não é validação simples de campo.

## Stack

- .NET 10 — ASP.NET Core Web API + gRPC
- Entity Framework Core + Npgsql
- PostgreSQL 16
- Docker / Docker Compose
- Swagger (REST) + gRPC Server Reflection

## Arquitetura

```
Controllers/   (REST)         Grpc/   (gRPC)
       └────────────┬────────────┘
                     ▼
              Services/  (Domínio — regra de negócio)
                     ▼
            Repositories/  (acesso a dados)
                     ▼
        Domain/Entities  +  Domain/Exceptions
```

Controllers REST e serviços gRPC injetam as **mesmas** interfaces de Service (`ILivroService`, `IEmprestimoService`). A regra de negócio existe em um único lugar — prova de baixo acoplamento entre Apresentação e Domínio.

## Como rodar

Pré-requisito: Docker Desktop instalado e em execução.

```bash
docker-compose up --build
```

Sobe dois containers (`api` e `db`), que expõem três portas:

| Container | Função | Porta | Protocolo |
|---|---|---|---|
| `api` | REST + Swagger | 8080 | HTTP/1.1 |
| `api` | gRPC | 8081 | HTTP/2 (sem TLS) |
| `db` | PostgreSQL 16 | 5432 | TCP |

> O compose define `ASPNETCORE_ENVIRONMENT=Development`, o que habilita o Swagger e o gRPC Server Reflection.

Migrations do Entity Framework são aplicadas automaticamente na inicialização — não precisa rodar comando manual.

## Testando REST

Abrir no navegador:

```
http://localhost:8080/swagger
```

Endpoints principais:

- `GET /api/Livro` — lista livros
- `POST /api/Livro` — cria livro
- `GET /api/Livro/{id}`
- `PUT /api/Livro/{id}`
- `DELETE /api/Livro/{id}`
- `GET /api/Emprestimo` — lista empréstimos
- `POST /api/Emprestimo` — realiza empréstimo (corpo: `{ "livroId": 1, "usuarioNome": "..." }`)
- `POST /api/Emprestimo/{id}/devolver` — devolve empréstimo

### Exemplos com curl

```bash
# Criar um livro com 1 exemplar
curl -X POST http://localhost:8080/api/Livro \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Dom Casmurro","autor":"Machado de Assis","exemplaresTotais":1}'

# Listar livros
curl http://localhost:8080/api/Livro

# Buscar um livro por ID
curl http://localhost:8080/api/Livro/1

# Atualizar um livro
curl -X PUT http://localhost:8080/api/Livro/1 \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Dom Casmurro","autor":"Machado de Assis","exemplaresTotais":3}'

# Emprestar o livro 1 para um usuário
curl -X POST http://localhost:8080/api/Emprestimo \
  -H "Content-Type: application/json" \
  -d '{"livroId":1,"usuarioNome":"Maria"}'

# Devolver o empréstimo 1
curl -X POST http://localhost:8080/api/Emprestimo/1/devolver

# Provocar erro de regra de negócio (livro sem exemplar) -> 400
curl -i -X POST http://localhost:8080/api/Emprestimo \
  -H "Content-Type: application/json" \
  -d '{"livroId":1,"usuarioNome":"Joao"}'

# Provocar erro de não encontrado -> 404
curl -i http://localhost:8080/api/Livro/999

# Remover um livro
curl -X DELETE http://localhost:8080/api/Livro/1
```

## Testando gRPC

O servidor gRPC roda em `localhost:8081`, em HTTP/2 **sem TLS** (por isso `-plaintext`). O Server Reflection está ativo, então não é preciso informar o arquivo `.proto`.

Serviços disponíveis:

- `LivroGrpc` — GetLivro, ListLivros, CreateLivro, UpdateLivro, DeleteLivro
- `EmprestimoGrpc` — GetEmprestimo, ListEmprestimos, Emprestar, Devolver

### Exemplos com grpcurl

```bash
# Listar serviços e métodos (via reflection)
grpcurl -plaintext localhost:8081 list
grpcurl -plaintext localhost:8081 list livro.LivroGrpc

# Criar um livro com 1 exemplar
grpcurl -plaintext \
  -d '{"titulo":"Dom Casmurro","autor":"Machado de Assis","exemplaresTotais":1}' \
  localhost:8081 livro.LivroGrpc/CreateLivro

# Listar livros
grpcurl -plaintext -d '{}' localhost:8081 livro.LivroGrpc/ListLivros

# Buscar um livro por ID
grpcurl -plaintext -d '{"id":1}' localhost:8081 livro.LivroGrpc/GetLivro

# Emprestar o livro 1 para um usuário
grpcurl -plaintext -d '{"livroId":1,"usuarioNome":"Maria"}' \
  localhost:8081 emprestimo.EmprestimoGrpc/Emprestar

# Devolver o empréstimo 1
grpcurl -plaintext -d '{"id":1}' localhost:8081 emprestimo.EmprestimoGrpc/Devolver

# Provocar erro de regra de negócio (livro sem exemplar) -> FailedPrecondition
grpcurl -plaintext -d '{"livroId":1,"usuarioNome":"Joao"}' \
  localhost:8081 emprestimo.EmprestimoGrpc/Emprestar

# Provocar erro de não encontrado -> NotFound
grpcurl -plaintext -d '{"id":999}' localhost:8081 livro.LivroGrpc/GetLivro
```

### Alternativa: Postman

Nova aba do tipo **gRPC**, servidor `localhost:8081`, e marcar **"Use Server Reflection"** para listar os métodos automaticamente.

## Cenário de teste da regra de negócio

1. Criar um livro com `exemplaresTotais: 1`
2. Emprestar esse livro para um usuário → sucesso (`201` REST / resposta normal gRPC)
3. Tentar emprestar o mesmo livro de novo → erro de exemplar indisponível (`400` REST / `FAILED_PRECONDITION` gRPC)
4. Buscar livro com ID inexistente → erro de não encontrado (`404` REST / `NOT_FOUND` gRPC)

O mesmo teste funciona idêntico nos dois protocolos, porque os dois chamam o mesmo Domínio.

## Tratamento de exceções

Erros de regra de negócio são lançados como exceções de domínio (`Domain/Exceptions/`) e traduzidos para o status correto de cada protocolo — nunca com `if` dentro de Controllers ou serviços gRPC:

| Exceção de domínio | Status HTTP | Status gRPC |
|---|---|---|
| `NotFoundException` | 404 | `NotFound` |
| `BusinessRuleException` | 400 | `FailedPrecondition` |

Tradução feita em `Middleware/ExceptionMiddleware.cs` (REST) e `Grpc/ExceptionInterceptor.cs` (gRPC).

## Estrutura de pastas

```
BibliotecaApi/
├── Controllers/          Controllers REST
├── Grpc/                 Serviços gRPC + interceptor de exceções
├── Services/              Regra de negócio (Domínio)
├── Repositories/          Acesso a dados (Entity Framework)
├── Domain/
│   ├── Entities/          Livro, Emprestimo
│   └── Exceptions/        Exceções de domínio
├── Data/                   AppDbContext
├── Protos/                 Contratos gRPC (livro.proto, emprestimo.proto)
├── Migrations/            Migrations do Entity Framework
├── Middleware/            Tradução de exceção → status HTTP
├── Dockerfile
├── docker-compose.yml
└── appsettings.json
```

## Integrantes do grupo

- Luiza Moro
- Estér Frank
- Rafael Serafim
- Bruno Ghisi
- João Victor Santa Catarina
