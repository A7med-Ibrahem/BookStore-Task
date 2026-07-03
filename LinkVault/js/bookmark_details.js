const BookmarkDetails = {
    bookmarkId: null,
    modalInstance: null,

    init() {
        const urlParams = new URLSearchParams(window.location.search);
        this.bookmarkId = urlParams.get('id');

        if (!this.bookmarkId) {
            window.location.href = 'bookmarks.html';
            return;
        }

        this.modalInstance = new bootstrap.Modal(Utils.getElement('noteModal'));
        
        this.loadDetails();
        
        Utils.getElement('noteForm').addEventListener('submit', this.handleAddNote.bind(this));
        
        Utils.getElement('noteModal').addEventListener('hidden.bs.modal', () => {
            Utils.getElement('noteForm').reset();
        });
    },

    async loadDetails() {
        Utils.showLoader();
        try {
            // Load all bookmarks and find this one, and categories to get the name
            // API lacks GET /bookmarks/{id} based on docs, so we fetch all 
            const [bookmarks, categories] = await Promise.all([
                Api.get('/bookmarks').catch(() => []),
                Api.get('/categories').catch(() => [])
            ]);

            const bookmark = bookmarks.find(b => b.id == this.bookmarkId);
            
            if (!bookmark) {
                Utils.showToast('Bookmark not found', 'danger');
                setTimeout(() => window.location.href = 'bookmarks.html', 2000);
                return;
            }

            const header = Utils.getElement('bookmarkHeader');
            header.style.display = 'block';
            
            Utils.getElement('bdTitle').textContent = bookmark.title;
            const urlEl = Utils.getElement('bdUrl');
            urlEl.href = bookmark.url;
            Utils.getElement('bdUrlText').textContent = bookmark.url;

            const categoryName = categories.find(c => c.id === bookmark.categoryId)?.categoryName || 'Uncategorized';
            Utils.getElement('bdCategory').innerHTML = `<i class="bi bi-folder2-open me-1"></i> ${this.escapeHtml(categoryName)}`;

            await this.loadNotes();

        } catch (error) {
            Utils.showToast('Failed to load bookmark details', 'danger');
        } finally {
            Utils.hideLoader();
        }
    },

    async loadNotes() {
        try {
            const notes = await Api.get(`/bookmarks/${this.bookmarkId}/notes`);
            this.renderNotes(notes);
        } catch (error) {
            this.renderNotes([]);
        }
    },

    renderNotes(notes) {
        const container = Utils.getElement('notesContainer');
        container.innerHTML = '';

        if (!notes || notes.length === 0) {
            container.innerHTML = `
                <div class="col-12 text-center py-5 text-muted glass-card">
                    <i class="bi bi-journal-x fs-1 opacity-50 mb-3 d-block"></i>
                    <p class="mb-3">No notes attached to this bookmark yet.</p>
                </div>
            `;
            return;
        }

        notes.forEach(note => {
            const col = document.createElement('div');
            col.className = 'col-md-6 col-lg-4 fade-in';
            
            // Format date if available
            const dateStr = note.createdAt ? new Date(note.createdAt).toLocaleDateString() : '';

            col.innerHTML = `
                <div class="card h-100 note-card border-0 glass-card">
                    <div class="card-body d-flex flex-column">
                        <p class="card-text flex-grow-1" style="white-space: pre-wrap;">${this.escapeHtml(note.content)}</p>
                        <hr class="text-muted opacity-25">
                        <div class="d-flex justify-content-between align-items-center mt-auto">
                            <small class="text-muted"><i class="bi bi-clock me-1"></i> ${dateStr}</small>
                            <button class="btn btn-sm btn-outline-danger border-0" onclick="BookmarkDetails.deleteNote(${note.id})" title="Delete Note">
                                <i class="bi bi-trash3"></i>
                            </button>
                        </div>
                    </div>
                </div>
            `;
            container.appendChild(col);
        });
    },

    async handleAddNote(e) {
        e.preventDefault();
        
        const content = Utils.getElement('noteContent').value;
        const btn = Utils.getElement('saveNoteBtn');
        const spinner = btn.querySelector('.spinner-border');

        btn.disabled = true;
        spinner.classList.remove('d-none');

        try {
            await Api.post(`/bookmarks/${this.bookmarkId}/notes`, { content });
            Utils.showToast('Note added successfully');
            this.modalInstance.hide();
            await this.loadNotes();
        } catch (error) {
            Utils.showToast(error.message || 'Failed to add note', 'danger');
        } finally {
            btn.disabled = false;
            spinner.classList.add('d-none');
        }
    },

    async deleteNote(id) {
        if (!confirm('Are you sure you want to delete this note?')) return;

        Utils.showLoader();
        try {
            await Api.delete(`/bookmarks/${this.bookmarkId}/notes/${id}`);
            Utils.showToast('Note deleted successfully');
            await this.loadNotes();
        } catch (error) {
            Utils.showToast(error.message || 'Failed to delete note', 'danger');
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
    if (window.location.pathname.endsWith('bookmark.html')) {
        BookmarkDetails.init();
    }
});
