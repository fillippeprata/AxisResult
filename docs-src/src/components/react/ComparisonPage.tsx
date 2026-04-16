import React from 'react';
import { Layout } from './shared/Layout';

export const ComparisonPage: React.FC = () => {
    return (
        <Layout>
            <h1 className="text-4xl font-bold mb-8 text-blue-400">Library Comparison</h1>

            <div className="prose prose-invert max-w-6xl">
                <section className="mb-12">
                    <p className="text-slate-300 mb-8">
                        There are other Result libraries for C#. Here's why AxisResult is different:
                    </p>

                    <div className="space-y-6">
                        <div className="bg-slate-900 p-6 rounded-lg border border-slate-800">
                            <h3 className="text-xl font-bold text-blue-400 mb-3">vs. FluentResults</h3>
                            <p className="text-slate-400 mb-2">
                                FluentResults is popular but limited. It lacks monadic composition — you can't chain async operations without manual unwrapping. There's no <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">ValueTask</code> support, no tuple composition, no recovery patterns, and no typed error categories. It's a container, not a railway.
                            </p>
                        </div>

                        <div className="bg-slate-900 p-6 rounded-lg border border-slate-800">
                            <h3 className="text-xl font-bold text-blue-400 mb-3">vs. ErrorOr</h3>
                            <p className="text-slate-400 mb-2">
                                ErrorOr offers basic chaining but misses the depth needed for production systems. No <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">ValueTask</code> variants, no <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Zip</code> for combining independent operations, no <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Recover</code>/<code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">RecoverWhen</code> for conditional fallbacks, no <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">RequireNotFound</code> for idempotent creation patterns.
                            </p>
                        </div>

                        <div className="bg-slate-900 p-6 rounded-lg border border-slate-800">
                            <h3 className="text-xl font-bold text-blue-400 mb-3">vs. LanguageExt</h3>
                            <p className="text-slate-400 mb-2">
                                LanguageExt is a 7.5MB functional programming framework. If you only need Result types, you're pulling in immutable collections, State monads, Reader monads, and an entirely non-idiomatic API. AxisResult gives you the composition power without the weight or the learning curve.
                            </p>
                        </div>

                        <div className="bg-slate-900 p-6 rounded-lg border border-slate-800">
                            <h3 className="text-xl font-bold text-blue-400 mb-3">vs. CSharpFunctionalExtensions</h3>
                            <p className="text-slate-400 mb-2">
                                Solid library, but no <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">ValueTask</code> support, no <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Zip</code>, no typed error categories, no recovery patterns, no parallel aggregation. It's good for basic Result patterns but falls short in complex domain scenarios.
                            </p>
                        </div>

                        <div className="bg-slate-900 p-6 rounded-lg border border-slate-800">
                            <h3 className="text-xl font-bold text-blue-400 mb-3">vs. Ardalis.Result</h3>
                            <p className="text-slate-400 mb-2">
                                Designed for ASP.NET controllers, not for domain logic. Basic <code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Map</code>/<code className="bg-slate-800 px-1 py-0.5 rounded text-blue-300">Bind</code> support, no composition depth, no async variants, no recovery. Great for HTTP response mapping, limited everywhere else.
                            </p>
                        </div>
                    </div>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-6">Feature Comparison Table</h2>

                    <div className="overflow-x-auto">
                        <table className="w-full border-collapse text-sm">
                            <thead>
                                <tr className="bg-slate-800 border border-slate-700">
                                    <th className="text-left p-3 font-bold text-blue-300">Feature</th>
                                    <th className="text-center p-3 font-bold">
                                        <span className="bg-blue-900/50 px-3 py-1 rounded text-blue-300">AxisResult</span>
                                    </th>
                                    <th className="text-center p-3 font-bold text-slate-300">FluentResults</th>
                                    <th className="text-center p-3 font-bold text-slate-300">ErrorOr</th>
                                    <th className="text-center p-3 font-bold text-slate-300">LanguageExt</th>
                                    <th className="text-center p-3 font-bold text-slate-300">CSharpFunctExt</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-700">
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Monadic composition (Then/Map)</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-yellow-400">~</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Task + ValueTask async</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Tuple composition (Zip)</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">Up to 4</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Conditional recovery (Recover/RecoverWhen)</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Typed error categories</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">12 types</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-yellow-400">~</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Transient error detection</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">RequireNotFound pattern</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">LINQ query syntax</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-yellow-400">~</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Parallel aggregation (Combine/All)</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Zero external dependencies</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-yellow-400">~</span></td>
                                    <td className="text-center p-3"><span className="text-yellow-400">~</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Lightweight (~240KB)</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗ (7.5MB)</span></td>
                                    <td className="text-center p-3"><span className="text-green-400">✓</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">CancellationToken-aware overloads</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-yellow-400">~</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                                <tr className="bg-slate-900/50 hover:bg-slate-900 transition">
                                    <td className="p-3 text-slate-300">Parallel zip (independent ops)</td>
                                    <td className="text-center p-3"><span className="text-green-400 font-bold">✓</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                    <td className="text-center p-3"><span className="text-yellow-400">~</span></td>
                                    <td className="text-center p-3"><span className="text-red-400">✗</span></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div className="mt-4 text-slate-400 text-sm">
                        <p>Legend: <span className="text-green-400 font-bold">✓</span> = Fully supported | <span className="text-yellow-400 font-bold">~</span> = Partial support | <span className="text-red-400 font-bold">✗</span> = Not supported</p>
                    </div>
                </section>

                <section className="bg-blue-900/20 p-6 rounded-lg border border-blue-800">
                    <h2 className="text-2xl font-bold text-blue-300 mb-4">The Verdict</h2>
                    <p className="text-slate-300 mb-4">
                        AxisResult is the only C# library that combines:
                    </p>
                    <ul className="text-slate-300 space-y-2">
                        <li className="flex gap-3">
                            <span className="text-blue-400 font-bold">→</span>
                            <span><strong>Complete monadic composition</strong> with full async/await support</span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-blue-400 font-bold">→</span>
                            <span><strong>Zero dependencies</strong> and minimal footprint</span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-blue-400 font-bold">→</span>
                            <span><strong>Idiomatic C#</strong> API designed for real-world codebases</span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-blue-400 font-bold">→</span>
                            <span><strong>Production-ready features</strong> for complex domain logic</span>
                        </li>
                    </ul>
                </section>
            </div>

            <div className="mt-12 pt-8 border-t border-slate-800 space-y-4">
                <a href="/AxisResult/docs/api-reference/" className="block text-blue-400 hover:text-blue-300 font-medium text-lg">
                    ← Next: API Reference
                </a>
            </div>
        </Layout>
    );
};
