class Utils {
    static init() {
        this.initDarkMode();
        this.renderNavbar();
        this.setupKeyboardShortcuts();
        this.setupToastContainer();
    }

    // --- DOM Elements ---
    static getElement(id) {
        return document.getElementById(id);
    }

    // --- Dark Mode ---
    static initDarkMode() {
        const isDark = localStorage.getItem('darkMode') === 'true';
        this.applyDarkMode(isDark);
    }

    static toggleDarkMode() {
        const isDark = localStorage.getItem('darkMode') === 'true';
        const newDark = !isDark;
        localStorage.setItem('darkMode', newDark);
        this.applyDarkMode(newDark);
    }

    static applyDarkMode(isDark) {
        document.documentElement.setAttribute('data-bs-theme', isDark ? 'dark' : 'light');
        const icon = this.getElement('darkModeIcon');
        if (icon) {
            icon.className = isDark ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
        }
    }

    // --- Navbar ---
    static renderNavbar() {
        const isLoginPage = window.location.pathname.endsWith('login.html');
        if (isLoginPage) return;

        const user = window.Api ? Api.decodeToken() : null;
        if (!user && window.location.pathname !== '/' && !window.location.pathname.endsWith('login.html')) {
            window.location.href = 'login.html';
            return;
        }

        const email = user ? (user.email || user.sub || 'User') : '';
        const currentPath = window.location.pathname;

        const navHtml = `
            <nav class="navbar navbar-expand-lg glass-nav sticky-top">
                <div class="container">
                    <a class="navbar-brand d-flex align-items-center fw-bold" href="categories.html">
                        <i class="bi bi-box-seam text-primary me-2"></i> LinkVault
                    </a>
                    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse" id="navbarNav">
                        <ul class="navbar-nav me-auto">
                            <li class="nav-item">
                                <a class="nav-link ${currentPath.includes('categories.html') ? 'active fw-bold' : ''}" href="categories.html">Categories</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link ${currentPath.includes('bookmarks.html') || currentPath.includes('bookmark.html') ? 'active fw-bold' : ''}" href="bookmarks.html">Bookmarks</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link ${currentPath.includes('notes.html') ? 'active fw-bold' : ''}" href="notes.html">Notes</a>
                            </li>
                        </ul>
                        <div class="d-flex align-items-center gap-3">
                            <span class="text-muted small"><i class="bi bi-person-circle me-1"></i>${email}</span>
                            <button class="btn btn-link text-body p-0" onclick="Utils.toggleDarkMode()" title="Toggle Dark Mode">
                                <i id="darkModeIcon" class="bi bi-moon-stars-fill fs-5"></i>
                            </button>
                            <button class="btn btn-outline-danger btn-sm" onclick="Utils.logout()">Logout</button>
                        </div>
                    </div>
                </div>
            </nav>
        `;

        // Insert at the beginning of body
        document.body.insertAdjacentHTML('afterbegin', navHtml);
        this.applyDarkMode(localStorage.getItem('darkMode') === 'true');
    }

    static logout() {
        if (window.Api) {
            Api.clearToken();
        }
        window.location.href = 'login.html';
    }

    // --- Toasts ---
    static setupToastContainer() {
        if (!this.getElement('toastContainer')) {
            const container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(container);
        }
    }

    static showToast(message, type = 'success') {
        const container = this.getElement('toastContainer');
        if (!container) return;

        const toastId = 'toast-' + Date.now();
        const iconInfo = type === 'success' ? 'bi-check-circle-fill text-success' : 'bi-exclamation-triangle-fill text-danger';
        
        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center border-0 mb-2 fade-in glass-card" role="alert" aria-live="assertive" aria-atomic="true" style="padding: 0; background: var(--surface-color)">
                <div class="d-flex">
                    <div class="toast-body d-flex align-items-center gap-2">
                        <i class="bi ${iconInfo} fs-5"></i>
                        <span>${message}</span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        container.insertAdjacentHTML('beforeend', toastHtml);
        const toastEl = this.getElement(toastId);
        
        // Use Bootstrap Toast API if available
        if (typeof bootstrap !== 'undefined') {
            const toast = new bootstrap.Toast(toastEl, { autohide: true, delay: 3000 });
            toast.show();
            toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
        } else {
            // Fallback
            setTimeout(() => {
                toastEl.classList.remove('show');
                setTimeout(() => toastEl.remove(), 300);
            }, 3000);
        }
    }

    // --- Loaders ---
    static showLoader() {
        if (this.getElement('mainLoader')) return;
        const loaderHtml = `
            <div id="mainLoader" class="spinner-overlay">
                <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
        `;
        document.body.insertAdjacentHTML('beforeend', loaderHtml);
    }

    static hideLoader() {
        const loader = this.getElement('mainLoader');
        if (loader) loader.remove();
    }

    // --- Empty States ---
    static getEmptyStateHtml(icon, message, actionText = '', actionTarget = '') {
        const btnHtml = actionText ? `<button class="btn btn-primary mt-3" data-bs-toggle="modal" data-bs-target="${actionTarget}">${actionText}</button>` : '';
        return `
            <div class="empty-state">
                <i class="bi ${icon}"></i>
                <h5 class="text-muted">${message}</h5>
                ${btnHtml}
            </div>
        `;
    }

    // --- Pagination ---
    static renderPagination(totalItems, currentPage, itemsPerPage, onPageChangeCallback) {
        const totalPages = Math.ceil(totalItems / itemsPerPage);
        if (totalPages <= 1) return '';

        let html = '<nav aria-label="Page navigation"><ul class="pagination justify-content-center">';
        
        // Prev button
        html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                    <a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a>
                 </li>`;
                 
        // Pages
        for (let i = 1; i <= totalPages; i++) {
            html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                        <a class="page-link" href="#" data-page="${i}">${i}</a>
                     </li>`;
        }

        // Next button
        html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                    <a class="page-link" href="#" data-page="${currentPage + 1}">Next</a>
                 </li>`;
                 
        html += '</ul></nav>';
        
        // Attach listener somehow... Best done by returning HTML and letting parent attach
        return html;
    }

    static attachPaginationListeners(containerId, onPageChangeCallback) {
        const container = this.getElement(containerId);
        if (!container) return;
        
        container.addEventListener('click', (e) => {
            if (e.target.tagName === 'A' && e.target.classList.contains('page-link')) {
                e.preventDefault();
                const page = parseInt(e.target.getAttribute('data-page'));
                if (!isNaN(page)) {
                    onPageChangeCallback(page);
                }
            }
        });
    }

    // --- Keyboard Shortcuts ---
    static setupKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Ignore if in input
            if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA') return;

            // 'n' to open the primary 'create' modal on the page
            if (e.key === 'n' || e.key === 'N') {
                const createBtn = document.querySelector('[data-shortcut="n"]');
                if (createBtn) {
                    e.preventDefault();
                    createBtn.click();
                }
            }
        });
    }
    
    // --- File Export ---
    static exportAsJson(data, filename) {
        const jsonStr = JSON.stringify(data, null, 2);
        const blob = new Blob([jsonStr], { type: "application/json" });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }
}

// Auto-init on load
document.addEventListener('DOMContentLoaded', () => {
    Utils.init();
});
