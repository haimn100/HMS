window.myApp = angular.module('myApp', ['ngMaterial', 'ngAnimate']);
window.App = {};
window.MyHostel = window.MyHostel || {};


myApp.service('tableService', function ($http, $rootScope) {

    var service = {
        toDataTable: function ($el, options) {

            var columns = [];

            var _options = {
                "processing": true,
                "serverSide": true,
                "searching": options.searching,
                "order": options.order || [[1, "desc"]],
                "ajax": {
                    url: options.url
                },
                "columns": columns,
                "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                    var params = $.extend({}, options.params);
                    params.sortBy = oSettings.aoColumns[oSettings.aaSorting[0][0]].data;
                    params.sortDesc = oSettings.aaSorting[0][1] == "desc";
                    params.start = aoData[3].value;
                    params.length = aoData[4].value;

                    oSettings.jqXHR = $.ajax({
                        "type": "GET",
                        "url": oSettings.ajax.url,
                        "data": params,
                        "success": fnCallback
                    });
                }
            };

            return $el.DataTable(_options);
        }
    };
    return service;
});

$(document).ready(function () {
    //Barcode reader
    if (window.Casa && Casa.settings.barcodePrefix.length > 0) {
        var barcode = "";
        $(document).keypress(function (e) {
            console.log(e);
            if (e.keyCode == 13) {
                if (barcode.startsWith(Casa.settings.barcodePrefix)) {

                    $.get("/guest/getguestbybarcode?barcode=" + barcode, function (user) {
                        if (user.error) {
                            console.error(user.error);
                        } else {
                            window.location = "/kitchen?userid=" + user.id;
                        }
                    })
                        .fail(function (error) {
                            console.error(error);
                        });
                }
                barcode = "";
            }
            else {
                var key = String.fromCharCode(e.which);
                barcode += key;
            }
        });
    }

    //Menu Generator

});


function AppNavBarMenu(spec) {
    let { menu, selectorId } = spec;
    setActiveMenu(menu);
    let html = buildMenuLinks(menu);
    document.getElementById(selectorId).innerHTML = html;

    function buildMenuLinks(menu) {
        return _.map(menu, (item) => {

            item.allow = _.isUndefined(item.allow) ? true: item.allow;
            if (!item.allow) return '';

            let title = item.icon ? ` <i class="material-icons">${item.icon}</i>${item.title || ''}` : `${item.title}${item.children ? `<span class="caret"></span>`: ''}`;

            if (item.children) {
                let subLinks = _.map(item.children, (child) => {
                    child.allow = _.isUndefined(child.allow) ? true : child.allow;
                    return child.allow ?
                        `<li><a href="${child.link}" class="menu-child">${child.title}</a></li>`
                        : '';
                }).join("");

                return `
                    <li role="presentation" class="nav-item dropdown ${item.active ? 'active' : ''}">
                        <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                            ${title}
                        </a>
                        <ul class="dropdown-menu"> `
                    + subLinks +
                    `</ul>
                    </li>
                    `;
            }

            return `<li class="nav-item ${item.active ? 'active' : ''}">
                           <a href="${item.link || 'javascript:void(0);'}">
                            ${title}
                           </a>
                        </li>`;
        }).join("");
    }
    function setActiveMenu(menu) {

        $.each(menu, (i, item) => {
            let isActive = matchUrlPath(item.link);

            //match by children
            if (item.children && !isActive) {
                isActive = _.any(item.children, (c) => { return matchUrlPath(c.link); })
            }

            item.active = isActive;
        });

        function matchUrlPath(url) {
            if (!url) return false;
            return window.location.pathname.toLowerCase() === url.toLowerCase();
        }
    } 
}

let $bootstrapModal;
function getBootstrapModal(title,body) {

    if (!$bootstrapModal) {
        $("body").prepend(`
            <div class="modal fade" id="bootstrapModal" tabindex="-1" role="dialog">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">${title}</h4>
                        </div>
                        <div class="modal-body">${body}</div>
                    </div>                   
                </div>
            </div>`);
        $bootstrapModal = $("#bootstrapModal");
        $bootstrapModal.modal();
    }

    return {
        show: () => { $bootstrapModal.modal('show'); },
        hide: () => { $bootstrapModal.modal('hide'); },
        content: (model) => { $bootstrapModal.find('.modal-body').html(model.body); $bootstrapModal.find('.modal-title').html(model.title); }        
    };
}

function printPopUp(data) {
    var myWindow = window.open('', 'Receipt', 'height=400,width=600');
    myWindow.document.write('<html><head><title>Receipt</title>');
    myWindow.document.write('<style type="text/css"> *, html {margin:0;padding:0;} </style>');
    myWindow.document.write('</head><body>');
    myWindow.document.write(
        `<style>

                body {
                        font- size: 10px;
                    font - family: Calibri;
                }

                table {
                    font - size: 10px;
                    font - family: Calibri;
                }

         </style >

        <table style="width:100%">

            <tr>
                <td align="left">ORDER NO</td>
                <td align="right">${data.orderID}</td>
            </tr>
            <tr>
                <td align="left">ORDER Date</td>
                <td align="right">${App.Date(data.orderDate).format()}</td>
            </tr>

            <tr>
                <td align="left">CUSTOMER</td>
                <td align="right">${data.customer}</td>
            </tr>

        </table>
            `);
    myWindow.document.write('</body></html>');
    myWindow.document.close(); // necessary for IE >= 10

    myWindow.onload = function () { // necessary if the div contain images

        myWindow.focus(); // necessary for IE >= 10
        myWindow.print();
        myWindow.close();
    };
}