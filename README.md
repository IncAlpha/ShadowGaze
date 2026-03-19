# ShadowGaze

## Локальный запуск
Для локального запуска нужно создать файл `secret.json` в проекте `ShadowGaze`:

```json
{
  "secret": {
    "token": "{BOT_TOKEN: string}"
  }
}
```

## Artifact-based deploy (без GitHub Actions)

В репозитории есть готовые скрипты:
- `deploy/bootstrap_server.sh` - одноразовая подготовка сервера.
- `deploy/build_artifact.sh` - локальная сборка релизного архива.
- `deploy/deploy.sh` - деплой на сервер одной командой.
- `deploy/remote_deploy.sh` - внутренний скрипт, запускается на сервере через SSH.

### 1) Что нужно на сервере

1. Linux-сервер с `systemd`.
2. Установленный `.NET Runtime 9` (команда `dotnet --info` должна работать).
3. Пользователь с SSH-доступом, который может выполнять `sudo` (лучше без пароля для команд `systemctl`, `chown`, `ln`, `tar`, `mkdir`).
4. Открытый SSH-порт.

### 2) Одноразовая инициализация сервера

С локальной машины:

```bash
scp -r ./deploy user@your-server:/tmp/shadowgaze-deploy
ssh -t user@your-server "sudo /tmp/shadowgaze-deploy/bootstrap_server.sh"
```

Что делает bootstrap:
- создает системного пользователя `shadowgaze` (можно переопределить параметрами);
- подготавливает директории:
  - `/opt/shadowgaze/releases`
  - `/opt/shadowgaze/shared`
- создает `/opt/shadowgaze/shared/secret.json` (шаблон, если файла нет);
- создает `/opt/shadowgaze/shared/appsettings.json` (шаблон, если файла нет);
- устанавливает `systemd` unit `/etc/systemd/system/shadowgaze.service`;
- выполняет `systemctl daemon-reload` и `systemctl enable`.

После bootstrap обязательно проверьте конфиг в:
- `/opt/shadowgaze/shared/secret.json`
- `/opt/shadowgaze/shared/appsettings.json`

### 3) Деплой одной командой

Из корня репозитория:

```bash
./deploy/deploy.sh user@your-server
```

Скрипт автоматически:
1. Делает `dotnet publish` проекта `ShadowGaze/ShadowGaze.csproj`.
2. Упаковывает артефакт в `.deploy/artifacts/shadowgaze-<timestamp>.tar.gz`.
3. Отправляет архив на сервер.
4. На сервере:
   - распаковывает в `/opt/shadowgaze/releases/<timestamp>`;
   - подключает `/opt/shadowgaze/shared/secret.json`;
   - подключает `/opt/shadowgaze/shared/appsettings.json`;
   - переключает симлинк `/opt/shadowgaze/current`;
   - перезапускает `shadowgaze.service`.

### 4) Параметры скриптов

Пример с параметрами:

```bash
./deploy/deploy.sh deploy@prod.example.com \
  --ssh-port 22 \
  --service shadowgaze.service \
  --app-dir /opt/shadowgaze \
  --app-user shadowgaze \
  --configuration Release
```

Доступные опции:
- `./deploy/deploy.sh --help`
- `./deploy/build_artifact.sh --help`
- `./deploy/bootstrap_server.sh --help`

### 5) Полезные команды на сервере

Проверить статус сервиса:

```bash
sudo systemctl status shadowgaze.service --no-pager
```

Смотреть live-логи:

```bash
sudo journalctl -u shadowgaze.service -f
```
