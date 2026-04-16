import React from 'react';

interface PageHeaderProps {
    title: string;
    subtitle?: string;
    description?: string;
}

export const PageHeader: React.FC<PageHeaderProps> = ({ title, subtitle, description }) => {
    return (
        <div className="mb-16">
            <h1 className="text-5xl font-bold mb-6 text-blue-400">{title}</h1>
            {subtitle && (
                <p className="text-2xl text-slate-300 mb-8">{subtitle}</p>
            )}
            {description && (
                <p className="text-slate-400 text-lg mb-8 max-w-2xl">{description}</p>
            )}
        </div>
    );
};
