# Recuperar · `Recover`

> O oposto do [`Then`](then.md): opera na **trilha de falha**. `Recover` e seus parentes trazem o pipeline de volta ao sucesso — sempre de forma **deliberada e explícita**.

---

## Quando usar

Fornecer um padrão quando algo não foi encontrado, cair para uma fonte alternativa quando um serviço está fora, tentar um segundo caminho.

## Quando *não* usar

| Você quer… | Use no lugar |
|---|---|
| só **observar** o erro (log) sem recuperar | [`TapError`](tap.md) |
| **reescrever** o erro, não recuperar | [`MapError`](map-errors.md) |

---

## Operadores

| Método | Recupera quando… |
|---|---|
| `Recover(value)` / `Recover(func)` | **qualquer** falha → valor padrão |
| `RecoverWhen(AxisErrorType, func)` | os erros são de um **tipo** |
| `RecoverWhen(code, func)` | os erros têm um **código** |
| `RecoverWhen(predicate, func)` | um **predicado** sobre os erros |
| `RecoverNotFound(func)` | **todos** os erros são `NotFound` |
| `OrElse(fallback)` | tenta uma **operação** alternativa |
| `OrElse(fallback, combineErrors: true)` | alternativa, **acumulando** os erros dos dois lados |

Todos têm variantes `Async` (`Task`/`ValueTask`).

---

## Exemplo 1 — padrão quando não existe

```csharp
var settings = await GetUserSettingsAsync(userId)
    .RecoverNotFoundAsync(() => DefaultSettings.Create());
```

**Por que compensa:** "não tem configuração salva? use o padrão" deixa de ser um `catch (NotFoundException)` e vira uma intenção explícita — e só recupera de `NotFound`, não de erros de verdade (ex.: falha de banco).

## Exemplo 2 — fallback condicional por tipo

```csharp
var data = await FetchFromPrimaryAsync()
    .RecoverWhenAsync(AxisErrorType.ServiceUnavailable, () => FetchFromFallbackAsync());
```

**Por que compensa:** só cai para o secundário quando o primário está **indisponível** (transiente); um erro de validação, por exemplo, continua falhando como deveria.

## Exemplo 3 — caminho alternativo com acúmulo de erros

```csharp
var user = await FindByEmailAsync(email)
    .OrElseAsync(_ => FindByPhoneAsync(phone), combineErrors: true);
// se os DOIS falharem, você recebe os erros das duas tentativas
```

---

## Veja também

- [Erros e tipos](errors-and-types.md) — `IsTransient`, tipos e códigos para condicionar a recuperação
- [Remapear erros · `MapError`](map-errors.md) — transformar o erro em vez de recuperar
- [Garantir · `Ensure`](ensure.md) — o oposto: levar do sucesso à falha
