document.addEventListener('DOMContentLoaded', function () {
    /// ------------------------------------
    /// Listener: Action Dropdown
    /// ------------------------------------
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

    /// ------------------------------------
    /// Listener: Update Job Status
    /// ------------------------------------
    document.querySelectorAll('.update-job-status').forEach(button => {
        button.addEventListener('click', () => {
            const jobId = button.getAttribute('data-jobid');
            const statusId = button.getAttribute('data-statusId');

            const selectElement = document.getElementById('updateJobStatus');
            const hiddenInput = document.getElementById('updateJobStatusJobId');

            hiddenInput.value = jobId;
            if (selectElement) {
                selectElement.value = statusId;
            }
        });
    });

    /// ------------------------------------
    /// Listener: Close Create Job Modal
    /// ------------------------------------
    let cancelClicked = false;
    const cancelButtons = ["closeCreateJobModal", "cancelCreateJob"];
    cancelButtons.forEach(id => {
        const button = document.getElementById(id);
        if (button) {
            button.addEventListener("click", () => {
                cancelClicked = true;
            });
        }
    });
    $('#createJobModal').on('hidden.bs.modal', function () {
        if (cancelClicked) {
            createJobForm.reset();
            $(".text-red-500").text('');
            $("#remainingTitleChars").text('100 characters remaining');
            $("#remainingDescriptionChars").text('3000 characters remaining');
            $("#skillWeights").text('30% - 70%');
            switchToTab(currentSectionIndex, 0);
            toggleCancelButton('show');
            toggleNextButton('Next');
            skillsSEmpty = true;
            skillsTEmpty = true;
            programsEmpty = true;
            currentSectionIndex = 0;
            initializeView();
        }
        cancelClicked = false;
    });

    /// ------------------------------------
    /// Listener: Skill Weights Range
    /// ------------------------------------
    const skillWeightsRange = document.getElementById("skillWeights_range");
    const customSkillWeightsSpan = document.getElementById("skillWeights");
    skillWeightsRange.addEventListener("input", () => {
        const culturalPercentage = ((1 - skillWeightsRange.value) * 100).toFixed(0);
        const technicalPercentage = (skillWeightsRange.value * 100).toFixed(0);
        customSkillWeightsSpan.textContent = `${culturalPercentage}% - ${technicalPercentage}%`;
    });

    /// ------------------------------------
    /// Listener: Title & Description character counter
    /// ------------------------------------
    function addCharCountListener(textareaId, remainingCharsId, maxChars) {
        let contentTextarea = document.getElementById(textareaId);
        let remainingCharsSpan = document.getElementById(remainingCharsId);

        function updateRemainingChars() {
            let remaining = maxChars - contentTextarea.value.length;
            remainingCharsSpan.textContent = remaining + ' characters remaining';
        }
        updateRemainingChars();
        contentTextarea.addEventListener('keyup', updateRemainingChars);
    }
    addCharCountListener('createJobTitle', 'remainingTitleChars', 100);
    addCharCountListener('createJobDescription', 'remainingDescriptionChars', 3000);

    /// ------------------------------------
    /// Summary: Update Summary
    /// ------------------------------------
    function updateSummary(index) {
        switch (index) {
            case 0:
                updateJobDetails();
                return;
            case 1:
                updateSkills();
                return;
            case 2:
                updateSchedule();
                updateSalary();
                return;
            case 3:
                updateAdditionalInformation();
                return;
            default:
                return;
        }

        // Job Details
        function updateJobDetails() {
            document.getElementById('summaryTitle').innerText = document.getElementById('createJobTitle').value || '-';
            document.getElementById('summaryAvailableSlots').innerText = document.getElementById('createJobSlotsAvailable').value || '-';
            document.getElementById('summaryLocation').innerText = document.getElementById('createJobLocation').value || '-';
            document.getElementById('summaryDescription').value = document.getElementById('createJobDescription').value || '-';
        }

        // Skills
        function updateSkills() {
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
        }

        // Schedule
        function updateSchedule() {
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
        }

        // Salary
        function updateSalary() {
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
        }

        // Additional Information
        function updateAdditionalInformation() {
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
    }

    /// ------------------------------------
    /// Form Handling: Create Job
    /// ------------------------------------
    const tabs = document.querySelectorAll('.tab-btn-create');
    const tabContents = document.querySelectorAll('.tab-content-create');
    const tabValidationSymbols = document.querySelectorAll('.tab-btn-create-validation');
    const nextButton = document.querySelector('#nextCreateJob');
    const previousButton = document.getElementById('backCreateJob');
    const cancelButton = document.getElementById('cancelCreateJob');
    const summaryValidationMessage = document.getElementById('summaryValidation');
    const sectionMapping = {
        "job-details-section": 0,
        "skills-section": 1,
        "job-details2-section": 2,
        "additional-section": 3,
        "summary-section": 4
    };
    const validSections = {
        0: false,
        1: false,
        2: false,
        3: false
    };
    let currentSectionIndex = 0;

    const initializeView = () => {
        tabContents.forEach((tab, index) => {
            tab.classList.toggle('hidden', index !== currentSectionIndex);
        });
        tabValidationSymbols.forEach(tab => tab.classList.remove('hidden'))
        for (let key in validSections) {
            validSections[key] = false;
        }
    };

    const toggleNextButton = (name) => {
        nextButton.textContent = name;
        if (name === 'Submit') {
            if (Object.values(validSections).some(value => value === false)) {
                nextButton.setAttribute('disabled', true);
                summaryValidationMessage.classList.remove('hidden');
            }
        }
        else {
            nextButton.removeAttribute('disabled');
            summaryValidationMessage.classList.add('hidden');
        }
    }

    const toggleCancelButton = (name) => {
        if (name === 'show') {
            cancelButton.classList.remove('hidden');
            previousButton.classList.add('hidden');
        }
        else {
            cancelButton.classList.add('hidden');
            previousButton.classList.remove('hidden');
        }
    }

    // switch highlighted and visible tab
    const switchToTab = (previousIndex, targetIndex) => {
        if (previousIndex !== targetIndex) {
            tabs[previousIndex].classList.remove('active-tab');
            tabs[previousIndex].classList.remove('bg-gray-100');

            tabs[targetIndex].classList.add('active-tab');
            tabs[targetIndex].classList.add('bg-gray-100');

            tabContents[previousIndex].classList.add('hidden');
            tabContents[targetIndex].classList.remove('hidden');

            if (targetIndex === 0) {
                toggleCancelButton('show');
            } else {
                toggleCancelButton('hide');
            }

            if (targetIndex === 4) {
                toggleNextButton('Submit');
            }

            if (previousIndex === 4) {
                toggleNextButton('Next');
            }
        }
    }

    // validate section
    const validateCurrentSection = () => {
        if (currentSectionIndex === 1) {
            if (skillsSEmpty) {
                $('#skillsSValidation').text('This field is required.');
            }
            if (skillsTEmpty) {
                $('#skillsTValidation').text('This field is required.');
            }
            let valid = tagifyValuesValid();
            toggleValidSections(currentSectionIndex, valid);
            return valid;
        }

        let form = $('#createJobForm');
        form.validate();
        if (!form.valid() || (currentSectionIndex === 3 && !programValuesValid())) {
            if (currentSectionIndex === 3 && programsEmpty) {
                $('#programsValidation').text('This field is required.');
            }
            toggleValidSections(currentSectionIndex, false);
            return;
        }
        toggleValidSections(currentSectionIndex, true);
        return true;
    };

    const toggleValidSections = (currentIndex, value) => {
        validSections[currentIndex] = value;
        tabValidationSymbols[currentIndex].classList.toggle('hidden', value);
    }

    // next button logic
    nextButton.addEventListener('click', () => {
        if (currentSectionIndex < tabContents.length - 1) {
            validateCurrentSection();
            updateSummary(currentSectionIndex);
            switchToTab(currentSectionIndex, currentSectionIndex + 1);
            currentSectionIndex++
        }
        else {
            submitCreateJob();
        }
    });

    // previous button logic
    previousButton.addEventListener('click', () => {
        if (currentSectionIndex === 0) {
            toggleCancelButton('show');
            return;
        }
        if (currentSectionIndex !== 4) {
            validateCurrentSection();
        }
        
        updateSummary(currentSectionIndex);
        switchToTab(currentSectionIndex, currentSectionIndex - 1);
        currentSectionIndex--;
    });

    // tab buttons logic
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const target = tab.getAttribute('data-target');

            if (sectionMapping[target] !== currentSectionIndex && currentSectionIndex !== 4) {
                validateCurrentSection();
            }

            tabs[currentSectionIndex].classList.remove('active-tab', 'bg-gray-100');
            tabContents[currentSectionIndex].classList.add('hidden');

            document.getElementById(target).classList.remove('hidden');
            tab.classList.add('active-tab', 'bg-gray-100');

            updateSummary(currentSectionIndex);
            currentSectionIndex = sectionMapping[target];
            if (currentSectionIndex === 0) {
                toggleCancelButton('show');
            } else {
                toggleCancelButton('hide');
            }

            if (currentSectionIndex === tabContents.length - 1) {
                toggleNextButton('Submit');
            } else {
                toggleNextButton('Next');
            }
        });
    });
    initializeView();

    /// ------------------------------------
    /// Tagify: Initialize
    /// ------------------------------------
    function initializeTagify(inputSelector, hiddenInputsSelector, whitelist, namePrefix, validationSelector) {
        const inputElement = document.querySelector(inputSelector);
        const hiddenInputsContainer = document.getElementById(hiddenInputsSelector);

        if (!inputElement || !hiddenInputsContainer) return;

        const tagify = new Tagify(inputElement, {
            whitelist: whitelist,
            enforceWhitelist: true,
            searchKeys: ['value'],
            pattern: null,
            dropdown: {
                enabled: 0,
                maxItems: 20,
                sortby: 'startsWith',
            }
        });

        tagify.DOM.scope.classList.add(
            'block',
            'w-full',
            'rounded-lg',
            'border',
            'border-gray-300',
            'p-2',
            'focus:ring-red-800',
            'focus:border-red-800',
            'overflow-y-auto',
            'max-h-16'
        );
        
        tagify.on('change', function () {
            const selectedValues = tagify.value;
            hiddenInputsContainer.innerHTML = '';

            selectedValues.forEach((item, index) => {
                const hiddenInput = document.createElement('input');
                hiddenInput.type = 'hidden';
                hiddenInput.name = `${namePrefix}[${index}].${namePrefix.endsWith('SkillsT') || namePrefix.endsWith('SkillsC') || namePrefix.endsWith('SkillsS') ? 'SkillId' : 'ProgramId'}`;
                hiddenInput.value = item.id;
                hiddenInputsContainer.appendChild(hiddenInput);
            });
            if (inputSelector.slice(1) !== 'createJobSkillsCInput') {
                if (!selectedValues.length) {
                    $(validationSelector).text('This field is required.');
                    updateValidation(namePrefix, true);
                }
                else {
                    $(validationSelector).text('');
                    updateValidation(namePrefix, false);
                }
            }
        });
    }

    function initializeAllTagify() {
        const skillsS = window.appData.skillsS;
        const skillsT = window.appData.skillsT;
        const skillsC = window.appData.skillsC;
        const programs = window.appData.programs;

        const tagifyConfigs = [
            {
                inputSelector: '#createJobSkillsTInput',
                hiddenInputsSelector: 'skillsTHiddenInputs',
                whitelist: skillsT,
                namePrefix: 'SkillsT',
                validationSelector: '#skillsTValidation'
            },
            {
                inputSelector: '#createJobSkillsCInput',
                hiddenInputsSelector: 'skillsCHiddenInputs',
                whitelist: skillsC,
                namePrefix: 'SkillsC',
                validationSelector: '#skillsCValidation'
            },
            {
                inputSelector: '#createJobSkillsSInput',
                hiddenInputsSelector: 'skillsSHiddenInputs',
                whitelist: skillsS,
                namePrefix: 'SkillsS',
                validationSelector: '#skillsSValidation'
            },
            {
                inputSelector: '#createJobProgramsInput',
                hiddenInputsSelector: 'programsHiddenInputs',
                whitelist: programs,
                namePrefix: 'Programs',
                validationSelector: '#programsValidation'
            }
        ];

        tagifyConfigs.forEach(config => {
            initializeTagify(
                config.inputSelector,
                config.hiddenInputsSelector,
                config.whitelist,
                config.namePrefix,
                config.validationSelector
            );
        });
    }
    initializeAllTagify();
});

/// ------------------------------------
/// Validation: Tagify
/// ------------------------------------
let skillsSEmpty = true;
let skillsTEmpty = true;
let programsEmpty = true;
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
function tagifyValuesValid() { return !skillsSEmpty && !skillsTEmpty; }
function programValuesValid() { return !programsEmpty; }

/// ------------------------------------
/// Route: Create Job
/// ------------------------------------
function submitCreateJob() {
    let form = $('#createJobForm');
    const submitButton = document.getElementById('nextCreateJob');
    submitButton.disabled = true;

    form.validate();
    if (!form.valid()) {
        submitButton.disabled = false;
        return;
    }

    let formData = form.serialize();

    $.ajax({
        url: '/jobs/create',
        type: 'POST',
        data: formData,
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
}

/// ------------------------------------
/// Route: Update Job Status
/// ------------------------------------
const submitButton = document.getElementById('updateJobStatusBtn');

submitButton.addEventListener('click', () => {
    const jobStatusId = document.getElementById('updateJobStatus').value;
    const jobId = document.getElementById('updateJobStatusJobId').value;
    submitButton.disabled = true;

    $.ajax({
        url: '/jobs/update-status',
        type: 'POST',
        data: { jobId: jobId, statusId: jobStatusId },
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