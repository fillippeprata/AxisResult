# Garantir · `Ensure`

> Valida um invariante **dentro** do pipeline. Se a guarda falhar, a trilha vira falha e o resto é pulado.

---

## Quando usar

Garantir uma condição sobre o valor atual (estoque suficiente, status válido) ou impor "só continue se ainda não existe".

## Quando *não* usar

| Você quer…                                 | Use no lugar            |
|--------------------------------------------|-------------------------|
| transformar o valor                        | [`Map`](map.md)         |
| encadear um passo que produz um novo valor | [`Then`](then.md)       |
| validação automática **antes** do handler  | (pipeline de validação) |

---

## Operadores

| Método | Assinatura | O que faz |
|---|---|---|
| `Ensure` | `(Func<T,bool> predicate, AxisError error)` | falha com `error` se o predicado for falso |
| `Ensure` | `(Func<T,AxisResult> validation)` | validação delegada que retorna `AxisResult` |
| `RequireNotFound` | `(AxisError errorIfFound)` | achou → falha; `NotFound` → segue como sucesso |
| `WithValue` | `(value)` | promove um `AxisResult` (sem valor) para `AxisResult<T>` |

Todos têm variantes `Async` (`Task`/`ValueTask`) e [com `CancellationToken`](cancellation.md).

---

## Exemplo 1 — guarda de regra de negócio

```csharp
return GetProductAsync(cmd.ProductId) // AxisResult<Product>
    .EnsureAsync(p => p.Stock >= cmd.Quantity, AxisError.BusinessRule("INSUFFICIENT_STOCK"))
    .ThenAsync(p => reserveStockPort.ReserveAsync(p.Id, cmd.Quantity));
```

**Por que compensa:** a regra "tem estoque?" fica **na própria trilha**, como um passo legível, em vez de um `if` solto com um `return BadRequest` no meio do handler.

## Exemplo 2 — criar só se não existe (idempotência)

`RequireNotFound` transforma "não encontrado" em sucesso, e qualquer outro desfecho em falha:

```csharp
public Task<AxisResult<IPersonAggregateApplication>> CreateAsync(NewArgs args)
    => readerPort.GetByNationalIdAsync(args.NationalId)                          // procura
        .RequireNotFoundAsync(AxisError.Conflict("DOCUMENT_ALREADY_EXISTS"))     // achou → falha
        .WithValueAsync(new PersonEntity(args.NationalId, args.DisplayName))     // não achou → cria
        .MapAsync(NewInstance);
```

**Por que compensa:** o padrão "crie se não existir" — que normalmente exige um `if (found) throw` — vira três passos declarativos que leem como a regra fala.

---

## Veja também

- [Encadear · `Then`](then.md) — o passo seguinte depois da guarda
- [Erros e tipos](errors-and-types.md) — escolher o `AxisError` certo para a falha
- [Recuperar · `Recover`](recover.md) — o oposto: tratar a falha e voltar ao sucesso
