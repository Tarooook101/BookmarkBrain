/**
 * BookMarkBrain - Theme Switcher
 * 
 * A dedicated module for handling theme switching (light/dark mode)
 * with proper state persistence and smooth transitions.
 */

class ThemeSwitcher {
    constructor() {
        this.darkModeClass = 'dark-mode';
        this.storageKey = 'bookmarkbrain-theme';
        this.defaultTheme = 'light'; // Start in light mode by default

        this.init();
    }

    init() {
        // Initialize theme from storage or system preference
        this.setInitialTheme();

        // Set up event listeners
        this.setupEventListeners();
    }

    setInitialTheme() {
        // First check localStorage
        const savedTheme = localStorage.getItem(this.storageKey);

        if (savedTheme) {
            // Use saved preference
            this.setTheme(savedTheme);
        } else {
            // Default to light mode as specified
            this.setTheme(this.defaultTheme);
        }
    }

    setupEventListeners() {
        // Find theme toggle buttons
        const toggleButtons = document.querySelectorAll('.theme-toggle, .dark-mode-toggle');

        toggleButtons.forEach(button => {
            button.addEventListener('click', () => {
                const currentTheme = this.getCurrentTheme();
                const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
                this.setTheme(newTheme);
            });
        });
    }

    getCurrentTheme() {
        return document.body.classList.contains(this.darkModeClass) ? 'dark' : 'light';
    }

    setTheme(theme) {
        if (theme === 'dark') {
            document.body.classList.add(this.darkModeClass);
            this.updateToggleIcons(true);
        } else {
            document.body.classList.remove(this.darkModeClass);
            this.updateToggleIcons(false);
        }

        // Save preference to localStorage
        localStorage.setItem(this.storageKey, theme);

        // Dispatch event for other components that might need to react
        document.dispatchEvent(new CustomEvent('themeChanged', {
            detail: { theme: theme }
        }));
    }

    updateToggleIcons(isDarkMode) {
        // Update toggle button icons
        const toggleButtons = document.querySelectorAll('.theme-toggle, .dark-mode-toggle');

        toggleButtons.forEach(button => {
            const icon = button.querySelector('i');
            if (icon) {
                if (isDarkMode) {
                    // In dark mode, show sun icon (to switch to light)
                    icon.className = icon.className.replace(/fa-moon|fa-dark-mode/g, 'fa-sun');
                } else {
                    // In light mode, show moon icon (to switch to dark)
                    icon.className = icon.className.replace(/fa-sun|fa-light-mode/g, 'fa-moon');
                }
            }

            // Update text if any
            const text = button.querySelector('span');
            if (text) {
                text.textContent = isDarkMode ? 'Light Mode' : 'Dark Mode';
            }
        });
    }

    // Method to toggle theme programmatically
    toggle() {
        const currentTheme = this.getCurrentTheme();
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        this.setTheme(newTheme);
    }
}

// Initialize when the DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // Create global instance
    window.themeSwitcher = new ThemeSwitcher();
});