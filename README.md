# Модуль 06. Домашнее задание: Products Auth API (JWT и Claims)

Домашнее задание (СРС) по дисциплине «Разработка веб-сервисов» (CSE5032).  
Кафедра Программной инженерии, Институт Автоматики и информационных технологий.

---

## 🎯 Цель работы
Научиться настраивать аутентификацию в ASP.NET Core Web API, создавать JWT-токены, ограничивать доступ к методам API с помощью атрибута `[Authorize]`, внедрять пользовательские утверждения (`Claims`) и извлекать их в защищенных методах контроллера через `HttpContext.User.Claims`.

---

## 🛠 Технологический стек
- **Платформа:** .NET 9.0 (C# 13)
- **Фреймворк:** ASP.NET Core Web API
- **Аутентификация:** `Microsoft.AspNetCore.Authentication.JwtBearer`
- **Документация и тестирование:** Swagger UI (Swashbuckle 6.6.2) с поддержкой схемы `Bearer`

---

## 📂 Структура проекта

```text
Web_service_development_modul_06-hw_06/
├── .gitignore
├── ProductsAuthApi.sln
├── README.md
├── DESIGN_AND_ANSWERS.md
└── src/
    └── ProductsAuthApi/
        ├── Controllers/
        │   ├── AuthController.cs       # Метод входа POST /api/auth/login и выдача JWT
        │   └── ProductsController.cs   # Публичные GET и защищенные POST/DELETE для товаров
        ├── Models/
        │   ├── LoginRequest.cs         # DTO входа (username, password)
        │   └── Product.cs              # Модель товара (Id, Name, Price, Category, AddedBy)
        ├── appsettings.json            # Параметры JWT (Issuer, Audience, Key, Expiry)
        ├── Program.cs                  # Настройка DI, JWT Middleware и Swagger
        └── ProductsAuthApi.csproj
```

---

## 🚀 Быстрый запуск

### 1. Сборка проекта
```powershell
cd E:\Web_service_development_RK1\week_6\Web_service_development_modul_06-hw_06
dotnet build
```

### 2. Запуск приложения
```powershell
dotnet run --project src\ProductsAuthApi
```
После запуска перейдите по адресу: `http://localhost:5000/swagger`.

---

## 📋 Спецификация API

| Метод | Маршрут | Авторизация | Описание |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/login` | Публичный | Получение JWT токена (`student` / `12345`) |
| `GET` | `/api/products` | Публичный | Просмотр списка товаров без токена |
| `GET` | `/api/products/{id}` | Публичный | Просмотр одного товара без токена |
| `POST` | `/api/products` | `[Authorize]` | Добавление товара (только с JWT, фиксирует Claim автора) |
| `DELETE` | `/api/products/{id}` | `[Authorize]` | Удаление товара (только с JWT) |

---

## 🧪 Примеры тестирования

### 1. Вход и получение токена
```http
POST /api/auth/login HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "username": "student",
  "password": "12345"
}
```

### 2. Добавление товара без токена
```http
POST /api/products HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "name": "Новый товар",
  "price": 10000,
  "category": "Тест"
}
```
**Ответ:** `HTTP 401 Unauthorized`.

### 3. Добавление товара с валидным JWT
```http
POST /api/products HTTP/1.1
Host: localhost:5000
Content-Type: application/json
Authorization: Bearer <JWT_ТОКЕН>

{
  "name": "Игровая приставка Sony PlayStation 5",
  "price": 310000,
  "category": "Игры и консоли"
}
```
**Ответ:** `HTTP 201 Created` с прикрепленным клеймом автора `"authorClaim": "student"`.
