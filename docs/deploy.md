# Deploy

Este documento descreve como colocar a BarberBoss API no ar e como o pipeline de
Continuous Deploy está montado.

---

## Configuração que nunca vai para o repositório

Três valores são obrigatórios em produção e nenhum deles fica versionado:

| Variável de ambiente | Para que serve |
| --- | --- |
| `ConnectionStrings__Connection` | Conexão com o MySQL de produção |
| `Settings__Jwt__SigningKey` | Chave HMAC que assina os tokens (mínimo 32 caracteres) |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

O duplo underscore é a convenção do .NET para seções aninhadas: `Settings__Jwt__SigningKey`
equivale a `Settings:Jwt:SigningKey` no `appsettings.json`.

Gerar uma chave:

```bash
openssl rand -base64 48
```

Trocar a chave invalida todos os tokens já emitidos — os usuários precisam logar de novo.

---

## Opção 1 — Azure App Service (deploy por zip)

É o caminho que o workflow `ci-cd.yml` usa.

**1. Criar os recursos**

```bash
az group create --name barberboss-rg --location brazilsouth

az appservice plan create \
  --name barberboss-plan \
  --resource-group barberboss-rg \
  --sku B1 --is-linux

az webapp create \
  --name barberboss-api \
  --resource-group barberboss-rg \
  --plan barberboss-plan \
  --runtime "DOTNETCORE:8.0"

az mysql flexible-server create \
  --name barberboss-mysql \
  --resource-group barberboss-rg \
  --location brazilsouth \
  --admin-user barberboss \
  --admin-password "<senha-forte>" \
  --tier Burstable --sku-name Standard_B1ms \
  --database-name barberboss
```

**2. Configurar as App Settings**

```bash
az webapp config appsettings set \
  --name barberboss-api --resource-group barberboss-rg \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__Connection="Server=barberboss-mysql.mysql.database.azure.com;Port=3306;Database=barberboss;Uid=barberboss;Pwd=<senha-forte>;SslMode=Required;" \
    Settings__Jwt__SigningKey="<saída do openssl rand>"
```

**3. Apontar o health check**

```bash
az webapp config set \
  --name barberboss-api --resource-group barberboss-rg \
  --health-check-path "/health/ready"
```

O App Service passa a tirar instâncias não saudáveis da rotação sozinho.

**4. Conectar o pipeline**

Baixe o perfil de publicação e salve como segredo do repositório:

```bash
az webapp deployment list-publishing-profiles \
  --name barberboss-api --resource-group barberboss-rg --xml
```

Em **Settings > Secrets and variables > Actions**, crie `AZURE_WEBAPP_PUBLISH_PROFILE`
com esse XML. A partir daí, todo push na `main` que passar nos testes vai para produção.

---

## Opção 2 — Imagem Docker

O workflow `docker-image.yml` publica a imagem no GitHub Container Registry. Para rodar
em qualquer host com Docker:

```bash
docker compose -f docker-compose.prod.yml up -d --build
```

Ou apontando o App Service para a imagem já publicada:

```bash
az webapp config container set \
  --name barberboss-api --resource-group barberboss-rg \
  --container-image-name ghcr.io/<owner>/<repo>:latest
```

---

## Como o pipeline está organizado

```
build  →  test  →  publish  →  deploy  →  smoke test
```

* **build** — restore e compilação em Release. Falha aqui derruba tudo.
* **test** — unidade e integração no mesmo passo. Os testes de integração usam o provider
  InMemory do EF Core, então o runner não precisa de um MySQL.
* **publish** — gera o artefato e o guarda no GitHub, com retenção própria.
* **deploy** — só roda em push na `main`; pull request para no `test`. Usa o
  *environment* `production`, onde dá para exigir aprovação manual.
* **smoke test** — consulta `/health/ready` por até 2,5 minutos. Como as migrations são
  aplicadas na inicialização, esse endpoint só devolve 200 quando o banco está migrado
  e acessível.

O `docker-image.yml` roda a suíte dentro do estágio `test` do próprio Dockerfile antes de
publicar a imagem, então a imagem que chega ao registry já passou pelos testes.

---

## Migrations

A aplicação chama `MigrateDatabase()` na inicialização, com retry de 10 tentativas a cada
5 segundos — o container da API costuma subir antes de o MySQL aceitar conexões.

Isso resolve o caso simples, mas tem um custo: com mais de uma instância, todas tentam
migrar ao mesmo tempo. Se o App Service for escalar horizontalmente, mova as migrations
para um passo próprio do pipeline:

```bash
dotnet ef migrations bundle \
  --project src/Backend/BarberBoss.Infrastructure \
  --startup-project src/Backend/BarberBoss.Api \
  --output efbundle

./efbundle --connection "$CONNECTION_STRING"
```

---

## Health checks

| Rota | O que verifica | Uso |
| --- | --- | --- |
| `/health` | Só se o processo responde | `HEALTHCHECK` do Docker, liveness |
| `/health/ready` | Também a conexão com o MySQL | App Service, load balancer, smoke test |

A separação importa: se o banco cair, `/health/ready` fica *Unhealthy* e o balanceador para
de mandar tráfego, mas `/health` continua respondendo e o orquestrador não fica reiniciando
um container que está funcionando.

---

## Notas de produção

* O Swagger vem **desligado** em produção (`Settings:Swagger:Enabled: false`). Para ligar
  temporariamente, basta uma App Setting `Settings__Swagger__Enabled=true`.
* O container roda com o usuário `app`, sem privilégios de root.
* O TLS termina no App Service; dentro do container só há HTTP na 8080, e por isso o
  `UseHttpsRedirection` fica desativado quando `DOTNET_RUNNING_IN_CONTAINER=true`.
* No `docker-compose.prod.yml` o MySQL não publica porta no host: só a rede interna
  do compose alcança o banco.
