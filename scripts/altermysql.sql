alter table cash_register_event
modify `event_date` datetime NULL DEFAULT CURRENT_TIMESTAMP,
modify `event_realted_entity_id` int(11) NULL;

alter table check_outs
add `credit_charge_amount` decimal(10,0) NULL;

alter table expense
modify `expense_date` datetime NULL,
add `report_date` datetime NULL,
add `comment` varchar(245) NULL;

alter table expense_category
add `expense_category_type` int(11) NULL,
add `is_active` bit(1) NULL;

alter table menu_order
modify `discount` int(11) NULL;

alter table reservation
modify `res_date` datetime NOT NULL,
add `bed_id` int(11) NULL,
modify `res_date_end` datetime NULL;

alter table room
add `is_cleaning_inspection_required` bit(1) NULL,
add `assigned_house_keeper_name` varchar(45) NULL,
add `assigned_house_keeper_id` int NULL,
add  `house_keeping_tracking_id` int(11) NULL,
add  `assigned_house_keeper_date` datetime NULL,
add  `is_cleaning_inspection_date` datetime NULL,
add  `last_cleaned` datetime NULL;

alter table room_bed
add `last_checkout` datetime NULL;

alter table shift_end
add `shift_total_checkouts` decimal(10,2) NULL,
add   `shift_total_kitchen` decimal(10,2) NULL,
add   `shift_total_services` decimal(10,2) NULL,
add   `checkouts_cash` decimal(10,2) NULL,
add   `checkouts_credit` decimal(10,2) NULL,
add   `expenses` decimal(10,2) NULL,
add   `incomes` decimal(10,2) NULL;

alter table staff
modify `pin` varchar(4) NULL;

alter table user
modify `birth_date` datetime NULL,
add `is_resident` bit(1) NULL DEFAULT b'0',
add `intended_codate` date NULL;

CREATE TABLE `expense_log` (
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`action_type` int(11) DEFAULT NULL,
	`expense_id` int(11) DEFAULT NULL,
	`expense_name` varchar(245) COLLATE utf8_unicode_ci DEFAULT NULL,
	`staff_id` int(11) DEFAULT NULL,
	`staff_name` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
	`_timestamp` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
	`expense_val` decimal(10,0) DEFAULT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE `house_keeper` (
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`name` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
	`create_date` datetime DEFAULT NULL,
	`is_active` bit(1) DEFAULT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE `house_keeping_tracking` (
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`house_keeper_id` int(11) DEFAULT NULL,
	`house_keeper_name` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
	`room_number` int(11) DEFAULT NULL,
	`assigned_date` datetime DEFAULT NULL,
	`finish_date` datetime DEFAULT NULL,
	`num_of_beds_cleaned` int(11) DEFAULT NULL,
	`comment` varchar(1024) COLLATE utf8_unicode_ci DEFAULT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE `income` (
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`date` datetime DEFAULT NULL,
	`val` decimal(10,0) DEFAULT NULL,
	`category_id` int(11) DEFAULT NULL,
	`report_date` datetime DEFAULT NULL,
	`comment` varchar(245) COLLATE utf8_unicode_ci DEFAULT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE `income_category` (
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`name` varchar(145) COLLATE utf8_unicode_ci DEFAULT NULL,
	`income_category_type` int(11) DEFAULT NULL,
	`is_active` bit(1) DEFAULT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE `income_log` (
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`action_type` int(11) DEFAULT NULL,
	`income_id` int(11) DEFAULT NULL,
	`income_name` varchar(245) COLLATE utf8_unicode_ci DEFAULT NULL,
	`staff_id` int(11) DEFAULT NULL,
	`staff_name` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
	`_timestamp` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
	`income_val` decimal(10,0) DEFAULT NULL,
	PRIMARY KEY (`id`)
)


insert into user_type (id,type) values(4,'Editor');
	insert into user_type (id,type) values(5,'HouseKeeper');
		insert into user_type (id,type) values(6,'KitchenManager');





