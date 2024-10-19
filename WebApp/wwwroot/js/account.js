document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById('registerUserForm');
    const registerButton = document.getElementById('registerButton');

    form.addEventListener('submit', function (event) {
        registerButton.disabled = true;
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById('loginForm');
    const loginButton = document.getElementById('loginButton');

    form.addEventListener('submit', function (event) {
        loginButton.disabled = true;
    });
});

function togglePasswordVisibility() {
    var passwordInput = document.getElementById("passwordInput");
    var eyeOpenIcon = document.getElementById("eyeOpenIcon");
    var eyeClosedIcon = document.getElementById("eyeClosedIcon");
    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        eyeOpenIcon.classList.add('hidden');
        eyeClosedIcon.classList.remove('hidden');
    } else {
        passwordInput.type = "password";
        eyeOpenIcon.classList.remove('hidden');
        eyeClosedIcon.classList.add('hidden');
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const emailInput = document.getElementById('emailInput');
    const passwordInput = document.getElementById('passwordInput');
    const loginButton = document.getElementById('loginButton');

    function toggleLoginButton() {
        if (emailInput.value.trim() && passwordInput.value.trim()) {
            loginButton.removeAttribute('disabled');
        } else {
            loginButton.setAttribute('disabled', 'true');
        }
    }

    const registerUserForm = document.getElementById('registerUserForm');
    const registerButton = document.getElementById('registerButton');
    const userIdInput = document.getElementById('email');
    const firstNameInput = document.getElementById('firstName');
    const lastNameInput = document.getElementById('lastName');
    const passwordInputRegister = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmpassword');

    function toggleRegisterButton() {
        if (
            userIdInput.value.trim() &&
            firstNameInput.value.trim() &&
            lastNameInput.value.trim() &&
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
    firstNameInput.addEventListener('input', toggleRegisterButton);
    lastNameInput.addEventListener('input', toggleRegisterButton);
    passwordInputRegister.addEventListener('input', toggleRegisterButton);
    confirmPasswordInput.addEventListener('input', toggleRegisterButton);

    $('#registerUserModal').on('hidden.bs.modal', function () {
        registerUserForm.reset();
        $(".text-red-500").text('');
        registerButton.setAttribute('disabled', 'true');
    });

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
                    toastr.success('User registered successfully!');
                } else {
                    toastr.error(response.error || "An error occurred.");
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "Too many requests made, try again later.";
                toastr.error(errorMessage);
            }
        });
    }

    $('#registerUserForm').on('submit', function (e) {
        e.preventDefault();
        registerUser();
    });


    $('#resetPasswordModal').on('hidden.bs.modal', function () {
        resetPasswordForm.reset();
        $(".text-red-500").text('');
    });

    function validateResetForm() {
        return $(resetPasswordForm).valid();
    }

    function resetPassword() {
        if (!validateResetForm()) {
            return;
        }

        var formData = new FormData(resetPasswordForm);

        $.ajax({
            url: '/Account/ForgotPassword',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#resetPasswordModal').modal('hide');
                    toastr.success('If your email is in our system, an email with instructions will be sent to you shortly.');
                } else {
                    $('#resetPasswordModal').modal('hide');
                    toastr.error(response.error || "An error occurred.");
                }
            },
            error: function (xhr, status, error) {
                $('#resetPasswordModal').modal('hide');
                var errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "Too many requests made, try again later.";
                toastr.error(errorMessage);
            }
        });
    }

    $('#resetPasswordForm').on('submit', function (e) {
        e.preventDefault();
        resetPassword();
    });
});
