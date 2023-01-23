let cachedTables = {};

$(document).ready(function () {

    //Legacy > replaced by _globalCreateDataTable(options) instead of window global options
    if (window.dtOptions) {
        let options = window.dtOptions[0];
        _globalCreateDataTable(options);
    }
});

/* 
    options: 
    {
        serverSide: bool,
        searching: bool //add search input
        order: [[1, "desc"]]
        noFooter: bool (false)
        buttons
        columns,
        pageLength
     }
        
    returns:
    {
        getTable: function
        getColDef: function
        getCellData: function
        updateCellData: function
        openRowEditModal: function
    }

}
 */
function _globalCreateDataTable(options) {
    let table;
    let srv;

    configureColumns();
    fillFiltersFromUrl();
    displayFilters();

    var buttons = options.defaultButtons || ['copy', 'csv', 'excel', 'pdf', 'print'];

    //add an add record button if the user specified an api endpoint to add a resource
    if (options.ajax.add) {
        buttons.unshift({
            text: 'Add Record',
            action: function (e, dt, node, config) {
                _dt_editRow(-1,options.el);
            }
        });
    }

    //adding and configuaring custom buttons to the array of default buttons
    if (options.buttons) {
        $.each(options.buttons, function (bi, button) {
            buttons.unshift({
                text: button.title,
                //params: element,dataTable,htmlNode,dataTableConfig
                action: function (e, dt, node, config) {
                    button.action.call(this, e, dt, node, config);
                }
            });
        });
    }
   
    var tableOptions = {
        "destroy":options.destroy || true,
        "pageLength": options.pageLength || 15,
        "processing": true,
        "serverSide": _.isUndefined(options.serverSide) ? true : options.serverSide,
        "searching": _.isUndefined(options.searching) ? true : options.searching,
        "order": options.order || [[1, "desc"]],       
        "dom":  'Bfrtip',
        "buttons": buttons,
        "columns": options.columns,
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            var params = $.extend({}, options.params);
            params.sortBy = oSettings.aoColumns[oSettings.aaSorting[0][0]].data;
            params.sortDesc = oSettings.aaSorting[0][1] === "desc";

            if (aoData.length > 0) {
                params.start = aoData[3].value;
                params.length = aoData[4].value;
                params.search = aoData[5].value.value;
            }

            if (oSettings.ajax) {
                oSettings.jqXHR = $.ajax({
                    "type": "GET",
                    "url": oSettings.ajax.url,
                    "data": params,
                    "success": fnCallback,
                    "error": (xhr, status) => {
                        alert(xhr.responseText);
                    }
                });
            }
        }  
    };

    if (options.ajax.read) {
        tableOptions.ajax =  {
            url: options.ajax.read,
            dataSrc: function (json) {
                $.each(json.data, function (i, row) {
                    $.each(row, function (key, value) {
                        if (_.isString(value) && value.toLowerCase().startsWith("/date(")) {
                            var newVal = value.toLowerCase().replace("\/date(", "").replace(")\/", "");
                            // get the unix time portiob from the asp.net date format(/date(000000)/) 
                            row[key] = Number(newVal);
                        }
                    });
                });
                return json.data;
            }
        }
    }

    if (!_.isUndefined(options.data)) {
        tableOptions['data'] = options.data;
        tableOptions['processing'] = true;
    }

    var thead = '<thead><tr>';
    $.each(options.columns, function (idx, col) { thead += '<th>' + (col.title || '') + '</th>'; });
    thead += '</tr></thead>';
    $("#" + options.el).append(thead);

    if (!_.isUndefined(options.filters)) {
        var urlWithFilters = getUrlWithFilters();
        tableOptions['ajax'].url = urlWithFilters;
    }

    if (!_.isUndefined(options.totalsColumn)) {
        addTotalsFooter(i, tableOptions);
    }

    if (options.rowCallback) {
        tableOptions['rowCallback'] = options.rowCallback;
    }

    let ready = false;
    if (options.onReady) {
 
        tableOptions["initComplete"] =  (settings, json) => {
            if (!ready) {
                ready = true;
                try {
                    options.onReady.call(this,srv);
                } catch (e) { alert(e);}
            }            
        }
    }


    table = $("#" + options.el).DataTable(tableOptions);
    srv = createTableService(table, options); 
    cachedTables[options.el] = srv;

   
   
    window['_dt_editRow'] = (rowIndex, tableElementId) => {       

        let tableService = cachedTables[tableElementId];
        var table = tableService.getTable();
        tableService.openRowEditModal(rowIndex);

        $frm = $('#dt_rowModalForm');
        $frm.unbind('submit');
        $frm.submit(function (e) {
            e.preventDefault();

            var form = $(this);
            var url = form.attr('action');

            $.ajax({
                type: "POST",
                url: url,
                async: false,
                data: form.serialize(),
                success: function (data) {
                    $('#__tblRowModal').modal('hide');
                    table.ajax.reload();
                },
                error: function (xhr, error) {
                    alert(xhr.statusText);                  
                }
            });
        });
    };
    window['_dt_deleteRow'] = () => {       
        if (confirm('Are you sure?')) {

            $frm = $('#dt_rowModalForm');
            $frm.attr('action', options.ajax.delete);
            $frm.submit();
          
        }

    };
    window['_dt_onFilterChange'] = () => {

        var url = getUrlWithFilters();
        var table = $("#" + options.el).DataTable();
        table.ajax.url(url).load();
    };
    window['_dt_cell_click'] = (cell) => {

        $td = $(cell).closest('td');
        $row = $(cell).closest('tr');

        let colIndex = table.column.index('fromVisible', $td.index());

        let columnDefinition = options.columns[colIndex];
        if (!columnDefinition.onCellClick) return;

        columnDefinition.onCellClick(
            createTableService(table,options),
            table.row($row).data(),
            $row.index()
        );
    };

    function getUrlWithFilters() {

        var params = [];
        $.each(options.filters, function (i, filter) {
            var val = $("#filter_" + filter.id).val();
            params.push(filter.param + "=" + val);
        });
        var qstring = params.join("&");
        var url = options['ajax'].read;
        if (_.isEmpty(qstring)) return url;

        url.indexOf('?') !== -1 ? url += qstring : url += "?" + qstring;
        return url;
    }

    function getFilterValue(filterId) {      
        return $("#filter_" + filterId).val();        
    }

    function configureColumns() {

        if (options.ajax.update) {
            options.columns.unshift({
                sortable: false,
                name: '_ajaxUpdate',
                data: null,
                width: '16px',
                render: function (data, type, row, meta) {
                    return `<img class='dt-row-edit rotate-hover-45' width='22' src='/images/pencil.png' onclick="_dt_editRow(${meta.row},'${options.el}')"/>`;
                }
            });
        }
        let columnDTypes = GetColumnDTypes();

        $.each(options.columns, (colIndex, col) => {

            if (!_.isUndefined(col.list)) { 
                (function (_col) {

                    let idField = _col.list.idField || 'id';
                    let nameField = _col.list.nameField || 'name';      
                    let fetchUrl = null;
                    if (_.isObject(_col.list)) fetchUrl = _col.list.url;
                    if (_.isString(_col.list)) fetchUrl = _col.list;
                    
                    if (fetchUrl) {
                        $.ajax({
                            async: false,
                            url: fetchUrl,
                            success: function (result) {
                                if (result !== null) {
                                    _col.listData = _.map(result, function (item) {
                                        return {
                                            id: item[idField],
                                            name: item[nameField]
                                        };
                                    });
                                    if (!_.isUndefined(col.list.extra)) {
                                        _col.listData.unshift(...col.list.extra);
                                    }
                                }
                            }
                        });
                    } else {
                        _col.listData = _col.list.data || _col.list || [];
                    }
  
                    _col.render = function (data, type, row, meta) {
                        var listItem = _.find(_col.listData, function (item) { return item.id === data; });
                        if (listItem) {
                            let html = listItem.name;
                            if (listItem.html) html = listItem.html.replace(/{name}/g, listItem.name);
                            return html;
                        }
                        return data;
                    };
                })(col);
            }
            else {
                col.render = columnDTypes[col.dType] || col.render;                
            }

            if (col.numberFormat) {
                col.render = function (data, type, row, meta) {
                    let format = col.numberFormat.split(':');
                    return data.toLocaleString(
                        format[0], // leave undefined to use the browser's locale,
                        // or use a string like 'en-US' to override it.
                        { minimumFractionDigits: parseInt(format[1]) }
                    );
                };
            }

            if (col.image) {
                col.render = (data, type, row, meta) => {
                    return `<img class='col-image' width='${col.image.width || 'auto'}' src='${col.image.src}'/>`;
                };
            }

            if (col.html) {
                col.render = () => {
                    return col.html;
                };
            }

            if (col.onCellClick) {
                col.render = (data, type, row, meta) => {
                    return `<div onclick='_dt_cell_click(this);' style='cursor:pointer;width:100%;height:100%;'>${col.html || data || ''}</div>`;
                };
            }
        });
    }

    function fillFiltersFromUrl() {
        if (_.isUndefined(options.filters)) return;

        let allowedFiltersParams = ['to', 'from'];
        let urlSearch = window.location.search.replace('?', '');
        let params = urlSearch.length > 0 ? urlSearch.split('&') : [];

        _.each(params, (param) => {
            let urlParamNameValue = param.split('=');
            let urlFilterParam = urlParamNameValue[0];
            let urlFilterVal = urlParamNameValue[1];

            if (_.contains(allowedFiltersParams, urlFilterParam)) {
                let matchedFilter = _.find(options.filters, (filter) => { return filter.param === urlFilterParam; });
                if (matchedFilter) matchedFilter.val = urlFilterVal;
            }
        });
    }

    /**
     * Filter object = {
     *      title,
     *      type: 'select',
     *      param: string # the query param to attach the option value in the table query url
     *      options: [{id,name}] or a url to load options from
     *      emptyValue,
     *      emptyText,
     *      val # inital value
     * }
     * */
    function displayFilters() {

        if (_.isUndefined(options.filters)) return;
        var result = "<div class='tbl_filters'>";
        result = `<div style='display:flex;margin-bottom:15px;'>`;
        $.each(options.filters, function (index, filter) {
            filter.id = filter.id || Math.floor(Math.random() * 1000000).toString();
            result += '<div>';
            result += `<div style="font-weight:bold;margin-bottom:5px;">${filter.title}:</div>`;
            result += `<div>`;
            switch (filter.type) {
                case 'select':
                    var selectOptions = [];

                    //use static list
                    if (_.isArray(filter.options)) { selectOptions = filter.options; }
                    // fetch list
                    else {
                        //options is a url to fetch select list
                        $.ajax({
                            async: false,
                            url: filter.options,
                            success: function (result) { selectOptions = result; }
                        });
                    }
                    result += `<select data-filter-param=${filter.param} id='filter_${filter.id}' style='width:100%;' onchange='_dt_onFilterChange()'>`;
                    if (!_.isUndefined(filter.emptyValue)) {
                        result += `<option value="${filter.emptyValue}">${filter.emptyText || ''}</option>`;
                    }

                    $.each(selectOptions, function (i, so) {
                        result += `<option ${so.id === filter.val ? 'selected' : ''} value='${so.id}'>${so.name}</option>`;
                    });
                    result += '</select>';
                    break;
                case 'date':
                    result += `<input data-filter-param=${filter.param} type="text" id="filter_${filter.id}" onchange='_dt_onFilterChange()' class="date-picker report-main-date" style="width:100%;" value="${filter.val}" />`;
                    break;
                default:
                    break;
            }
            result += '</div></div>';
        });
        result += '</div>';
        $("#" + options.el).before(result);
    }

    function rowModal(rowIndex, rowData = {}, modalOptions = {}) {
        $.fn.modal.Constructor.prototype.enforceFocus = function () { };
        let modal = document.getElementById('__tblRowModal');
        let isNew = rowIndex === -1;

        if (modal === null) {
            $('body').append('<div id="__tblRowModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true"></div>');
            modal = document.getElementById('__tblRowModal');
        }

        var html = `              
                  <div class="modal-dialog" role="document">
                    <div class="modal-content">
                      <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                          <span aria-hidden="true">&times;</span>
                        </button>
                        ${modalOptions.title ? `<h4 class='modal-title'>${modalOptions.title}</h4>` : ''}
                      </div>
                      <div class="modal-body">
                        ${rowModalForm(rowIndex, rowData)}
                      </div>                     
                    </div>
                  </div>              
            `;
        modal.innerHTML = html;
        let form = modal.querySelector('form');

        try {
            $("#__tblRowModal select").each(function (i, el) { $(el).select2({ theme: "classic" }); });
            $("#__tblRowModal .date-picker").each(function (i, el) {

                $(el).datepicker({
                    dateFormat:"dd/mm/yy",
                    onSelect: function (dateText) {
                        let name = this.getAttribute('name');

                        let ev = new CustomEvent('AppTable.Modal.Input.Change',
                            {
                                bubbles: true,
                                detail: {
                                    form: Object.fromEntries(new FormData(form)),
                                    type: 'date',
                                    val: this.value,
                                    text: dateText,
                                    target: modal,
                                    name: name
                                }
                            });
                        this.dispatchEvent(ev);
                    }
                });

            });

            //bind extended form buttons events
            $("#__tblRowModal button.frmBtn").click(function (ev) {
                let name = $(this).attr('name');
                let button = isNew ?
                    _.find(options.createFormButtons, (btn) => { return btn.name === name; }) :
                    _.find(options.editFormButtons, (btn) => { return btn.name === name; });
                if (button && button.onClick) {
                    $('#__tblRowModal').remove();
                    $('.modal-backdrop').remove();
                    button.onClick.call(this, srv.getRow(rowIndex), srv, ev);
                }
            });

            modal.addEventListener('AppTable.Modal.Input.Change.Mutate', (ev) => {

                let { name, val, type } = ev.detail;
                $input = $(`#__tblRowModal input[name='${name}']`);                
                $input.val(val);                      
            });
        } catch (e) { console.error(e); }

        $('#__tblRowModal').modal();
    }

    function rowModalForm(rowIndex, _rowData) {
        var rowNodes = [];

        var table = $("#" + options.el).DataTable();
        var isNew = rowIndex === -1;
        var enableDelete = !isNew && !_.isUndefined(options.ajax.delete);
        var rowData = isNew ? _rowData : table.row(rowIndex).data();


        $.each(options.columns, function (index, col) {

            if (!col.data || col.data === null) return;
            let editState;
            if (isNew) editState = col.addRecord || col.create || '';
            if (!isNew) editState = col.edit || '';

            let val = '';
            if (isNew) {
                if (!_.isUndefined(col.newRowVal)) { val = col.newRowVal; }
                if (rowData[col.data]) val = rowData[col.data];
            }
            else {
                val = rowData[col.data] === null ? '' : rowData[col.data];
            }

            if (editState === 'false' || editState === false) return;

            if (editState === 'hidden') {
                rowNodes.push(`<input type="hidden" name='${col.data}' value="${val}">                           `);
            }
            else {
                var isListType = !_.isUndefined(col.list) && col.listData && col.listData.length > 0;
                var readonly = editState === 'readonly' ? 'readonly' : '';

                if (isListType) {
                    let options = [];

                    if (col.list.placeholder) options.push(`<option>${col.list.placeholder}</option>`);

                    $.each(col.listData, function (index, data) {
                        var selected = rowData !== null && data.id === rowData[col.data];
                        options.push(`<option ${selected ? 'selected' : ''} value='${data.id}'>${data.name}</option>`);
                    });

                    rowNodes.push(`
                         <div class="${readonly} col-md-12" style='margin-bottom:15px;'>
                            <label class="col-md-3 col-xs-12" for="${col.data}">${col.title}</label>                            
                            <div class="col-md-9 col-xs-12">
                                <select ${readonly === 'readonly' ? 'disabled' : ''} style='width:100%' ${col.requiredInForm ? 'required': ''} name='${col.data}'>
                                ${options.join('')}
                                </select>
                            </div>
                          </div>
                    `);
                } else if (col.type === 'number') {
                    rowNodes.push(`
                         <div class="${readonly} col-md-12" style='margin-bottom:15px;'>
                            <label class="col-md-3 col-xs-12" for="${col.data}">${col.title}</label>                            
                            <input ${col.requiredInForm ? 'required': ''} ${readonly} class="col-md-9 col-xs-12" type="number" placeholder="0" step="${col.numType === 'decimal' ? '0.01' : '1'}" name='${col.data}' value="${val}">                           
                          </div>
                    `);
                }
                else if (col.type === 'bool') {
                    rowNodes.push(`
                         <div class="${readonly} col-md-12" style='margin-bottom:15px;'>
                            <label class="col-md-3 col-xs-12">${col.title}</label>  
                            <input ${col.requiredInForm ? 'required': ''} ${readonly} ${readonly ? 'disabled' : ''} class='native-look' type="checkbox" name="${col.data}" value="true" ${val === true ? 'checked' : ''}>                               
                          </div>
                    `);
                }
                else if (col.type === 'date' || col.dType === 'date') {     
                    let date = val;
                    if (isNew) {
                        date = val ?
                            new Date(val.split('/')[2], parseInt(val.split('/')[1]) - 1, val.split('/')[0]).toLocaleDateString("es-CO") :
                            new Date().toLocaleDateString("es-CO");

                    } else {                       
                        if (isAspDateFormat(date)) val = stripAspDateFormat(date);
                        date = new Date(date).toLocaleDateString("es-CO");
                    }
                    rowNodes.push(`
                         <div class="${readonly} col-md-12" style='margin-bottom:15px;'>
                            <label class="col-md-3 col-xs-12" for="${col.data}">${col.title}</label>                            
                            <input ${col.requiredInForm ? 'required': ''} ${readonly} class="col-md-9 col-xs-12 date-picker" name='${col.data}' value="${date}">                           
                          </div>
                    `);
                }
                else {
                    rowNodes.push(`
                         <div class="${readonly} col-md-12" style='margin-bottom:15px;'>
                            <label class="col-md-3 col-xs-12" for="${col.data}">${col.title}</label>                            
                            <input ${col.requiredInForm ? 'required': ''} ${readonly} class="col-md-9 col-xs-12" type="text" class="form-control-plaintext" name='${col.data}' value="${val}" placeholder="${col.placeholder || ""}">                           
                          </div>
                    `);
                }
            }
        });

        let buttons = [
            `<button type="submit" class="btn btn-primary">Submit</button>`,
        ];

        if (enableDelete)
            buttons.push(`<button type="button" class="btn btn-danger" onclick='_dt_deleteRow()'>Delete</button>`);

        if (isNew) {
            _.each(options.createFormButtons || [], (btn) => {
                buttons.push(`<button class='btn btn-primary frmBtn' name='${btn.name}'>${btn.title}</button>`)
            });
        } else {
            _.each(options.editFormButtons || [], (btn) => {
                buttons.push(`<button class='btn btn-primary frmBtn' name='${btn.name}'>${btn.title}</button>`)
            });
        }            
        buttons.push(`<button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>`)
                          
        var form = `
            <form id='dt_rowModalForm' action='${isNew ? `${options.ajax.add}` : `${options.ajax.update}`}' method='post'>
                <div class="row row-nodes">
                    ${rowNodes.join('')}
                </div>
                <div class="row" style='display: flex;column-gap: 1em;'>
                  ${buttons.join('')}
                </div> 
           </form>
        `;

        return form;
    }

    function addTotalsFooter(tableOptions) {
        $("#" + options.el).append(`
            <tfoot>
                <tr>
                    <th colspan="${options.totalsColumn}" style="text-align:left">Total:</th>
                    <th></th>
                </tr>
            </tfoot>
        `);
        tableOptions["footerCallback"] = function () {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ?
                    i.replace(/[\$,]/g, '') * 1 :
                    typeof i === 'number' ?
                        i : 0;
            };

            // Total over all pages
            total = api
                .column(options.totalsColumn)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            // Total over this page
            pageTotal = api
                .column(options.totalsColumn, { page: 'current' })
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            // Update footer
            $(api.column(options.totalsColumn).footer()).html(
                '$' + pageTotal + ' ($' + total + ')'
            );
        };

    }
   
    function GetColumnDTypes()
    {
        return {
            'wight': function (data, type, row, meta) {
                var result = data.toLocaleString('en-US', 0) + 'g';
                if (data > 0 && !_.isUndefined(col.plusSign)) {
                    result = `<span style="color:green;font-weight:bold;">+${result}</span>`;
                }
                return result;
            },

            'actionButton': function (data, type, row, meta) {
                return `<img class='dt-row-action-button-image width='${col.width || 22}' src='${col.imageSrc}' onclick="_dt_editRow(${meta.row})"/>`;
            },

            'currency': function (data, type, row, meta) {
                return data.toLocaleString('en-US', 0) + '$';
            },

            'template': function (data, type, row, meta) {
                try {

                    var result = col.template;
                    var template = col.template;
                    var tempValues = [];
                    while (template.indexOf('{') !== -1) {
                        var start = template.indexOf('{');
                        var end = template.indexOf('}');
                        var prop = template.substring(start + 1, end);
                        tempValues.push({
                            placeholder: '{' + prop + '}',
                            val: row[prop]
                        });
                        template = template.substring(end + 1);
                    }
                    for (var i = 0; i < tempValues.length; i++) {
                        result = result.replace(tempValues[i].placeholder, tempValues[i].val);
                    }
                    if (_.isUndefined(result)) return data;
                    return result;
                } catch (ex) { console.error(ex); }
                return data;
            },

            'date': function (data, type, row, meta) {
                if (data !== null) {
                    var d = moment(data);
                    return d.format('DD/MM/YYYY');
                }
                return data;
            },
            'datetime': function (data, type, row, meta) {
                if (data !== null) {
                    var d = moment(data);
                    return d.format('DD/MM/YYYY H:mm');
                }
                return data;
            },

            'bool-icon': function (data, type, row, meta) {
                return data === true ? `<i class="material-icons" style='color:limegreen;'>check_circle</i>` : `<i class="material-icons" style='color:gray;'>check_circle</i>`;
            }
        }
    }

    function isAspDateFormat(value) {
        return _.isString(value) && value.indexOf('/Date') !== -1;
    }

    function stripAspDateFormat(value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        return results[1];
    }

    function createTableService(table, options) {
        return {
            getTable: () => { return table; },
            getColDef: (colIndex) => { return options.columns[colIndex]; },
            getCellData: (rowIndex, colIndex) => {
                return table.cell(rowIndex, colIndex).data();
            },
            updateCellData: (rowIndex, colIndex, data) => {
                table.cell(rowIndex, colIndex).data(data).draw();
            },
            openRowEditModal: (rowIndex, rowData = {}, modalOptions = {}) => {
                rowModal(rowIndex, rowData, modalOptions);
            },
            getRow: (rowIndex) => {
                return table.row(rowIndex).data();
            },
            reload: () => {
                table.ajax.reload();
            }
        };
    }

    return srv; 
}

