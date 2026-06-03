# Parser

Кроссплатформенное .NET MAUI приложение для парсинга и обработки данных.

## Возможности

- Парсинг HTML-контента (HtmlAgilityPack)
- Локальное хранение данных (SQLite)
- Кроссплатформенность: Android, iOS, macOS Catalyst, Windows
- Современный UI на базе .NET MAUI

## Технологии

- **[.NET MAUI](https://learn.microsoft.com/dotnet/maui)** — фреймворк для создания нативных кроссплатформенных приложений
- **HTML Agility Pack** — парсинг HTML
- **sqlite-net-pcl** — локальная база данных SQLite
- **CommunityToolkit.Maui** — набор UI-компонентов и утилит
- **FFImageLoading** — оптимизированная загрузка изображений

## Требования

- .NET 9 SDK
- Visual Studio 2022 (рекомендуется) или JetBrains Rider
- Для сборки под iOS/macOS — macOS с Xcode
- Для сборки под Android — Android SDK

## Сборка и запуск

```bash
# Восстановление зависимостей
dotnet restore

# Запуск на Windows
dotnet build -t:Run -f net9.0-windows10.0.19041.0

# Запуск на Android
dotnet build -t:Run -f net9.0-android
```

## Структура проекта

```
Parser/
├── Converters/       # Value converters для XAML-привязок
├── Models/           # Модели данных
├── Services/         # Сервисы (парсинг, БД и др.)
├── ViewModels/       # ViewModel (MVVM)
├── Platforms/        # Платформозависимый код
├── Resources/        # Ресурсы (иконки, изображения, шрифты)
├── App.xaml(.cs)     # Точка входа приложения
├── AppShell.xaml(.cs)# Навигация
├── MainPage.xaml(.cs)# Главная страница
└── MauiProgram.cs    # Конфигурация DI и сервисов
```

## Лицензия

MIT
