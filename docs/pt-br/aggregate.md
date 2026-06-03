# Agregar · `Combine` / `All`

> Reduz **muitos** resultados a **um**. Diferente do [`Zip`](zip.md) (que combina valores diferentes numa tupla), aqui você dobra uma **coleção** — e coleta **todos** os erros, não só o primeiro.

---

## Quando usar

Validar vários campos de uma vez (querendo ver todas as falhas), ou consolidar uma lista de operações do mesmo tipo num único resultado.

---

## Operadores

| Método | Entra | Sai |
|---|---|---|
| `Combine(params results)` | N × `AxisResult` (sem valor) | `AxisResult` — **todos** os erros juntos |
| `All(results)` | N × `AxisResult<T>` | `AxisResult<IReadOnlyList<T>>` |
| `CombineAsync` / `AllAsync` | versões para `Task`/`ValueTask` | idem |

---

## Exemplo 1 — validar tudo e mostrar todas as falhas

```csharp
var result = AxisResult.Combine(
    ValidateName(cmd.Name),
    ValidateEmail(cmd.Email),
    ValidateAge(cmd.Age));
// coleta TODOS os erros, não só o primeiro
```

**Por que compensa:** o usuário vê de uma vez "nome vazio **e** e-mail inválido", em vez de corrigir um, reenviar, e só então descobrir o próximo. Um único *round-trip* de validação.

## Exemplo 2 — consolidar uma lista do mesmo tipo

```csharp
var result = await AxisResult.AllAsync(
    userIds.Select(id => GetUserAsync(id)));
// AxisResult<IReadOnlyList<User>> — ou todos os usuários, ou todos os erros
```

**Por que compensa:** "buscar N e seguir só se todos vieram" vira uma linha; se algum falhar, os erros agregados sobem juntos.

---

## `Combine`/`All` vs `Zip`

- **`Combine`/`All`** → N itens do **mesmo** tipo → uma lista (ou um void agregado).
- **[`Zip`](zip.md)** → 2–4 valores **diferentes** → uma tupla.

---

## Veja também

- [Combinar · `Zip`](zip.md) — para valores heterogêneos numa tupla
- [Erros e tipos](errors-and-types.md) — por que acumular todos os erros importa
- [Garantir · `Ensure`](ensure.md) — validação de um único valor na trilha
