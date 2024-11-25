document.addEventListener('DOMContentLoaded', function () {
    /// ------------------------------------
    /// Listener: Resume Upload
    /// ------------------------------------
    const resumeInput = document.getElementById('registrationUploadResume');
    const resumeValidationMessage = document.getElementById('resumeValidation');
    const uploadResumeText = document.getElementById('uploadResumeText');

    resumeInput.addEventListener('change', validateResume);
    resumeInput.addEventListener('blur', validateResume);

    function validateResume() {
        const file = resumeInput.files[0];
        const maxSizeMB = 5;
        const maxSizeBytes = maxSizeMB * 1024 * 1024;

        if (!file) {
            resumeValidationMessage.textContent = 'Resume is required.';
            resumeValidationMessage.classList.remove('hidden');
            updateValidation("Resume", true);
            uploadResumeText.textContent = 'Upload';
            return;
        }

        const fileName = file.name;
        const fileExtension = fileName.slice(fileName.lastIndexOf('.')).toLowerCase();
        const displayName = fileName.length > 20
            ? `${fileName.slice(0, 17)}...${fileExtension}`
            : fileName;

        let validationMessage = '';
        if (fileExtension !== '.pdf') {
            validationMessage = 'File type is not PDF.';
        } else if (file.size > maxSizeBytes) {
            validationMessage = `File size exceeds ${maxSizeMB} MB.`;
        }

        if (validationMessage) {
            resumeValidationMessage.textContent = validationMessage;
            resumeValidationMessage.classList.remove('hidden');
            updateValidation("Resume", true);
        } else {
            resumeValidationMessage.textContent = '';
            resumeValidationMessage.classList.add('hidden');
            updateValidation("Resume", false);
        }

        uploadResumeText.textContent = displayName;
    }


    /// ------------------------------------
    /// Listener: Update Job Status
    /// ------------------------------------
    const departments = window.appData.departments;
    const programs = window.appData.programs;

    const collegeDropdown = document.getElementById('registrationCollegeId');
    const departmentDropdown = document.getElementById('registrationDepartmentId');
    const programDropdown = document.getElementById('registrationProgramId');

    collegeDropdown.addEventListener('change', function () {
        const selectedCollegeId = this.value;

        departmentDropdown.innerHTML = '<option value="">-- Select Department --</option>';
        programDropdown.innerHTML = '<option value="">-- Select Program --</option>';
        departmentDropdown.disabled = true;
        programDropdown.disabled = true;

        if (selectedCollegeId) {
            const filteredDepartments = departments.filter(dept => dept.collegeId === selectedCollegeId);
            filteredDepartments.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.departmentId;
                option.textContent = dept.name;
                departmentDropdown.appendChild(option);
            });
            departmentDropdown.disabled = false;
        }
    });

    departmentDropdown.addEventListener('change', function () {
        const selectedDepartmentId = this.value;

        programDropdown.innerHTML = '<option value="">-- Select Program --</option>';
        programDropdown.disabled = true;

        if (selectedDepartmentId) {
            const filteredPrograms = programs.filter(prog => prog.departmentId === selectedDepartmentId);
            filteredPrograms.forEach(prog => {
                const option = document.createElement('option');
                option.value = prog.programId;
                option.textContent = prog.name;
                programDropdown.appendChild(option);
            });
            programDropdown.disabled = false;
        }
    });

    /// ------------------------------------
    /// Summary: Update Summary
    /// ------------------------------------
    function updateSummary(index) {
        switch (index) {
            case 0:
                updateEducationalDetails();
                return;
            case 1:
                updateSkills();
                return;
            default:
                return;
        }

        // Educational Details
        function updateEducationalDetails() {
            document.getElementById('summaryIdNumber').innerText = document.getElementById('registrationIdNumber').value || '-';
            document.getElementById('summaryYearLevelId').innerText =
                document.getElementById('registrationYearLevelId').selectedOptions[0]?.text || '-';
            document.getElementById('summaryCollegeId').innerText =
                document.getElementById('registrationCollegeId').selectedOptions[0]?.text || '-';
            document.getElementById('summaryDepartmentId').innerText =
                document.getElementById('registrationDepartmentId').selectedOptions[0]?.text || '-';
            document.getElementById('summaryProgramId').innerText =
                document.getElementById('registrationProgramId').selectedOptions[0]?.text || '-';
        }

        // Skills
        function updateSkills() {
            const skillsSInput = document.getElementById('registrationSkillsSInput').value;
            const skillsTInput = document.getElementById('registrationSkillsTInput').value;
            const skillsCInput = document.getElementById('registrationSkillsCInput').value;
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
        }
    }
    
    /// ------------------------------------
    /// Form Handling: Create Job
    /// ------------------------------------
    const tabs = document.querySelectorAll('.tab-btn-registration');
    const tabContents = document.querySelectorAll('.tab-content-registration');
    const tabValidationSymbols = document.querySelectorAll('.tab-btn-registration-validation');
    const nextButton = document.querySelector('#nextCompleteRegistration');
    const previousButton = document.getElementById('backCompleteRegistration');
    const summaryValidationMessage = document.getElementById('summaryValidation');
    const sectionMapping = {
        "educational-details-section": 0,
        "skills-section": 1,
        "summary-section": 2
    };
    const validSections = {
        0: false,
        1: false
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

    const togglePreviousButton = (name) => {
        if (name === 'show') {
            previousButton.classList.add('hidden');
        }
        else {
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
                togglePreviousButton('show');
            } else {
                togglePreviousButton('hide');
            }

            if (targetIndex === 2) {
                toggleNextButton('Submit');
            }

            if (previousIndex === 2) {
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

        let form = $('#registrationForm');
        form.validate();
        if (!form.valid() || (currentSectionIndex === 0 && !resumeValid())) {
            if (currentSectionIndex === 0) validateResume();
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
            submitCompleteRegistration();
        }
    });

    // previous button logic
    previousButton.addEventListener('click', () => {
        if (currentSectionIndex === 0) {
            togglePreviousButton('show');
            return;
        }
        if (currentSectionIndex !== 2) {
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

            if (sectionMapping[target] !== currentSectionIndex && currentSectionIndex !== 2) {
                validateCurrentSection();
            }

            tabs[currentSectionIndex].classList.remove('active-tab', 'bg-gray-100');
            tabContents[currentSectionIndex].classList.add('hidden');

            document.getElementById(target).classList.remove('hidden');
            tab.classList.add('active-tab', 'bg-gray-100');

            updateSummary(currentSectionIndex);
            currentSectionIndex = sectionMapping[target];
            if (currentSectionIndex === 0) {
                togglePreviousButton('show');
            } else {
                togglePreviousButton('hide');
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
                hiddenInput.name = `${namePrefix}[${index}].${'SkillId'}`;
                hiddenInput.value = item.id;
                hiddenInputsContainer.appendChild(hiddenInput);
            });
            if (inputSelector.slice(1) !== 'registrationSkillsCInput') {
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

        const tagifyConfigs = [
            {
                inputSelector: '#registrationSkillsTInput',
                hiddenInputsSelector: 'skillsTHiddenInputs',
                whitelist: skillsT,
                namePrefix: 'SkillsT',
                validationSelector: '#skillsTValidation'
            },
            {
                inputSelector: '#registrationSkillsCInput',
                hiddenInputsSelector: 'skillsCHiddenInputs',
                whitelist: skillsC,
                namePrefix: 'SkillsC',
                validationSelector: '#skillsCValidation'
            },
            {
                inputSelector: '#registrationSkillsSInput',
                hiddenInputsSelector: 'skillsSHiddenInputs',
                whitelist: skillsS,
                namePrefix: 'SkillsS',
                validationSelector: '#skillsSValidation'
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
let resumeInvalidOrEmpty = true;
function updateValidation(name, bool) {
    if (name === "SkillsS") {
        skillsSEmpty = bool;
    }
    else if (name === "SkillsT") {
        skillsTEmpty = bool;
    }
    else if (name === "Resume") {
        resumeInvalidOrEmpty = bool;
    }
}
function tagifyValuesValid() { return !skillsSEmpty && !skillsTEmpty; }
function resumeValid() { return !resumeInvalidOrEmpty; }

/// ------------------------------------
/// Route: Complete Registration
/// ------------------------------------
function submitCompleteRegistration() {
    let form = $('#registrationForm');
    const submitButton = document.getElementById('nextCompleteRegistration');
    submitButton.disabled = true;

    form.validate();
    if (!form.valid()) {
        submitButton.disabled = false;
        return;
    }

    let formData = new FormData(form[0]);

    $.ajax({
        url: '/welcome-complete',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                window.location.href = '/home';
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