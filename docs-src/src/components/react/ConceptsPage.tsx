import React from 'react';
import { Layout } from './shared/Layout';
import { CodeBlock } from './shared/CodeBlock';

export const ConceptsPage: React.FC = () => {
    const railwayDiagram = `Success ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━▶  Result
              ┃              ┃              ┃              ┃
           Validate       GetUser      CreateOrder      Save
              ┃              ┃              ┃              ┃
Failure ━━━━━━╋━━━━━━━━━━━━━━╋━━━━━━━━━━━━━━╋━━━━━━━━━━━━━━╋━▶  Errors`;

    return (
        <Layout>
            <h1 className="text-4xl font-bold mb-8 text-blue-400">Railway-Oriented Programming</h1>

            <div className="prose prose-invert max-w-4xl">
                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">What is Railway-Oriented Programming?</h2>

                    <p className="text-slate-300 mb-6">
                        Imagine your code as a railway track with two rails:
                    </p>

                    <div className="bg-slate-900 p-6 rounded-lg border border-slate-800 mb-6 font-mono text-sm overflow-x-auto">
                        <pre>{railwayDiagram}</pre>
                    </div>

                    <h3 className="text-2xl font-bold text-blue-300 mb-4">How It Works</h3>

                    <div className="space-y-6 text-slate-300">
                        <div className="bg-slate-900 p-4 rounded border border-blue-800">
                            <h4 className="font-bold text-blue-400 mb-2">On the Success Rail</h4>
                            <p>Data flows from one operation to the next. Each operation transforms or validates the data, then passes it forward.</p>
                        </div>

                        <div className="bg-slate-900 p-4 rounded border border-red-800">
                            <h4 className="font-bold text-blue-400 mb-2">On the Failure Rail</h4>
                            <p>Errors propagate automatically. The moment any operation fails, all subsequent operations are <strong>skipped</strong> — no <code className="bg-slate-800 px-1 py-0.5 rounded">if (result.IsFailure) return result;</code> boilerplate.</p>
                        </div>

                        <div className="bg-slate-900 p-4 rounded border border-yellow-800">
                            <h4 className="font-bold text-blue-400 mb-2">The Magic: Railway Switches</h4>
                            <p>Operations like <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">ThenAsync</code>, <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">MapAsync</code>, and <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">EnsureAsync</code> are railway switches (the <code className="bg-slate-800 px-1 py-0.5 rounded">┃</code> points). They only execute when on the success rail. If you're already on the failure rail, they let you pass through untouched.</p>
                        </div>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">The Historical Context</h2>

                    <p className="text-slate-300 mb-6">
                        This isn't a new idea — it comes from functional programming:
                    </p>

                    <ul className="text-slate-300 space-y-3 mb-6">
                        <li className="flex gap-3">
                            <span className="text-blue-400 font-bold">→</span>
                            <span><strong className="text-blue-300">Haskell</strong> has <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Either</code></span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-blue-400 font-bold">→</span>
                            <span><strong className="text-blue-300">F#</strong> has <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Result</code></span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-blue-400 font-bold">→</span>
                            <span><strong className="text-blue-300">Rust</strong> has <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Result&lt;T, E&gt;</code></span>
                        </li>
                    </ul>

                    <p className="text-slate-300 mb-6 text-lg font-semibold text-blue-300">
                        AxisResult is the first C# library that implements it <strong>completely</strong>, with full async support, zero dependencies, and APIs designed for how C# developers actually write code.
                    </p>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Core Operations</h2>

                    <div className="space-y-4">
                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2"><code className="text-blue-300">.Then() / .ThenAsync()</code></h4>
                            <p className="text-slate-300">Chain operations. If current result is success, execute the next operation. If failure, skip and pass error forward.</p>
                        </div>

                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2"><code className="text-blue-300">.Map() / .MapAsync()</code></h4>
                            <p className="text-slate-300">Transform the success value without changing the result type. Pure transformation pipeline.</p>
                        </div>

                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2"><code className="text-blue-300">.Ensure() / .EnsureAsync()</code></h4>
                            <p className="text-slate-300">Add validation. If condition fails, switch to failure rail with specified error.</p>
                        </div>

                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2"><code className="text-blue-300">.Tap() / .TapAsync()</code></h4>
                            <p className="text-slate-300">Side effects without breaking the pipeline. Useful for logging, metrics, notifications.</p>
                        </div>

                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2"><code className="text-blue-300">.Recover() / .RecoverWhen()</code></h4>
                            <p className="text-slate-300">Conditional fallback from failure rail back to success. For retry logic or graceful degradation.</p>
                        </div>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Why This Matters</h2>

                    <div className="bg-blue-900/20 p-6 rounded-lg border border-blue-800 text-slate-300">
                        <ul className="space-y-3">
                            <li className="flex gap-3">
                                <span className="text-green-400 font-bold">✓</span>
                                <span><strong>Readability:</strong> Your code reads like a description of business intent.</span>
                            </li>
                            <li className="flex gap-3">
                                <span className="text-green-400 font-bold">✓</span>
                                <span><strong>Correctness:</strong> Errors cannot be forgotten. Every path is explicit.</span>
                            </li>
                            <li className="flex gap-3">
                                <span className="text-green-400 font-bold">✓</span>
                                <span><strong>Composability:</strong> Build complex flows from simple, reusable pieces.</span>
                            </li>
                            <li className="flex gap-3">
                                <span className="text-green-400 font-bold">✓</span>
                                <span><strong>Performance:</strong> No overhead. No allocations on the happy path.</span>
                            </li>
                        </ul>
                    </div>
                </section>
            </div>

            <div className="mt-12 pt-8 border-t border-slate-800 space-y-4">
                <a href="/AxisResult/docs/getting-started/" className="block text-blue-400 hover:text-blue-300 font-medium text-lg">
                    ← Next: Getting Started
                </a>
            </div>
        </Layout>
    );
};
