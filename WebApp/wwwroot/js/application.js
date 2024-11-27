document.addEventListener('DOMContentLoaded', function () {
    /// ------------------------------------
    /// Listener: Action Dropdown
    /// ------------------------------------
    document.addEventListener('click', function (e) {
        const actionBtn = e.target.closest('.actionDropdownApplicationBtn');
        if (actionBtn) {
            const ApplicationId = actionBtn.dataset.applicationid;
            const menu = document.getElementById(`actionDropdownApplicationMenu-${ApplicationId}`);
            menu.classList.toggle('hidden');

            document.querySelectorAll('.actionDropdownApplicationMenu').forEach(m => {
                if (m.id !== `actionDropdownApplicationMenu-${ApplicationId}`) {
                    m.classList.add('hidden');
                }
            });
            return;
        }

        if (!e.target.closest('.actionDropdownApplicationMenu') && !e.target.closest('.actionDropdownApplicationBtn')) {
            document.querySelectorAll('.actionDropdownApplicationMenu').forEach(menu => {
                menu.classList.add('hidden');
            });
        }
    });

    /// ------------------------------------
    /// Listener: Update Application Status
    /// ------------------------------------
    document.querySelectorAll('.update-application-status').forEach(button => {
        button.addEventListener('click', () => {
            const applicationId = button.getAttribute('data-applicationId');
            const applicationStatusTypeId = button.getAttribute('data-statusid');

            const selectElement = document.getElementById('updateApplicationStatus');
            const hiddenInput = document.getElementById('updateApplicationStatusAppId');

            if (hiddenInput) {
                hiddenInput.value = applicationId;
            }
            if (selectElement) {
                if (applicationStatusTypeId !== 'Submitted'
                    && applicationStatusTypeId !== 'Viewed') {
                    selectElement.value = applicationStatusTypeId;
                }
            }
        });
    });

    /// ------------------------------------
    /// Listener: Withdraw Application
    /// ------------------------------------
    document.querySelectorAll('.withdraw-application').forEach(button => {
        button.addEventListener('click', () => {
            const applicationId = button.getAttribute('data-applicationId');
            const jobTitle = button.getAttribute('data-position');

            const spanElement = document.getElementById('withdrawPositionTitle');
            const hiddenInput = document.getElementById('withdrawApplicationId');

            if (hiddenInput) {
                hiddenInput.value = applicationId;
            }
            if (spanElement) {
                spanElement.innerText = jobTitle;
            }
        });
    });
});

/// ------------------------------------
/// Route: Update Application Status
/// ------------------------------------
const submitButton = document.getElementById('submitUpdateApplicationStatus');

submitButton.addEventListener('click', () => {
    const appStatusId = document.getElementById('updateApplicationStatus').value;
    const appId = document.getElementById('updateApplicationStatusAppId').value;
    submitButton.disabled = true;

    $.ajax({
        url: '/applications/update-application',
        type: 'POST',
        data: { applicationId: appId, appStatusId: appStatusId },
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                submitButton.disabled = false;
                toastr.error(response.error || "An error occurred.");
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = xhr.responseJSON?.error ? xhr.responseJSON.error : "An unexpected error occurred.";
            toastr.error(errorMessage);
        }
    });
});

/// ------------------------------------
/// Route: Withdraw Application
/// ------------------------------------
const withdrawButton = document.getElementById('submitWithdrawApplication');

withdrawButton.addEventListener('click', () => {
    const appId = document.getElementById('withdrawApplicationId').value;
    withdrawButton.disabled = true;

    $.ajax({
        url: '/applications/withdraw',
        type: 'POST',
        data: { applicationId: appId },
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                withdrawButton.disabled = false;
                toastr.error(response.error || "An error occurred.");
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = xhr.responseJSON?.error ? xhr.responseJSON.error : "An unexpected error occurred.";
            toastr.error(errorMessage);
        }
    });
});
