import React from 'react';
import { Layout } from './shared/Layout';
import { CodeBlock } from './shared/CodeBlock';

export const GettingStartedPage: React.FC = () => {
    const installCode = `dotnet add package AxisResult`;

    const successCode = `// Non-generic success
AxisResult success = AxisResult.Ok();

// Generic success with value
AxisResult<int> typed = AxisResult.Ok(42);

// Implicit conversion from values
AxisResult<string> name = "John";  // auto-wraps in Ok`;

    const failureCode = `// Explicit failure with typed error
AxisResult failure = AxisError.NotFound("USER_NOT_FOUND");

// Generic failure
AxisResult<int> typed = AxisError.BusinessRule("INSUFFICIENT_STOCK");

// From exceptions (safe boundary for external code)
AxisResult result = AxisResult.Try(() => riskyOperation());
AxisResult<int> parsed = AxisResult.Try(() => int.Parse(input));`;

    const pipelineCode = `public Task<AxisResult<UserDto>> GetUserAsync(int userId)
    => userRepository.GetByIdAsync(userId)
        .EnsureAsync(
            user => user != null,
            () => AxisError.NotFound($"User {userId} not found")
        )
        .MapAsync(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        });`;

    const handlingCode = `// Pattern matching
var result = await userService.GetUserAsync(userId);
var response = result switch
{
    { IsSuccess: true, Value: var user } => Ok(user),
    { IsFailure: true, Error.Type: AxisErrorType.NotFound } => NotFound(),
    _ => BadRequest()
};

// Or using extension methods
if (result.IsSuccess)
{
    var user = result.Value;
    return Ok(user);
}
else
{
    return BadRequest(result.Error.Message);
}`;

    const tapCode = `public Task<AxisResult<Order>> CreateOrderAsync(CreateOrderCommand cmd)
    => orderFactory.CreateAsync(cmd)
        .ThenAsync(order => unitOfWork.SaveChangesAsync())
        .TapAsync(order => logger.LogInformation("Order {OrderId} created", order.Id))
        .TapAsync(order => eventBus.PublishAsync(new OrderCreatedEvent(order.Id)))
        .MapAsync(order => new OrderDto(order.Id, order.Status));`;

    const recoverCode = `// Recover on specific error types
var result = await userService.GetUserAsync(userId)
    .RecoverWhen(
        error => error.Type == AxisErrorType.NotFound,
        () => GetDefaultUserAsync()
    );

// Or recover with any failure
var result = await risky.Operation()
    .RecoverAsync(error => AxisResult.Ok(new DefaultValue()));`;

    return (
        <Layout>
            <h1 className="text-4xl font-bold mb-8 text-blue-400">Getting Started</h1>

            <div className="prose prose-invert max-w-4xl">
                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Installation</h2>

                    <p className="text-slate-300 mb-4">Install AxisResult from NuGet:</p>

                    <CodeBlock code={installCode} />

                    <p className="text-slate-400 text-sm">
                        Or via the NuGet Package Manager in Visual Studio.
                    </p>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Creating Results</h2>

                    <h3 className="text-xl font-bold text-blue-300 mb-3">Success Results</h3>
                    <CodeBlock code={successCode} />

                    <h3 className="text-xl font-bold text-blue-300 mb-3">Failure Results</h3>
                    <CodeBlock code={failureCode} />
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Basic Pipeline</h2>

                    <p className="text-slate-300 mb-4">
                        Build composable pipelines using <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">.ThenAsync()</code>, <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">.MapAsync()</code>, and <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">.EnsureAsync()</code>:
                    </p>

                    <CodeBlock code={pipelineCode} />

                    <div className="bg-blue-900/20 p-4 rounded-lg border border-blue-800 text-slate-300 mb-6">
                        <p><strong className="text-blue-300">What happens:</strong></p>
                        <ul className="mt-2 space-y-2">
                            <li>✓ If <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">GetByIdAsync</code> succeeds → continue to <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">EnsureAsync</code></li>
                            <li>✗ If user is null → switch to failure rail with NotFound error</li>
                            <li>✓ If validation passes → map to <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">UserDto</code></li>
                            <li>✗ If repository throws → automatically caught and wrapped in AxisResult</li>
                        </ul>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Handling Results</h2>

                    <p className="text-slate-300 mb-4">
                        Once you have a result, inspect it with pattern matching or extension methods:
                    </p>

                    <CodeBlock code={handlingCode} />
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Side Effects with Tap</h2>

                    <p className="text-slate-300 mb-4">
                        Use <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">.TapAsync()</code> for logging, metrics, or notifications without breaking the pipeline:
                    </p>

                    <CodeBlock code={tapCode} />

                    <p className="text-slate-400 text-sm">
                        All the <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">.TapAsync()</code> calls execute only if the pipeline is on the success rail. No nested conditionals needed.
                    </p>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Recovery and Fallback</h2>

                    <p className="text-slate-300 mb-4">
                        Handle failures gracefully with recovery patterns:
                    </p>

                    <CodeBlock code={recoverCode} />
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Error Types</h2>

                    <p className="text-slate-300 mb-4">
                        AxisResult provides 12 typed error categories for precise error handling:
                    </p>

                    <div className="grid grid-cols-2 gap-3">
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">NotFound</p>
                            <p className="text-slate-400 text-xs">Resource doesn't exist</p>
                        </div>
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">BusinessRule</p>
                            <p className="text-slate-400 text-xs">Violated business logic</p>
                        </div>
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">Validation</p>
                            <p className="text-slate-400 text-xs">Input validation failed</p>
                        </div>
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">Conflict</p>
                            <p className="text-slate-400 text-xs">Resource already exists</p>
                        </div>
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">Unauthorized</p>
                            <p className="text-slate-400 text-xs">Authentication failed</p>
                        </div>
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">Forbidden</p>
                            <p className="text-slate-400 text-xs">Not authorized for resource</p>
                        </div>
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">Transient</p>
                            <p className="text-slate-400 text-xs">Temporary failure, retry viable</p>
                        </div>
                        <div className="bg-slate-900 p-3 rounded border border-slate-800">
                            <p className="text-blue-300 font-mono text-sm">Internal</p>
                            <p className="text-slate-400 text-xs">Unexpected system error</p>
                        </div>
                    </div>
                </section>

                <section className="mb-12 bg-blue-900/20 p-6 rounded-lg border border-blue-800">
                    <h2 className="text-2xl font-bold text-blue-300 mb-4">Next Steps</h2>
                    <ul className="text-slate-300 space-y-3">
                        <li className="flex gap-3">
                            <span className="text-blue-400">→</span>
                            <span>Check the <a href="/AxisResult/docs/api-reference/" className="text-blue-400 hover:text-blue-300">API Reference</a> for complete method signatures</span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-blue-400">→</span>
                            <span>See how AxisResult compares to <a href="/AxisResult/docs/comparison/" className="text-blue-400 hover:text-blue-300">other libraries</a></span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-blue-400">→</span>
                            <span>Explore advanced patterns in the <a href="https://github.com/fillippeprata/axisresult" className="text-blue-400 hover:text-blue-300">GitHub repository</a></span>
                        </li>
                    </ul>
                </section>
            </div>

            <div className="mt-12 pt-8 border-t border-slate-800 space-y-4">
                <a href="/AxisResult/docs/comparison/" className="block text-blue-400 hover:text-blue-300 font-medium text-lg">
                    ← Next: Compare with Other Libraries
                </a>
            </div>
        </Layout>
    );
};
