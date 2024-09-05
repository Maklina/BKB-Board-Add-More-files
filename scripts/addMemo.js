$(document).ready(function () {
    $(document).on("click", ".addMore01", function () {
        debugger;
        var rowCount = $('.memoRows').length + 1;
        var memoRows = '<tr class="memoRows">' +
            '<td>' +
            '<input type="text" id="slNo' + rowCount + '" value="' + rowCount + '" name="slNo' + rowCount + '" class="form-control input-sm slNo01" readonly/>' +
            '</td>' +
            '<td>' +
            '<input type="text" id="subject' + rowCount + '" name="MemoSubject" class="form-control input-sm subject01" required/>' +
            '</td>' +
            '<td>' +
            '<select name="department" id="department' + rowCount + '" class="form-control input-sm department01" required>' +
            '</select>' +
            '</td>' +
            '<td>' +
            '<input type="file" id="Memofile' + rowCount + '" name="Memofile" class="form-control input-sm memofile01"/>' +
            '</td>' +
            '<td>' +
            '<input type="button" id="remove" name="remove" value="Remove" class="form-control btn-danger input-sm remove01" />' +
            '</td>' +
            '</tr>';

        // Append the new row to the table
        $('#memoTable').append(memoRows);

        // Fetch and populate the dropdown options
        $.ajax({
            type: 'POST',
            url: "/Meeting/getDepartment",
            dataType: 'json',
            data: {},
            success: function (response) {
                var departmentDropdown = $("#department" + rowCount);
                departmentDropdown.empty(); // Clear existing options
                departmentDropdown.append('<option value="">---Select department---</option>');
                $.each(response, function (i, response) {
                    departmentDropdown.append('<option value="' + response.Value + '">' + response.Text + '</option>');
                });

                // Initialize Select2 after options are added
                departmentDropdown.select2();
            },
            error: function (ex) {
                alert('Failed to load departments.' + ex);
            }
        });
        $("#department" + rowCount).val($("#department" + rowCount + " option:selected").val());
        $("#department" + rowCount).select2();

        //taking local storage start


        $("#subject" + rowCount).focusout(function () {
            localStorage.setItem("subject" + rowCount, $("#subject" + rowCount).val());
        });

        $("#subject" + rowCount).val(localStorage.getItem("subject" + rowCount));


        //taking local storage end
    });

    $(document).on("click", ".remove01", function () {
        $(this).closest("tr").remove();
        updateSerialNumbers(); // Update serial numbers after removing a row
    });

    // Function to update serial numbers
    function updateSerialNumbers() {
        $('.memoRows').each(function (index) {
            $(this).find('.slNo01').val(index + 1);
        });
    }
});