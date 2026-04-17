import React from 'react';

interface FeatureCardProps {
    title: string;
    description: string;
}

export const FeatureCard: React.FC<FeatureCardProps> = ({ title, description }) => {
    return (
        <div className="bg-slate-900 p-6 rounded-lg border border-slate-800">
            <h4 className="text-lg font-bold text-blue-400 mb-3">{title}</h4>
            <p className="text-slate-400">{description}</p>
        </div>
    );
};
