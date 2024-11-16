$(document).ready(function () {
    var url = new URL(window.location);
    var showModal = url.searchParams.get('showModal');

    if (showModal === 'editJob') {
        $('#editJobModal').modal('show');
        url.searchParams.delete('showModal');
        window.history.replaceState(null, null, url);
    }
});

async function withdrawApplicationHandler(applicationId) {
    //const actionUrl = $('#withdrawButton').data('action-url');
    const actionUrl = $('#actionUrl').val();

    if (!applicationId) {
        toastr.error('Application ID is missing.');
        return;
    }

    const confirmed = confirm('Are you sure you want to withdraw this application?');
    if (!confirmed) return;

    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: { applicationId: applicationId },
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

document.addEventListener('DOMContentLoaded', function () {
    // Function to update the summary
    function updateSummary() {
        // Job Details
        document.getElementById('summaryTitle').innerText = document.getElementById('createJobTitle').value || '-';
        document.getElementById('summaryAvailableSlots').innerText = document.getElementById('createJobSlotsAvailable').value || '-';
        document.getElementById('summaryLocation').innerText = document.getElementById('createJobLocation').value || '-';
        document.getElementById('summaryDescription').value = document.getElementById('createJobDescription').value || '-';

        // Skills
        const skillsSInput = document.getElementById('createJobSkillsSInput').value;
        const skillsTInput = document.getElementById('createJobSkillsTInput').value;
        const skillsCInput = document.getElementById('createJobSkillsCInput').value;
        let skillS = [];
        let skillT = [];
        let skillC = [];
        skillS = skillsSInput ? JSON.parse(skillsSInput) : [];
        skillT = skillsTInput ? JSON.parse(skillsTInput) : [];
        skillC = skillsCInput ? JSON.parse(skillsCInput) : [];
        document.getElementById('summarySkillsS').innerText = skillS.length
            ? skillS.map(skill => skill.value).join(', ')
            : '-';
        document.getElementById('summarySkillsT').innerText = skillT.length
            ? skillT.map(skill => skill.value).join(', ')
            : '-';
        document.getElementById('summarySkillsC').innerText = skillC.length
            ? skillC.map(skill => skill.value).join(', ')
            : '-';
        document.getElementById('summarySkillWeights').innerText = document.getElementById('skillWeights').innerText || '-';

        // Schedule and Salary
        let scheduleDays = document.getElementById('createJobScheduleDays').value;
        let scheduleHours = document.getElementById('createJobScheduleHours').value;
        if (scheduleDays === "0") {
            scheduleDays = 'Flexible';
        }
        if (scheduleHours === "0") {
            scheduleHours = 'Flexible';
        }
        document.getElementById('summaryScheduleDays').innerText = scheduleDays || '-';
        document.getElementById('summaryScheduleHours').innerText = scheduleHours || '-';

        const salaryLower = document.getElementById('createJobSalaryLower').value;
        const salaryUpper = document.getElementById('createJobSalaryUpper').value;
        if (salaryLower === "0" && salaryUpper === "0") {
            document.getElementById('summarySalary').innerText = 'Unpaid';
        } else if (salaryLower && salaryUpper) {
            if (salaryLower === salaryUpper) {
                document.getElementById('summarySalary').innerText = `PHP ${salaryLower}`;
            }
            else {
                document.getElementById('summarySalary').innerText = `PHP ${salaryLower} - PHP ${salaryUpper}`;
            }
        } else {
            document.getElementById('summarySalary').innerText = '-';
        }

        const setupTypeSelect = document.getElementById('createJobSetupTypeId');
        const selectedSetupType = setupTypeSelect.options[setupTypeSelect.selectedIndex]?.text || '-';
        document.getElementById('summarySetupType').innerText = selectedSetupType;

        const employmentTypeSelect = document.getElementById('createJobEmploymentTypeId');
        const selectedEmploymentType = employmentTypeSelect.options[employmentTypeSelect.selectedIndex]?.text || '-';
        document.getElementById('summaryEmploymentType').innerText = selectedEmploymentType;

        const yearLevelSelect = document.getElementById('createJobYearLevelId');
        const selectedYearLevel = yearLevelSelect.options[yearLevelSelect.selectedIndex]?.text || '-';
        document.getElementById('summaryYearLevel').innerText = selectedYearLevel;

        const programsInput = document.getElementById('createJobProgramsInput').value;
        let programs = [];
        programs = programsInput ? JSON.parse(programsInput) : [];
        document.getElementById('summaryPrograms').innerText = programs.length
            ? programs.map(program => program.value).join(', ')
            : '-';
    }

    const inputs = document.querySelectorAll('#createJobForm input, #createJobForm textarea, #createJobForm select');
    inputs.forEach(function (input) {
        input.addEventListener('input', updateSummary);
    });

    updateSummary();
});

let skillsSEmpty = true;
let skillsTEmpty = true;
let programsEmpty = true;
let setupTypeInvalid = true;
let employmentTypeInvalid = true;
let yearLevelInvalid = true;
function updateValidation(name, bool) {
    if (name === "SkillsS") {
        skillsSEmpty = bool;
    }
    else if (name === "SkillsT") {
        skillsTEmpty = bool;
    }
    else if (name === "Programs") {
        programsEmpty = bool;

        const programsInput = document.getElementById('createJobProgramsInput').value;
        let programs = [];
        programs = programsInput ? JSON.parse(programsInput) : [];
        document.getElementById('summaryPrograms').innerText = programs.length
            ? programs.map(program => program.value).join(', ')
            : '-';
    }
}

document.getElementById('createJobSetupTypeId').addEventListener('change', function () {
    if (this.value === '') {
        $('#createJobSetupTypeIdValidation').text('This field is required.');
        setupTypeInvalid = true;
    } else {
        $('#createJobSetupTypeIdValidation').text('');
        setupTypeInvalid = false;
    }
});

document.getElementById('createJobEmploymentTypeId').addEventListener('change', function () {
    if (this.value === '') {
        $('#createJobEmploymentTypeIdValidation').text('This field is required.');
        employmentTypeInvalid = true;
    } else {
        $('#createJobEmploymentTypeIdValidation').text('');
        employmentTypeInvalid = false;
    }
});

document.getElementById('createJobYearLevelId').addEventListener('change', function () {
    if (this.value === '') {
        $('#createJobYearLevelIdValidation').text('This field is required.');
        yearLevelInvalid = true;
    } else {
        $('#createJobYearLevelIdValidation').text('');
        yearLevelInvalid = false;
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const tabs = document.querySelectorAll('.tab-btn-create');
    const tabContents = document.querySelectorAll('.tab-content-create');
    const nextButton = document.querySelector('#nextCreateJob');
    const previousButton = document.getElementById('backCreateJob');
    const cancelButton = document.getElementById('cancelCreateJob');
    const closeButton = document.getElementById('closeCreateJobModal');
    let currentSectionIndex = 0;
    const skillWeightsRange = document.getElementById("skillWeights_range");
    const customSkillWeightsSpan = document.getElementById("skillWeights");

    skillWeightsRange.addEventListener("input", () => {
        const culturalPercentage = ((1 - skillWeightsRange.value) * 100).toFixed(0);
        const technicalPercentage = (skillWeightsRange.value * 100).toFixed(0);
        customSkillWeightsSpan.textContent = `${culturalPercentage}% - ${technicalPercentage}%`;
    });

    const updateTabVisibility = () => {
        tabContents.forEach((tab, index) => {
            tab.classList.toggle('hidden', index !== currentSectionIndex);
        });
    };

    const validateCurrentSection = () => {
        if (currentSectionIndex === 1) {
            if (skillsSEmpty) {
                $('#skillsSValidation').text('This field is required.');
            }
            if (skillsTEmpty) {
                $('#skillsTValidation').text('This field is required.');
            }
            return !skillsSEmpty && !skillsTEmpty;
        }

        if (currentSectionIndex === 3) {
            if (programsEmpty) {
                $('#programsValidation').text('This field is required.');
            }
            if (setupTypeInvalid) {
                $('#createJobSetupTypeIdValidation').text('This field is required.');
            }
            if (employmentTypeInvalid) {
                $('#createJobEmploymentTypeIdValidation').text('This field is required.');
            }
            if (yearLevelInvalid) {
                $('#createJobYearLevelIdValidation').text('This field is required.');
            }
            return !programsEmpty && !setupTypeInvalid && !employmentTypeInvalid && !yearLevelInvalid;
        }

        let form = $('#createJobForm');
        form.validate();
        if (!form.valid()) {
            return;
        }
        return true;
    };

    function toggleHighlight(nextIndex) {
        tabs[currentSectionIndex].classList.remove('active-tab');
        tabs[currentSectionIndex].classList.remove('bg-gray-100');
        tabs[nextIndex].classList.add('active-tab');
        tabs[nextIndex].classList.add('bg-gray-100');
    }
    
    nextButton.addEventListener('click', (e) => {
        e.preventDefault();
        if (!validateCurrentSection()) {
            return;
        }

        if (currentSectionIndex < tabContents.length - 1) {
            cancelButton.classList.add('hidden');
            previousButton.classList.remove('hidden');
            toggleHighlight(currentSectionIndex + 1);
            currentSectionIndex++;
            if (currentSectionIndex === tabContents.length - 1) {
                nextButton.textContent = "Submit";
            }
            updateTabVisibility();
        }
        else {
            submitCreateJob();
        }
    });

    previousButton.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentSectionIndex === 0) {
            cancelButton.classList.remove('hidden');
            previousButton.classList.add('hidden');
            return;
        }

        if (currentSectionIndex > 0) {
            cancelButton.classList.add('hidden');
            previousButton.classList.remove('hidden');
            toggleHighlight(currentSectionIndex - 1);
            currentSectionIndex--;
            if (currentSectionIndex === 0) {
                cancelButton.classList.remove('hidden');
                previousButton.classList.add('hidden');
            }
            else if (currentSectionIndex < tabContents.length - 1) {
                nextButton.textContent = "Next";
            }
            updateTabVisibility();
        }
    });

    closeButton.addEventListener('click', () => {
        toggleHighlight(0);
        currentSectionIndex = 0;
        cancelButton.classList.remove('hidden');
        previousButton.classList.add('hidden');
        updateTabVisibility();
    });

    const sectionMapping = {
        "job-details-section": 0,
        "skills-section": 1,
        "job-details2-section": 2,
        "additional-section": 3,
        "summary-section": 4
    };

    tabs.forEach(tab => {
        tab.addEventListener('click', (e) => {
            const target = tab.getAttribute('data-target');

            if (sectionMapping[target] > currentSectionIndex) {
                e.preventDefault();
                if (!validateCurrentSection()) {
                    return;
                }
            }

            tabs[currentSectionIndex].classList.remove('active-tab', 'bg-gray-100');
            tabContents[currentSectionIndex].classList.add('hidden');
            
            document.getElementById(target).classList.remove('hidden');
            tab.classList.add('active-tab', 'bg-gray-100');

            currentSectionIndex = sectionMapping[target];
            if (currentSectionIndex === 0) {
                cancelButton.classList.remove('hidden');
                previousButton.classList.add('hidden');
            } else {
                cancelButton.classList.add('hidden');
                previousButton.classList.remove('hidden');
            }

            if (currentSectionIndex === tabContents.length - 1) {
                nextButton.textContent = "Submit";
            } else {
                nextButton.textContent = "Next";
            }
        });
    });

    updateTabVisibility();
});