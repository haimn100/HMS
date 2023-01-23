
myApp.controller('KitchenController', function ($scope,$timeout,$http) {

    var allMenu = Casa.settings.menu
    var menu = [];

    for (var i = 0; i < allMenu.length; i++) {

        if (allMenu[i].category.is_active) {
            var cat = { category: allMenu[i].category, menuItems: [] };
            menu.push(cat);
            for (var j = 0; j < allMenu[i].menuItems.length; j++) {
                if (allMenu[i].menuItems[j].menuItem.is_active) {
                    cat.menuItems.push(allMenu[i].menuItems[j]);
                }
            }
        }

     
    }

    $scope.date = new Date();
    $scope.searchMenu = "";

    $scope.extraAmountToCharge = 0;
    $scope.onCreditCardPercentChange = function () {
        if (order.creditCardChargePercentage != 0 && order.total != 0) {
            $scope.extraAmountToCharge = (parseFloat(order.creditCardChargePercentage) / 100) * order.total;
        }
    };

    $scope.$watch('searchMenu', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            var searchMenuitems = [];

            if (newValue === "") {
                if (kitchen.menuCategory.menu_category_type === 1) {
                    kitchen.menuItems = kitchenMenu;
                } else {
                    kitchen.menuItems = servicesMenu;
                }
                return;
            }

            var menu = kitchen.menuCategory.menu_category_type === 1 ? kitchenMenu : servicesMenu;

            $.each(menu,function (k,v) {               
              
                $.each(v.menuItems, function (key, val) {
                    if (val.menuItem.name.toLowerCase().indexOf(newValue.toLowerCase()) !== -1 || val.menuItem.id.toString() == newValue.toLowerCase()) {
                        searchMenuitems.push(val);
                    }
                });

            });
            kitchen.menuItems = searchMenuitems;
        }
    });

    var kitchenMenu = _.filter(menu, function (item) {
        return item.category.menu_category_type === 1;
    });

    var servicesMenu = _.filter(menu, function (item) {
        return item.category.menu_category_type === 2;
    });

    var otherItemIndex = -100;
    var discountItemIndex = -200; 

    var order = {
        items: [],
        total: 0,
        pay_type_id: null,
        splitUsers: [],
        splitCashCount: 0,
        staffName: "",
        staffId:0,
        userId: 0,
        userName: "",
        userBed: 0,
        isCashUser: false,
        menu_category_type: 1,
        discounts: [],
        creditCardChargePercentage:5
    };
    var emptyDisplayRows = [];

    var kitchen = {
        splitUserAcText: "",
        saving: false,
        menuCategory: kitchenMenu.length> 0 ? kitchenMenu[0].category: null,
        /* { menuitem-id: [groups of ingredients] } 
         * This is from preventing to create new list every time and cause angularjs error
        */
        menuItemIngredientsGroup: {},

        menuItems: kitchenMenu,
        servicesMenuItems: servicesMenu,
        onMenuItemClick: function (item) {
            if (!_.isUndefined(item.category)) {
                kitchen.menuCategory = item.category;
                kitchen.menuItems = item.menuItems;
            } 
        },
        resetMenu: function () {
            if (order.menu_category_type === 1) {
                kitchen.menuItems = kitchenMenu;
            } else {
                kitchen.menuItems = servicesMenu;
            }
        },
        showKitchenMenu: function () {
            kitchen.menuItems = kitchenMenu;
            order.menu_category_type = 1;
            kitchen.menuCategory = kitchenMenu[0].category;
        },
        showServicesMenu: function () {
            kitchen.menuItems = servicesMenu;
            order.menu_category_type = 2;
            kitchen.menuCategory = servicesMenu[0].category;
        },
        removeOrderItem: function (index) {

            var indx = 0;
            for (var i = 0; i < order.items.length; i++) {
                if (order.items[i].index === index) {
                    index = i;
                    break;
                }
            }
            order.total -= order.items[index].total;
            delete order.items.splice(index, 1);
            emptyDisplayRows.push({});
        },
        removeDiscountItem: function (index) {
            var indx = 0;
            for (var i = 0; i < order.discounts.length; i++) {
                if (order.discounts[i].index === index) {
                    index = i;
                    break;
                }
            }

            order.total += order.discounts[indx].price;
            delete order.discounts.splice(indx, 1);
            emptyDisplayRows.push({});
        },
        addOrderItem: function (menuItem) {

            if (order.items.length > 0) {
                var itemsFromDifferentMenu = _.filter(order.items, function (item) {
                    return item.menu_category_type !== order.menu_category_type;
                });

                if (itemsFromDifferentMenu.length > 0) {
                    if (confirm("All Items will be removed from order. Continue?")) {
                        kitchen._resetOrder();
                    } else {
                        return;
                    }
                }
            }
         
            //check if ingredients dropdown exists
            var ingredients = [];
            var dropDown = $("#mi-" + menuItem.menuItem.id);
            if (dropDown.length > 0) {
                var checkedItems = dropDown.find("input[type='checkbox']:checked,input[type='radio']:checked");
                if (checkedItems.length > 0) {
                    checkedItems.each(function (index,item) {
                        var id = $(item).attr("id").split("-")[3];
                        var grpNumber = $(item).attr("id").split("-")[2];
                        var ingItem = _.find(menuItem.ingredients, function (item) { return item.ingredient_id == id && item.ingredients_group_number == grpNumber; });
                        if (ingItem) {
                            ingredients.push(ingItem);
                        }
                    });
                }

                checkedItems.each(function (i,el) {
                    $(el).removeAttr("checked");
                });
            }

            var orderItem = {
                menu_item_id: menuItem.menuItem.id,
                menu_item_name: menuItem.menuItem.name,
                price: menuItem.menuItem.price,
                total: menuItem.menuItem.price,
                ingredients: ingredients,
                menu_item_ingredients_ids: [],
                menu_item_ingredients: "",
                comment: "",
                index: order.items.length,
                menu_category_type: menuItem.menuItemCategory.menu_category_type,
                menu_category_id: menuItem.menuItemCategory.id,
                menu_category_name: menuItem.menuItemCategory.name
            };

            if (ingredients.length > 0) {
                orderItem.total += _.reduce(ingredients, function (memo, ing) { return memo + ing.ingredient_price; }, 0);
                _.each(ingredients, function (v, k) {
                    var ing = _.find(Casa.settings.ingredients, function (item) { return item.id == v.ingredient_id; });
                    orderItem.menu_item_ingredients += ing.name + "(" + v.ingredient_price + "),";
                    orderItem.menu_item_ingredients_ids.push(v.ingredient_id);
                });
            }
            order.items.push(orderItem);
            order.total += orderItem.total;
            
            emptyDisplayRows.shift();
            kitchen.closeDropDown();

            $scope.onCreditCardPercentChange();

        },
        cancelOrderItem: function (menuItem) {
            kitchen.staticIngArray = undefined;
            kitchen.closeDropDown();
        },
        addOrderOtherItem: function () {
            var comment = $("#otherItemComment").val();
            var price = parseFloat($("#otherItemPrice").val());


            var orderItem = {
                menu_item_name: "Other",
                menu_item_id: -1,
                menu_category_type: order.menu_category_type,
                price: price,
                total: price,
                comment: comment,
                index: otherItemIndex--
            }

            order.items.push(orderItem);
            order.total += orderItem.total;

            emptyDisplayRows.shift();

            kitchen.closeDropDown();
            $("#otherItemComment").val("");
            $("#otherItemPrice").val("");

            $scope.onCreditCardPercentChange();

        },
        addUserDiscount: function () {

            var comment = $("#discountComment").val();
            var price = parseFloat($("#discountPrice").val());

            if (price === 0) {
                alertify.alert("Put Discount Price");
                return;
            }

            if (price < 0) {
                price *= -1;
            }

            if (price > order.total) {
                alertify.alert("Discount can't be greater that order");
                return;
            }

            var userDiscount = {
                user_id: kitchen.user.id,
                comment: comment,
                price: price,
                index: discountItemIndex--
            }

            order.discounts.push(userDiscount);

            order.total -= userDiscount.price;

            emptyDisplayRows.shift();

            kitchen.closeDropDown();
            $("#discountComment").val("");
            $("#discountPrice").val("");

        },
        getIngredientName: function (id) {
            var ing = _.find(Casa.settings.ingredients, function (ing) { return ing.id === id });
            return ing ? ing.name : "";
        },
        resetIngredients: function (id) {
            $('#' + id).find("input[type='checkbox']").prop('checked', false);
        },
        closeDropDown: function () {
            $(".modal-body input").removeAttr("checked");
            $("body").click();
        },
        addSplitUser: function () {

            var acText = $("#users-ac").val();
            var id = acText.split("(")[1].replace(")", "");

            if (kitchen.user.id == id) {
                return;
            }

            var exists = _.find(order.splitUsers, function (u) {
                return u.id == id;
            });

            if (exists != undefined){
                return;
            }

            var user = _.find(Casa.settings.users, function (u) {
                return u.id == id;
            });
            order.splitUsers.push({id: user.id, bed_id: user.bed_id, name: user.name, pic:user.pic});
            kitchen.splitUserAcText = "";
        },
        addCashSplitUser: function () {
            order.splitUsers = [];
            for (var i = 0; i < order.splitCashCount; i++) {
                order.splitUsers.push({});
            }
            $("#splitUsersModal").modal("toggle");
        },
        removeSplitUser: function (u) {
            var indx = 0;
            for (var i = 0; i < order.splitUsers.length; i++) {
                if (order.splitUsers[i].id === u.id) {
                    index = i;
                    break;
                }
            }
            delete order.splitUsers.splice(indx, 1);
        },
        getSplitPrice: function () {
            if (order.splitUsers.length > 0) {
                return Math.round(order.total / (order.splitUsers.length + 1));
            }
        },
        saveOrder: function (print) {

            if (order.items.length == 0 && order.discounts.length == 0) {
                alertify.alert("No Items In Order");
                return;
            }

            var payType = $("input[name='pay-type']:checked");
            if (payType.length == 0) {
                alertify.alert("Choose Payment");
                return;
            } else {
                order.pay_type_id = payType.val();
            }

            if ($("select[name='employee'] option:selected").val() == "" ||  $("select[name='employee'] option:selected").length === 0) {
                alertify.alert("Choose Employee");
                return;
            } else {
                order.staffName = $("select[name='employee'] option:selected").text();
                order.staffId = $("select[name='employee']").val();
            }

            order.creditCardChargePercentage = $("#creditcharge").val();

            kitchen.saving = true;

            $http.post('/kitchen/saveorder', order).then(
                function success(response) {
                    if (print) {
                        window.print();
                    }
                    window.location = "/?alert=Added order " + response.data + " to " + kitchen.user.name + "&alerttype=success";
                }, function error(response) {
                    alert(response.data);
                    kitchen.saving = false;
                });
        },
        getIngredientsGroups: function (menuItem) {         
            if (!_.isUndefined(kitchen.menuItemIngredientsGroup[menuItem.menuItem.id])) {
                return kitchen.menuItemIngredientsGroup[menuItem.menuItem.id];
            }
            kitchen.menuItemIngredientsGroup[menuItem.menuItem.id] = _.groupBy(menuItem.ingredients, function (ing) { return ing.ingredients_group_number; });
            return kitchen.menuItemIngredientsGroup[menuItem.menuItem.id];
        },
        getIngredientsGroupCol: function (menuItem) {
            var len = _.toArray(_.groupBy(menuItem.ingredients, function (ing) { return ing.ingredients_group_number; })).length;
            return 12 / len;
        },
        getIngredientsGroupDialogSize: function (menuItem) {
            var len = kitchen.getIngredientsGroupCol(menuItem);
            if (len > 6){            
                return "modal-sm";
            } else if (len == 4) {
                return "modal-lg";
            }
            return "";
        },
        updateStaff: function () {
            order.staffName = $("select[name='employee'] option:selected").text();
            order.staffId = $("select[name='employee']").val();
        },
        _resetOrder: function () {
            order.items = [];
            order.total = 0;
            order.splitUsers = [];
            order.splitCashCount = 0;          
        }
    };

    $scope.moveUserToBed = function () {
        var destBed = kitchen.user.destBed;
        var destPrice = kitchen.user.destPrice;
        if (destBed == null) {
            alertify.alert("Choose Bed");
            return;
        }
        if (destPrice == null) {
            alertify.alert("Choose Price");
            return;
        }
    }

    $scope.focusOn = function (elId) {
        setTimeout(function () {
            $('#' + elId).focus();
        }, 300);
        
    }

    var userId = $("#userId").val();
    if (userId.length === 0) {
        kitchen.user = { isCash: true, name: "SYSTEM" };    
        order.isCashUser = kitchen.user.isCash ? true : false;

    } else {
        kitchen.user = _.find(Casa.settings.users, function (v,k) {
            return v.id == userId;
        });       
        kitchen.destBed = null;
        kitchen.destPrice = null;
        order.userId = kitchen.user.id;
        order.userBed = kitchen.user.bed_id;
        order.userName = kitchen.user.name +  " " + kitchen.user.last_name;
    }


    $scope.kitchen = kitchen;
    $scope.order = order;
    $scope.emptyDisplayRows = emptyDisplayRows;

    $timeout(function () {
        var el = document.querySelectorAll('img.lightense');
        Lightense(el);
    }, 200);
});

$(document).ready(function () {
    tippy('.tippy', {
        position: 'bottom',
        animation: 'shift',
        theme: 'light',
        arrow: true
    });

   

    window.removeSplit = function (id) {
        $("#splitTR-" + id).remove();
    }

    var acUsers = _.map(Casa.settings.users, function (item) { item.label = item.name; return item; });

    if ($("#users-ac").length > 0) {
        $("#users-ac").autocomplete({
            minLength: 1,
            source: acUsers,
            appendTo: "#splitUsersModal",
            focus: function (event, ui) {
                $("#users-ac").val(ui.item.label + " (" + ui.item.id + ")");
                return false;
            },
            select: function (event, ui) {
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
    }
   
});