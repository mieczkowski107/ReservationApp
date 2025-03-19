var dataTable;

$(document).ready(function () {
    var companyId = window.location.pathname.split('/').pop();
    companyId = parseInt(companyId, 10);

    loadDataTable(companyId);

    // Date filter buttons
    $('#filterAll').click(function () {
        filterByDate('all');
    });

    $('#filterPast').click(function () {
        filterByDate('past');
    });

    $('#filterFuture').click(function () {
        filterByDate('future');
    });
});

function loadDataTable(companyId) {
    dataTable = $('#tblData').DataTable({
        order: [[2, 'desc']],
        "ajax": {
            url: `/Admin/Appointment/GetCompanyAppointments/${companyId}`,
            dataSrc: 'data'  
        },
        "columns": [
            {
                "data": "serviceName",  
                "width": "20%"
            },
            {
                "data": "date", 
                "width": "20%"
            },
            {
                "data": "time",  
                "width": "15%"
            },
            {
                "data": "userFirstName",  
                "width": "15%"
            },
            {
                "data": "userLastName",
                "width": "15%"
            },
            {
                "data": "userEmail",
                "width": "15%"
            },
            {
                "data": "userPhoneNumber",
                "width": "15%"
            },
            {
                "data": "appointmentStatus",  
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

                    if (row.appointmentStatus === 0 && row.isPrepaymentRequired) {
                        actionButtons += `<a href="/Customer/Appointment/Payment?id=${data}" 
                                             class="btn btn-sm btn-outline-info mx-2">
                                             Payment<i class="bi bi-credit-card"></i>
                                         </a>`;
                    } else if (row.appointmentStatus === 0 && !row.isPrepaymentRequired) {
                        actionButtons += `<a href="/Customer/Appointment/Details?id=${data}"
                                             class="btn btn-sm btn-primary mx-2">
                                             Click to confirm <i class="bi bi-check-lg"></i>
                                          </a>`;
                    } else {
                        actionButtons += `<a href="/Admin/Appointment/Details?id=${data}"
                                             class="btn btn-sm btn-outline-primary mx-2 w-auto">
                                             Details <i class="bi bi-search"></i>
                                         </a>`;
                    }

                    return actionButtons;
                },
                "width": "30%"
            }
        ],
        initComplete: function () {
            this.api()
                .columns([0, 1, 2, 3,4,5,6,7])
                .every(function () {
                    var column = this;
                    var select = $('<select class="w-75"><option value=""></option></select>')
                        .appendTo($(column.header()))
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
function filterByDate(filterType) {
    var today = new Date();
    today.setHours(0, 0, 0, 0);

    function parseDate(dateString) {
        var parts = dateString.split('-');
        return new Date(parts[0], parts[1] - 1, parts[2]);
    }

    $.fn.dataTable.ext.search.pop();

    if (filterType === 'all') {
        dataTable.draw();
    } else {
        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var rowDate = parseDate(data[1]);

            if (filterType === 'past') {
                return rowDate < today;
            } else if (filterType === 'future') {
                return rowDate >= today;
            }
            return true;
        });

        dataTable.draw();
    }
}