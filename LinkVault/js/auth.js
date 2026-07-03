const Auth = {
    init() {
        if (Api.getToken() && window.location.pathname.endsWith('login.html')) {
            window.location.href = 'categories.html';
        }

        const loginForm = Utils.getElement('loginForm');
        const registerForm = Utils.getElement('registerForm');
        
        if (loginForm) {
            loginForm.addEventListener('submit', this.handleLogin.bind(this));
            Utils.getElement('showRegisterBtn').addEventListener('click', (e) => {
                e.preventDefault();
                this.toggleForms(true);
            });
        }

        if (registerForm) {
            registerForm.addEventListener('submit', this.handleRegister.bind(this));
            Utils.getElement('showLoginBtn').addEventListener('click', (e) => {
                e.preventDefault();
                this.toggleForms(false);
            });
        }
    },

    toggleForms(showRegister) {
        Utils.getElement('loginForm').classList.toggle('d-none', showRegister);
        Utils.getElement('registerForm').classList.toggle('d-none', !showRegister);
        this.clearAlert();
    },

    setLoading(btnId, spinnerId, isLoading) {
        const btn = Utils.getElement(btnId);
        const spinner = Utils.getElement(spinnerId);
        if (btn && spinner) {
            btn.disabled = isLoading;
            spinner.classList.toggle('d-none', !isLoading);
        }
    },

    showAlert(message, type = 'danger') {
        const container = Utils.getElement('alertContainer');
        if (container) {
            container.innerHTML = `
                <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                    ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>
            `;
        }
    },

    clearAlert() {
        const container = Utils.getElement('alertContainer');
        if (container) container.innerHTML = '';
    },

    async handleLogin(e) {
        e.preventDefault();
        this.clearAlert();
        
        const email = Utils.getElement('loginEmail').value;
        const password = Utils.getElement('loginPassword').value;

        try {
            this.setLoading('loginBtn', 'loginSpinner', true);
            
            const response = await Api.post('/auth/login', { email, password });
            
            if (response && response.token) {
                Api.setToken(response.token);
                window.location.href = 'categories.html';
            } else {
                throw new Error('Invalid response from server');
            }
        } catch (error) {
            this.showAlert(error.message || 'Login failed. Please check your credentials.');
        } finally {
            this.setLoading('loginBtn', 'loginSpinner', false);
        }
    },

    async handleRegister(e) {
        e.preventDefault();
        this.clearAlert();

        const firstName = Utils.getElement('regFirstName').value;
        const lastName = Utils.getElement('regLastName').value;
        const email = Utils.getElement('regEmail').value;
        const password = Utils.getElement('regPassword').value;

        try {
            this.setLoading('registerBtn', 'registerSpinner', true);
            
            await Api.post('/auth/register', { firstName, lastName, email, password });
            
            this.showAlert('Registration successful! Please sign in.', 'success');
            
            // Switch to login form
            setTimeout(() => {
                this.toggleForms(false);
                Utils.getElement('loginEmail').value = email;
                Utils.getElement('loginPassword').focus();
            }, 1500);
            
        } catch (error) {
            this.showAlert(error.message || 'Registration failed. Please try again.');
        } finally {
            this.setLoading('registerBtn', 'registerSpinner', false);
        }
    }
};

document.addEventListener('DOMContentLoaded', () => {
    Auth.init();
});
