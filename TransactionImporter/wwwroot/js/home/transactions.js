(function () {

    function showError(message) {
        $('<span />', { text: message }).appendTo('#error-region');
        $('#error-region').show();
    }

    function hideError() {
        $('#error-region').html('').hide();
    }

    $('#customFile').on('change', function () {
        hideError();
    })

    $('#my-form').on('submit', function (e) {

        e.preventDefault();

        var formData = new FormData();
        var files = $('#customFile')[0].files;

        if (files.length <= 0) {
            showError('Please choose a file');
            return false;
        }

        formData.append('file', files[0]);

        $.ajax({
            url: '/api/transactions',
            method: 'post',
            data: formData,
            processData: false,
            contentType: false,
            success: function (r) {
                alert('Success! ' + r.message);
            },
            error: function (e) {
                showError(e.responseJSON.message);
            },
        });

        return false;
    })

})(jQuery)