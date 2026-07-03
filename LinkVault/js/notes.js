const Notes = {
    modalInstance: null,
    notes: [],
    categories: [],
    currentPage: 1,
    itemsPerPage: 10,

    init() {
        this.modalInstance = new bootstrap.Modal(Utils.getElement('noteModal'));
        
        this.loadCategories().then(() => {
            this.loadNotes();
        });
        
        Utils.getElement('filterForm').addEventListener('submit', (e) => {
            e.preventDefault();
            this.applyFilters();
        });
        
        Utils.getElement('noteForm').addEventListener('submit', this.handleSave.bind(this));
        
        Utils.getElement('noteModal').addEventListener('hidden.bs.modal', () => {
            Utils.getElement('noteForm').reset();
            Utils.getElement('noteId').value = '';
            Utils.getElement('noteModalLabel').textContent = 'New Note';
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
        const modalSelect = Utils.getElement('noteCategory');
        const filterSelect = Utils.getElement('filterCategory');
        
        if (!modalSelect || !filterSelect) return;

        let optionsHtml = '';
        this.categories.forEach(c => {
            optionsHtml += `<option value="${c.id}">${this.escapeHtml(c.categoryName)}</option>`;
        });

        modalSelect.innerHTML = '<option value="" disabled selected>Select a category</option>' + optionsHtml;
        filterSelect.innerHTML = '<option value="">All Categories</option>' + optionsHtml;
    },

    async loadNotes(params = {}) {
        Utils.showLoader();
        try {
            const queryParams = new URLSearchParams(params).toString();
            const url = queryParams ? `/notes?${queryParams}` : '/notes';
            this.notes = await Api.get(url);
            this.renderTable();
        } catch (error) {
            Utils.showToast('Failed to load notes', 'danger');
        } finally {
            Utils.hideLoader();
        }
    },

    renderTable() {
        const tbody = Utils.getElement('notesTableBody');
        tbody.innerHTML = '';

        // Apply text-based search filtering locally if backend doesn't support it fully, 
        // though API docs mention searchWord
        let filtered = [...this.notes];

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
                        ${Utils.getEmptyStateHtml('bi-journal-text', 'No notes found', 'Create First Note', '#noteModal')}
                    </td>
                </tr>
            `;
            Utils.getElement('paginationContainer').innerHTML = '';
            return;
        }

        paginated.forEach(note => {
            const tr = document.createElement('tr');
            const categoryName = this.categories.find(c => c.id === note.categoryId)?.categoryName || 'Unknown';
            
            // Truncate content for display
            const contentSnippet = note.content && note.content.length > 50 
                ? note.content.substring(0, 50) + '...' 
                : note.content;

            tr.innerHTML = `
                <td class="text-center">
                    <button class="icon-action ${note.isPinned ? 'active bi-pin-angle-fill' : 'bi-pin-angle'}" 
                            onclick="Notes.togglePinned(${note.id})" title="Toggle Pin">
                    </button>
                </td>
                <td class="fw-medium">
                    ${this.escapeHtml(note.title || 'Untitled')}
                </td>
                <td class="text-muted">
                    ${this.escapeHtml(contentSnippet)}
                </td>
                <td>
                    <span class="badge bg-light text-dark border"><i class="bi bi-folder me-1"></i>${this.escapeHtml(categoryName)}</span>
                </td>
                <td class="text-end pe-4">
                    <button class="btn btn-sm btn-light me-1 rounded-circle" onclick="Notes.editNote(${note.id})" title="Edit">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-light text-danger rounded-circle" onclick="Notes.deleteNote(${note.id})" title="Delete">
                        <i class="bi bi-trash3"></i>
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });

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
        const searchWord = Utils.getElement('searchWord').value;
        const Category = Utils.getElement('filterCategory').value;
        const Pinned = Utils.getElement('filterPinned').checked;

        const params = {};
        if (searchWord) params.searchWord = searchWord;
        if (Category) params.Category = Category;
        if (Pinned) params.Pinned = true;

        this.loadNotes(params);
    },

    clearFilters() {
        Utils.getElement('searchWord').value = '';
        Utils.getElement('filterCategory').value = '';
        Utils.getElement('filterPinned').checked = false;
        this.applyFilters();
    },

    async handleSave(e) {
        e.preventDefault();
        
        const id = Utils.getElement('noteId').value;
        const title = Utils.getElement('noteTitle').value;
        const content = Utils.getElement('noteContent').value;
        const categoryId = parseInt(Utils.getElement('noteCategory').value);
        
        const btn = Utils.getElement('saveNoteBtn');
        const spinner = btn.querySelector('.spinner-border');

        btn.disabled = true;
        spinner.classList.remove('d-none');

        try {
            const payload = { title, content, categoryId };
            
            if (id) {
                await Api.put(`/notes/${id}`, payload);
                Utils.showToast('Note updated successfully');
            } else {
                await Api.post('/notes', payload);
                Utils.showToast('Note created successfully');
            }
            
            this.modalInstance.hide();
            this.applyFilters(); // Reload with current filters
        } catch (error) {
            Utils.showToast(error.message || 'Failed to save note', 'danger');
        } finally {
            btn.disabled = false;
            spinner.classList.add('d-none');
        }
    },

    editNote(id) {
        const note = this.notes.find(n => n.id === id);
        if (!note) return;

        Utils.getElement('noteId').value = note.id;
        Utils.getElement('noteTitle').value = note.title;
        Utils.getElement('noteContent').value = note.content;
        Utils.getElement('noteCategory').value = note.categoryId;
        Utils.getElement('noteModalLabel').textContent = 'Edit Note';
        
        this.modalInstance.show();
    },

    async deleteNote(id) {
        if (!confirm('Are you sure you want to delete this note?')) return;

        Utils.showLoader();
        try {
            await Api.delete(`/notes/${id}`);
            Utils.showToast('Note deleted successfully');
            this.applyFilters();
        } catch (error) {
            Utils.showToast(error.message || 'Failed to delete note', 'danger');
        } finally {
            Utils.hideLoader();
        }
    },

    async togglePinned(id) {
        try {
            await Api.patch(`/notes/${id}/pin`);
            const n = this.notes.find(x => x.id === id);
            if (n) n.isPinned = !n.isPinned;
            this.renderTable();
            Utils.showToast(n.isPinned ? 'Note pinned' : 'Note unpinned');
        } catch (error) {
            Utils.showToast('Failed to update pin status', 'danger');
        }
    },

    exportData() {
        if (this.notes.length === 0) {
            Utils.showToast('No notes to export', 'warning');
            return;
        }
        Utils.exportAsJson(this.notes, 'linkvault_notes.json');
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
    if (window.location.pathname.endsWith('notes.html')) {
        Notes.init();
    }
});
