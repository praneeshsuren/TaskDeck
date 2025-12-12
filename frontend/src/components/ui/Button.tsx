import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: 'primary' | 'secondary' | 'ghost' | 'danger';
    size?: 'sm' | 'md' | 'lg';
    loading?: boolean;
    children: React.ReactNode;
}

export function cn(...inputs: ClassValue[]) {
    return twMerge(clsx(inputs));
}

export function Button({
    variant = 'primary',
    size = 'md',
    loading = false,
    disabled,
    className,
    children,
    ...props
}: ButtonProps) {
    const baseStyles =
        'inline-flex items-center justify-center font-medium rounded-lg transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed';

    const variants = {
        primary:
            'bg-primary-600 hover:bg-primary-700 text-white focus:ring-primary-500',
        secondary:
            'bg-gray-200 hover:bg-gray-300 dark:bg-dark-card dark:hover:bg-dark-border text-gray-800 dark:text-white focus:ring-gray-500',
        ghost:
            'bg-transparent hover:bg-gray-100 dark:hover:bg-dark-border text-gray-700 dark:text-gray-300 focus:ring-gray-500',
        danger:
            'bg-red-600 hover:bg-red-700 text-white focus:ring-red-500',
    };

    const sizes = {
        sm: 'px-3 py-1.5 text-sm',
        md: 'px-4 py-2 text-sm',
        lg: 'px-6 py-3 text-base',
    };

    return (
        <button
            className={cn(baseStyles, variants[variant], sizes[size], className)}
            disabled={disabled || loading}
            {...props}
        >
            {loading && (
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-current mr-2" />
            )}
            {children}
        </button>
    );
}
