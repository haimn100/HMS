window.AppEventBus = {
    onDataTableTDListeners: [],
    "onDataTableTD": function (callback) {
        AppEventBus.onDataTableTDListeners.push(callback);
    }
};


$(document).ready(function () {
    $('.dataTable tbody').on('dblclick', 'td', function () {

        var table = $(this).closest("table").DataTable();

        var row = $(this).parent();
        var rowData = table.row(row).data();
        var tdData = table.cell(this).data();
        var tdIndex = $(this).parent().find("td").index(this);

        for (var i = 0; i < AppEventBus.onDataTableTDListeners.length; i++) {
            AppEventBus.onDataTableTDListeners[i].call(this, table, rowData, tdData,tdIndex);
        }

    });

});
