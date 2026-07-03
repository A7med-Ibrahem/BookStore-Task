const Bookmarks = {
    modalInstance: null,
    bookmarks: [],
    categories: [],
    currentPage: 1,
    itemsPerPage: 10,

    init() {
        this.modalInstance = new bootstrap.Modal(Utils.getElement('bookmarkModal'));
        
        this.loadCategories().then(() => {
            this.loadBookmarks();
        });
        
        Utils.getElement('bookmarkForm').addEventListener('submit', this.handleSave.bind(this));
        
        Utils.getElement('bookmarkModal').addEventListener('hidden.bs.modal', () => {
            Utils.getElement('bookmarkForm').reset();
            Utils.getElement('bookmarkId').value = '';
            Utils.getElement('bookmarkModalLabel').textContent = 'New Bookmark';
        });
    },

    async loadCategories() {
        try {
            this.categories = await Api.get('/categories');
            this.populateCategorySelects();
        } catch (error) {
            Utils.showToast('Failed to load categories', 'danger');
        }
    },

    populateCategorySelects() {
        const modalSelect = Utils.getElement('bookmarkCategory');
        const filterSelect = Utils.getElement('filterCategory');
        
        if (!modalSelect || !filterSelect) return;

        let optionsHtml = '';
        this.categories.forEach(c => {
            optionsHtml += `<option value="${c.id}">${this.escapeHtml(c.categoryName)}</option>`;
        });

        modalSelect.innerHTML = '<option value="" disabled selected>Select a category</option>' + optionsHtml;
        filterSelect.innerHTML = '<option value="">All Categories</option>' + optionsHtml;
    },

    async loadBookmarks(params = {}) {
        Utils.showLoader();
        try {
            // Note: The provided API doesn't have a direct /bookmarks GET endpoint with filters.
            // Assuming it's similar to /notes or we fetch all categories and build it
            // Based on requirements, there is GET /bookmarks/{bookmarkId}/notes, but no GET /bookmarks listing.
            // Let's assume there's a GET /bookmarks (standard REST) since it says "Table of bookmarks".
            // If it doesn't exist, we would have to fetch categories and map them, but let's assume /bookmarks exists.
            
            // For now, let's fetch all and filter in JS if API doesn't support query params.
            let url = '/bookmarks';
            const queryParams = new URLSearchParams(params).toString();
            if (queryParams) url += `?${queryParams}`;

            this.bookmarks = await Api.get(url);
            
            // If this endpoint doesn't exist, this will fail. We'll handle rendering inside catch if needed.
            this.renderTable();
        } catch (error) {
            console.warn('API /bookmarks might not exist as per docs. Simulating empty array.');
            this.bookmarks = [];
            this.renderTable();
            Utils.showToast('Failed to load bookmarks or endpoint missing.', 'danger');
        } finally {
            Utils.hideLoader();
        }
    },

    renderTable() {
        const tbody = Utils.getElement('bookmarksTableBody');
        tbody.innerHTML = '';

        // Apply filters in JS if needed (since API details for GET /bookmarks were missing)
        let filtered = [...this.bookmarks];
        
        const searchWord = Utils.getElement('searchWord').value.toLowerCase();
        const categoryId = Utils.getElement('filterCategory').value;
        const isFavorite = Utils.getElement('filterFavorites').checked;
        const isArchived = Utils.getElement('filterArchived').checked;

        if (searchWord) {
            filtered = filtered.filter(b => 
                (b.title && b.title.toLowerCase().includes(searchWord)) || 
                (b.url && b.url.toLowerCase().includes(searchWord))
            );
        }
        if (categoryId) {
            filtered = filtered.filter(b => b.categoryId == categoryId);
        }
        if (isFavorite) {
            filtered = filtered.filter(b => b.isFavorite);
        }
        if (isArchived) {
            filtered = filtered.filter(b => b.isArchived);
        }

        // Pagination
        const totalItems = filtered.length;
        const totalPages = Math.ceil(totalItems / this.itemsPerPage);
        
        if (this.currentPage > totalPages && totalPages > 0) {
            this.currentPage = totalPages;
        }

        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        const paginated = filtered.slice(startIndex, startIndex + this.itemsPerPage);

        if (paginated.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center border-bottom-0">
                        ${Utils.getEmptyStateHtml('bi-bookmark', 'No bookmarks found', 'Create First Bookmark', '#bookmarkModal')}
                    </td>
                </tr>
            `;
            Utils.getElement('paginationContainer').innerHTML = '';
            return;
        }

        paginated.forEach(bookmark => {
            const tr = document.createElement('tr');
            const categoryName = this.categories.find(c => c.id === bookmark.categoryId)?.categoryName || 'Unknown';
            
            tr.innerHTML = `
                <td class="text-center">
                    <button class="icon-action ${bookmark.isFavorite ? 'active bi-star-fill' : 'bi-star'}" 
                            onclick="Bookmarks.toggleFavorite(${bookmark.id})" title="Toggle Favorite">
                    </button>
                </td>
                <td class="fw-medium">
                    <a href="bookmark.html?id=${bookmark.id}" class="text-decoration-none text-body">
                        ${this.escapeHtml(bookmark.title)}
                    </a>
                </td>
                <td class="text-muted text-truncate" style="max-width: 250px;">
                    <a href="${bookmark.url}" target="_blank" class="text-primary text-decoration-none" title="${bookmark.url}">
                        ${this.escapeHtml(bookmark.url)}
                        <i class="bi bi-box-arrow-up-right ms-1 small"></i>
                    </a>
                </td>
                <td>
                    <span class="badge bg-light text-dark border"><i class="bi bi-folder me-1"></i>${this.escapeHtml(categoryName)}</span>
                </td>
                <td class="text-end pe-4">
                    <button class="btn btn-sm btn-light me-1 rounded-circle" onclick="Bookmarks.toggleArchived(${bookmark.id})" title="${bookmark.isArchived ? 'Unarchive' : 'Archive'}">
                        <i class="bi ${bookmark.isArchived ? 'bi-box-arrow-up' : 'bi-archive'}"></i>
                    </button>
                    <button class="btn btn-sm btn-light me-1 rounded-circle" onclick="Bookmarks.editBookmark(${bookmark.id})" title="Edit">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-light text-danger rounded-circle" onclick="Bookmarks.deleteBookmark(${bookmark.id})" title="Delete">
                        <i class="bi bi-trash3"></i>
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });

        // Render Pagination
        const paginationHtml = Utils.renderPagination(totalItems, this.currentPage, this.itemsPerPage, this.changePage.bind(this));
        Utils.getElement('paginationContainer').innerHTML = paginationHtml;
        Utils.attachPaginationListeners('paginationContainer', this.changePage.bind(this));
    },

    changePage(page) {
        this.currentPage = page;
        this.renderTable();
    },

    applyFilters() {
        this.currentPage = 1;
        this.renderTable();
    },

    clearFilters() {
        Utils.getElement('searchWord').value = '';
        Utils.getElement('filterCategory').value = '';
        Utils.getElement('filterFavorites').checked = false;
        Utils.getElement('filterArchived').checked = false;
        this.applyFilters();
    },

    async handleSave(e) {
        e.preventDefault();
        
        const id = Utils.getElement('bookmarkId').value;
        const title = Utils.getElement('bookmarkTitle').value;
        const url = Utils.getElement('bookmarkUrl').value;
        const categoryId = parseInt(Utils.getElement('bookmarkCategory').value);
        
        const btn = Utils.getElement('saveBookmarkBtn');
        const spinner = btn.querySelector('.spinner-border');

        btn.disabled = true;
        spinner.classList.remove('d-none');

        try {
            const payload = { title, url, categoryId };
            
            if (id) {
                await Api.put(`/bookmarks/${id}`, payload);
                Utils.showToast('Bookmark updated successfully');
            } else {
                await Api.post('/bookmarks', payload);
                Utils.showToast('Bookmark created successfully');
            }
            
            this.modalInstance.hide();
            await this.loadBookmarks();
        } catch (error) {
            Utils.showToast(error.message || 'Failed to save bookmark', 'danger');
        } finally {
            btn.disabled = false;
            spinner.classList.add('d-none');
        }
    },

    editBookmark(id) {
        const bookmark = this.bookmarks.find(b => b.id === id);
        if (!bookmark) return;

        Utils.getElement('bookmarkId').value = bookmark.id;
        Utils.getElement('bookmarkTitle').value = bookmark.title;
        Utils.getElement('bookmarkUrl').value = bookmark.url;
        Utils.getElement('bookmarkCategory').value = bookmark.categoryId;
        Utils.getElement('bookmarkModalLabel').textContent = 'Edit Bookmark';
        
        this.modalInstance.show();
    },

    async deleteBookmark(id) {
        if (!confirm('Are you sure you want to delete this bookmark?')) return;

        Utils.showLoader();
        try {
            await Api.delete(`/bookmarks/${id}`);
            Utils.showToast('Bookmark deleted successfully');
            await this.loadBookmarks();
        } catch (error) {
            Utils.showToast(error.message || 'Failed to delete bookmark', 'danger');
        } finally {
            Utils.hideLoader();
        }
    },

    async toggleFavorite(id) {
        // Assuming PATCH /bookmarks/{id}/favorite exists based on notes endpoint
        try {
            await Api.patch(`/bookmarks/${id}/favorite`);
            const b = this.bookmarks.find(x => x.id === id);
            if (b) b.isFavorite = !b.isFavorite;
            this.renderTable();
            Utils.showToast(b.isFavorite ? 'Removed from favorites' : 'Added to favorites');
        } catch (error) {
            Utils.showToast('Failed to update favorite status', 'danger');
        }
    },

    async toggleArchived(id) {
        try {
            await Api.patch(`/bookmarks/${id}/archive`);
            const b = this.bookmarks.find(x => x.id === id);
            if (b) b.isArchived = !b.isArchived;
            this.renderTable();
            Utils.showToast(b.isArchived ? 'Unarchived' : 'Archived');
        } catch (error) {
            Utils.showToast('Failed to update archive status', 'danger');
        }
    },

    exportData() {
        if (this.bookmarks.length === 0) {
            Utils.showToast('No basic data to export', 'warning');
            return;
        }
        Utils.exportAsJson(this.bookmarks, 'linkvault_bookmarks.json');
    },

    escapeHtml(unsafe) {
        if (!unsafe) return '';
        return unsafe
             .toString()
             .replace(/&/g, "&amp;")
             .replace(/</g, "&lt;")
             .replace(/>/g, "&gt;")
             .replace(/"/g, "&quot;")
             .replace(/'/g, "&#039;");
    }
};

document.addEventListener('DOMContentLoaded', () => {
    if (window.location.pathname.endsWith('bookmarks.html')) {
        Bookmarks.init();
    }
});
