$(document).ready(function () {
  //Plugin for DataTable
  jQuery.fn.dataTable.Api.register("sum()", function () {
    return this.flatten().reduce(function (a, b) {
      if (typeof a === "string") {
        a = a.replace(/[^\d.-]/g, "") * 1;
      }
      if (typeof b === "string") {
        b = b.replace(/[^\d.-]/g, "") * 1;
      }

      return a + b;
    }, 0);
  });

  $(".select2").each(function (i, el) {
    var options = {
      allowClear: $(el).attr("data-no-clear") ? true : false,
      placeholder: $(el).attr("placeholder"),
      width: "100%",
    };

    //https://stackoverflow.com/questions/18487056/select2-doesnt-work-when-embedded-in-a-bootstrap-modal
    var modal = $(this).closest(".modal");
    if (modal.length > 0) {
      options.dropdownParent = modal;
    }

    var countries;
    var $el;
    if (!_.isUndefined($(el).attr("data-countries"))) {
      countries = MyHostel.i18n.getCountries({ useSelect2Format: true });
      countries.unshift({});

      $el = $(el);

      options.data = countries;
      $el.select2(options);

      if (!_.isUndefined($el.attr("data-default-code"))) {
        var ccode = $el.attr("data-default-code");
        $el.val(ccode).change();
      }
    } else {
      $(el).select2(options);
    }

    if (!_.isUndefined($(el).attr("data-countries-cities"))) {
      countries = MyHostel.i18n.getCountriesAndCitiesList({
        useSelect2Format: true,
      });
      countries.unshift({});
      $el = $(el);

      options.data = countries;
      $el.select2(options);

      if (!_.isUndefined($el.attr("data-default-code"))) {
        var ccode = $el.attr("data-default-code");
        $el.val(ccode).change();
      }
    } else if (!_.isUndefined($(el).attr("data-cities"))) {
      var cities = MyHostel.i18n.getCitiesList({ useSelect2Format: true });

      var $el = $(el);

      options.data = cities;
      $el.select2(options);

      if (!_.isUndefined($el.attr("data-default-code"))) {
        var ccode = $el.attr("data-default-code");
        $el.val(ccode).change();
      }
    } else {
      $(el).select2(options);
    }

    if ($(el).hasClass("jsStaff")) {
      $(el).val(Casa.settings.staff.id).change();
    }
  });

  $('[data-toggle="tooltip"]').tooltip();

  $(".date-picker").each(function () {
    $(this).datepicker({
      dateFormat:
        $(this).attr("data-format") || MyHostel.i18n.getUIDatePickerString(),
      minDate: $(this).attr("data-minDate") || null,
      changeMonth: $(this).attr("data-change-month") || false,
      changeYear: $(this).attr("data-change-year") || false,
      yearRange: "1940:2050",
    });
  });

  $(".dateMonthPicker").datepicker({
    dateFormat: MyHostel.i18n.getUIDatePickerString(true),
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,

    onClose: function (dateText, inst) {
      var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
      var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
      $(this).val(
        $.datepicker.formatDate(
          MyHostel.i18n.getUIDatePickerString(true),
          new Date(year, month, 1)
        )
      );
      $(this).change();
    },
  });

  $(".dateMonthPicker").focus(function () {
    $(".ui-datepicker-calendar").hide();
    $("#ui-datepicker-div").position({
      my: "center top",
      at: "center bottom",
      of: $(this),
    });
  });

  $("select.js-hours").each(function () {
    $this = $(this);
    var str = "";
    var selected = "";
    var defaultHour = $this.attr("data-hour") || 0;
    defaultHour = parseInt(defaultHour);
    for (var i = 0; i < 24; i++) {
      if (i == defaultHour) {
        selected = "selected='selected'";
      } else {
        selected = "";
      }

      if (i < 10) {
        str = "0" + i + ":00";
        $this.append(
          "<option " + selected + " val='" + str + "'>" + str + "</option>"
        );
      } else {
        str = i + ":00";
        $this.append(
          "<option " + selected + " val='" + str + "'>" + str + "</option>"
        );
      }
    }
  });

  $(document).ready(function () {
    $(".dataTable").each(function () {
      var $this = $(this);

      var initSearch = getQueryVariable("datatable-search");
      initSearch = initSearch == null ? "" : initSearch;

      var title = $this.attr("data-title") ? $this.attr("data-title") : "";

      if (!$.fn.DataTable.isDataTable($this)) {
        var pageSize = $this.attr("data-page-size")
          ? $this.attr("data-page-size")
          : 20;
        var order = [];
        $orderBy = $this.find("th[data-order]");

        if ($orderBy.length > 0) {
          order = [
            [
              $this.find("th").index($orderBy.first()),
              $orderBy.first().attr("data-order"),
            ],
          ];
        }
        pageTotalCol = $this.attr("data-page-total-col")
          ? $this.attr("data-page-total-col")
          : null;

        var options = {
          responsive: true,
          columnDefs: [
            {
              targets: "no-sort",
              orderable: false,
            },
          ],
          pageLength: pageSize,
          order: order,
          dom: "Bfrtip",
          search: { search: initSearch, caseInsensitive: true },
          buttons: [
            "copy",
            "csv",
            "excel",
            "pdf",
            {
              extend: "print",
              text: "Print",
              title: title,
              autoPrint: true,
              footer: true,
              customize: function (win) {
                $(win.document.body)
                  .css("font-size", "10pt")
                  .css("margin", "20px");

                $(win.document.body).find("tfoot tr").addClass("total-row");
              },
            },
            {
              text: "TSV",
              extend: "csv",
              fieldSeparator: "\t",
              extension: ".tsv",
            },
          ],
        };

        if (pageTotalCol != null) {
          options["footerCallback"] = function (
            row,
            data,
            start,
            end,
            display
          ) {
            var api = this.api(),
              data;
            var total = $this.attr("data-total")
              ? $this.attr("data-total")
              : null;

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
              return typeof i === "string"
                ? i.replace(/[\$,A-Za-z]/g, "") * 1
                : typeof i === "number"
                ? i
                : 0;
            };

            // Total over this page
            pageTotal = api
              .column(pageTotalCol, { page: "current" })
              .data()
              .reduce(function (a, b) {
                return intVal(a) + intVal(b);
              }, 0);

            // Update footer

            $(api.column(pageTotalCol).footer()).html(
              "$" +
                pageTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") +
                (total == null
                  ? ""
                  : " ($" +
                    total.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") +
                    ")")
            );
          };
        }

        var tbl = $this.DataTable(options);
      }
    });

    $(".dataTable tbody").on("click", "tr", function () {
      var table = $(this).closest("table").DataTable();
      if ($(this).hasClass("selected")) {
        $(this).removeClass("selected");
      } else {
        table.$("tr.selected").removeClass("selected");
        $(this).addClass("selected");
      }
    });

    $(".datetimepicker").bootstrapMaterialDatePicker({
      format: MyHostel.i18n.getUIDateTimePickerString(),
      clearButton: true,
      weekStart: 1,
    });

    $('input[name="daterange"]').daterangepicker(
      {
        locale: {
          format: "YYYY-MM-DD",
        },
        startDate: "2013-01-01",
        endDate: "2013-12-31",
      },
      function (start, end, label) {
        alert(
          "A new date range was chosen: " +
            start.format("YYYY-MM-DD") +
            " to " +
            end.format("YYYY-MM-DD")
        );
      }
    );
  });
});

window["dtService"] = {
  toDataTable: function ($el, options) {
    var columns = [];

    for (var i = 0; i < options.columns.length; i++) {
      var superCol = $.extend({}, options.columns[i]);

      if (superCol.type) {
        switch (superCol.type) {
          case "date":
            superCol.render = function (data, type, row, meta) {
              return moment(date).format("dd/MM/yyyy HH:mm");
            };
            break;
          default:
        }
      }

      columns.push(superCol);
    }

    var _options = {
      processing: true,
      serverSide: true,
      searching: options.searching,
      order: options.order || [[1, "desc"]],
      ajax: {
        url: options.url,
      },
      columns: columns,
      fnServerData: function (sSource, aoData, fnCallback, oSettings) {
        var params = $.extend({}, options.params);
        params.sortBy = oSettings.aoColumns[oSettings.aaSorting[0][0]].data;
        params.sortDesc = oSettings.aaSorting[0][1] == "desc";
        params.start = aoData[3].value;
        params.length = aoData[4].value;

        oSettings.jqXHR = $.ajax({
          type: "GET",
          url: oSettings.ajax.url,
          data: params,
          success: fnCallback,
        });
      },
    };

    return $el.DataTable(_options);
  },
};

function getQueryVariable(variable) {
  var query = window.location.search.substring(1);
  var vars = query.split("&");
  for (var i = 0; i < vars.length; i++) {
    var pair = vars[i].split("=");
    if (pair[0] == variable) {
      return pair[1];
    }
  }
  return null;
}

(function (factory) {
  if (typeof define === "function" && define.amd) {
    define(["jquery", "moment", "datatables.net"], factory);
  } else {
    factory(jQuery, moment);
  }
})(function ($, moment) {
  $.fn.dataTable.moment = function (format, locale) {
    var types = $.fn.dataTable.ext.type;

    // Add type detection
    types.detect.unshift(function (d) {
      if (d) {
        // Strip HTML tags and newline characters if possible
        if (d.replace) {
          d = d.replace(/(<.*?>)|(\r?\n|\r)/g, "");
        }

        // Strip out surrounding white space
        d = $.trim(d);
      }

      // Null and empty values are acceptable
      if (d === "" || d === null) {
        return "moment-" + format;
      }

      var result = moment(d, format, locale, true).isValid()
        ? "moment-" + format
        : null;

      return result;
    });

    // Add sorting method - use an integer for the sorting
    types.order["moment-" + format + "-pre"] = function (d) {
      if (d) {
        // Strip HTML tags and newline characters if possible
        if (d.replace) {
          d = d.replace(/(<.*?>)|(\r?\n|\r)/g, "");
        }

        // Strip out surrounding white space
        d = $.trim(d);
      }

      return !moment(d, format, locale, true).isValid()
        ? Infinity
        : parseInt(moment(d, format, locale, true).format("x"), 10);
    };
  };
});

$.fn.dataTable.moment("DD/MM/YYYY");
