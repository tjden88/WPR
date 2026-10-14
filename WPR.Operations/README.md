# WPR.Operations

[![NuGet](https://img.shields.io/nuget/v/WPR.Operations.svg)](https://www.nuget.org/packages/WPR.Operations)
[![Downloads](https://img.shields.io/nuget/dt/WPR.Operations.svg)](https://www.nuget.org/packages/WPR.Operations)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
![.NET](https://img.shields.io/badge/.NET-net8.0%20%7C%20net9.0%20%7C%20net10.0-512BD4)

Лёгкая библиотека-обёртка для передачи результата операции: **успех или ошибка**, и в случае успеха —
значение. Поддерживает сериализацию (например, для Web-API) и не тянет за собой никаких зависимостей.

Идея — заменить исключения и `bool`/`out`-паттерны явным объектом результата, который можно безопасно
возвращать из методов, передавать между слоями и отдавать клиенту по HTTP.

## Возможности

- Три типа результата: без значения, со значением и со значением + типизированной ошибкой.
- Неизменяемые `readonly struct` — без аллокаций в куче при передаче.
- Фабрики `Result` для лаконичного создания результатов.
- Удобное извлечение значения: `GetValueOrThrow`, `GetValueOrDefault`, `TryGetValue`.
- Готовность к `System.Text.Json` (сериализация и десериализация).
- Нулевые зависимости.

## Установка

```bash
dotnet add package WPR.Operations
```

Целевые платформы: `net8.0`, `net9.0`, `net10.0`.

## Быстрый старт

```csharp
using WPR.Operations;

// Результат без значения
OperationResult ok = OperationResult.Success();
OperationResult bad = OperationResult.Failure("Файл не найден");

if (bad) { /* ... */ }        // неявное приведение к bool
Console.WriteLine(bad.Error); // "Файл не найден"

// Результат со значением
OperationResult<int> number = OperationResult<int>.Success(42);
if (number.IsSuccess)
    Console.WriteLine(number.Value);

// Результат со значением и типизированной ошибкой
OperationResult<User, ErrorCode> user = OperationResult<User, ErrorCode>.Success(someUser);
```

## Фабрика `Result`

Короткий синтаксис вместо длинных имён типов:

```csharp
using WPR.Operations;

var a = Result.Success();                    // OperationResult
var b = Result.Failure("Что-то пошло не так"); // OperationResult

var c = Result.Success(42);                  // OperationResult<int>
var d = Result.Failure<int>("Нет данных");   // OperationResult<int>

var e = Result.Success<User, ErrorCode>(user);        // OperationResult<User, ErrorCode>
var f = Result.Failure<User, ErrorCode>(ErrorCode.NotFound);

// Из булевого признака и результата внешнего API
var g = Result.FromResult(someBool, "Ошибка");
var h = Result.FromResult<User>(success: true, value: user, error: ""); // ошибка не нужна при успехе
```

## Извлечение значения

```csharp
OperationResult<int> r = GetNumber();

// 1. Исключение при неудаче — если ошибка действительно исключительна
int value = r.GetValueOrThrow();

// 2. Значение по умолчанию — когда fallback допустим
int safe = r.GetValueOrDefault(-1);

// 3. Безопасная попытка
if (r.TryGetValue(out var v))
    Console.WriteLine(v);

// Явное приведение (неявного больше нет, чтобы не терять ошибки)
int casted = (int)r;
```

## Преобразование результата без значения в результат со значением

```csharp
OperationResult result = Save();

OperationResult<User> withValue = result.As(user);
```

## Сериализация (System.Text.Json)

Типы сериализуются «как есть» и восстанавливаются с сохранением статуса:

```csharp
var json = JsonSerializer.Serialize(OperationResult<int>.Success(5));
// {"Value":5,"Error":"","IsSuccess":true}

var restored = JsonSerializer.Deserialize<OperationResult<int>>(json);
// restored.IsSuccess == true, restored.Value == 5

JsonSerializer.Serialize(OperationResult.Failure("boom"));
// {"Error":"boom","IsSuccess":false}
```

## Семантика и инварианты

- **Единый источник истины.** Для строковых результатов успех определяется по `Error`:
  успех — это пустая строка, любая непустая (или `null`) ошибка — неудача. Для
  `OperationResult<T, TError>` используется явный флаг `IsSuccess`, так как у произвольного
  `TError` нет «пустого» значения.
- **`default` — это неудача (fail-safe).** `default(OperationResult)`,
  `default(OperationResult<T>)` и `default(OperationResult<T, TError>)` дают `IsSuccess == false`.
  Неинициализированный результат никогда не будет ошибочно принят за успех.
- **Ошибка обязательна.** `Failure(...)` не принимает `null`/пустую строку (bросает
  `ArgumentException`/`ArgumentNullException`).
- **Значение может быть `null`** для ссылочных и nullable-типов — это допустимый успешный результат.

### Работа с `OperationResult<T, TError>` и десериализацией

Признак успеха этого типа хранится в поле `IsSuccess` и при десериализации берётся из JSON как есть.
Для данных, которым вы не доверяете, проверяйте бизнес-инварианты (`Value != null`, `Error`) в коде,
а не полагайтесь только на пришедший `IsSuccess`.

## API — краткий справочник

| Тип / метод | Назначение |
|-------------|------------|
| `OperationResult` | Результат без значения, ошибка — `string`. |
| `OperationResult<T>` | Результат со значением `T`, ошибка — `string`. |
| `OperationResult<T, TError>` | Результат со значением `T` и ошибкой типа `TError`. |
| `IOperationResult<TError>` | Общий контракт (`Error`, `IsSuccess`). |
| `OperationResult.Success()` / `.Failure(string)` | Фабрики для результата без значения. |
| `OperationResult<T>.Success(T)` / `.Failure(string)` | Фабрики для результата со значением. |
| `OperationResult<T, TError>.Success(T)` / `.Failure(TError)` | Фабрики с типизированной ошибкой. |
| `Result.*` | Те же фабрики в виде сокращённых статических методов. |
| `OperationResultExtensions.As<T>()` | Результат без значения → результат со значением. |
| `IsSuccess` / `IsFailure` / `Error` / `Value` | Состояние результата. |
| `GetValueOrThrow()` / `GetValueOrDefault()` / `TryGetValue(out T)` | Доступ к значению. |

## Изменения 1.0.x → 1.1.0

Версия 1.1.0 содержит исправления корректности, часть из которых меняет поведение:

- `OperationResult<T, TError>.IsSuccess` больше не зависит от сравнения ошибки с `null`
  (`Error == null`) — для значимых типов ошибок это всегда давало `false`. Теперь используется явный флаг.
- `Result.FromResult(true, "")` больше не бросает исключение — текст ошибки проверяется только при неудаче.
- `default(OperationResult<T>)` теперь неудача (раньше — успех); состояние `default` согласовано fail-safe.
- `IsSuccess` вычисляется из `Error`, поэтому состояние нельзя рассогласовать (`with`, инициализатор, JSON).
- Извлечение значения стало `explicit`: `int x = result;` → `int x = (int)result;`
  (либо `GetValueOrThrow`/`GetValueOrDefault`/`TryGetValue`).
- `OperationResult<T, TError>.Failure(null)` теперь бросает `ArgumentNullException`.
- Удалены таргеты `net6.0`/`net7.0` (срок поддержки истёк).

## Лицензия

[MIT](https://opensource.org/licenses/MIT)

## Ссылки

- Исходный код: <https://github.com/tjden88/WPR>
