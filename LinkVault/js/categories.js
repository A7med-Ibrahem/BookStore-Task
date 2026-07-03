const Categories = {
    modalInstance: null,
    categories: [],

    init() {
        this.modalInstance = new bootstrap.Modal(Utils.getElement('categoryModal'));
        
        this.loadCategories();
        
        Utils.getElement('categoryForm').addEventListener('submit', this.handleSave.bind(this));
        
        // Reset modal on close
        Utils.getElement('categoryModal').addEventListener('hidden.bs.modal', () => {
            Utils.getElement('categoryForm').reset();
            Utils.getElement('categoryId').value = '';
            Utils.getElement('categoryModalLabel').textContent = 'New Category';
        });
    },

    async loadCategories() {
        Utils.showLoader();
        try {
            this.categories = await Api.get('/categories');
            this.renderTable();
        } catch (error) {
            Utils.showToast('Failed to load categories', 'danger');
        } finally {
            Utils.hideLoader();
        }
    },

    renderTable() {
        const tbody = Utils.getElement('categoriesTableBody');
        tbody.innerHTML = '';

        if (!this.categories || this.categories.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="3" class="text-center border-bottom-0">
                        ${Utils.getEmptyStateHtml('bi-folder', 'No categories found', 'Create First Category', '#categoryModal')}
                    </td>
                </tr>
            `;
            return;
        }

        this.categories.forEach(category => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td class="fw-medium">
                    <div class="d-flex align-items-center gap-2">
                        <i class="bi bi-folder-fill text-warning fs-5"></i>
                        ${this.escapeHtml(category.categoryName)}
                    </div>
                </td>
                <td class="text-muted text-truncate" style="max-width: 300px;">
                    ${this.escapeHtml(category.description || '-')}
                </td>
                <td class="text-end">
                    <button class="btn btn-sm btn-light me-2 rounded-circle" onclick="Categories.editCategory(${category.id})" title="Edit">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-light text-danger rounded-circle" onclick="Categories.deleteCategory(${category.id})" title="Delete">
                        <i class="bi bi-trash3"></i>
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    },

    async handleSave(e) {
        e.preventDefault();
        
        const id = Utils.getElement('categoryId').value;
        const name = Utils.getElement('categoryName').value;
        const description = Utils.getElement('categoryDescription').value;
        const btn = Utils.getElement('saveCategoryBtn');
        const spinner = btn.querySelector('.spinner-border');

        btn.disabled = true;
        spinner.classList.remove('d-none');

        try {
            const payload = { categoryName: name, description: description };
            
            if (id) {
                await Api.put(`/categories/${id}`, payload);
                Utils.showToast('Category updated successfully');
            } else {
                await Api.post('/categories', payload);
                Utils.showToast('Category created successfully');
            }
            
            this.modalInstance.hide();
            await this.loadCategories();
            
        } catch (error) {
            Utils.showToast(error.message || 'Failed to save category', 'danger');
        } finally {
            btn.disabled = false;
            spinner.classList.add('d-none');
        }
    },

    editCategory(id) {
        const category = this.categories.find(c => c.id === id);
        if (!category) return;

        Utils.getElement('categoryId').value = category.id;
        Utils.getElement('categoryName').value = category.categoryName;
        Utils.getElement('categoryDescription').value = category.description || '';
        Utils.getElement('categoryModalLabel').textContent = 'Edit Category';
        
        this.modalInstance.show();
    },

    async deleteCategory(id) {
        if (!confirm('Are you sure you want to delete this category? All associated bookmarks and notes will be deleted.')) {
            return;
        }

        Utils.showLoader();
        try {
            await Api.delete(`/categories/${id}`);
            Utils.showToast('Category deleted successfully');
            await this.loadCategories();
        } catch (error) {
            Utils.showToast(error.message || 'Failed to delete category', 'danger');
        } finally {
            Utils.hideLoader();
        }
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
    // Only init if we are on categories page
    if (window.location.pathname.endsWith('categories.html')) {
        Categories.init();
    }
});
