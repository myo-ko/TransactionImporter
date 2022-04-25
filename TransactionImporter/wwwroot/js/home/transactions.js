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


    // Fetch with Status
    $('#frmStatus').on('submit', function (e) {
 
        $('#tblStatus tbody tr').remove();

        e.preventDefault();

        $.ajax({
            url: '/api/transactions/status',
            method: 'post',
            data: JSON.stringify({
                status: $('#cboStatus').val(),
            }),
            contentType: 'application/json',
            success: function (r) {
                var data = r.data;

                if (!data || data.length <= 0) {
                    $('<tr />', {
                        html: `<td colspan="3" class="text-center">No data</td>`
                    }).appendTo('#tblStatus tbody');
                    return;
                }

                $.each(data, function (index, item) {
                    $('<tr />', {
                        html: `<td>${item.transactionId}</td><td>${item.payment}</td><td>${item.status}</td>`
                    })
                    .appendTo('#tblStatus tbody');
                })
            },
            error: function (e) {
                alert('An error occurred.');
            },
        });

    })

    // Fetch with Currency
    $('#frmCurrency').on('submit', function (e) {

        $('#tblCurrency tbody tr').remove();

        e.preventDefault();

        $.ajax({
            url: '/api/transactions/currency',
            method: 'post',
            data: JSON.stringify({
                currency: $('#cboCurrency').val(),
            }),
            contentType: 'application/json',
            success: function (r) {
                var data = r.data;

                if (!data || data.length <= 0) {
                    $('<tr />', {
                        html: `<td colspan="3" class="text-center">No data</td>`
                    }).appendTo('#tblCurrency tbody');
                    return;
                }

                $.each(data, function (index, item) {
                    $('<tr />', {
                        html: `<td>${item.transactionId}</td><td>${item.payment}</td><td>${item.status}</td>`
                    }).appendTo('#tblCurrency tbody');
                })
            },
            error: function (e) {
                alert(e.responseJSON.message ?? 'An error occurred.');
            },
        });

    })

    // Fetch with Date Range
    $('#frmDate').on('submit', function (e) {

        $('#tblDate tbody tr').remove();

        e.preventDefault();

        $.ajax({
            url: '/api/transactions/date-range',
            method: 'post',
            data: JSON.stringify({
                start: $('#dtpStart').val(),
                end: $('#dtpEnd').val(),
            }),
            contentType: 'application/json',
            success: function (r) {
                var data = r.data;

                if (!data || data.length <= 0) {
                    $('<tr />', {
                        html: `<td colspan="3" class="text-center">No data</td>`
                    }).appendTo('#tblDate tbody');
                    return;
                }

                $.each(data, function (index, item) {
                    $('<tr />', {
                        html: `<td>${item.transactionId}</td><td>${item.payment}</td><td>${item.status}</td>`
                    }).appendTo('#tblDate tbody');
                })
            },
            error: function (e) {
                alert(e.responseJSON.message ?? 'An error occurred.');
            },
        });

    })

    // Get all currencies for currency choice
    $.ajax({
        url: '/api/currency/all',
        method: 'get',
        success: function (r) {
            var currencies = r.data;
            if (currencies.length > 0) {
                $.each(currencies, function (index, item) {
                    $('<option >', { value: item, text: item }).appendTo('#cboCurrency');
                })
            }
        }
    })
})(jQuery)