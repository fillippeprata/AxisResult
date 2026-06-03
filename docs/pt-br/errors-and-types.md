# Erros e tipos · `AxisError`

> Um erro no AxisResult é um **valor**, não uma exceção. Cada `AxisError` carrega só duas coisas: um **`Code`** estável e um **`Type`** (categoria).

```csharp
AxisError err = AxisError.NotFound("USER_NOT_FOUND");
// err.Code  → "USER_NOT_FOUND"            (chave estável: logs, métricas, retry)
// err.Type  → AxisErrorType.NotFound
```

---

## As 12 categorias

Todo `AxisError` tem um `AxisErrorType`. Elas mapeiam naturalmente para status HTTP — o que torna a tradução na borda da API trivial:

| Fábrica | Tipo | HTTP |
|---|---|:--:|
| `AxisError.ValidationRule(code)` | `ValidationRule` | 400 |
| `AxisError.Unauthorized(code)` | `Unauthorized` | 401 |
| `AxisError.Forbidden(code)` | `Forbidden` | 403 |
| `AxisError.NotFound(code)` | `NotFound` | 404 |
| `AxisError.Conflict(code)` | `Conflict` | 409 |
| `AxisError.BusinessRule(code)` | `BusinessRule` | 422 |
| `AxisError.TooManyRequests(code)` | `TooManyRequests` | 429 |
| `AxisError.InternalServerError(code)` | `InternalServerError` | 500 |
| `AxisError.Mapping(code)` | `Mapping` | 500 |
| `AxisError.ServiceUnavailable(code)` | `ServiceUnavailable` | 503 |
| `AxisError.Timeout(code)` | `Timeout` | 504 |
| `AxisError.GatewayTimeout(code)` | `GatewayTimeout` | 504 |

---

## Por que não existe um campo `Message`?

Decisão deliberada: `AxisError` carrega **só** `Code` + `Type`.

- **O `Code` é a chave primária.** Estável entre versões, para que logs, métricas, alertas e políticas de retry pivotem nele sem fazer *parse* de string.
- **Mensagem é outra responsabilidade.** Localização, tom e a decisão de expor (ou não) um detalhe interno ao usuário final não pertencem a um valor que trafega no pipeline.

O padrão recomendado é um **resolver `code → mensagem` na borda de apresentação** (controller, interceptor gRPC), com os textos em arquivos de recurso:

```csharp
return result.Match(
    onSuccess: value  => Ok(value),
    onFailure: errors => Problem(
        title:  errors[0].Type.ToString(),
        detail: messageResolver.Resolve(errors[0], CultureInfo.CurrentUICulture)));
```

Assim: códigos pequenos e canônicos (`USER_NOT_FOUND`); várias UIs (REST, gRPC, CLI) renderizam o mesmo código de formas diferentes; testes verificam **códigos**, não prosa em inglês; nenhum dado pessoal vaza no payload de erro.

> Precisa passar **detalhes** (id, quantidade tentada)? Emita **vários `AxisError`** — a lista de erros já é a coleção natural para isso. Veja [Agregar · `Combine`/`All`](aggregate.md).

---

## Erros transientes (retry)

```csharp
if (error.IsTransient)   // true p/ ServiceUnavailable, Timeout, TooManyRequests, GatewayTimeout
    await RetryAsync();
```

`IsTransient` está embutido no tipo. *Circuit breakers*, políticas de retry e *health checks* inspecionam isso sem fazer *parse* de mensagens nem manter listas de strings.

**Por que compensa:** "isto vale a pena tentar de novo?" vira uma propriedade do sistema de tipos, não uma convenção frágil baseada em texto.

---

## Veja também

- [Remapear erros · `MapError`](map-errors.md) — reescrever códigos/tipos ao cruzar camadas
- [Recuperar · `Recover`](recover.md) — voltar da trilha de falha para a de sucesso
- [Sair · `Match`](match.md) — converter o resultado final em resposta HTTP
