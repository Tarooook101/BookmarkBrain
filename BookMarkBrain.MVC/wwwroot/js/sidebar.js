
document.addEventListener('DOMContentLoaded', function () {
    // Initialize sidebar components
    initCategoryTree();
    initSidebarResponsive();
    initCategoryActions();
    initHashtagCloud();
});

/**
 * Category tree management
 * - Expand/collapse categories
 * - Active state management
 */
function initCategoryTree() {
    // Category tree toggle
    const categoryToggles = document.querySelectorAll('.category-toggle');
    categoryToggles.forEach(toggle => {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const categoryItemWrapper = this.closest('.category-item-wrapper');
            const nestedCategories = categoryItemWrapper.querySelector('.nested-categories');
            const isExpanded = this.getAttribute('aria-expanded') === 'true';

            // Update the toggle state
            this.setAttribute('aria-expanded', !isExpanded);

            // Toggle icon rotation
            const icon = this.querySelector('i');
            if (icon) {
                icon.style.transform = isExpanded ? '' : 'rotate(90deg)';
            }

            // Toggle nested categories visibility with animation
            if (nestedCategories) {
                if (isExpanded) {
                    // Collapse
                    nestedCategories.style.maxHeight = nestedCategories.scrollHeight + 'px';
                    // Force reflow
                    nestedCategories.offsetHeight;
                    nestedCategories.style.maxHeight = '0';
                    nestedCategories.style.opacity = '0';

                    // After animation completes
                    setTimeout(() => {
                        if (this.getAttribute('aria-expanded') === 'false') {
                            nestedCategories.style.display = 'none';
                        }
                    }, 300);
                } else {
                    // Expand
                    nestedCategories.style.display = 'block';
                    nestedCategories.style.maxHeight = '0';
                    nestedCategories.style.opacity = '0';

                    // Force reflow
                    nestedCategories.offsetHeight;

                    nestedCategories.style.maxHeight = nestedCategories.scrollHeight + 'px';
                    nestedCategories.style.opacity = '1';

                    // After animation completes
                    setTimeout(() => {
                        if (this.getAttribute('aria-expanded') === 'true') {
                            nestedCategories.style.maxHeight = 'none'; // Allow natural height
                        }
                    }, 300);
                }

                // Store expanded state in localStorage
                const categoryId = categoryItemWrapper.getAttribute('data-category-id');
                if (categoryId) {
                    const expandedCategories = JSON.parse(localStorage.getItem('expanded-categories') || '[]');

                    if (!isExpanded) {
                        // Add to expanded list
                        if (!expandedCategories.includes(categoryId)) {
                            expandedCategories.push(categoryId);
                        }
                    } else {
                        // Remove from expanded list
                        const index = expandedCategories.indexOf(categoryId);
                        if (index > -1) {
                            expandedCategories.splice(index, 1);
                        }
                    }

                    localStorage.setItem('expanded-categories', JSON.stringify(expandedCategories));
                }
            }
        });
    });

    // Apply stored expanded states
    const expandedCategories = JSON.parse(localStorage.getItem('expanded-categories') || '[]');
    expandedCategories.forEach(categoryId => {
        const categoryWrapper = document.querySelector(`.category-item-wrapper[data-category-id="${categoryId}"]`);
        if (categoryWrapper) {
            const toggle = categoryWrapper.querySelector('.category-toggle');
            const nestedCategories = categoryWrapper.querySelector('.nested-categories');

            if (toggle && nestedCategories) {
                toggle.setAttribute('aria-expanded', 'true');
                const icon = toggle.querySelector('i');
                if (icon) {
                    icon.style.transform = 'rotate(90deg)';
                }

                nestedCategories.style.display = 'block';
                nestedCategories.style.maxHeight = 'none';
                nestedCategories.style.opacity = '1';
            }
        }
    });

    // Set active category based on URL
    const currentPath = window.location.pathname;
    const categoryLinks = document.querySelectorAll('.category-item');

    categoryLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href && currentPath.includes(href)) {
            link.classList.add('active');

            // Expand all parent categories
            let parentWrapper = link.closest('.nested-categories');
            while (parentWrapper) {
                const parentToggle = parentWrapper.previousElementSibling.querySelector('.category-toggle');
                if (parentToggle) {
                    parentToggle.setAttribute('aria-expanded', 'true');
                    const icon = parentToggle.querySelector('i');
                    if (icon) {
                        icon.style.transform = 'rotate(90deg)';
                    }
                }

                parentWrapper.style.display = 'block';
                parentWrapper.style.maxHeight = 'none';
                parentWrapper.style.opacity = '1';

                parentWrapper = parentWrapper.closest('.category-item-wrapper').parentElement.closest('.nested-categories');
            }
        }
    });
}

/**
 * Sidebar responsive behavior
 */
function initSidebarResponsive() {
    const sidebarToggleBtn = document.querySelector('.mobile-menu-btn');
    if (sidebarToggleBtn) {
        sidebarToggleBtn.addEventListener('click', function () {
            const sidebar = document.querySelector('.sidebar');
            if (sidebar) {
                sidebar.classList.toggle('show');

                // Create backdrop for mobile if it doesn't exist
                let backdrop = document.querySelector('.sidebar-backdrop');
                if (!backdrop) {
                    backdrop = document.createElement('div');
                    backdrop.className = 'sidebar-backdrop';
                    document.body.appendChild(backdrop);

                    backdrop.addEventListener('click', function () {
                        sidebar.classList.remove('show');
                        backdrop.style.opacity = '0';
                        setTimeout(() => {
                            backdrop.remove();
                        }, 300);
                    });
                }

                if (sidebar.classList.contains('show')) {
                    backdrop.style.display = 'block';
                    // Force reflow
                    backdrop.offsetHeight;
                    backdrop.style.opacity = '1';
                } else {
                    backdrop.style.opacity = '0';
                    setTimeout(() => {
                        backdrop.remove();
                    }, 300);
                }
            }
        });
    }

    // Close sidebar when window resizes to desktop
    window.addEventListener('resize', function () {
        if (window.innerWidth >= 992) {
            const sidebar = document.querySelector('.sidebar');
            const backdrop = document.querySelector('.sidebar-backdrop');

            if (sidebar) {
                sidebar.classList.remove('show');
            }

            if (backdrop) {
                backdrop.style.opacity = '0';
                setTimeout(() => {
                    backdrop.remove();
                }, 300);
            }
        }
    });
}

/**
 * Category actions (add, edit, delete)
 */
function initCategoryActions() {
    // Add category button
    const addCategoryBtn = document.querySelector('.add-category-btn');
    if (addCategoryBtn) {
        addCategoryBtn.addEventListener('click', function (e) {
            e.preventDefault();
            // You can replace this with your backend logic or modal trigger
            const addCategoryUrl = this.getAttribute('href') || '/Categories/Create';
            window.location.href = addCategoryUrl;
        });
    }

    // Quick add category (if applicable)
    const quickAddCategoryForm = document.querySelector('.quick-add-category-form');
    if (quickAddCategoryForm) {
        quickAddCategoryForm.addEventListener('submit', function (e) {
            e.preventDefault();

            // Get form data
            const formData = new FormData(this);

            // Send AJAX request to create category
            fetch(this.getAttribute('action'), {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Refresh category tree or add new category to DOM
                        if (data.categoryHtml) {
                            const categoriesContainer = document.querySelector('.categories-group');
                            if (categoriesContainer) {
                                categoriesContainer.innerHTML = data.categoryHtml;
                                initCategoryTree(); // Re-initialize tree
                            }
                        }

                        // Clear form
                        quickAddCategoryForm.reset();

                        // Show success message
                        showToast('Category created successfully', 'success');
                    } else {
                        showToast(data.message || 'Failed to create category', 'error');
                    }
                })
                .catch(error => {
                    console.error('Error creating category:', error);
                    showToast('An error occurred while creating the category', 'error');
                });
        });
    }
}

/**
 * Hashtag cloud interactions
 */
function initHashtagCloud() {
    const hashtagLinks = document.querySelectorAll('.hashtag-badge');
    hashtagLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            // You can add hashtag-specific behavior here
            const hashtag = this.textContent.trim();
            if (hashtag.startsWith('#')) {
                const hashtagValue = hashtag.substring(1);
                // Example: Set active state
                hashtagLinks.forEach(h => h.classList.remove('active'));
                this.classList.add('active');
            }
        });
    });
}

/**
 * Show toast notification
 * @param {string} message - The message to display
 * @param {string} type - The toast type (success, error, warning, info)
 */
function showToast(message, type = 'info') {
    // Create toast container if it doesn't exist
    let toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container';
        toastContainer.style.position = 'fixed';
        toastContainer.style.top = '20px';
        toastContainer.style.right = '20px';
        toastContainer.style.zIndex = '1050';
        document.body.appendChild(toastContainer);
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.style.opacity = '0';
    toast.style.transform = 'translateY(-20px)';
    toast.style.transition = 'opacity 0.3s ease, transform 0.3s ease';

    // Create toast content
    toast.innerHTML = `
        <div class="toast-header">
            <strong class="me-auto">${type.charAt(0).toUpperCase() + type.slice(1)}</strong>
            <button type="button" class="btn-close" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;

    // Add to container
    toastContainer.appendChild(toast);

    // Force reflow
    toast.offsetHeight;

    // Show toast
    toast.style.opacity = '1';
    toast.style.transform = 'translateY(0)';

    // Add close button handler
    const closeBtn = toast.querySelector('.btn-close');
    if (closeBtn) {
        closeBtn.addEventListener('click', function () {
            hideToast(toast);
        });
    }

    // Auto hide after 5 seconds
    setTimeout(() => {
        hideToast(toast);
    }, 5000);
}

/**
 * Hide toast notification
 * @param {HTMLElement} toast - The toast element to hide
 */
function hideToast(toast) {
    toast.style.opacity = '0';
    toast.style.transform = 'translateY(-20px)';

    setTimeout(() => {
        toast.remove();

        // Remove container if empty
        const toastContainer = document.querySelector('.toast-container');
        if (toastContainer && !toastContainer.hasChildNodes()) {
            toastContainer.remove();
        }
    }, 300);
}



// code of theme-switcher.js in folder js in wwwroot folder : 


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
