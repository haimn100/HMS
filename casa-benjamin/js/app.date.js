App.Date = function (date) {

    let _format = 'DD/MM/YYYY';

    let DateStruct = function (momentDate) {
        return {
            val: () => momentDate,
            format: (format) => momentDate.format(format || _format),
            add: (unit, duration) => {
                return new DateStruct(momentDate.add(unit, duration));
            }
        }
    }
    return new DateStruct(date ? moment(date, _format) : moment());
};
