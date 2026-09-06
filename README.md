# BarberBoss API — Parte II

Continuação da [Parte I](../barberboss-pt1). A API de faturamento agora tem **gestão de
usuários e autenticação**: cadastro, perfil, edição, exclusão, senha criptografada com
**BCrypt** e login que devolve um **token JWT**.

Todas as rotas de faturamento passaram a exigir autenticação e cada usuário enxerga
**apenas o próprio faturamento**.

Inclui **testes de unidade** das regras de negócio e **testes de integração** que sobem a API
inteira em memória.

---

## O que mudou em relação à Parte I

| Área | Mudança |
| --- | --- |
| Entidades | Nova entidade `User`; `Billing` ganhou `UserId` com FK e cascade |
| Segurança | `PasswordEncripter` (BCrypt, work factor 12) e `JwtTokenGenerator` |
| Autorização | `[Authorize]` em faturamentos e relatórios; só o dono (ou admin) edita/exclui usuário |
| Repositórios | Consultas de faturamento escopadas por `userId` |
| Swagger | Botão **Authorize** com esquema Bearer |
| Testes | Novo projeto `WebApi.Test` com `WebApplicationFactory` + EF InMemory |
| Erros | Novos `401 Unauthorized` e `403 Forbidden` |

---

## Como rodar

```bash
docker compose up --build
```

O `.env` já vem no projeto, com uma `JWT_SIGNING_KEY` gerada. Se quiser trocá-la
(`openssl rand -base64 48`), lembre que isso invalida os tokens já emitidos. O compose
falha de propósito se a variável estiver vazia.

Swagger em <http://localhost:8080/swagger>.

Ao subir, a API imprime no log a URL do host:

```
info: BarberBoss[0] BarberBoss API pronta em http://localhost:8080
info: BarberBoss[0] Swagger em http://localhost:8080/swagger
```

O Kestrel também loga o endereço de *bind* (`http://0.0.0.0:8080`): é o endereço interno do
container, onde ele escuta todas as interfaces. Quem você abre no navegador é `localhost` na
porta de `API_PORT` — trocando `API_PORT` no `.env`, a linha do log acompanha.


Para rodar só o banco no Docker:

```bash
docker compose up -d mysql
dotnet run --project src/Backend/BarberBoss.Api
```

Testes (unidade + integração):

```bash
dotnet test
```

---

## Fluxo de autenticação

1. `POST /api/users` cria a conta e já devolve um token.
2. `POST /api/login` troca e-mail + senha por um token.
3. As demais rotas exigem `Authorization: Bearer <token>`.

```bash
# 1. cadastro
curl -X POST http://localhost:8080/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Rafael Souza","email":"rafael@barberboss.com","password":"barberboss123"}'

# 2. login
TOKEN=$(curl -s -X POST http://localhost:8080/api/login \
  -H "Content-Type: application/json" \
  -d '{"email":"rafael@barberboss.com","password":"barberboss123"}' | jq -r .token)

# 3. rota protegida
curl http://localhost:8080/api/billings -H "Authorization: Bearer $TOKEN"
```

No Swagger, clique em **Authorize** e cole apenas o token (sem o prefixo `Bearer`).

---

## Endpoints de usuário

| Método | Rota | Auth | Descrição |
| --- | --- | --- | --- |
| POST | `/api/login` | — | Autentica e devolve o token |
| POST | `/api/users` | — | Cria um usuário (`201`) |
| GET | `/api/users` | ✔ | Perfil do usuário autenticado |
| GET | `/api/users/{id}` | ✔ | Dados de um usuário (próprio ou admin) |
| PUT | `/api/users` | ✔ | Atualiza o próprio perfil (`204`) |
| PUT | `/api/users/{id}` | ✔ | Atualiza um usuário (próprio ou admin) |
| PUT | `/api/users/password` | ✔ | Troca a senha do usuário autenticado |
| DELETE | `/api/users/{id}` | ✔ | Exclui um usuário (próprio ou admin) |

Os endpoints de faturamento e relatórios continuam iguais aos da Parte I, mas agora todos
exigem token.

---

## Regras de segurança

* Senha nunca é armazenada nem devolvida em texto puro: só o hash BCrypt fica no banco.
* Cada usuário tem um salt próprio, então senhas iguais geram hashes diferentes.
* E-mail é único: índice único no banco e checagem no caso de uso, devolvendo `409`.
* E-mail inexistente e senha errada devolvem a **mesma** mensagem, para não revelar quais
  e-mails estão cadastrados.
* Faturamento de outro usuário devolve `404`, não `403`: a API não confirma que o recurso existe.
* O token carrega `sid` (id), nome, e-mail e role, e expira em 120 minutos por padrão.
* `Settings:Jwt:SigningKey` precisa ter no mínimo 32 caracteres — a aplicação recusa subir sem isso.

---

## Testes

```
tests
├── CommonTestUtilities   → builders (Bogus), mocks (Moq), mapper, encripter e token
├── Validators.Tests      → regras de faturamento e de usuário
├── UseCases.Tests        → casos de uso, hash de senha, autorização
└── WebApi.Test           → integração ponta a ponta com WebApplicationFactory
```

Os testes de integração trocam o MySQL pelo provider **InMemory** do EF Core, mantendo
o restante do pipeline (filtros, autenticação, casos de uso) idêntico ao de produção.
Eles cobrem: cadastro, login com senha correta e incorreta, acesso a rota protegida sem
token, e isolamento entre usuários.

---

## Próxima parte

**Parte III** — Dockerfile de produção, `appsettings.Production.json`, health check e
pipeline Build → Test → Deploy com Continuous Deploy.
