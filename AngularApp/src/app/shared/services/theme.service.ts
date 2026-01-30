import { Injectable, signal } from '@angular/core';

export type Theme = 'dark' | 'light' | 'emerald';

@Injectable({
    providedIn: 'root'
})
export class ThemeService {
    private readonly THEME_KEY = 'app-theme';
    private themes: Theme[] = ['dark', 'light', 'emerald'];

    theme = signal<Theme>('dark');

    constructor() {
        const savedTheme = localStorage.getItem(this.THEME_KEY) as Theme;
        if (savedTheme && this.themes.includes(savedTheme)) {
            this.setTheme(savedTheme);
        } else {
            this.setTheme('dark');
        }
    }

    toggleTheme() {
        const currentIndex = this.themes.indexOf(this.theme());
        const nextIndex = (currentIndex + 1) % this.themes.length;
        this.setTheme(this.themes[nextIndex]);
    }

    setTheme(theme: Theme) {
        this.theme.set(theme);
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(this.THEME_KEY, theme);

        // Manual Dark Mode Control for Tailwind
        if (theme === 'dark' || theme === 'emerald') {
            document.documentElement.classList.add('dark');
        } else {
            document.documentElement.classList.remove('dark');
        }
    }

    get isDarkMode() {
        return this.theme() === 'dark' || this.theme() === 'emerald';
    }
}
