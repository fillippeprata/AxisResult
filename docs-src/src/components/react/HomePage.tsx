import React from 'react';
import { Layout } from './shared/Layout';
import { CodeBlock } from './shared/CodeBlock';
import { FeatureCard } from './shared/FeatureCard';

export const HomePage: React.FC = () => {
    const codeExample = `public Task<AxisResult<AddCellphoneResponse>> HandleAsync(AddCellphoneCommand cmd)
    => personFactory.GetByIdAsync(cmd.PersonId)
        .ThenAsync(person => cellphoneMediator.AddAsync(new()
        {
            CountryId = cmd.CountryId,
            Number = cmd.Number
        }))
        .ThenAsync(response => response.AddCellphoneAsync(cmd.CellphoneId))
        .ThenAsync(_ => unitOfWork.SaveChangesAsync())
        .MapAsync(_ => new AddCellphoneResponse { CellphoneId = cmd.CellphoneId });`;

    return (
        <Layout>
            {/* Hero Section */}
            <div className="mb-16">
                <h1 className="text-5xl font-bold mb-6 text-blue-400">AxisResult</h1>
                <p className="text-2xl text-slate-300 mb-8">
                    <span className="text-blue-300">Railway-Oriented Programming</span> for C# that actually works in production.
                </p>
                <p className="text-slate-400 text-lg mb-8 max-w-2xl">
                    A zero-dependency Result monad built for real-world .NET applications. No exceptions for control flow. No <code className="bg-slate-800 px-2 py-1 rounded text-blue-300">null</code> checks scattered everywhere. No <code className="bg-slate-800 px-2 py-1 rounded text-blue-300">try/catch</code> in your business logic. Just clean, composable pipelines that make your intent crystal clear.
                </p>

                {/* CTA Buttons */}
                <div className="flex gap-4">
                    <a href="/AxisResult/docs/why-axisresult/" className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded transition">
                        Learn Why →
                    </a>
                    <a href="/AxisResult/docs/getting-started/" className="bg-slate-700 hover:bg-slate-600 text-white font-bold py-3 px-6 rounded transition">
                        Get Started
                    </a>
                </div>
            </div>

            {/* Code Example */}
            <CodeBlock code={codeExample} title="Clean, Composable Pipelines" />
            <p className="text-slate-400 text-sm mb-16">
                Every operation either succeeds and flows forward, or fails and short-circuits. No nesting. No branching. No ambiguity.
            </p>

            {/* Key Features */}
            <div className="mb-16">
                <h2 className="text-3xl font-bold text-blue-300 mb-8">Why AxisResult?</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <FeatureCard
                        title="Zero Dependencies"
                        description="Lightweight, focused library with no external dependencies. Just ~240KB of pure C#."
                    />
                    <FeatureCard
                        title="Async-First"
                        description="Full Task and ValueTask support for modern async/await patterns without callback hell."
                    />
                    <FeatureCard
                        title="Typed Errors"
                        description="12 typed error categories for precise error handling. Errors are part of your type system."
                    />
                    <FeatureCard
                        title="Composable"
                        description="Zip operations, recovery patterns, parallel aggregation. Build complex flows from simple pieces."
                    />
                </div>
            </div>

            {/* Quick Stats */}
            <div className="mb-16 bg-gradient-to-r from-blue-900/50 to-slate-800 p-8 rounded-lg border border-slate-700">
                <div className="grid grid-cols-4 gap-4 text-center">
                    <div>
                        <p className="text-3xl font-bold text-blue-400">12</p>
                        <p className="text-slate-400">Error Types</p>
                    </div>
                    <div>
                        <p className="text-3xl font-bold text-blue-400">0</p>
                        <p className="text-slate-400">Dependencies</p>
                    </div>
                    <div>
                        <p className="text-3xl font-bold text-blue-400">∞</p>
                        <p className="text-slate-400">Composability</p>
                    </div>
                    <div>
                        <p className="text-3xl font-bold text-blue-400">5+</p>
                        <p className="text-slate-400">Comparison Libs</p>
                    </div>
                </div>
            </div>

            {/* Next Steps */}
            <div className="bg-slate-900 p-8 rounded-lg border border-slate-800">
                <h2 className="text-2xl font-bold text-blue-300 mb-4">Ready to explore?</h2>
                <div className="flex gap-4">
                    <a href="/AxisResult/docs/concepts/" className="text-blue-400 hover:text-blue-300 font-medium">
                        Understand Railway-Oriented Programming →
                    </a>
                </div>
                <div className="flex gap-4 mt-3">
                    <a href="/AxisResult/docs/why-axisresult/" className="text-blue-400 hover:text-blue-300 font-medium">
                        See why it's different →
                    </a>
                </div>
            </div>
        </Layout>
    );
};
