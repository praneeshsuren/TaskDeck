/**
 * Format a date relative to now (e.g., "2 days ago", "in 3 hours")
 */
export function formatDistanceToNow(date: Date): string {
    const now = new Date();
    const diffMs = date.getTime() - now.getTime();
    const diffDays = Math.round(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.round(diffMs / (1000 * 60 * 60));
    const diffMinutes = Math.round(diffMs / (1000 * 60));

    if (Math.abs(diffDays) >= 1) {
        if (diffDays > 0) {
            return diffDays === 1 ? 'Tomorrow' : `In ${diffDays} days`;
        }
        return diffDays === -1 ? 'Yesterday' : `${Math.abs(diffDays)} days ago`;
    }

    if (Math.abs(diffHours) >= 1) {
        if (diffHours > 0) {
            return diffHours === 1 ? 'In 1 hour' : `In ${diffHours} hours`;
        }
        return diffHours === -1 ? '1 hour ago' : `${Math.abs(diffHours)} hours ago`;
    }

    if (Math.abs(diffMinutes) >= 1) {
        if (diffMinutes > 0) {
            return `In ${diffMinutes} minutes`;
        }
        return `${Math.abs(diffMinutes)} minutes ago`;
    }

    return 'Just now';
}

/**
 * Format a date to a readable string
 */
export function formatDate(date: Date | string, options?: Intl.DateTimeFormatOptions): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
        ...options,
    });
}

/**
 * Format a date to ISO string for input fields
 */
export function toISODateString(date: Date): string {
    return date.toISOString().split('T')[0];
}

/**
 * Check if a date is in the past
 */
export function isPastDate(date: Date | string): boolean {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d < new Date();
}

/**
 * Check if a date is today
 */
export function isToday(date: Date | string): boolean {
    const d = typeof date === 'string' ? new Date(date) : date;
    const today = new Date();
    return (
        d.getDate() === today.getDate() &&
        d.getMonth() === today.getMonth() &&
        d.getFullYear() === today.getFullYear()
    );
}
