myApp.controller('NavbarController', function ($scope, $timeout, $http) {

    var nav = {
        getReservations: function () {
            var reservationsList = [];

            var d = new Date();
            var today = d.getDate();
            var maxAlertDay = (d.getDate() + 3);

            _.each(Casa.settings.reservations, function (res) {

                var resDay = new Date(res.res_date).getDate();

                if (resDay >= today && resDay <= maxAlertDay) {
                    var info = {
                        text: "Reservation for " + res.res_name + " for " + res.nights + " nights (" + (res.room_type == 1 ? "Single" : "Dorm") + ")" + (res.room_id == 0 ? "" : (" Assigned by " + res.employee_name + " to room " + res.room_id)),
                        date: res.res_date.substring(0, 10),
                        status: res.status
                    }

                    reservationsList.push(info);
                }

            });

            return reservationsList;
        }
    }

    $scope.checkOutsDueCount = 0;
    $scope.reservationsList = [];
    $scope.stockAlerts = Casa.settings.stockAlerts;
    $scope.nav = nav;

    $http.get("/home/CheckOutsDueCount").then(function (answer) {
        $scope.checkOutsDueCount = answer.data;
    });

    var rooms = JSON.parse(JSON.stringify(Casa.settings.rooms));
    var users = JSON.parse(JSON.stringify(Casa.settings.users));

    // init users in beds and reservation info
    _.each(rooms, function (room) {

        var partnersIds = [];
        _.each(room.beds, function (bed) {
            var user = _.find(users, function (user) { return user.bed_id === bed.bed_id; });
            if (!_.isUndefined(user)) {
                user.bed_type_id = bed.bed_type_id;
                bed.user = user;
            }

        });


        //This shitttt is for grouping double beds to one fucking object
        var newBeds = [];
        var idsToIgnore = [];
        _.each(room.beds, function (bed) {

            if (!_.contains(idsToIgnore, bed.bed_id)) {
                if (bed.double_bed_partner_id != null) {
                    var partner = _.find(room.beds, function (b) { return b.bed_id === bed.double_bed_partner_id; });
                    bed.partner = partner;
                    idsToIgnore.push(bed.double_bed_partner_id);
                }
                newBeds.push(bed);
            };
        });
        room.beds = newBeds;
    });


    var dirtyRoomsAlerts = 0;
    for (var i = 0; i < rooms.length; i++) {
        var room = rooms[i].room;
        var isOccupied = _.some(rooms[i].beds, function (a) { return !_.isUndefined(a.user); });
        if (room.last_cleaned != null && isOccupied) {
            var d = moment(room.last_cleaned);
            var customPeriod = moment().subtract(3, 'days');

            if (d.isBefore(customPeriod)) {
                try {
                    var firstCheckinUser = _.chain(rooms[i].beds)
                        .map(function (a, b) { return a.user; })
                        .sortBy(function (a) { return _.isUndefined(a) ? Number.MAX_SAFE_INTEGER : moment(a.cidate).unix(); })
                        .first()
                        .value();
                    var firstCheckin = moment(firstCheckinUser.cidate);
                    if (firstCheckin.isBefore(customPeriod)) {
                        dirtyRoomsAlerts++;
                    }
                } catch (e) {
                    console.error(e);
                }
            }            
        }
    }
    $scope.dirtyRoomsAlerts = dirtyRoomsAlerts;

});