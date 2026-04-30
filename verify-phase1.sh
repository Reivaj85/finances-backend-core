#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT_DIR"

echo "==> Verificando prerequisitos de Fase 1"
command -v dotnet >/dev/null 2>&1 || { echo "dotnet no está instalado."; exit 1; }

if [[ ! -f ".env" ]]; then
  echo "No existe .env en $ROOT_DIR. Crea .env basado en .env.example."
  exit 1
fi

# Configura variables para Testcontainers con Podman si DOCKER_HOST no está definido.
if [[ -z "${DOCKER_HOST:-}" ]] && command -v podman >/dev/null 2>&1; then
  PODMAN_SOCKET="$(podman machine inspect --format '{{.ConnectionInfo.PodmanSocket.Path}}' 2>/dev/null | tr -d '"')"
  if [[ -n "${PODMAN_SOCKET}" ]]; then
    export DOCKER_HOST="unix://${PODMAN_SOCKET}"
    export TESTCONTAINERS_RYUK_DISABLED="${TESTCONTAINERS_RYUK_DISABLED:-true}"
    export TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE="${TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE:-/var/run/docker.sock}"
    echo "DOCKER_HOST configurado para Podman: ${DOCKER_HOST}"
  fi
fi

echo "==> Cargando variables de entorno desde .env"
set -a
source ".env"
set +a

echo "==> dotnet restore"
dotnet restore

echo "==> dotnet build --no-restore"
dotnet build --no-restore

echo "==> Ejecutando DBUp"
dotnet run --project "src/Finances.DatabaseMigrator/Finances.DatabaseMigrator.csproj"

echo "==> Ejecutando tests Fase 1"
dotnet test "tests/Finances.Tests/Finances.Tests.csproj" --no-build

echo ""
echo "✅ Verificación integral de Fase 1 completada en verde."
