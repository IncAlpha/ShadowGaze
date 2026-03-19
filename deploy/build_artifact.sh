#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

PROJECT_PATH="ShadowGaze/ShadowGaze.csproj"
CONFIGURATION="Release"
OUTPUT_DIR="$ROOT_DIR/.deploy/artifacts"
ARTIFACT_NAME=""

print_usage() {
  cat <<'EOF'
Usage:
  ./deploy/build_artifact.sh [options]

Options:
  --project <path>         Path to .csproj relative to repository root.
  --configuration <name>   Build configuration (default: Release).
  --output-dir <path>      Directory for resulting archive (default: ./.deploy/artifacts).
  --artifact-name <name>   Archive name without extension (default: shadowgaze-<utc timestamp>).
  -h, --help               Show help.
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --project)
      PROJECT_PATH="$2"
      shift 2
      ;;
    --configuration)
      CONFIGURATION="$2"
      shift 2
      ;;
    --output-dir)
      OUTPUT_DIR="$2"
      shift 2
      ;;
    --artifact-name)
      ARTIFACT_NAME="$2"
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

if [[ -z "$ARTIFACT_NAME" ]]; then
  ARTIFACT_NAME="shadowgaze-$(date -u +%Y%m%d%H%M%S)"
fi

PROJECT_ABS="$ROOT_DIR/$PROJECT_PATH"
if [[ ! -f "$PROJECT_ABS" ]]; then
  echo "Project file not found: $PROJECT_ABS" >&2
  exit 1
fi

BUILD_ROOT="$ROOT_DIR/.deploy/build"
PUBLISH_DIR="$BUILD_ROOT/publish"
ARCHIVE_PATH="$OUTPUT_DIR/$ARTIFACT_NAME.tar.gz"

rm -rf "$PUBLISH_DIR"
mkdir -p "$PUBLISH_DIR" "$OUTPUT_DIR"

echo "Publishing $PROJECT_PATH ($CONFIGURATION)..."
dotnet publish "$PROJECT_ABS" -c "$CONFIGURATION" -o "$PUBLISH_DIR"

# Server configuration files are managed in /opt/shadowgaze/shared.
rm -f "$PUBLISH_DIR/secret.json"
rm -f "$PUBLISH_DIR/appsettings.json"

if git -C "$ROOT_DIR" rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  GIT_SHA="$(git -C "$ROOT_DIR" rev-parse --short HEAD)"
else
  GIT_SHA="unknown"
fi

cat >"$PUBLISH_DIR/BUILD_INFO.txt" <<EOF
artifact_name=$ARTIFACT_NAME
created_utc=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
git_sha=$GIT_SHA
configuration=$CONFIGURATION
project=$PROJECT_PATH
EOF

tar -czf "$ARCHIVE_PATH" -C "$PUBLISH_DIR" .
echo "Artifact created: $ARCHIVE_PATH"
