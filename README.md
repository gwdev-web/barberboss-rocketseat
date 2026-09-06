# BarberBoss API — Parte I

API REST em **.NET 8** para gerenciar o faturamento de uma barbearia: CRUD de faturamentos,
total do período, relatórios semanais em **PDF** e **Excel**, tratamento global de erros,
testes de unidade e documentação no **Swagger**.

Persistência em **MySQL**, que sobe junto com a API via **Docker Compose**.

A arquitetura segue a linha do projeto [CashFlow](https://github.com/welissonArley/CashFlow):
camadas separadas por responsabilidade e um caso de uso por operação.

---

## Stack

| Camada | Tecnologia |
| --- | --- |
| Runtime | .NET 8 |
| Web | ASP.NET Core + Swashbuckle (Swagger) |
| ORM | Entity Framework Core 8 + Pomelo MySQL |
| Banco | MySQL 8 (container) |
| Mapeamento | AutoMapper |
| Validação | FluentValidation |
| PDF | QuestPDF |
| Excel | ClosedXML |
| Testes | xUnit + FluentAssertions + Moq + Bogus |

---

## Estrutura

```
BarberBoss.sln
├── src/Backend
│   ├── BarberBoss.Api              → Controllers, filtro de exceção, Swagger, Program
│   ├── BarberBoss.Application      → Casos de uso, validators, AutoMapper, relatórios
│   ├── BarberBoss.Communication    → Requests, Responses e enums (contrato público)
│   ├── BarberBoss.Domain           → Entidades, DTOs e interfaces de repositório
│   ├── BarberBoss.Exception        → Exceções de domínio e mensagens
│   └── BarberBoss.Infrastructure   → DbContext, repositórios, migrations, DI
└── tests
    ├── CommonTestUtilities         → Builders (Bogus), mocks (Moq), mapper
    ├── Validators.Tests            → Testes das regras de validação
    └── UseCases.Tests              → Testes dos casos de uso
```

Direção das dependências: `Api → Application → Domain`. A `Infrastructure` implementa as
interfaces do `Domain` e só é conhecida pela `Api` no momento da injeção de dependência.

---

## Como rodar

### Opção 1 — tudo no Docker (recomendado)

```bash
docker compose up --build
```

* API: <http://localhost:8080/swagger>
* MySQL: `localhost:3306`

Ao subir, a API imprime no log a URL do host:

```
info: BarberBoss[0] BarberBoss API pronta em http://localhost:8080
info: BarberBoss[0] Swagger em http://localhost:8080/swagger
```

O Kestrel também loga o endereço de *bind* (`http://0.0.0.0:8080`): é o endereço interno do
container, onde ele escuta todas as interfaces. Quem você abre no navegador é `localhost` na
porta de `API_PORT` — trocando `API_PORT` no `.env`, a linha do log acompanha.

As migrations são aplicadas automaticamente no start da API, com retry enquanto o container
do MySQL termina de subir.

### Opção 2 — só o banco no Docker, API na máquina

```bash
docker compose up -d mysql
dotnet run --project src/Backend/BarberBoss.Api
```

A connection string de desenvolvimento já aponta para `localhost:3306` em
`appsettings.Development.json`.

### Rodando os testes

```bash
dotnet test
```

---

## Endpoints

Base: `/api`

| Método | Rota | Descrição | Sucesso |
| --- | --- | --- | --- |
| POST | `/api/billings` | Cria um faturamento | `201 Created` |
| GET | `/api/billings` | Lista com filtros, paginação e ordenação | `200 OK` |
| GET | `/api/billings/summary` | Total do período (apenas pagos) | `200 OK` |
| GET | `/api/billings/{id}` | Busca por id | `200 OK` |
| PUT | `/api/billings/{id}` | Atualiza | `204 No Content` |
| DELETE | `/api/billings/{id}` | Exclui | `204 No Content` |
| GET | `/api/reports/pdf` | Relatório semanal em PDF | `200 OK` / `204` |
| GET | `/api/reports/excel` | Relatório semanal em Excel | `200 OK` / `204` |

### Filtros de `GET /api/billings`

`startDate`, `endDate`, `barberName`, `clientName`, `serviceName`, `paymentMethod`, `status`,
`pageNumber` (padrão 1), `pageSize` (padrão 20, máx. 100), `sortBy`
(`Date`, `Amount`, `BarberName`, `ClientName`, `CreatedAt`) e `sortDirection` (`Asc`, `Desc`).

### Relatórios

`referenceDate` aceita qualquer data dentro da semana desejada; a API resolve o intervalo de
segunda a domingo. Sem o parâmetro, usa a semana corrente. Semana sem lançamentos devolve
`204 No Content` em vez de um arquivo vazio.

---

## Exemplos

Criar:

```bash
curl -X POST http://localhost:8080/api/billings \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2025-03-04",
    "barberName": "Rafael Souza",
    "clientName": "João Pedro",
    "serviceName": "Corte + Barba",
    "amount": 75.00,
    "paymentMethod": 2,
    "status": 0,
    "notes": "Cliente da fidelidade"
  }'
```

Total da semana:

```bash
curl "http://localhost:8080/api/billings/summary?startDate=2025-03-03&endDate=2025-03-09"
```

Baixar o PDF:

```bash
curl -OJ "http://localhost:8080/api/reports/pdf?referenceDate=2025-03-04"
```

### Enums

| `paymentMethod` | | `status` | |
| --- | --- | --- | --- |
| `0` | Cartão | `0` | Pago |
| `1` | Dinheiro | `1` | Cancelado |
| `2` | Pix | | |
| `3` | Outro | | |

---

## Regras de negócio

* Data, barbeiro, cliente, serviço, valor, forma de pagamento e status são obrigatórios.
* `barberName` 2–80 caracteres; `clientName` e `serviceName` 2–120; `notes` até 500.
* `amount` deve ser maior ou igual a zero.
* Faturamento com status **Cancelado** precisa ter `amount = 0`.
* A data não pode estar no futuro.
* **Cancelados não entram no total do período** nem no ticket médio.

---

## Erros

Toda resposta de erro sai no mesmo formato, montado pelo `ExceptionFilter`:

```json
{ "errors": ["O nome do barbeiro é obrigatório."] }
```

| Status | Situação |
| --- | --- |
| `400` | Validação de campos ou período inválido |
| `404` | Faturamento inexistente |
| `409` | Conflito de dados |
| `500` | Erro inesperado (detalhes só no log) |

---

## Migrations

O projeto já vem com a migration inicial e a aplica sozinho no start. Para criar novas:

```bash
dotnet ef migrations add NomeDaMigration \
  --project src/Backend/BarberBoss.Infrastructure \
  --startup-project src/Backend/BarberBoss.Api \
  --output-dir DataAccess/Migrations
```

---

## Próximas partes

* **Parte II** — usuários, senha criptografada, login com JWT e testes de integração.
* **Parte III** — Dockerfile de produção, health check e pipeline Build → Test → Deploy.
