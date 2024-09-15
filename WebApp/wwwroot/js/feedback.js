﻿$(document).ready(function () {
    var url = new URL(window.location);
    var showModal = url.searchParams.get('showModal');

    if (showModal === 'provideFeedback') {
        $('#feedbackModal').modal('show');
        url.searchParams.delete('showModal');
        window.history.replaceState(null, null, url);
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var contentTextarea = document.getElementById('content');
    var remainingCharsSpan = document.getElementById('remainingFeedbackChars');

    contentTextarea.addEventListener('keyup', function () {
        var remaining = 800 - this.value.length;
        remainingCharsSpan.textContent = remaining + ' characters remaining';
    });
});

var formData;
function validateFeedback() {
    formData = {
        UserId: $('#userId').val(),
        TicketId: $('#ticketId').val(),
        FeedbackRating: $('input[name="FeedbackRating"]:checked').val(),
        FeedbackContent: $('#content').val(),
    };

    var isValid = true;

    if (!formData.FeedbackRating) {
        $('#ratingError').show();
        isValid = false;
    } else {
        $('#ratingError').hide();
    }

    if (!formData.FeedbackContent) {
        $('#contentError').show();
        isValid = false;
    } else {
        $('#contentError').hide();
    }

    if (!isValid) {
        return;
    }

    $('#feedbackModal').modal('hide');
    setTimeout(function () {
        var message = 'This feedback is final';
        displayConfirmationModal(submitFeedback, message, 'feedbackModal');
    }, 250);
}

function submitFeedback() {
    $.ajax({
        url: $('#feedbackForm').data('url'),
        type: 'POST',
        data: formData,
        success: function (response) {
            $('#feedbackModal').modal('hide');
            $('#feedbackForm')[0].reset();
            formData = null;
            location.reload();
        },
        error: function () {
            formData = null;
            location.reload();
        }
    });
}
