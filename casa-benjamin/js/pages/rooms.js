
myApp.controller('RoomsController', function ($scope, $timeout, $http, $mdDialog) {

    var maleCount = 0;
    var femaleCount = 0;
    var totalCount = 0;

    // init users in beds and reservation info
    _.each(Casa.settings.rooms, function (room) {

        var partnersIds = [];
        _.each(room.beds, function (bed) {
            var user = _.find(Casa.settings.users, function (user) { return user.bed_id === bed.bed_id; });
            if (!_.isUndefined(user)) {
                user.bed_type_id = bed.bed_type_id;
                bed.user = user;

                if (!user.is_resident) {
                    maleCount += user.sex ? 1 : 0;
                    femaleCount += user.sex ? 0 : 1;
                    totalCount++;
                }
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

        //init amenities
        try {
            room.room.amenitiesDisplay = [];
            if (room.room.amenities !== null && room.room.amenities.length > 0) {
                
                _.each(room.room.amenities.split(','), function (id) {
                    var am = {};
                    am.name = id;
                    switch (am.name) {
                        case "1":
                            am.title = "TV";
                            break;
                        case "2":
                            am.title = "REFRIGERADOR";
                            break;
                        case "3":
                            am.title = "SOFA";
                            break;
                    }
                    room.room.amenitiesDisplay.push(am);
                });
            }
        } catch (e) {
            console.error(e);
        }
    });

    $scope.maleCount = maleCount;
    $scope.femaleCount = femaleCount;
    $scope.totalCount = totalCount;

    $scope.filter = {
        roomNumber: "",
        bedNumber: "",
        needCleaning: false
    };  

    var rooms = Casa.settings.rooms;

    $scope.printPage = function () {

        if ($scope.filter.showNonEmptyRooms) {
            $("body").removeClass("no-print");
            $("body").addClass("no-print");

            var arr = [];
            $.each($scope.roomRows, function (i,roomRow) {
                $.each(roomRow.rooms, function (j,room) {
                    arr.push(room.room.id);
                })
            });

            var h = "<div style='margin:20px;text-align:center;'><h1>Occupied Rooms</h1><h3>"+ arr.join() +"</h3></div>";
            var d = $("<div>").addClass("print").html(h).appendTo("html");
            window.print();
            d.remove();
        } else {
            $("body").removeClass("no-print");
            window.print();
        }

    }

    $scope.$watch('filter.roomNumber', function (newValue, oldValue) {
        if (newValue !== oldValue) {            
            var roomsToDisplay = _.filter(rooms, function (item) {
                return item.room.room_number == newValue;
            });
            filterRooms(newValue == "" ? rooms : roomsToDisplay);
        } else {
            filterRooms(rooms);
        }
       
    }, true);

    $scope.$watch('filter.bedNumber', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            var roomsToDisplay = _.filter(rooms, function (item) {
                for (var i = 0; i < item.beds.length; i++) {
                    if (item.beds[i].bed_id == newValue || item.beds[i].double_bed_partner_id == newValue) {
                        return true;
                    }
                }
                return false;
            });
            
            filterRooms(newValue == "" ? rooms: roomsToDisplay);
        } else {
            filterRooms(rooms);
        }
    }, true);

    $scope.$watch('filter.needCleaning', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue === false) {
                filterRooms(rooms);
            } else {
                var roomsToDisplay = _.filter(rooms, function (item) {
                    return (item.room.is_clean_required || item.room.is_cleaning_inspection_required);
                });
                filterRooms(roomsToDisplay);
            }          
        }
    }, true);

    $scope.$watch('filter.showNonEmptyRooms', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue === false) {
                filterRooms(rooms);
            } else {
                var roomsToDisplay = _.filter(rooms, function (room) {
                    for (var i = 0; i < room.beds.length; i++) {
                        if (!_.isUndefined(room.beds[i].user)) {
                            return true;
                        }

                        if (!_.isUndefined(room.beds[i].partner) && !_.isUndefined(room.beds[i].partner.user)) {
                            return true;
                        }
                    }
                    return false;
                });
                filterRooms(roomsToDisplay);
            }
        }
    }, true);

    $scope.assignHouseKeeper = function (roomId) {
        var rows = $scope.roomRows;
        var selectedRoom = null;

        for (var i = 0; i < rows.length; i++) {
            for (var j = 0; j < rows[i].rooms.length; j++) {
                if (rows[i].rooms[j].room.id == roomId) {
                    selectedRoom = rows[i].rooms[j];
                }
            }           
        }

        var flatBeds = [];
        for (i = 0; i < selectedRoom.beds.length; i++) {
            flatBeds.push(selectedRoom.beds[i]);
            if (!_.isUndefined(selectedRoom.beds[i].partner)) {
                flatBeds.push(selectedRoom.beds[i].partner);
            }
        }

        var emptyBeds = _.filter(flatBeds, function (bed) {
            return _.isUndefined(bed.user);
        });

        var dirtyBeds = [];
        if (selectedRoom.room.last_cleaned == null) {
            dirtyBeds = [];
        } else {
            var lastCleaned = moment(selectedRoom.room.last_cleaned);
            for (i = 0; i < emptyBeds.length; i++) {
                var d = emptyBeds[i].last_checkout;
                if (d != null) {
                    d = moment(d);
                    if (d.isAfter(lastCleaned)) {
                        dirtyBeds.push(emptyBeds[i]);
                    }
                } 
            }
        }

        $mdDialog.show({
            locals: { room: selectedRoom.room, dirtyBeds: dirtyBeds },
            templateUrl: 'assign-cleaning-dialog.htm',
            controller: function ($scope, room, dirtyBeds) {
                var _staff = null;
                $scope.room = room;
                $scope.dirtyBeds = dirtyBeds;
                $http.get("/housekeeping/gethousekeepers").then(
                    function (res) {
                        $scope.staffList = res.data;
                    }, function () {
                        alert("Having trouble getting housekeepers list");
                    });

                $scope.hide = function () {
                    $mdDialog.hide();
                };

                $scope.cancel = function () {
                    $mdDialog.cancel();
                };

                $scope.onStaffSelected = function (selectedStaff) {
                    _staff = selectedStaff;
                }

                $scope.answer = function () {
                    if (_staff == null) {
                        alert("choose staff");
                        return;
                    }                   
                    $mdDialog.hide(_staff);
                };

            },
            clickOutsideToClose: false,
            fullscreen: true
        }).then(function (staff) {

            $http({
                url: "/housekeeping/requireclean",
                method: "GET",
                params: {
                    house_keeper_id: staff.id,
                    house_keeper_name: staff.name,
                    room_number: selectedRoom.room.room_number
                }
            });
            selectedRoom.room.is_clean_required = !selectedRoom.room.is_clean_required;
            selectedRoom.room.assigned_house_keeper_name = staff.name;
            selectedRoom.room.assigned_house_keeper_id = staff.id;

        });    
    };

    $scope.unAssignHouseKeeper = function (roomId) {
        var rows = $scope.roomRows;
        var selectedRoom = null;
        for (var i = 0; i < rows.length; i++) {
            for (var j = 0; j < rows[i].rooms.length; j++) {
                if (rows[i].rooms[j].room.id == roomId) {
                    selectedRoom = rows[i].rooms[j].room;
                }
            }
        }

        $mdDialog.show({
            locals: { room: selectedRoom },
            templateUrl: 'unassign-cleaning-dialog.htm',
            controller: function ($scope, room) {
                $scope.room = room;

                $scope.hide = function () {
                    $mdDialog.hide();
                };

                $scope.cancel = function () {
                    $mdDialog.cancel();
                };

                $scope.answer = function () {                   
                    $mdDialog.hide({ beds: $scope.numOfBeds, comment: $scope.comment });
                };

            },
            clickOutsideToClose: false,
            fullscreen: true
        }).then(function (res) {
            $http({
                url: "/housekeeping/finishclean",
                method: "GET",
                params: {
                    num_of_beds: res.beds,
                    room_number: selectedRoom.room_number,
                    comment: res.comment
                }
            });
            selectedRoom.is_clean_required = !selectedRoom.is_clean_required;
        });    
    };

    $scope.addNote = function (room) {
        var note = prompt("Note");

        var rows = $scope.roomRows;
        var selectedRoom = null;
        for (var i = 0; i < rows.length; i++) {
            for (var j = 0; j < rows[i].rooms.length; j++) {
                if (rows[i].rooms[j].room.id == room.id) {
                    selectedRoom = rows[i].rooms[j].room;
                }
            }
        }
        if (note.length > 0) {
            $.get("/room/AddNote?roomid=" + room.id + "&note=" + note);
        }

        selectedRoom.note = note;
        $timeout(function () {
            tippy('.jsTippy', {
                position: 'bottom',
                animation: 'shift',
                theme: 'light',
                arrow: true,
                dynamicTitle: true
            });
        });
    };

    $scope.deleteNote = function (room) {
        $.get("/room/DeleteNote?roomid=" + room.id);

        var rows = $scope.roomRows;
        var selectedRoom = null;
        for (var i = 0; i < rows.length; i++) {
            for (var j = 0; j < rows[i].rooms.length; j++) {
                if (rows[i].rooms[j].room.id == room.id) {
                    selectedRoom = rows[i].rooms[j].room;
                }
            }
        }

        selectedRoom.note = null;
    }

    $scope.getSexCount = function () {
        var users = Casa.settings.users;
        var result = [0, 0];
        for (var i = 0; i < users.length; i++) {
            if (users[i].sex) {
                result[0] += 1;
            } else {
                result[1] += 1;
            }
        }
        return result;
    }

    $scope.getReservationClass = function (bed) {
        if (_.isUndefined(bed.user) && bed.reservation_id != null) {
            return "sex-" + bed.reservation_sex + "-color bed-reservation";
        }
        return "";
    }

    $scope.getDoubleBedColor = function (bed) {
        var sex1 = bed.user ? bed.user.sex : null;
        var sex2 = bed.partner.user ? bed.partner.user.sex : null;

        if (sex1 === null && sex2 === null) {
            return "initial";
        }

        if (sex1 !== null && sex2 === null) {
            return sex1 === true ? "#0029ff" : "#ff007f";
        }

        if (sex2 !== null && sex1 === null) {
            return sex2 === true ? "#0029ff" : "#ff007f";
        }

        if (sex1 !== null && sex2 != null) {
            if (sex1 !== sex2) {
                return "#7317C5";
            } else {
                return sex1 === false ? "#ff007f" : "#0029ff";
            }
        }

        return "";
    }

    $scope.getDoubleBedIconClass = function (bed) {
        switch (bed.bed_type_id) {
            case 2:
                return "flaticon-large-double-bed";
            case 3:
                return "flaticon-berth-with-stairs";
            default:
        }
    }

    $scope.getReservedInfo = function (bed){
        if (bed.reservation_id != null) {
            return "<div class='reservation-info'>" + bed.reservationInfo + "</div>";
        }
        return "";
    }

    $scope.getBedClass = function (bed) {
        return bed.bed_type_id === 1 ? "flaticon-big-bed-with-one-pillow-1" : "flaticon-large-double-bed"
    };

    $scope.goToCheckIn = function (bed) {
        window.location = "/guest/checkin?bedid=" + bed.bed_id + "&bednumber=" + bed.bed_id;
    };

    $scope.goToKitchen = function (bed) {
        window.location = '/kitchen?userid=' + bed.user.id;
    };

    function filterRooms(roomsToDisplay) {    

        var rows = [], rowIndex = -1;
        for (var i = 0; i < roomsToDisplay.length; i++) {
            if ((i % 4) === 0) {
                rows.push({ rooms: [] });
                rowIndex++;
            }
            rows[rowIndex].rooms.push(roomsToDisplay[i]);
        }

        $scope.roomRows = rows;

        $timeout(function () {

            tippy(".jsTippy", {
                position: 'bottom',
                animation: 'shift',
                theme: 'light',
                arrow: true,
                dynamicTitle:true
            });

            $(".tippy").each(function (k, v) {
                (function ($this) {
                    tippy(v, {
                        position: 'bottom',
                        animation: 'shift',
                        theme: 'light',
                        arrow: true,
                        onShow: function () {
                            var name = $this.attr("data-user-name");
                            var image = $this.attr("data-user-image");

                            var content = "<div class='tooltip-container'><p style='font-size:14px;'>$$username$$</p><img class='tooltip-user-image' src='$$userimage$$'/></div>";
                            content = content.replace("$$username$$", name);
                            content = content.replace("$$userimage$$", image);
                            $(this).find(".tippy-tooltip-content").html(content);
                        }
                    });
                })($(v));              
            });
        });
    }

    filterRooms(Casa.settings.rooms);   

});


$(document).ready(function () {  

    //$("body").on("click", "i[data-note]", function () {
    //    alert($(this).attr("data-note"));
    //});

    var acUsers = _.map(Casa.settings.users, function (item) { item.label = item.name; return item; });
    $("#users-ac").autocomplete({
        minLength: 1,
        source: acUsers,
        focus: function (event, ui) {
            $("#users-ac").val(ui.item.label);
            return false;
        },
        select: function (event, ui) {
            window.location = "/kitchen?userid=" + ui.item.id;
            return false;
        },
        source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response($.grep(acUsers, function (value) {
                return (matcher.test(value.bed_id) || matcher.test(value.name));
            }));
        }
    })
        .autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li class='ac-item'>")
                .append("<div><img src='/guestimages/" + item.pic + "' width='64' height='64'/><label>" + item.name + "</label><br/><small>Bed: " + item.bed_id + "</small></div>")
                .appendTo(ul);
        };
});
