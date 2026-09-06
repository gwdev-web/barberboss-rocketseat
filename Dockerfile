# =============================================================================
# BarberBoss API — imagem de produção
#
# Build em múltiplos estágios: o SDK só existe no estágio de compilação, então a
# imagem final não carrega compilador, código-fonte nem pacotes NuGet.
# =============================================================================

# ---------- restore ----------
# Copiar só os .csproj antes do resto do código faz o Docker reaproveitar a camada
# de restore enquanto as dependências não mudarem.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /source

COPY BarberBoss.sln ./
COPY src/Backend/BarberBoss.Api/BarberBoss.Api.csproj                     src/Backend/BarberBoss.Api/
COPY src/Backend/BarberBoss.Application/BarberBoss.Application.csproj     src/Backend/BarberBoss.Application/
COPY src/Backend/BarberBoss.Communication/BarberBoss.Communication.csproj src/Backend/BarberBoss.Communication/
COPY src/Backend/BarberBoss.Domain/BarberBoss.Domain.csproj               src/Backend/BarberBoss.Domain/
COPY src/Backend/BarberBoss.Exception/BarberBoss.Exception.csproj         src/Backend/BarberBoss.Exception/
COPY src/Backend/BarberBoss.Infrastructure/BarberBoss.Infrastructure.csproj src/Backend/BarberBoss.Infrastructure/
COPY tests/CommonTestUtilities/CommonTestUtilities.csproj                 tests/CommonTestUtilities/
COPY tests/UseCases.Tests/UseCases.Tests.csproj                           tests/UseCases.Tests/
COPY tests/Validators.Tests/Validators.Tests.csproj                       tests/Validators.Tests/
COPY tests/WebApi.Test/WebApi.Test.csproj                                 tests/WebApi.Test/

RUN dotnet restore BarberBoss.sln

# ---------- build ----------
FROM restore AS build
COPY . .
RUN dotnet build BarberBoss.sln -c Release --no-restore

# ---------- test ----------
# Estágio opcional: `docker build --target test .` roda a suíte dentro do container.
# O pipeline usa este alvo antes de publicar a imagem.
FROM build AS test
RUN dotnet test BarberBoss.sln -c Release --no-build --verbosity normal

# ---------- publish ----------
FROM build AS publish
RUN dotnet publish src/Backend/BarberBoss.Api/BarberBoss.Api.csproj \
    -c Release -o /app/publish --no-build /p:UseAppHost=false

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# libfontconfig1 + libfreetype6: o QuestPDF (SkiaSharp) precisa deles para gerar o PDF.
# curl: usado pelo HEALTHCHECK abaixo.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 libfreetype6 curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

# A imagem oficial do .NET 8 já traz o usuário sem privilégios `app` (UID 1654).
# Rodar como root dentro do container é desnecessário aqui.
RUN chown -R app:app /app
USER app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://0.0.0.0:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_gcServer=1

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BarberBoss.Api.dll"]
