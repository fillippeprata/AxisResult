import React from 'react';

interface CodeBlockProps {
    code: string;
    language?: string;
    title?: string;
    highlight?: boolean;
}

export const CodeBlock: React.FC<CodeBlockProps> = ({ code, language = 'csharp', title, highlight = false }) => {
    return (
        <div className={`mb-6 rounded-lg border overflow-x-auto ${highlight ? 'bg-slate-900 border-blue-800' : 'bg-slate-900 border-slate-800'} p-6`}>
            {title && <h4 className="text-lg font-bold text-blue-300 mb-4">{title}</h4>}
            <pre className="text-sm overflow-x-auto">
                <code className="language-csharp text-slate-300">
                    {code}
                </code>
            </pre>
        </div>
    );
};
