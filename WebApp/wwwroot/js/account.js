document.addEventListener('DOMContentLoaded', function () {
    const emailInput = document.getElementById('emailInput');
    const passwordInput = document.getElementById('passwordInput');
    const loginButton = document.getElementById('loginButton');
    const registerUserForm = document.getElementById('registerUserForm');
    const registerButton = document.getElementById('registerButton');
    const userIdInput = document.getElementById('email');
    const nameInput = document.getElementById('name');
    const passwordInputRegister = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmpassword');
    const registerError = document.getElementById('registerError');

    function toggleLoginButton() {
        if (emailInput.value.trim() && passwordInput.value.trim()) {
            loginButton.removeAttribute('disabled');
        } else {
            loginButton.setAttribute('disabled', 'true');
        }
    }

    function toggleRegisterButton() {
        if (
            userIdInput.value.trim() &&
            nameInput.value.trim() &&
            passwordInputRegister.value.trim() &&
            confirmPasswordInput.value.trim()
        ) {
            registerButton.removeAttribute('disabled');
        } else {
            registerButton.setAttribute('disabled', 'true');
        }
    }

    emailInput.addEventListener('input', toggleLoginButton);
    passwordInput.addEventListener('input', toggleLoginButton);

    userIdInput.addEventListener('input', toggleRegisterButton);
    nameInput.addEventListener('input', toggleRegisterButton);
    passwordInputRegister.addEventListener('input', toggleRegisterButton);
    confirmPasswordInput.addEventListener('input', toggleRegisterButton);

    function validateForm() {
        return $(registerUserForm).valid();
    }

    function registerUser() {
        if (!validateForm()) {
            return;
        }
        
        var formData = new FormData(registerUserForm);

        $.ajax({
            url: '/Account/Register',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#registerUserModal').modal('hide');
                    toastr.success('User Registered Successfully!');
                } else {
                    toastr.error(response.error || "An error occurred.");
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "An unexpected error occurred.";
                toastr.error(errorMessage);
            }
        });
    }

    $('#registerUserForm').on('submit', function (e) {
        e.preventDefault();
        registerUser();
    });
});
