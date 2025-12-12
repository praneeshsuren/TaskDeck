'use client';

import { TaskForm } from '@/components/TaskForm';

export default function NewTaskPage() {
    return (
        <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            <h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-8">
                Create New Task
            </h1>
            <div className="card p-6">
                <TaskForm />
            </div>
        </div>
    );
}
