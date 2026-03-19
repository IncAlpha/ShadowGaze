#!/usr/bin/env bash
set -euo pipefail

APP_USER="shadowgaze"
APP_GROUP="shadowgaze"
APP_DIR="/opt/shadowgaze"
SERVICE_NAME="shadowgaze.service"
DOTNET_CMD="dotnet"
TEMPLATE_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/systemd/shadowgaze.service.template"

print_usage() {
  cat <<'EOF'
Usage:
  sudo ./deploy/bootstrap_server.sh [options]

Options:
  --app-user <name>        Linux user for bot process (default: shadowgaze)
  --app-group <name>       Linux group for bot process (default: shadowgaze)
  --app-dir <path>         Base directory for app (default: /opt/shadowgaze)
  --service <name>         Systemd service filename (default: shadowgaze.service)
  --dotnet-cmd <path>      Dotnet command path for service ExecStart (default: dotnet)
  -h, --help               Show help
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --app-user)
      APP_USER="$2"
      shift 2
      ;;
    --app-group)
      APP_GROUP="$2"
      shift 2
      ;;
    --app-dir)
      APP_DIR="$2"
      shift 2
      ;;
    --service)
      SERVICE_NAME="$2"
      shift 2
      ;;
    --dotnet-cmd)
      DOTNET_CMD="$2"
      shift 2
      ;;
    -h|--help)
      print_usage
      exit 0
      ;;
    *)
      echo "Unknown argument: $1" >&2
      print_usage
      exit 1
      ;;
  esac
done

if [[ "$EUID" -ne 0 ]]; then
  echo "Run this script as root (sudo)." >&2
  exit 1
fi

if [[ ! -f "$TEMPLATE_PATH" ]]; then
  echo "Service template not found: $TEMPLATE_PATH" >&2
  exit 1
fi

if ! id "$APP_USER" >/dev/null 2>&1; then
  groupadd --system "$APP_GROUP" || true
  useradd --system --gid "$APP_GROUP" --create-home --home-dir "/home/$APP_USER" "$APP_USER"
fi

mkdir -p "$APP_DIR/releases" "$APP_DIR/shared"
chown -R "$APP_USER:$APP_GROUP" "$APP_DIR"

if [[ ! -f "$APP_DIR/shared/secret.json" ]]; then
  cat >"$APP_DIR/shared/secret.json" <<'EOF'
{
  "secret": {
    "token": "PUT_TELEGRAM_BOT_TOKEN_HERE"
  }
}
EOF
  chown "$APP_USER:$APP_GROUP" "$APP_DIR/shared/secret.json"
  chmod 600 "$APP_DIR/shared/secret.json"
fi

if [[ ! -f "$APP_DIR/shared/appsettings.json" ]]; then
  cat >"$APP_DIR/shared/appsettings.json" <<'EOF'
{
  "ConnectionStrings": {
    "DefaultConnection": "PUT_PRODUCTION_CONNECTION_STRING_HERE"
  }
}
EOF
  chown "$APP_USER:$APP_GROUP" "$APP_DIR/shared/appsettings.json"
  chmod 600 "$APP_DIR/shared/appsettings.json"
fi

SERVICE_FILE="/etc/systemd/system/$SERVICE_NAME"
sed \
  -e "s|__APP_USER__|$APP_USER|g" \
  -e "s|__APP_GROUP__|$APP_GROUP|g" \
  -e "s|__APP_DIR__|$APP_DIR|g" \
  -e "s|__DOTNET_CMD__|$DOTNET_CMD|g" \
  "$TEMPLATE_PATH" > "$SERVICE_FILE"

systemctl daemon-reload
systemctl enable "$SERVICE_NAME"

echo "Server bootstrap complete."
echo "1) Verify/replace token in: $APP_DIR/shared/secret.json"
echo "2) Verify/replace app settings in: $APP_DIR/shared/appsettings.json"
echo "3) Deploy from local machine: ./deploy/deploy.sh <user@host> --service $SERVICE_NAME --app-dir $APP_DIR --app-user $APP_USER"
