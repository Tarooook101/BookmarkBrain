/**
 * BookMarkBrain - Main JavaScript
 * 
 * This file contains the core JavaScript functionality for the BookMarkBrain application
 * including sidebar toggle, dark mode switcher, notifications, and other UI interactions.
 */

document.addEventListener('DOMContentLoaded', function () {
    // Initialize all components
    initSidebar();
    initDarkMode();
    initNotifications();
    initBackToTop();
    initDropdowns();
    initAlertDismiss();
    initCategoryTree();
});

/**
 * Sidebar functionality
 * - Toggle sidebar collapse
 * - Mobile sidebar toggle
 */
function initSidebar() {
    // Sidebar collapse toggle
    const sidebarCollapseBtn = document.querySelector('.sidebar-collapse-btn');
    if (sidebarCollapseBtn) {
        sidebarCollapseBtn.addEventListener('click', function () {
            document.body.classList.toggle('sidebar-collapsed');
            localStorage.setItem('sidebar-collapsed', document.body.classList.contains('sidebar-collapsed'));
        });

        // Check for stored preference
        if (localStorage.getItem('sidebar-collapsed') === 'true') {
            document.body.classList.add('sidebar-collapsed');
        }
    }

    // Mobile sidebar toggle
    const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
    if (mobileMenuBtn) {
        mobileMenuBtn.addEventListener('click', function () {
            document.body.classList.toggle('sidebar-open');

            // Create backdrop if it doesn't exist
            let backdrop = document.querySelector('.sidebar-backdrop');
            if (!backdrop) {
                backdrop = document.createElement('div');
                backdrop.className = 'sidebar-backdrop';
                document.body.appendChild(backdrop);

                backdrop.addEventListener('click', function () {
                    document.body.classList.remove('sidebar-open');
                });
            }
        });
    }

    // Add hover tooltips for collapsed sidebar
    const navLinks = document.querySelectorAll('.sidebar .nav-link');
    navLinks.forEach(link => {
        const text = link.querySelector('span')?.textContent.trim();
        if (text) {
            link.setAttribute('data-title', text);
        }
    });

    const categoryItems = document.querySelectorAll('.category-item');
    categoryItems.forEach(item => {
        const name = item.querySelector('.category-name')?.textContent.trim();
        if (name) {
            item.setAttribute('data-title', name);
        }
    });
}

/**
 * Dark mode toggle functionality
 */
function initDarkMode() {
    const darkModeToggle = document.querySelector('.dark-mode-toggle');
    if (darkModeToggle) {
        darkModeToggle.addEventListener('click', function () {
            document.body.classList.toggle('dark-mode');
            const isDarkMode = document.body.classList.contains('dark-mode');
            localStorage.setItem('dark-mode', isDarkMode);

            // Update toggle text/icon if needed
            const toggleIcon = darkModeToggle.querySelector('i');
            if (toggleIcon) {
                if (isDarkMode) {
                    toggleIcon.className = 'fas fa-sun'; // Change to sun icon in dark mode
                } else {
                    toggleIcon.className = 'fas fa-moon'; // Change to moon icon in light mode
                }
            }
        });

        // Check for stored preference
        if (localStorage.getItem('dark-mode') === 'true') {
            document.body.classList.add('dark-mode');
            const toggleIcon = darkModeToggle.querySelector('i');
            if (toggleIcon) {
                toggleIcon.className = 'fas fa-sun';
            }
        }
    }
}

/**
 * Notification functionality
 * - Dropdown toggle
 * - Mark as read
 */
function initNotifications() {
    const notificationToggle = document.querySelector('.notification-toggle');
    const notificationDropdown = document.querySelector('.notification-dropdown');

    if (notificationToggle && notificationDropdown) {
        notificationToggle.addEventListener('click', function (e) {
            e.preventDefault();
            notificationDropdown.classList.toggle('show');

            // Close when clicking outside
            document.addEventListener('click', function closeNotifications(e) {
                if (!notificationToggle.contains(e.target) && !notificationDropdown.contains(e.target)) {
                    notificationDropdown.classList.remove('show');
                    document.removeEventListener('click', closeNotifications);
                }
            });
        });

        // Mark all as read
        const markAllReadBtn = document.querySelector('.mark-all-read');
        if (markAllReadBtn) {
            markAllReadBtn.addEventListener('click', function (e) {
                e.preventDefault();
                const notifications = document.querySelectorAll('.notification-item:not(.read)');
                notifications.forEach(item => {
                    item.classList.add('read');
                });

                // Hide indicator
                const indicator = document.querySelector('.notification-indicator');
                if (indicator) {
                    indicator.style.display = 'none';
                }
            });
        }
    }
}

/**
 * Back to top button functionality
 */
function initBackToTop() {
    const backToTopBtn = document.querySelector('.back-to-top-btn');
    if (backToTopBtn) {
        window.addEventListener('scroll', function () {
            if (window.pageYOffset > 300) {
                backToTopBtn.classList.add('show');
            } else {
                backToTopBtn.classList.remove('show');
            }
        });

        backToTopBtn.addEventListener('click', function () {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
}

/**
 * Dropdown functionality
 */
function initDropdowns() {
    const dropdownToggles = document.querySelectorAll('[data-toggle="dropdown"]');
    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(toggle.getAttribute('data-target'));
            if (target) {
                target.classList.toggle('show');

                // Close when clicking outside
                document.addEventListener('click', function closeDropdown(e) {
                    if (!toggle.contains(e.target) && !target.contains(e.target)) {
                        target.classList.remove('show');
                        document.removeEventListener('click', closeDropdown);
                    }
                });
            }
        });
    });
}

/**
 * Alert dismiss functionality
 */
function initAlertDismiss() {
    const alertCloseButtons = document.querySelectorAll('.alert .close');
    alertCloseButtons.forEach(button => {
        button.addEventListener('click', function () {
            const alert = button.closest('.alert');
            if (alert) {
                alert.style.opacity = '0';
                setTimeout(() => {
                    alert.style.display = 'none';
                }, 300);
            }
        });
    });
}

/**
 * Category tree toggle functionality
 */
function initCategoryTree() {
    const categoryToggles = document.querySelectorAll('.category-toggle');
    categoryToggles.forEach(toggle => {
        toggle.addEventListener('click', function () {
            const isExpanded = toggle.getAttribute('aria-expanded') === 'true';
            toggle.setAttribute('aria-expanded', !isExpanded);

            // Find the nested categories container
            const categoryItem = toggle.closest('.category-item-wrapper');
            const nestedCategories = categoryItem.querySelector('.nested-categories');

            if (nestedCategories) {
                if (isExpanded) {
                    nestedCategories.style.height = '0';
                    setTimeout(() => {
                        nestedCategories.style.display = 'none';
                    }, 300);
                } else {
                    nestedCategories.style.display = 'block';
                    const height = nestedCategories.scrollHeight;
                    nestedCategories.style.height = height + 'px';
                    setTimeout(() => {
                        nestedCategories.style.height = 'auto';
                    }, 300);
                }
            }
        });
    });

    // Initially collapse all nested categories
    const allNestedCategories = document.querySelectorAll('.nested-categories');
    allNestedCategories.forEach(category => {
        if (!category.closest('.category-item-wrapper').querySelector('.category-toggle').getAttribute('aria-expanded') === 'true') {
            category.style.display = 'none';
            category.style.height = '0';
        }
    });
}