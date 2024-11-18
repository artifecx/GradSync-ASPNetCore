$(document).ready(function () {
    /// ------------------------------------
    /// Back Button & Url Manipulation
    /// ------------------------------------
    const urlParams = new URLSearchParams(window.location.search);
    sessionStorage.setItem('sortBy', urlParams.get('sortBy') || '');
    sessionStorage.setItem('filterByEmploymentType', JSON.stringify(urlParams.getAll('filterByEmploymentType')) || '[]');
    sessionStorage.setItem('filterByStatusType', urlParams.get('filterByStatusType') || '');
    sessionStorage.setItem('filterByWorkSetup', JSON.stringify(urlParams.getAll('filterByWorkSetup')) || '[]');
    sessionStorage.setItem('search', urlParams.get('search') || '');
    sessionStorage.setItem('pageIndex', urlParams.get('pageIndex') || '1');
    sessionStorage.setItem('filterByDatePosted', urlParams.get('filterByDatePosted') || '');
    sessionStorage.setItem('filterBySalary', urlParams.get('filterBySalary') || '');

    if (urlParams.get('showModal') === 'editJob') {
        $('#editJobModal').modal('show');
    }

    const idParam = urlParams.get('id') ? `?id=${urlParams.get('id')}` : '';
    const newUrl = window.location.origin + window.location.pathname + idParam;
    window.history.replaceState({}, document.title, newUrl);

    document.querySelector('#back-button').addEventListener('click', function (event) {
        event.preventDefault();
        const baseUrl = $('#baseUrl').val();
        const params = new URLSearchParams();
        const filterByEmploymentType = JSON.parse(sessionStorage.getItem('filterByEmploymentType') || '[]');
        const filterByWorkSetup = JSON.parse(sessionStorage.getItem('filterByWorkSetup') || '[]');
        filterByEmploymentType.forEach(employmentType => {
            params.append('filterByEmploymentType', employmentType);
        });
        filterByWorkSetup.forEach(workSetup => {
            params.append('filterByWorkSetup', workSetup);
        });
        params.set('sortBy', sessionStorage.getItem('sortBy') || '');
        params.set('filterByStatusType', sessionStorage.getItem('filterByStatusType') || '');
        params.set('search', sessionStorage.getItem('search') || '');
        params.set('pageIndex', sessionStorage.getItem('pageIndex') || '1');
        params.set('filterByDatePosted', sessionStorage.getItem('filterByDatePosted') || '');
        params.set('filterBySalary', sessionStorage.getItem('filterBySalary') || '');
        window.location.href = `${baseUrl}?${params.toString()}`;
    });
});

document.addEventListener('DOMContentLoaded', function () {
    /// ------------------------------------
    /// Listener: Close Edit Job Modal
    /// ------------------------------------
    let cancelClicked = false;
    const cancelButtons = ["closeEditJobModal", "cancelEditJob"];
    cancelButtons.forEach(id => {
        const button = document.getElementById(id);
        if (button) {
            button.addEventListener("click", () => {
                cancelClicked = true;
            });
        }
    });
    $('#editJobModal').on('hidden.bs.modal', function () {
        if (cancelClicked) {
            editJobForm.reset();
            $(".text-red-500").text('');
            addCharCountListener('editJobTitle', 'remainingTitleChars', 100);
            addCharCountListener('editJobDescription', 'remainingDescriptionChars', 800);
            initializeAllTagify();
            updateSkillWeights();
        }
        cancelClicked = false;
    });

    /// ------------------------------------
    /// Listener: Skills weight
    /// ------------------------------------
    const skillWeightsRange = document.getElementById("skillWeights_range");
    const customSkillWeightsSpan = document.getElementById("skillWeights");
    const updateSkillWeights = () => {
        const rangeValue = parseFloat(skillWeightsRange.value);
        const culturalPercentage = ((1 - rangeValue) * 100).toFixed(0);
        const technicalPercentage = (rangeValue * 100).toFixed(0);
        customSkillWeightsSpan.textContent = `${culturalPercentage}% - ${technicalPercentage}%`;
    };
    updateSkillWeights();
    skillWeightsRange.addEventListener("input", updateSkillWeights);

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
    addCharCountListener('editJobTitle', 'remainingTitleChars', 100);
    addCharCountListener('editJobDescription', 'remainingDescriptionChars', 800);

    /// ------------------------------------
    /// Tagify: Initialize
    /// ------------------------------------
    function initializeTagify(inputSelector, hiddenInputsSelector, whitelist, namePrefix, validationSelector, existingTags) {
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
                sortby: 'startsWith'
            },
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

        tagify.addTags(existingTags);

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
            if (inputSelector.slice(1) !== 'editJobSkillsCInput') {
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
        const existingSkillsS = window.appData.existingSkillsS;
        const existingSkillsT = window.appData.existingSkillsT;
        const existingSkillsC = window.appData.existingSkillsC;
        const existingPrograms = window.appData.existingPrograms;
        const skillsS = window.appData.skillsS;
        const skillsT = window.appData.skillsT;
        const skillsC = window.appData.skillsC;
        const programs = window.appData.programs;

        const tagifyConfigs = [
            {
                inputSelector: '#editJobSkillsTInput',
                hiddenInputsSelector: 'skillsTHiddenInputs',
                whitelist: skillsT,
                namePrefix: 'SkillsT',
                validationSelector: '#skillsTValidation',
                existingTags: existingSkillsT
            },
            {
                inputSelector: '#editJobSkillsCInput',
                hiddenInputsSelector: 'skillsCHiddenInputs',
                whitelist: skillsC,
                namePrefix: 'SkillsC',
                validationSelector: '#skillsCValidation',
                existingTags: existingSkillsC
            },
            {
                inputSelector: '#editJobSkillsSInput',
                hiddenInputsSelector: 'skillsSHiddenInputs',
                whitelist: skillsS,
                namePrefix: 'SkillsS',
                validationSelector: '#skillsSValidation',
                existingTags: existingSkillsS
            },
            {
                inputSelector: '#editJobProgramsInput',
                hiddenInputsSelector: 'programsHiddenInputs',
                whitelist: programs,
                namePrefix: 'Programs',
                validationSelector: '#programsValidation',
                existingTags: existingPrograms
            }
        ];

        tagifyConfigs.forEach(config => {
            initializeTagify(
                config.inputSelector,
                config.hiddenInputsSelector,
                config.whitelist,
                config.namePrefix,
                config.validationSelector,
                config.existingTags
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
    }
}
function tagifyValuesValid() {
    return !skillsSEmpty && !skillsTEmpty && !programsEmpty;
}

/// ------------------------------------
/// Route: Submit Edit Job
/// ------------------------------------
function submitEditJob() {
    let form = $('#editJobForm');

    form.validate();
    if (!form.valid() || !tagifyValuesValid()) {
        return;
    }

    let formData = form.serialize();

    $.ajax({
        url: '/jobs/update',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                let errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "An unexpected error occurred.";
            toastr.error(errorMessage);
        }
    });
}

/// ------------------------------------
/// Route: Submit Application
/// ------------------------------------
async function applyJobHandler(jobId) {
    const actionUrl = $('#actionUrl').val();

    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: { jobId: jobId },
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                let errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "An unexpected error occurred.";
            toastr.error(errorMessage);
        }
    });
}

/// ------------------------------------
/// Route: Withdraw Application
/// ------------------------------------
async function withdrawApplicationHandler(applicationId) {
    const actionUrl = $('#withdrawUrl').val();

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
                let errorMessage = response.error || "An error occurred.";
                toastr.error(errorMessage);
            }
        },
        error: function (xhr, status, error) {
            let errorMessage = xhr.responseJSON && xhr.responseJSON.error ? xhr.responseJSON.error : "An unexpected error occurred.";
            toastr.error(errorMessage);
        }
    });
}