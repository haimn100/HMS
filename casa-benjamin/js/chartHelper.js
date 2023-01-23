Highcharts.setOptions({
    lang: {
        thousandsSep: ','
    }
});

window.ChartHelper = {
    tools: {
        getIntervalByPeriod: function (period) {
            var interval = 1000 * 60 * 60;
            if (period === "week" || period === "month") interval *= 24;
            if (period === "year") interval = null;
            return interval;
        }
    },
    pie: function (selector, title, series, options) {
        options = _.isUndefined(options) ? {} : options;

        return $.extend(Highcharts.chart(selector, {
            credits: {
                enabled: false
            },
            chart: {
                backgroundColor: 'transparent',
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
                //margin: 0
            },
            title: {
                text: title.length > 0 ? title : null,
                enabled: title.length > 0
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.point.name + '</b>: ' + this.y + ' ( ' + this.point.percentage.toFixed(0) + '%)';
                }
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: _.isUndefined(options.enableDataLables) ? true : options.enableDataLables,
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                        style: {
                            color: _.isUndefined(options.dataLabelColor) ? "#fff" : options.dataLabelColor
                        }
                    },
                    showInLegend: _.isUndefined(options.showInLegend) ? false : options.showInLegend
                }
            },
            legend: {
                labelFormatter: function () {
                    var total = 0, percentage;
                    $.each(this.series.data, function () {
                        total += this.y;
                    });
                    percentage = ((this.y / total) * 100).toFixed(1);
                    return (this.name + ' <span style="color:' + this.color + '">' + percentage + '%</span>');
                }
            },
            series: series
        }), options);
    },
    timeGraph: function (selector, title, series, options) {
        options = _.isUndefined(options) ? {} : options;
        return Highcharts.chart(selector, {
            credits: {
                enabled: false
            },
            chart: {
                type: 'spline'
            },
            title: {
                text: title.length > 0 ? title : null
            },
            subtitle: {
                text: _.isUndefined(options.subtitle) ? null : options.subtitle
            },
            xAxis: {
                type: 'datetime',
                dateTimeLabelFormats: _.isUndefined(options.dateTimeLabelFormats) ? {} : options.dateTimeLabelFormats,
                title: {
                    text: options.xTitle
                }
            },
            yAxis: {
                title: {
                    text: options.yTitle
                },
                labels: {
                    format: '${value}'
                },
                min: 0
            },
            tooltip: _.isUndefined(options.tooltip) ? {} : options.tooltip,

            plotOptions: {
                spline: {
                    marker: {
                        enabled: true
                    }
                }
            },

            series: series
        });
    },
    timePeriodGraph: function (options) {

        var interval = ChartHelper.tools.getIntervalByPeriod(options.period);
        var chartDef = {
            credits: {
                enabled: false
            },
            title: {
                text: options.title
            },
            xAxis: {
                type: 'datetime',
                tickInterval: interval,
                labels: {
                    enabled: true
                }
            },
            plotOptions: {
                series: {
                    dataLabels: {
                        enabled: _.isUndefined(options.enableSeriesDataLables) ? true : options.enableSeriesDataLables
                    }
                }
            },
            yAxis: {
                gridLineWidth: 0,
                title: {
                    text: null
                }
            },
            legend: {
                enabled: false
            },           
            series: options.series
        };

        if (!_.isUndefined(options.yLabel)) {
            chartDef.yAxis.labels = {
                format: options.yLabel
            };
        }

        return Highcharts.chart(options.selector,chartDef );
    },
    toDateTimeSeries: function (data) {
        var result = [];
        _.each(data, function (item) {
            if (item.pointDate.startsWith('/Date')) {
                item.pointDate = Number(item.pointDate.replace("/Date(", "").replace(")/", ""));
            }
            result.push([item.pointDate,item.pointVal]);
        });

        return result;
    },
    widgetSearchCtrl: function (container) {

        var d = new Date(); 
        var dd = d.getDate();
        var mm = d.getMonth() + 1;
        var yyyy = d.getFullYear();
        if (dd < 10) { dd = '0' + dd; }
        if (mm < 10) { mm = '0' + mm; }

        var initDate =  dd + "/" + mm + "/" + yyyy; 

        var htm = `
        <div class="card clearfix">
            <div class="body">
                <div class="pull-left">
                    <span>Start Date:</span> <input class="date-picker report-main-date" id="widgetCtrlDate" type="text" value="${initDate}" />
                </div>
                <div class="pull-left">
                    <span>Period</span>:
                    <select id="widgetCtrlPeriod">
                        <option value="day">Day</option>
                        <option selected="selected" value="week">Week</option>
                        <option value="month">Month</option>
                        <option value="year">Year</option>
                    </select>
                </div>
                <div class="pull-left" style="margin-left:10px;">
                    <button id="widgetCtrlSubmit">Search</button>
                </div>
            </div>
        </div>
        `;
        $("#" + container).html(htm);       
        
        var ctrl = {
            onSearch: [],
            search: function () {
                $("#widgetCtrlSubmit").click();
            }
        }; 

        $("#widgetCtrlSubmit").click(function () {
            _.each(ctrl.onSearch, function (handler) {

                var params = {
                    date: $("#widgetCtrlDate").val().split('/'),
                    period: $("#widgetCtrlPeriod").val()
                };               
                handler.call(this,params);
            });
        });

        return ctrl;

    },
    widget: function (options) {

        var widgetHeight = _.isUndefined(options.height) ? "100%" : (options.height + "px"); 
        var chartContainerId = (Math.random() * 100000).toFixed(0).toString();
        var chartCard = `<div class='card'>
                            <div class='header'>
                                <h5>${options.title}</h5>
                            </div>
                            <div class='body'>
                                <div style='height:${widgetHeight};' id='${chartContainerId}'></div>
                            </div>
                         </div>
                        `;

        $("#" + options.container).append(chartCard);

        var chart;
        
        switch (options.chartType) {
            case "timePeriodGraph":
                chart = ChartHelper.timePeriodGraph({
                    selector: chartContainerId,
                    series: [],
                    title: null,
                    period: "day",
                    yLabel: options.yLabel,
                    enableSeriesDataLables: options.enableSeriesDataLables
                });

                (function (options) {
                    options.searchCtrl.onSearch.push(function (params) {
                        var interval = ChartHelper.tools.getIntervalByPeriod(params['period']);

                        chart.update({
                            xAxis: {
                                tickInterval:interval
                            },
                            plotOptions: {
                                series: {
                                    dataLabels: {
                                        enabled: (params['period'] === 'week' || params['period'] === 'year')  ? true:false
                                    }
                                }
                            },
                            legend: {
                                enabled: options.series.length > 1
                            }
                        });

                        _.each(options.series, function (series) {
                            removeSeries(series.id, chart);                           
                            addSeries(series, params['date'], params['period'], chart);
                        });
                    });
                })(options);
               
                break;
            default:
        }      

        function addSeries(series, date, period, chart) {

            chart.showLoading();
            $.ajax({
                dataType: "json",
                url: series.endpoint,
                data: {
                    year: date[2],
                    month: date[1],
                    day: date[0],
                    interval: period
                },
                success: function (data) {                 
                    chart.hideLoading();
                    var cleanData = ChartHelper.toDateTimeSeries(data);
                    series.data = cleanData;
                    chart.addSeries(series);
                }
            });
        }
        function removeSeries(seriesId,chart) {
            var s = chart.get(seriesId);
            if (s) s.remove();
        }
    }
};