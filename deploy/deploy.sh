#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

TARGET_HOST=""
SSH_PORT="22"
APP_DIR="/opt/shadowgaze"
SERVICE_NAME="shadowgaze.service"
APP_USER="shadowgaze"
CONFIGURATION="Release"
PROJECT_PATH="ShadowGaze/ShadowGaze.csproj"
REMOTE_TMP_DIR="/tmp"

print_usage() {
  cat <<'EOF'
Usage:
  ./deploy/deploy.sh <user@host> [options]

Options:
  --ssh-port <port>        SSH port (default: 22)
  --app-dir <path>         App base directory on server (default: /opt/shadowgaze)
  --service <name>         Systemd service name (default: shadowgaze.service)
  --app-user <name>        Linux user that owns app files (default: shadowgaze)
  --configuration <name>   dotnet publish configuration (default: Release)
  --project <path>         Path to .csproj relative to repo root
  --remote-tmp <path>      Temporary directory on server (default: /tmp)
  -h, --help               Show help

Examples:
  ./deploy/deploy.sh deploy@192.168.1.20
  ./deploy/deploy.sh deploy@prod.example.com --service shadowgaze.service --app-dir /opt/shadowgaze
EOF
}

if [[ $# -eq 0 ]]; then
  print_usage
  exit 1
fi

if [[ "$1" == "-h" || "$1" == "--help" ]]; then
  print_usage
  exit 0
fi

TARGET_HOST="$1"
shift

while [[ $# -gt 0 ]]; do
  case "$1" in
    --ssh-port)
      SSH_PORT="$2"
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
    --configuration)
      CONFIGURATION="$2"
      shift 2
      ;;
    --project)
      PROJECT_PATH="$2"
      shift 2
      ;;
    --remote-tmp)
      REMOTE_TMP_DIR="$2"
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

if ! command -v ssh >/dev/null 2>&1; then
  echo "ssh is not installed." >&2
  exit 1
fi
if ! command -v scp >/dev/null 2>&1; then
  echo "scp is not installed." >&2
  exit 1
fi

RELEASE_NAME="shadowgaze-$(date -u +%Y%m%d%H%M%S)"
ARTIFACT_OUTPUT_DIR="$ROOT_DIR/.deploy/artifacts"
ARTIFACT_PATH="$ARTIFACT_OUTPUT_DIR/$RELEASE_NAME.tar.gz"
REMOTE_ARCHIVE="$REMOTE_TMP_DIR/$RELEASE_NAME.tar.gz"

echo "Step 1/3: Building artifact..."
"$SCRIPT_DIR/build_artifact.sh" \
  --project "$PROJECT_PATH" \
  --configuration "$CONFIGURATION" \
  --output-dir "$ARTIFACT_OUTPUT_DIR" \
  --artifact-name "$RELEASE_NAME"

echo "Step 2/3: Uploading artifact to $TARGET_HOST..."
scp -P "$SSH_PORT" "$ARTIFACT_PATH" "$TARGET_HOST:$REMOTE_ARCHIVE"

echo "Step 3/3: Activating new release on server..."
ssh -tt -p "$SSH_PORT" "$TARGET_HOST" \
  "bash -s -- --archive '$REMOTE_ARCHIVE' --release '$RELEASE_NAME' --app-dir '$APP_DIR' --service '$SERVICE_NAME' --app-user '$APP_USER'" \
  < "$SCRIPT_DIR/remote_deploy.sh"

echo "Done."
