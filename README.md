# BarberBoss API — Parte III

Fecha a trilha: pega a API da [Parte II](../barberboss-pt2) e a deixa pronta para produção.
Nenhuma regra de negócio mudou — o que muda é tudo o que cerca a aplicação: imagem Docker
endurecida, configuração por variável de ambiente, health checks e um pipeline
**Build → Test → Deploy** com Continuous Deploy.

---

## O que mudou em relação à Parte II

| Área | Mudança |
| --- | --- |
| Dockerfile | Estágios separados (restore/build/test/publish/runtime), usuário não-root, `HEALTHCHECK`, cache de restore |
| Health checks | `/health` (liveness) e `/health/ready` (checa o MySQL) |
| Configuração | `appsettings.Production.json`; segredos só por variável de ambiente |
| Swagger | Desligado por padrão em produção, com chave para religar |
| Compose | `docker-compose.prod.yml` com senhas obrigatórias e banco sem porta exposta |
| CI/CD | `ci-cd.yml`, `docker-image.yml` e `azure-pipelines.yml` |
| Docs | [`docs/deploy.md`](docs/deploy.md) com o passo a passo do Azure |

---

## Como rodar

Desenvolvimento:

```bash
docker compose up --build
```

A API imprime no log a URL que você abre no navegador:

```
info: BarberBoss[0] BarberBoss API pronta em http://localhost:8080
info: BarberBoss[0] Ambiente: Development
info: BarberBoss[0] Health check em http://localhost:8080/health/ready
info: BarberBoss[0] Swagger em http://localhost:8080/swagger
```

O Kestrel também loga `http://0.0.0.0:8080`: esse é o endereço de *bind*, interno ao
container. Quem vale para você é o `localhost` na porta de `API_PORT`.

Simulando produção na mesma máquina:

```bash
docker compose -f docker-compose.prod.yml up -d --build
curl http://localhost:8080/health/ready
```

### Dev x Prod

Os dois compose usam o mesmo `Dockerfile` e o mesmo estágio `final`. O que muda é a
configuração injetada:

| | `docker-compose.yml` | `docker-compose.prod.yml` |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Development` | `Production` |
| Swagger | ligado | desligado (religável por App Setting) |
| Nível de log | `Information` | `Warning`, exceto a categoria `BarberBoss` |
| Porta do MySQL | publicada no host | só na rede interna do compose |
| Senhas | têm valor padrão | obrigatórias; o compose falha sem elas |
| Validade do token | 120 min | 60 min |
| Healthcheck do serviço | — | `/health/ready` a cada 30s |
| `restart` | `unless-stopped` | `always` |

A imagem já nasce com `ASPNETCORE_ENVIRONMENT=Production` embutida e o compose de
desenvolvimento sobrescreve. Assim, qualquer host que rode a imagem sem configuração
explícita cai em produção, que é o padrão mais seguro dos dois.

Rodando a suíte dentro do container, do mesmo jeito que o pipeline faz:

```bash
docker build --target test .
```

---

## Health checks

| Rota | Verifica | Quem consome |
| --- | --- | --- |
| `GET /health` | Se o processo responde | `HEALTHCHECK` do Docker |
| `GET /health/ready` | Também a conexão com o MySQL | App Service, balanceador, smoke test do deploy |

```json
{
  "status": "Healthy",
  "totalDurationMs": 12.4,
  "checks": [
    { "name": "database", "status": "Healthy", "durationMs": 11.8, "description": null }
  ]
}
```

Ambas são públicas, sem token.

---

## Pipeline

```
push / PR ──> build ──> test ─┬─> (PR para aqui)
                              │
              push na main ───┴─> publish ──> deploy ──> smoke test
```

`.github/workflows/ci-cd.yml`:

* **build** — restore com cache de NuGet e compilação em Release.
* **test** — unidade e integração juntos; resultados e cobertura viram artefato.
  Os testes de integração usam o EF InMemory, então o runner não precisa de MySQL.
* **publish** — `dotnet publish` e upload do artefato.
* **deploy** — só em push na `main`, via `azure/webapps-deploy`, no *environment*
  `production` (onde dá para exigir aprovação manual).
* **smoke test** — consulta `/health/ready` por até 2,5 minutos após o deploy.

`.github/workflows/docker-image.yml` é o caminho alternativo: roda o estágio `test` do
Dockerfile e, passando, publica a imagem no GitHub Container Registry.

`azure-pipelines.yml` reproduz o mesmo fluxo em Azure DevOps.

### Segredos do repositório

| Segredo | Onde é usado |
| --- | --- |
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Job de deploy |
| `CONNECTION_STRING` | App Settings do App Service |
| `JWT_SIGNING_KEY` | App Settings do App Service |

---

## Configuração em produção

Nada sensível fica versionado. O `appsettings.Production.json` guarda só o que é público
(nível de log, issuer, audience, expiração); o resto chega por variável de ambiente:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Connection=Server=...;Database=barberboss;Uid=...;Pwd=...;SslMode=Required;
Settings__Jwt__SigningKey=<openssl rand -base64 48>
```

O duplo underscore mapeia para as seções aninhadas do `appsettings.json`.

O passo a passo completo — criar os recursos no Azure, configurar o health check, ligar o
pipeline e lidar com migrations em múltiplas instâncias — está em
[`docs/deploy.md`](docs/deploy.md).

---

## Endurecimento da imagem

* Multi-stage: o SDK, o código-fonte e os pacotes NuGet ficam nos estágios de build; a
  imagem final tem só o runtime e os binários publicados.
* Roda como `app` (UID 1654), o usuário sem privilégios das imagens oficiais do .NET 8.
* Os `.csproj` são copiados antes do resto do código, então o `dotnet restore` só refaz
  quando alguma dependência muda.
* `HEALTHCHECK` embutido apontando para `/health`.
* `.dockerignore` mais amplo, para o contexto de build não carregar `bin`, `obj`, `.git`,
  `.env` nem os workflows.
