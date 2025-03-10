var dataTable;


$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
       
        order: [[2, 'desc']],
        "ajax": {
            url: '/Customer/Appointment/GetUserAppointments',
            dataSrc: 'data' 
        },
        "columns": [
            {
                "data": "service.company.name", 
                "width": "20%"
            },
            {
                "data": "service.name", 
                "width": "20%"
            },
            {
                "data": "date", 
                "width": "15%"
            },
            {
                "data": "time", 
                "width": "15%"
            },
            {
                "data": "status", 
                "render": function (data) {
                    var statusClass = '';
                    var statusText = '';

                    switch (data) {
                        case 0: statusClass = 'btn-outline-warning'; statusText = 'Pending'; break;
                        case 1: statusClass = 'btn-outline-primary'; statusText = 'Paid'; break;
                        case 2: statusClass = 'btn-outline-success'; statusText = 'Confirmed'; break;
                        case 3: statusClass = 'btn-success'; statusText = 'Completed'; break;
                        case 4: statusClass = 'btn-outline-info'; statusText = 'No Show'; break;
                        case 5: statusClass = 'btn-outline-danger'; statusText = 'Cancelled'; break;
                        default: statusClass = 'btn-outline-secondary'; statusText = 'Unknown';
                    }

                    return `<div class="w-75 btn-group" role="group">
                                <button class="btn ${statusClass}" style="pointer-events: none;">
                                    ${statusText}
                                </button>
                            </div>`;
                },
                "width": "15%"
            },
            {
                "data": "id", 
                "render": function (data, type, row) {
                    var actionButtons = `<div class="w-75 btn-group" role="group">`;

                    if (row.status === 0 && row.service.isPrepaymentRequired) {
                        actionButtons += `<a href="/Customer/Appointment/Payment?id=${data}" class="btn btn-outline-info mx-2">Payment<i class="bi bi-credit-card"></i></a>`;
                    } else if (row.status === 0 && !row.service.isPrepaymentRequired) {
                        actionButtons += `<a href="/Customer/Appointment/Details?id=${data}" class="btn btn-primary mx-2">Click to confirm <i class="bi bi-check-lg"></i></a>`;
                    } else  {
                        actionButtons += `<a href="/Customer/Appointment/Details?id=${data}" class="btn btn-outline-primary mx-2">Details <i class="bi bi-search"></i></a>`;
                    }
                    return actionButtons;
                },
                "width": "30%"
            }
        ],
        initComplete: function () {
            this.api()
                .columns([0,1,2,3])
                .every(function () {
                    var column = this;
                    var select = $('<select class="w-75"><option value=""></option></select>')
                        .appendTo($(column.footer()).empty())
                        .on('change', function () {
                            var val = $.fn.dataTable.util.escapeRegex($(this).val());

                            column.search(val ? '^' + val + '$' : '', true, false).draw();
                        });

                    column
                        .data()
                        .unique()
                        .sort()
                        .each(function (d) {
                            select.append('<option value="' + d + '">' + d + '</option>');
                        });
                });
        }
    });
}

