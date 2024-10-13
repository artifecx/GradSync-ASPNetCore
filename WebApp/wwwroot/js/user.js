var itemId;
function displayResetPasswordModal(id, name) {
    itemId = id;
    itemName = name;
    document.getElementById('resetPasswordModalMessage').textContent = `Are you sure you want to reset ${itemName}'s password?`;
    $('#resetPasswordModal').modal('show');
}

$('#confirmResetBtn').on('click', function () {
    var resetUrl = $('#resetUrl').val();
    var baseUrl = $('#baseUrl').val();
    $.ajax({
        url: resetUrl,
        type: 'POST',
        data: { id: itemId },
        success: function (response) {
            if (response.success) {
                window.location.href = baseUrl;
            }
            else {
                var errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        },
        error: function (jqXHR) {
            if (jqXHR.status === 429) {
                var response = jqXHR.responseJSON;
                var errorMessage = response.message || "Too many requests. Please try again later.";
                $('#resetPasswordModal').modal('hide');

                toastr.error(errorMessage);
            }
            else {
                var errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        }
    });
});

function submitAddUser() {
    var form = $('#addUserForm');

    form.validate();
    if (!form.valid()) {
        return;
    }

    var formData = form.serialize();

    $.ajax({
        url: '/users/create',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                var errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "An unexpected error occurred.";
            toastr.error(errorMessage);
        }
    });
}

function displayEditModal(userId, firstname, lastname, middlename, suffix, email, roleId, isverified) {
    $('#editUserId').val(userId);
    $('#editUserFirstName').val(firstname);
    $('#editUserLastName').val(lastname);
    $('#editUserMiddleName').val(middlename);
    $('#editUserSuffix').val(suffix);
    $('#editUserEmail').val(email);
    $('#editUserRole').val(roleId);
    $('#editIsVerified').val(isverified);
    $('#editUserModal').modal('show');
}

function submitEditUser() {
    var form = $('#editUserForm');

    form.validate();
    if (!form.valid()) {
        return;
    }

    var formData = form.serialize();

    $.ajax({
        url: '/users/update',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                var errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "An unexpected error occurred.";
            toastr.error(errorMessage);
        }
    });
}
