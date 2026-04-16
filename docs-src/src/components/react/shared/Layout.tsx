import React from 'react';

interface LayoutProps {
    children: React.ReactNode;
}

interface NavItem {
    label: string;
    href: string;
}

const navItems: NavItem[] = [
    { label: 'Home', href: '/AxisResult/' },
    { label: 'Why AxisResult?', href: '/AxisResult/docs/why-axisresult/' },
    { label: 'Concepts', href: '/AxisResult/docs/concepts/' },
    { label: 'Getting Started', href: '/AxisResult/docs/getting-started/' },
    { label: 'Comparison', href: '/AxisResult/docs/comparison/' },
    { label: 'API Reference', href: '/AxisResult/docs/api-reference/' },
];

export const Layout: React.FC<LayoutProps> = ({ children }) => {
    const currentPath = typeof window !== 'undefined' ? window.location.pathname : '/AxisResult/';

    const isActive = (href: string) => {
        if (href === '/AxisResult/') return currentPath === '/AxisResult/' || currentPath === '/';
        return currentPath.startsWith(href);
    };

    return (
        <div className="flex flex-col min-h-screen">
            {/* Navbar */}
            <nav className="bg-slate-950 border-b border-blue-900/30 sticky top-0 z-50 backdrop-blur-md">
                <div className="px-6 py-3">
                    <div className="flex items-center justify-between">
                        <a href="/AxisResult/" className="flex items-center gap-2 group">
                            <span className="text-2xl font-bold text-blue-400 group-hover:text-blue-300 transition">
                                AxisResult
                            </span>
                        </a>
                        <div className="flex items-center gap-4">
                            <a
                                href="https://github.com/fillippeprata/axisresult"
                                className="text-slate-400 hover:text-blue-400 transition"
                                title="GitHub"
                            >
                                <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                                    <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v 3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z" />
                                </svg>
                            </a>
                            <a
                                href="https://www.nuget.org/packages/AxisResult"
                                className="text-slate-400 hover:text-blue-400 transition"
                                title="NuGet"
                            >
                                <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                                    <path d="M7.5 0C3.36 0 0 3.36 0 7.5v9C0 20.64 3.36 24 7.5 24h9c4.14 0 7.5-3.36 7.5-7.5v-9C24 3.36 20.64 0 16.5 0h-9zm2.1 4.2h4.8v4.8h-4.8V4.2zm6.9 0h4.8v4.8h-4.8V4.2zm-6.9 6.9h4.8v4.8h-4.8v-4.8zm6.9 0h4.8v4.8h-4.8v-4.8z" />
                                </svg>
                            </a>
                        </div>
                    </div>
                </div>
            </nav>

            {/* Main Content with Sidebar */}
            <div className="flex flex-1">
                {/* Sidebar */}
                <aside className="w-64 bg-slate-900/50 border-r border-slate-800 p-6 hidden md:block sticky top-16 h-[calc(100vh-4rem)]">
                    <h3 className="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4">Documentation</h3>
                    <nav className="space-y-2">
                        {navItems.map(item => (
                            <a
                                key={item.href}
                                href={item.href}
                                className={`block px-4 py-2 rounded-lg transition-colors ${isActive(item.href)
                                    ? 'bg-blue-900/30 text-blue-400 border-l-2 border-blue-400'
                                    : 'text-slate-300 hover:text-slate-100 hover:bg-slate-800/50'
                                    }`}
                            >
                                {item.label}
                            </a>
                        ))}
                    </nav>
                </aside>

                {/* Main Content */}
                <main className="flex-1">
                    <div className="max-w-4xl mx-auto px-6 py-8">
                        {children}
                    </div>
                </main>
            </div>

            {/* Footer */}
            <footer className="bg-slate-950 border-t border-slate-800 mt-16">
                <div className="px-6 py-8">
                    <div className="flex flex-col md:flex-row justify-between items-center text-slate-400 text-sm gap-4">
                        <p>&copy; 2026 AxisResult. Zero-dependency Result monad for C#.</p>
                        <div className="flex gap-6">
                            <a href="https://github.com/fillippeprata/axisresult" className="hover:text-slate-200 transition">
                                GitHub Repository
                            </a>
                            <a href="https://www.nuget.org/packages/AxisResult" className="hover:text-slate-200 transition">
                                NuGet Package
                            </a>
                        </div>
                    </div>
                </div>
            </footer>
        </div>
    );
};
