# ── Build stage ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS build
WORKDIR /src

# Copy solution and restore NuGet packages
COPY RetailFlow.sln .
COPY packages.config .
COPY src/ ./src/
COPY tests/ ./tests/

# Restore NuGet packages
RUN nuget restore RetailFlow.sln

# Build in Release mode
RUN msbuild RetailFlow.sln /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=FileSystem /p:PublishUrl=C:\publish

# ── Runtime stage ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022 AS runtime
WORKDIR /inetpub/wwwroot

# Copy published output
COPY --from=build C:\publish .

# IIS is pre-configured in the base image
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD powershell -command "try { $r = Invoke-WebRequest -Uri http://localhost/api/health -UseBasicParsing; if ($r.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }"
