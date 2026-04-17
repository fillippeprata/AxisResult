import React from 'react';
import { Layout } from './shared/Layout';

export const ApiReferencePage: React.FC = () => {
    return (
        <Layout>
            <h1 className="text-4xl font-bold mb-8 text-blue-400">API Reference</h1>

            <div className="prose prose-invert max-w-4xl">
                <p className="text-slate-400 mb-8 text-lg">
                    Complete reference for AxisResult's core methods and operations.
                </p>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Creating Results</h2>

                    <div className="space-y-4">
                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">AxisResult.Ok()</h4>
                            <p className="text-slate-400 text-sm mb-2">Create a successful non-generic result.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public static AxisResult Ok();`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">AxisResult.Ok&lt;T&gt;(value)</h4>
                            <p className="text-slate-400 text-sm mb-2">Create a successful result with a value.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public static AxisResult<T> Ok<T>(T value);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">AxisError.NotFound(message)</h4>
                            <p className="text-slate-400 text-sm mb-2">Create a "not found" error result.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public static AxisResult NotFound(string message);
public static AxisResult<T> NotFound<T>(string message);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">AxisError.BusinessRule(message)</h4>
                            <p className="text-slate-400 text-sm mb-2">Create a business rule violation error.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public static AxisResult BusinessRule(string message);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">AxisResult.Try(action)</h4>
                            <p className="text-slate-400 text-sm mb-2">Wrap exception-throwing code safely.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public static AxisResult Try(Action action);
public static AxisResult<T> Try<T>(Func<T> func);
public static Task<AxisResult> TryAsync(Func<Task> func);`}</code></pre>
                        </div>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Core Operations</h2>

                    <div className="space-y-4">
                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.Then() / .ThenAsync()</h4>
                            <p className="text-slate-400 text-sm mb-2">Chain operations in a monadic pipeline. Skips execution if already failed.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public AxisResult Then(Func<AxisResult> func);
public Task<AxisResult> ThenAsync(Func<Task<AxisResult>> func);
public AxisResult<TOut> Then<TOut>(Func<AxisResult<TOut>> func);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.Map() / .MapAsync()</h4>
                            <p className="text-slate-400 text-sm mb-2">Transform the success value without changing the result type.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public AxisResult<TOut> Map<TOut>(Func<T, TOut> func);
public Task<AxisResult<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> func);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.Tap() / .TapAsync()</h4>
                            <p className="text-slate-400 text-sm mb-2">Execute side effects without breaking the pipeline.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public AxisResult<T> Tap(Action<T> action);
public Task<AxisResult<T>> TapAsync(Func<T, Task> func);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.Ensure() / .EnsureAsync()</h4>
                            <p className="text-slate-400 text-sm mb-2">Add validation to the pipeline. Switches to failure if condition fails.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public AxisResult<T> Ensure(Predicate<T> predicate, AxisError error);
public Task<AxisResult<T>> EnsureAsync(
    Func<T, Task<bool>> predicate, 
    Func<AxisError> errorFactory);`}</code></pre>
                        </div>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Error Handling</h2>

                    <div className="space-y-4">
                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.Recover() / .RecoverAsync()</h4>
                            <p className="text-slate-400 text-sm mb-2">Recover from failure with a default value or operation.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public AxisResult<T> Recover(T defaultValue);
public Task<AxisResult<T>> RecoverAsync(Func<AxisError, Task<T>> func);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.RecoverWhen()</h4>
                            <p className="text-slate-400 text-sm mb-2">Conditionally recover based on error type.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public AxisResult<T> RecoverWhen(
    Predicate<AxisError> condition, 
    AxisResult<T> fallback);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">IsSuccess / IsFailure</h4>
                            <p className="text-slate-400 text-sm mb-2">Check result state.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public bool IsSuccess { get; }
public bool IsFailure { get; }`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">Value / Error</h4>
                            <p className="text-slate-400 text-sm mb-2">Access result data (throws if accessing wrong state without checking).</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public T Value { get; }          // Throws if IsFailure
public AxisError Error { get; }  // Throws if IsSuccess`}</code></pre>
                        </div>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Composition & Aggregation</h2>

                    <div className="space-y-4">
                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.Zip() / .ZipAsync()</h4>
                            <p className="text-slate-400 text-sm mb-2">Combine independent operations (up to 4 results).</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public AxisResult<(T1, T2)> Zip<T1, T2>(
    AxisResult<T1> result1, 
    AxisResult<T2> result2);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.Combine()</h4>
                            <p className="text-slate-400 text-sm mb-2">Combine multiple results sequentially.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public static AxisResult Combine(
    params AxisResult[] results);`}</code></pre>
                        </div>

                        <div className="bg-slate-900 p-4 rounded-lg border border-slate-800">
                            <h4 className="font-mono text-blue-300 font-bold mb-2">.All()</h4>
                            <p className="text-slate-400 text-sm mb-2">Run operations in parallel and aggregate results.</p>
                            <pre className="text-xs overflow-x-auto"><code>{`public static Task<AxisResult<T[]>> All<T>(
    params Task<AxisResult<T>>[] tasks);`}</code></pre>
                        </div>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Error Types</h2>

                    <p className="text-slate-300 mb-4">
                        AxisResult provides 12 typed error categories for precise error classification:
                    </p>

                    <div className="bg-slate-900 p-4 rounded-lg border border-slate-800 mb-4">
                        <p className="font-mono text-blue-300 mb-2">
                            public enum AxisErrorType
                        </p>
                        <pre className="text-sm text-slate-300">{`{
    NotFound,           // Resource doesn't exist
    BusinessRule,       // Violated business logic
    Validation,         // Input validation failed
    Conflict,           // Resource already exists
    Unauthorized,       // Authentication failed
    Forbidden,          // User not authorized for resource
    Transient,          // Temporary failure, retry viable
    Internal,           // Unexpected system error
    // ... and more
}`}</pre>
                    </div>
                </section>

                <section className="bg-blue-900/20 p-6 rounded-lg border border-blue-800">
                    <h2 className="text-2xl font-bold text-blue-300 mb-4">Need More Details?</h2>
                    <p className="text-slate-300 mb-4">
                        For comprehensive documentation, method signatures, and advanced patterns:
                    </p>
                    <div className="flex gap-4">
                        <a href="https://github.com/fillippeprata/axisresult" className="text-blue-400 hover:text-blue-300 font-medium">
                            → GitHub Repository
                        </a>
                        <a href="https://www.nuget.org/packages/AxisResult" className="text-blue-400 hover:text-blue-300 font-medium">
                            → NuGet Package
                        </a>
                    </div>
                </section>
            </div>

            <div className="mt-12 pt-8 border-t border-slate-800">
                <a href="/AxisResult/" className="text-blue-400 hover:text-blue-300 font-medium text-lg">
                    ← Back to Home
                </a>
            </div>
        </Layout>
    );
};
