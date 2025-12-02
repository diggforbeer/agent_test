// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Password strength checker
function checkPasswordStrength(password) {
    let strength = 0;
    const requirements = {
        length: password.length >= 8,
        uppercase: /[A-Z]/.test(password),
        lowercase: /[a-z]/.test(password),
        number: /[0-9]/.test(password),
        special: /[!@#$%^&*(),.?":{}|<>]/.test(password)
    };

    // Count met requirements
    Object.values(requirements).forEach(met => {
        if (met) strength++;
    });

    // Determine strength level
    let level = 'weak';
    if (strength >= 4) {
        level = 'strong';
    } else if (strength >= 3) {
        level = 'medium';
    }

    return { level, requirements };
}

// Initialize password strength indicator
function initPasswordStrength() {
    const passwordInput = document.getElementById('Password');
    if (!passwordInput) return;

    const strengthIndicator = document.createElement('div');
    strengthIndicator.className = 'password-strength weak';
    passwordInput.parentElement.appendChild(strengthIndicator);

    const requirementsList = document.querySelector('.password-requirements');

    passwordInput.addEventListener('input', function () {
        const password = this.value;
        const result = checkPasswordStrength(password);

        // Update strength indicator
        strengthIndicator.className = `password-strength ${result.level}`;

        // Update requirements list if it exists
        if (requirementsList) {
            const items = requirementsList.querySelectorAll('li');
            items.forEach((item, index) => {
                const requirementKeys = ['length', 'uppercase', 'lowercase', 'number', 'special'];
                if (result.requirements[requirementKeys[index]]) {
                    item.classList.add('valid');
                } else {
                    item.classList.remove('valid');
                }
            });
        }
    });
}

// Show loading spinner
function showLoading() {
    let overlay = document.getElementById('loadingOverlay');
    if (!overlay) {
        overlay = document.createElement('div');
        overlay.id = 'loadingOverlay';
        overlay.className = 'spinner-overlay';
        overlay.innerHTML = `
            <div class="spinner-border text-light" role="status" style="width: 3rem; height: 3rem;">
                <span class="visually-hidden">Loading...</span>
            </div>
        `;
        document.body.appendChild(overlay);
    }
    overlay.classList.add('show');
}

// Hide loading spinner
function hideLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.classList.remove('show');
    }
}

// Auto-dismiss alerts after 5 seconds
function initAutoDismissAlerts() {
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
}

// Initialize all features on page load
document.addEventListener('DOMContentLoaded', function () {
    initPasswordStrength();
    initAutoDismissAlerts();

    // Show loading on form submit
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function () {
            if (this.checkValidity()) {
                showLoading();
            }
        });
    });
});

