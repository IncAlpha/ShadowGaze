#!/usr/bin/env bash
set -euo pipefail

ARCHIVE_PATH=""
APP_DIR="/opt/shadowgaze"
SERVICE_NAME="shadowgaze.service"
APP_USER="shadowgaze"
RELEASE_NAME=""

print_usage() {
  cat <<'EOF'
Usage:
  ./remote_deploy.sh --archive <path> --release <name> [options]

Options:
  --app-dir <path>         Base directory for application (default: /opt/shadowgaze)
  --service <name>         Systemd service name (default: shadowgaze.service)
  --app-user <name>        Owner user for app files (default: shadowgaze)
  -h, --help               Show help
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --archive)
      ARCHIVE_PATH="$2"
      shift 2
      ;;
    --release)
      RELEASE_NAME="$2"
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
    --app-user)
      APP_USER="$2"
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

if [[ -z "$ARCHIVE_PATH" || -z "$RELEASE_NAME" ]]; then
  echo "--archive and --release are required." >&2
  print_usage
  exit 1
fi

if [[ ! -f "$ARCHIVE_PATH" ]]; then
  echo "Archive not found: $ARCHIVE_PATH" >&2
  exit 1
fi

if [[ "$EUID" -eq 0 ]]; then
  SUDO=""
else
  if ! command -v sudo >/dev/null 2>&1; then
    echo "sudo is required when not running as root." >&2
    exit 1
  fi
  SUDO="sudo"
fi

run_root() {
  if [[ -n "$SUDO" ]]; then
    sudo "$@"
  else
    "$@"
  fi
}

RELEASES_DIR="$APP_DIR/releases"
SHARED_DIR="$APP_DIR/shared"
CURRENT_LINK="$APP_DIR/current"
RELEASE_DIR="$RELEASES_DIR/$RELEASE_NAME"

echo "Preparing directories..."
run_root mkdir -p "$RELEASES_DIR" "$SHARED_DIR"
run_root mkdir -p "$RELEASE_DIR"

echo "Extracting artifact to $RELEASE_DIR..."
run_root tar -xzf "$ARCHIVE_PATH" -C "$RELEASE_DIR"

if ! run_root test -f "$SHARED_DIR/secret.json"; then
  echo "Missing $SHARED_DIR/secret.json" >&2
  echo "Create it before deployment." >&2
  exit 1
fi

echo "Linking shared secret.json..."
run_root ln -sfn "$SHARED_DIR/secret.json" "$RELEASE_DIR/secret.json"

echo "Switching current release..."
run_root ln -sfn "$RELEASE_DIR" "$CURRENT_LINK"

echo "Setting ownership to $APP_USER..."
run_root chown -R "$APP_USER:$APP_USER" "$RELEASE_DIR"
run_root chown -h "$APP_USER:$APP_USER" "$CURRENT_LINK"

echo "Restarting service $SERVICE_NAME..."
run_root systemctl restart "$SERVICE_NAME"
run_root systemctl is-active --quiet "$SERVICE_NAME"

echo "Cleaning temporary archive..."
run_root rm -f "$ARCHIVE_PATH"

echo "Deployment successful. Active release: $RELEASE_DIR"
