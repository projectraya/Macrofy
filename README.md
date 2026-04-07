# Macrofy – ASP.NET Core MVC Platform

Платформа, свързваща клиенти с персонализирани готвачи на база макронутриенти.

## Технологии
- **Backend:** ASP.NET Core 8 MVC
- **ORM:** Entity Framework Core 8
- **Auth:** ASP.NET Core Identity
- **БД (локално):** SQLite (автоматично, zero setup)
- **БД (production):** SQL Server (Railway)
- **Хостинг:** Railway (безплатен tier)

## Бърз старт (локално)

```bash
# 1. Клонирай / разархивирай проекта
cd Macrofy

# 2. Restore packages
dotnet restore

# 3. Стартирай (SQLite + seed данни автоматично)
dotnet run

# 4. Отвори браузъра на http://localhost:5000
```

## Demo акаунти (seed автоматично)
| Роля    | Имейл                  | Парола      |
|---------|------------------------|-------------|
| Клиент  | demo@macrofy.bg        | Client@123! |
| Готвач  | ivan@macrofy.bg        | Chef@123!   |
| Админ   | admin@macrofy.bg       | Admin@123!  |

## Структура на проекта

```
Macrofy/
├── Controllers/
│   ├── AccountController.cs    – Регистрация, Вход, Изход
│   ├── DashboardController.cs  – Клиентско табло
│   ├── ChefController.cs       – Готвашко табло
│   ├── AdminController.cs      – Администраторски панел
│   ├── OrderController.cs      – Управление на поръчки
│   ├── HomeController.cs       – Публична landing страница
│   └── PartnerController.cs    – Партньорски заявки
├── Models/
│   ├── ApplicationUser.cs      – Потребител (Identity + макро профил)
│   ├── ChefProfile.cs          – Профил на готвач
│   └── Models.cs               – MealPlan, Order, Review, PartnerApplication
├── Data/
│   ├── AppDbContext.cs         – EF Core DbContext
│   └── DbSeeder.cs             – Demo данни при стартиране
├── Services/
│   └── MacroCalculatorService.cs  – Mifflin-St Jeor формула
├── ViewModels/
│   └── ViewModels.cs           – Всички ViewModels
├── Views/                      – Всички Razor Views
├── wwwroot/
│   ├── css/site.css            – Пълен design system
│   └── images/logo.png         – Macrofy лого
├── Dockerfile                  – За Railway deployment
├── railway.toml                – Railway конфигурация
└── Program.cs                  – DI + middleware конфигурация
```

## Деплой на Railway

### 1. Създай SQL Server база на Railway
- Влез в [railway.app](https://railway.app)
- New Project → Add Service → Database → SQL Server
- Копирай connection string

### 2. Конфигурирай environment variables
В Railway dashboard → Variables:
```
ConnectionStrings__DefaultConnection = Server=...;Database=macrofy;...
ASPNETCORE_ENVIRONMENT = Production
```

### 3. Деплой
```bash
# Инсталирай Railway CLI
npm install -g @railway/cli

# Логни се
railway login

# Деплой
railway up
```

Или просто свържи GitHub repo в Railway dashboard.

## Функционалности

### За клиенти
- Регистрация и вход
- Макро калкулатор (Mifflin-St Jeor) с live preview
- Разглеждане на готвачи с филтри по специализация
- Поръчка при готвач (избор на период и адрес)
- Преглед на поръчки и техния статус
- Отмяна на чакащи поръчки

### За готвачи
- Регистрация като готвач
- Редактиране на профил (биография, специалности, цена)
- Tabло с pending/активни поръчки
- Потвърждаване/отказване на поръчки
- Обновяване на статус (Приготвя се → Доставена)
- Преглед на клиенти

### За администратори
- Dashboard с обобщена статистика
- Верификация на готвачи
- Преглед на всички потребители, поръчки, партньорски заявки
- Одобряване/отхвърляне на партньорства

## База данни (схема)

```
Users (ApplicationUser + Identity)
  ├── ChefProfiles (1:1)
  ├── Orders (1:N като клиент)
  └── Reviews (1:N)

ChefProfiles
  ├── Orders (1:N)
  ├── Reviews (1:N)
  └── MealPlans (1:N)

Orders
  ├── Client (N:1)
  ├── ChefProfile (N:1)
  ├── MealPlan (N:1, optional)
  └── Review (1:1, optional)

PartnerApplications (standalone)
```

## Следващи стъпки (v2)
- [ ] Upload на снимки за готвачи
- [ ] Система за оценки и reviews
- [ ] Email нотификации (SendGrid)
- [ ] Stripe плащания
- [ ] Sedмично меню от готвача
- [ ] Мобилна версия (PWA)
