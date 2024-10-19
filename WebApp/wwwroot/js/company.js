function submitRegisterCompany() {
    var form = $('#registerCompanyForm');
    var redirectUrl = $('#redirectUrl').val();
    var actionUrl = $('#actionUrl').val();

    form.validate();
    if (!form.valid()) {
        return;
    }

    var formData = form.serialize();

    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                window.location.href = redirectUrl;
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

$(document).ready(function () {
    var url = new URL(window.location);
    var showModal = url.searchParams.get('showModal');

    if (showModal === 'editCompany') {
        enableEditMode();
        url.searchParams.delete('showModal');
        window.history.replaceState(null, null, url);
    }
});

function submitAddCompany() {
    var form = $('#addCompanyForm');

    form.validate();
    if (!form.valid()) {
        return;
    }

    var formData = form.serialize();

    $.ajax({
        url: '/companies/add',
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

function enableEditMode() {
    var companyNameField = document.getElementById('companyName');
    var companyEmailField = document.getElementById('companyEmail');
    var companyNumberField = document.getElementById('companyNumber');
    var companyAddressField = document.getElementById('companyAddress');
    var companyDescriptionField = document.getElementById('companyDescription');

    companyNameField.removeAttribute('readonly');
    companyEmailField.removeAttribute('readonly');
    companyNumberField.removeAttribute('readonly');
    companyAddressField.removeAttribute('readonly');
    companyDescriptionField.removeAttribute('readonly');

    companyNameField.classList.remove('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyNameField.classList.add('text-gray-700', 'bg-white');

    companyEmailField.classList.remove('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyEmailField.classList.add('text-gray-700', 'bg-white');

    companyNumberField.classList.remove('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyNumberField.classList.add('text-gray-700', 'bg-white');

    companyAddressField.classList.remove('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyAddressField.classList.add('text-gray-700', 'bg-white');

    companyDescriptionField.classList.remove('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyDescriptionField.classList.add('text-gray-700', 'bg-white');

    document.getElementById('editButtons').classList.remove('hidden');
    document.getElementById('editIcon').classList.add('hidden');

    // Store original values
    window.originalCompanyName = companyNameField.value;
    window.originalCompanyEmail = companyEmailField.value;
    window.originalCompanyNumber = companyNumberField.value;
    window.originalCompanyAddress = companyAddressField.value;
    window.originalCompanyDescription = companyDescriptionField.value;
}

function cancelEdit() {
    var companyNameField = document.getElementById('companyName');
    var companyEmailField = document.getElementById('companyEmail');
    var companyNumberField = document.getElementById('companyNumber');
    var companyAddressField = document.getElementById('companyAddress');
    var companyDescriptionField = document.getElementById('companyDescription');

    companyNameField.setAttribute('readonly', true);
    companyEmailField.setAttribute('readonly', true);
    companyNumberField.setAttribute('readonly', true);
    companyAddressField.setAttribute('readonly', true);
    companyDescriptionField.setAttribute('readonly', true);

    companyNameField.classList.add('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyNameField.classList.remove('text-gray-700', 'bg-white');

    companyEmailField.classList.add('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyEmailField.classList.remove('text-gray-700', 'bg-white');

    companyNumberField.classList.add('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyNumberField.classList.remove('text-gray-700', 'bg-white');

    companyAddressField.classList.add('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyAddressField.classList.remove('text-gray-700', 'bg-white');

    companyDescriptionField.classList.add('text-gray-500', 'bg-gray-100', 'cursor-not-allowed');
    companyDescriptionField.classList.remove('text-gray-700', 'bg-white');

    document.getElementById('editButtons').classList.add('hidden');
    document.getElementById('editIcon').classList.remove('hidden');

    $(".text-red-500").text('');

    // Reset to original values
    companyNameField.value = window.originalCompanyName;
    companyEmailField.value = window.originalCompanyEmail;
    companyNumberField.value = window.originalCompanyNumber;
    companyAddressField.value = window.originalCompanyAddress;
    companyDescriptionField.value = window.originalCompanyDescription;
}

function submitChanges() {
    var form = $('#editCompanyForm');

    form.validate();
    if (!form.valid()) {
        return;
    }

    var formData = form.serialize();

    $.ajax({
        url: '/companies/update',
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