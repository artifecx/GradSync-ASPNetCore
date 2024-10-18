﻿var itemId;
function displayDeleteModal(id, name) {
    itemId = id;
    var itemName = name;
    document.getElementById('deleteModalMessage').textContent = `Are you sure you want to delete ${itemName}? This action is permanent and cannot be undone.`;
    $('#deleteModal').modal('show');
}

$('#confirmDeleteBtn').on('click', function () {
    var deleteUrl = $('#deleteUrl').val();
    var baseUrl = $('#baseUrl').val();
    $.ajax({
        url: deleteUrl,
        type: 'POST',
        data: { id: itemId },
        success: function (response) {
            if (response.success) {
                window.location.href = baseUrl;
            } else {
                var errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        }
    });
});