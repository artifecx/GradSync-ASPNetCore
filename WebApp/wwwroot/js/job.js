$(document).ready(function () {
    var url = new URL(window.location);
    var showModal = url.searchParams.get('showModal');

    if (showModal === 'editJob') {
        $('#editJobModal').modal('show');
        url.searchParams.delete('showModal');
        window.history.replaceState(null, null, url);
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