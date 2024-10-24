$(document).ready(function () {
    var url = new URL(window.location);
    var showModal = url.searchParams.get('showModal');

    if (showModal === 'editJob') {
        $('#editJobModal').modal('show');
        url.searchParams.delete('showModal');
        window.history.replaceState(null, null, url);
    }
});

async function applyJobHandler(jobId) {
    const actionUrl = $('#actionUrl').val();
    console.log(jobId);
    console.log(actionUrl);
    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: { jobId: jobId },
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

function submitCreateJob() {
    var form = $('#createJobForm');

    form.validate();
    if (!form.valid()) {
        return;
    }

    var formData = form.serialize();

    $.ajax({
        url: '/jobs/create',
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

function submitEditJob() {
    var form = $('#editJobForm');

    form.validate();
    if (!form.valid()) {
        return;
    }

    var formData = form.serialize();

    $.ajax({
        url: '/jobs/update',
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

document.addEventListener('click', function (e) {
    const actionBtn = e.target.closest('.actionDropdownJobBtn');
    if (actionBtn) {
        const JobId = actionBtn.dataset.jobid;
        const menu = document.getElementById(`actionDropdownJobMenu-${JobId}`);
        menu.classList.toggle('hidden');

        document.querySelectorAll('.actionDropdownJobMenu').forEach(m => {
            if (m.id !== `actionDropdownJobMenu-${JobId}`) {
                m.classList.add('hidden');
            }
        });
        return;
    }

    if (!e.target.closest('.actionDropdownJobMenu') && !e.target.closest('.actionDropdownJobBtn')) {
        document.querySelectorAll('.actionDropdownJobMenu').forEach(menu => {
            menu.classList.add('hidden');
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var contentTextarea = document.getElementById('createJobTitle');
    var remainingCharsSpan = document.getElementById('remainingTitleChars');

    function updateRemainingChars() {
        var remaining = 100 - contentTextarea.value.length;
        remainingCharsSpan.textContent = remaining + ' characters remaining';
    }

    updateRemainingChars();

    contentTextarea.addEventListener('keyup', updateRemainingChars);
});

document.addEventListener('DOMContentLoaded', function () {
    var contentTextarea = document.getElementById('createJobDescription');
    var remainingCharsSpan = document.getElementById('remainingDescriptionChars');

    function updateRemainingChars() {
        var remaining = 800 - contentTextarea.value.length;
        remainingCharsSpan.textContent = remaining + ' characters remaining';
    }

    updateRemainingChars();

    contentTextarea.addEventListener('keyup', updateRemainingChars);
});

document.addEventListener('DOMContentLoaded', function () {
    var contentTextarea = document.getElementById('editJobTitle');
    var remainingCharsSpan = document.getElementById('remainingTitleChars');

    function updateRemainingChars() {
        var remaining = 100 - contentTextarea.value.length;
        remainingCharsSpan.textContent = remaining + ' characters remaining';
    }

    updateRemainingChars();

    contentTextarea.addEventListener('keyup', updateRemainingChars);
});

document.addEventListener('DOMContentLoaded', function () {
    var contentTextarea = document.getElementById('editJobDescription');
    var remainingCharsSpan = document.getElementById('remainingDescriptionChars');

    function updateRemainingChars() {
        var remaining = 800 - contentTextarea.value.length;
        remainingCharsSpan.textContent = remaining + ' characters remaining';
    }

    updateRemainingChars();

    contentTextarea.addEventListener('keyup', updateRemainingChars);
});