# AzertaNodes

AzertaNodes — backend-платформа для создания и выполнения workflow с использованием ИИ-узлов, HTTP-запросов и интеграций с внешними сервисами. Логика процессов представлена в виде графа, состоящего из связанных узлов.

## Возможности

- Создание и хранение workflow-графов.
- Выполнение узлов с учётом зависимостей.
- Интеграция с LLM через Semantic Kernel.
- HTTP-запросы и обработка данных.
- Интеграция с Discord.
- Управление проектами и credentials через REST API.
- Хранение данных в PostgreSQL.

## Архитектура

Проект разделён на несколько слоёв:

- `src/AzertaNodes.Api` — REST API на ASP.NET Core.
- `src/AzertaNodes.Application` — бизнес-логика и валидация графов.
- `src/AzertaNodes.Core` — основные модели и интерфейсы.
- `src/Infrastructure` — реализация узлов, интеграции и работа с базой данных.

## Технологический стек

- C# / .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Docker / Docker Compose
- Semantic Kernel
- DSharpPlus
- Polly

## Типы узлов

- `LlmNode` — обработка текста с помощью языковых моделей.
- `HttpRequestNode` — отправка HTTP-запросов.
- `HttpResponseNode` — формирование HTTP-ответов.
- `LogicNode` — выполнение логических операций.
- `TestConsoleWriteLineNode` — вывод данных в консоль.
- `DiscordTriggerNode` — обработка событий Discord.
- `DiscordSendMessageNode` — отправка сообщений в Discord.

## Запуск

### Требования

- .NET 10 SDK
- Docker и Docker Compose

### Docker Compose

```bash
docker compose up --build
```

После запуска:

- API: `http://localhost:8080`
- PostgreSQL: `localhost:5432`

## Конфигурация

Строка подключения к PostgreSQL задаётся через переменную окружения:

```env
ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword
```
