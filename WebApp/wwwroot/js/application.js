
    /// ------------------------------------
    /// Listener: Update Application Status
    /// ------------------------------------
    document.querySelectorAll('.update-application-status').forEach(button => {
        button.addEventListener('click', () => {
            const applicationId = button.getAttribute('data-applicationId');
            const applicationStatusTypeId = button.getAttribute('data-statusid');

            const selectElement = document.getElementById('updateJobStatus');
            const hiddenInput = document.getElementById('updateJobStatusJobId');

            if (hiddenInput) {
                hiddenInput.value = applicationId;
            }
            if (selectElement) {
                selectElement.value = applicationStatusTypeId;
            }
        });
    });

    /// ------------------------------------
    /// Submit: Update Application Status
    /// ------------------------------------
    document.getElementById("submitUpdateApplicationStatus").addEventListener('click', function () {
        const form = document.getElementById("updateApplicationStatusForm");
        form.submit();
    });

    /// ------------------------------------
    /// Listener: Reject and Shortlist
    /// ------------------------------------
    document.getElementById("submitUpdateApplicationStatusShortlist").addEventListener('click', function () {
        const form = document.getElementById("updateApplicationStatusForm");
        document.getElementById("updateJobStatus").value = "Shortlisted"; 
        form.submit();
    });

    /// ------------------------------------
    /// Listener: Modal Handling for Update Status Modal
    /// ------------------------------------
    document.querySelectorAll('.actionDropdownApplicationBtn').forEach(button => {
        button.addEventListener('click', () => {
            $('#updateApplicationStatusModal').modal('show'); 
        });
    });

});
