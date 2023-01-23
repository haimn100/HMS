-- MySQL dump 10.13  Distrib 8.0.28, for Win64 (x86_64)
--
-- Host: localhost    Database: emptydb
-- ------------------------------------------------------
-- Server version	8.0.20

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `app_settings`
--

DROP TABLE IF EXISTS `app_settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `app_settings` (
  `id` int NOT NULL AUTO_INCREMENT,
  `addCreditCardChargeToRegister` bit(1) DEFAULT NULL,
  `requireImmigrationInfo` bit(1) DEFAULT NULL,
  `countryIsoAlpha2Code` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `checkInDefaultNationality` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `barcodePrefix` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `language` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `city` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `hotel_code` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `hotel_print_info` varchar(1024) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `app_settings`
--

LOCK TABLES `app_settings` WRITE;
/*!40000 ALTER TABLE `app_settings` DISABLE KEYS */;
INSERT INTO `app_settings` VALUES (3,_binary '\0',_binary '\0','co','383',NULL,NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `app_settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bed`
--

DROP TABLE IF EXISTS `bed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bed` (
  `id` int NOT NULL AUTO_INCREMENT,
  `bed_type_id` int DEFAULT NULL,
  `double_bed_partner_id` int DEFAULT NULL,
  `is_hidden` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `room_bed_unq` (`id`),
  KEY `double_bed_partner_fk_idx` (`double_bed_partner_id`),
  CONSTRAINT `double_bed_partner_fk` FOREIGN KEY (`double_bed_partner_id`) REFERENCES `bed` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=501 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bed`
--

LOCK TABLES `bed` WRITE;
/*!40000 ALTER TABLE `bed` DISABLE KEYS */;
INSERT INTO `bed` VALUES (500,1,NULL,_binary '');
/*!40000 ALTER TABLE `bed` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bed_type`
--

DROP TABLE IF EXISTS `bed_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bed_type` (
  `id` int NOT NULL AUTO_INCREMENT,
  `type` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bed_type`
--

LOCK TABLES `bed_type` WRITE;
/*!40000 ALTER TABLE `bed_type` DISABLE KEYS */;
INSERT INTO `bed_type` VALUES (1,'single'),(2,'double');
/*!40000 ALTER TABLE `bed_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cash_register_event`
--

DROP TABLE IF EXISTS `cash_register_event`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cash_register_event` (
  `id` int NOT NULL AUTO_INCREMENT,
  `event_type_id` int DEFAULT NULL,
  `event_value` decimal(10,0) DEFAULT NULL,
  `current_register_amount` decimal(10,0) NOT NULL,
  `event_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `staff_id` int DEFAULT NULL,
  `staff_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `comment` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `event_realted_entity_id` int DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=84058 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cash_register_event`
--

LOCK TABLES `cash_register_event` WRITE;
/*!40000 ALTER TABLE `cash_register_event` DISABLE KEYS */;
/*!40000 ALTER TABLE `cash_register_event` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `check_outs`
--

DROP TABLE IF EXISTS `check_outs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `check_outs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `bed_id` int DEFAULT NULL,
  `user_sex` bit(1) DEFAULT NULL,
  `check_in_date` datetime DEFAULT NULL,
  `check_out_date` datetime DEFAULT NULL,
  `total_kitchen` decimal(10,0) DEFAULT NULL,
  `total_services` decimal(10,0) DEFAULT NULL,
  `total_accommodation` decimal(10,0) DEFAULT NULL,
  `total_nights` int DEFAULT NULL,
  `total_cash` decimal(10,0) DEFAULT NULL,
  `total_credit` decimal(10,0) DEFAULT NULL,
  `total_discount` decimal(10,0) DEFAULT NULL,
  `total_canceled` decimal(10,0) DEFAULT NULL,
  `total_debit` decimal(10,0) DEFAULT NULL,
  `total` decimal(10,0) DEFAULT NULL,
  `price_per_night` decimal(10,0) DEFAULT NULL,
  `staff` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `credit_charge_percentage` decimal(10,0) DEFAULT NULL,
  `cash_deposit` decimal(10,0) DEFAULT NULL,
  `credit_deposit` decimal(10,0) DEFAULT NULL,
  `credit_charge_amount` decimal(10,0) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `co_uid_idx` (`user_id`),
  CONSTRAINT `co_uid` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17736 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `check_outs`
--

LOCK TABLES `check_outs` WRITE;
/*!40000 ALTER TABLE `check_outs` DISABLE KEYS */;
/*!40000 ALTER TABLE `check_outs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `event_type`
--

DROP TABLE IF EXISTS `event_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `event_type` (
  `id` int NOT NULL,
  `type` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `event_type`
--

LOCK TABLES `event_type` WRITE;
/*!40000 ALTER TABLE `event_type` DISABLE KEYS */;
INSERT INTO `event_type` VALUES (1,'Moved Bed'),(2,'Canceled Order'),(3,'Canceled Order Item');
/*!40000 ALTER TABLE `event_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `expense`
--

DROP TABLE IF EXISTS `expense`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `expense` (
  `id` int NOT NULL AUTO_INCREMENT,
  `expense_date` datetime DEFAULT NULL,
  `expense_val` decimal(10,0) DEFAULT NULL,
  `expense_category_id` int DEFAULT NULL,
  `report_date` datetime DEFAULT NULL,
  `comment` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `payment_type` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `exp-unq` (`expense_date`,`expense_val`,`expense_category_id`),
  KEY `exp_date` (`expense_date`),
  KEY `expense_cid_idx` (`expense_category_id`),
  CONSTRAINT `expense_cid` FOREIGN KEY (`expense_category_id`) REFERENCES `expense_category` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8507 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `expense`
--

LOCK TABLES `expense` WRITE;
/*!40000 ALTER TABLE `expense` DISABLE KEYS */;
/*!40000 ALTER TABLE `expense` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `expense_category`
--

DROP TABLE IF EXISTS `expense_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `expense_category` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `expense_category_type` int DEFAULT NULL,
  `is_active` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=149 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `expense_category`
--

LOCK TABLES `expense_category` WRITE;
/*!40000 ALTER TABLE `expense_category` DISABLE KEYS */;
/*!40000 ALTER TABLE `expense_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `expense_log`
--

DROP TABLE IF EXISTS `expense_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `expense_log` (
  `id` int NOT NULL AUTO_INCREMENT,
  `action_type` int DEFAULT NULL,
  `expense_id` int DEFAULT NULL,
  `expense_name` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `staff_id` int DEFAULT NULL,
  `staff_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `_timestamp` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `expense_val` decimal(10,0) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=495 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `expense_log`
--

LOCK TABLES `expense_log` WRITE;
/*!40000 ALTER TABLE `expense_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `expense_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `guest_count`
--

DROP TABLE IF EXISTS `guest_count`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `guest_count` (
  `id` int NOT NULL AUTO_INCREMENT,
  `guestcount_date` date DEFAULT NULL,
  `count` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `guestcount_date_UNIQUE` (`guestcount_date`)
) ENGINE=InnoDB AUTO_INCREMENT=14965 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `guest_count`
--

LOCK TABLES `guest_count` WRITE;
/*!40000 ALTER TABLE `guest_count` DISABLE KEYS */;
/*!40000 ALTER TABLE `guest_count` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `house_keeper`
--

DROP TABLE IF EXISTS `house_keeper`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `house_keeper` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `create_date` datetime DEFAULT NULL,
  `is_active` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `house_keeper`
--

LOCK TABLES `house_keeper` WRITE;
/*!40000 ALTER TABLE `house_keeper` DISABLE KEYS */;
/*!40000 ALTER TABLE `house_keeper` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `house_keeping_tracking`
--

DROP TABLE IF EXISTS `house_keeping_tracking`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `house_keeping_tracking` (
  `id` int NOT NULL AUTO_INCREMENT,
  `house_keeper_id` int DEFAULT NULL,
  `house_keeper_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `room_number` int DEFAULT NULL,
  `assigned_date` datetime DEFAULT NULL,
  `finish_date` datetime DEFAULT NULL,
  `num_of_beds_cleaned` int DEFAULT NULL,
  `comment` varchar(1024) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11888 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `house_keeping_tracking`
--

LOCK TABLES `house_keeping_tracking` WRITE;
/*!40000 ALTER TABLE `house_keeping_tracking` DISABLE KEYS */;
/*!40000 ALTER TABLE `house_keeping_tracking` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `income`
--

DROP TABLE IF EXISTS `income`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `income` (
  `id` int NOT NULL AUTO_INCREMENT,
  `date` datetime DEFAULT NULL,
  `val` decimal(10,0) DEFAULT NULL,
  `category_id` int DEFAULT NULL,
  `report_date` datetime DEFAULT NULL,
  `comment` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=405 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `income`
--

LOCK TABLES `income` WRITE;
/*!40000 ALTER TABLE `income` DISABLE KEYS */;
/*!40000 ALTER TABLE `income` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `income_category`
--

DROP TABLE IF EXISTS `income_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `income_category` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `income_category_type` int DEFAULT NULL,
  `is_active` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `income_category`
--

LOCK TABLES `income_category` WRITE;
/*!40000 ALTER TABLE `income_category` DISABLE KEYS */;
/*!40000 ALTER TABLE `income_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `income_log`
--

DROP TABLE IF EXISTS `income_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `income_log` (
  `id` int NOT NULL AUTO_INCREMENT,
  `action_type` int DEFAULT NULL,
  `income_id` int DEFAULT NULL,
  `income_name` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `staff_id` int DEFAULT NULL,
  `staff_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `_timestamp` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `income_val` decimal(10,0) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `income_log`
--

LOCK TABLES `income_log` WRITE;
/*!40000 ALTER TABLE `income_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `income_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ingredient`
--

DROP TABLE IF EXISTS `ingredient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=266 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
/*!40000 ALTER TABLE `ingredient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_category`
--

DROP TABLE IF EXISTS `menu_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_category` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `number` int DEFAULT NULL,
  `parent_cat_id` int DEFAULT NULL,
  `menu_category_type` int NOT NULL DEFAULT '1',
  `is_active` bit(1) NOT NULL DEFAULT b'1',
  `is_deleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`id`),
  KEY `cattype_idx` (`menu_category_type`),
  CONSTRAINT `cmenutype` FOREIGN KEY (`menu_category_type`) REFERENCES `menu_category_type` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_category`
--

LOCK TABLES `menu_category` WRITE;
/*!40000 ALTER TABLE `menu_category` DISABLE KEYS */;
/*!40000 ALTER TABLE `menu_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_category_type`
--

DROP TABLE IF EXISTS `menu_category_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_category_type` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_category_type`
--

LOCK TABLES `menu_category_type` WRITE;
/*!40000 ALTER TABLE `menu_category_type` DISABLE KEYS */;
INSERT INTO `menu_category_type` VALUES (1,'Kitchen'),(2,'Services');
/*!40000 ALTER TABLE `menu_category_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_item`
--

DROP TABLE IF EXISTS `menu_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_item` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `number` int DEFAULT NULL,
  `cat_id` int NOT NULL,
  `menu_category_type` int NOT NULL DEFAULT '1',
  `price` decimal(10,2) NOT NULL,
  `is_active` bit(1) NOT NULL DEFAULT b'1',
  `consumption` decimal(10,2) DEFAULT NULL,
  `product_weight` decimal(10,2) DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `is_deleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`id`),
  KEY `menu_item_cat_id_idx` (`cat_id`),
  CONSTRAINT `menu_item_cat_id` FOREIGN KEY (`cat_id`) REFERENCES `menu_category` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=431 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_item`
--

LOCK TABLES `menu_item` WRITE;
/*!40000 ALTER TABLE `menu_item` DISABLE KEYS */;
/*!40000 ALTER TABLE `menu_item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_item_ingredient`
--

DROP TABLE IF EXISTS `menu_item_ingredient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_item_ingredient` (
  `id` int NOT NULL AUTO_INCREMENT,
  `menu_item_id` int NOT NULL,
  `ingredient_id` int NOT NULL,
  `ingredient_price` decimal(10,0) DEFAULT NULL,
  `ingredients_group` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `ingredients_group_number` int NOT NULL,
  `ingredients_group_single_select` bit(1) NOT NULL DEFAULT b'0',
  `product_weight` decimal(10,2) DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ing_uniq` (`menu_item_id`,`ingredient_id`,`ingredients_group`),
  KEY `menu_item_id_idx` (`menu_item_id`),
  KEY `ingredient_id_idx` (`ingredient_id`),
  CONSTRAINT `ingredient_id` FOREIGN KEY (`ingredient_id`) REFERENCES `ingredient` (`id`),
  CONSTRAINT `menu_item_id` FOREIGN KEY (`menu_item_id`) REFERENCES `menu_item` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1262 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_item_ingredient`
--

LOCK TABLES `menu_item_ingredient` WRITE;
/*!40000 ALTER TABLE `menu_item_ingredient` DISABLE KEYS */;
/*!40000 ALTER TABLE `menu_item_ingredient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `menu_order`
--

DROP TABLE IF EXISTS `menu_order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_order` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `user_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_bed` int DEFAULT NULL,
  `order_date` datetime NOT NULL,
  `pay_type_id` int NOT NULL,
  `is_canceled` bit(1) NOT NULL DEFAULT b'0',
  `staff_id` int DEFAULT NULL,
  `staff_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `total` decimal(10,0) NOT NULL,
  `split_count` int DEFAULT NULL,
  `split_total` decimal(10,0) DEFAULT NULL,
  `splited_by` int DEFAULT NULL,
  `splited_order_id` int DEFAULT NULL,
  `canceled_by_staff_id` int DEFAULT NULL,
  `menu_category_type` int NOT NULL,
  `comment` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `credit_charge_percentage` decimal(10,0) DEFAULT NULL,
  `discount` int DEFAULT NULL,
  `credit_charge` decimal(10,0) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `user_order_unq` (`user_id`,`id`),
  KEY `pay_type_id_idx` (`pay_type_id`),
  KEY `cat_type_indx` (`menu_category_type`),
  KEY `order_date_indx` (`order_date`),
  CONSTRAINT `order_user_id_fk` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`),
  CONSTRAINT `pay_type_id` FOREIGN KEY (`pay_type_id`) REFERENCES `pay_type` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=221743 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_order`
--

LOCK TABLES `menu_order` WRITE;
/*!40000 ALTER TABLE `menu_order` DISABLE KEYS */;
/*!40000 ALTER TABLE `menu_order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order_items`
--

DROP TABLE IF EXISTS `order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_date` datetime NOT NULL,
  `order_id` int NOT NULL,
  `menu_item_id` int NOT NULL,
  `menu_item_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `menu_category_id` int DEFAULT NULL,
  `menu_category_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `menu_category_type` int NOT NULL,
  `menu_item_ingredients` varchar(2048) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `comment` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `split_total` decimal(10,0) DEFAULT NULL,
  `total` decimal(10,0) NOT NULL,
  `user_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `order_id_fk_idx` (`order_id`),
  KEY `order_item_user_fk_idx` (`user_id`),
  KEY `menu_cat_type_indx` (`menu_category_type`),
  KEY `mii_indx` (`menu_item_id`),
  CONSTRAINT `order_id_fk` FOREIGN KEY (`order_id`) REFERENCES `menu_order` (`id`),
  CONSTRAINT `order_item_user_fk` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=343376 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_items`
--

LOCK TABLES `order_items` WRITE;
/*!40000 ALTER TABLE `order_items` DISABLE KEYS */;
/*!40000 ALTER TABLE `order_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pay_type`
--

DROP TABLE IF EXISTS `pay_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pay_type` (
  `id` int NOT NULL AUTO_INCREMENT,
  `type` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pay_type`
--

LOCK TABLES `pay_type` WRITE;
/*!40000 ALTER TABLE `pay_type` DISABLE KEYS */;
INSERT INTO `pay_type` VALUES (1,'tab'),(2,'credit'),(3,'cash');
/*!40000 ALTER TABLE `pay_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `title` varchar(45) DEFAULT NULL,
  `code` varchar(15) DEFAULT NULL,
  `weight` decimal(10,2) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `is_deleted` tinyint DEFAULT NULL,
  `note` varchar(145) DEFAULT NULL,
  `created` datetime DEFAULT NULL,
  `brand` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `product_unq` (`code`),
  KEY `prod_ex_idx` (`supplier_id`),
  CONSTRAINT `prod_ex` FOREIGN KEY (`supplier_id`) REFERENCES `expense_category` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=200 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product`
--

LOCK TABLES `product` WRITE;
/*!40000 ALTER TABLE `product` DISABLE KEYS */;
/*!40000 ALTER TABLE `product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_assign`
--

DROP TABLE IF EXISTS `product_assign`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_assign` (
  `id` int NOT NULL AUTO_INCREMENT,
  `menu_item_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `product_weight` decimal(10,2) DEFAULT NULL,
  `menu_item_ing_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `productassignfk_idx` (`product_id`),
  CONSTRAINT `productassignfk` FOREIGN KEY (`product_id`) REFERENCES `product` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_assign`
--

LOCK TABLES `product_assign` WRITE;
/*!40000 ALTER TABLE `product_assign` DISABLE KEYS */;
/*!40000 ALTER TABLE `product_assign` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_inventory`
--

DROP TABLE IF EXISTS `product_inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product_inventory` (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int DEFAULT NULL,
  `change` decimal(10,2) DEFAULT NULL,
  `current_amount` decimal(10,2) DEFAULT NULL,
  `action_type` int DEFAULT NULL,
  `related_entity` int DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=365 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product_inventory`
--

LOCK TABLES `product_inventory` WRITE;
/*!40000 ALTER TABLE `product_inventory` DISABLE KEYS */;
/*!40000 ALTER TABLE `product_inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reservation`
--

DROP TABLE IF EXISTS `reservation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservation` (
  `id` int NOT NULL AUTO_INCREMENT,
  `res_id` int DEFAULT NULL,
  `room_type` int DEFAULT NULL,
  `res_date` date NOT NULL,
  `sex` bit(1) DEFAULT NULL,
  `nights` int DEFAULT NULL,
  `res_name` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `employee_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `status` int NOT NULL DEFAULT '1',
  `allow_mix_dorm` bit(1) DEFAULT b'0',
  `bed_id` int DEFAULT NULL,
  `room_id` int DEFAULT NULL,
  `res_date_end` date DEFAULT NULL,
  `number_of_people` int DEFAULT NULL,
  `comment` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `from_channel_manager` bit(1) DEFAULT NULL,
  `res_email` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `deleted_at` datetime DEFAULT NULL,
  `origin` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `res_date_indx` (`res_date`)
) ENGINE=InnoDB AUTO_INCREMENT=23164 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reservation`
--

LOCK TABLES `reservation` WRITE;
/*!40000 ALTER TABLE `reservation` DISABLE KEYS */;
/*!40000 ALTER TABLE `reservation` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reservation_status`
--

DROP TABLE IF EXISTS `reservation_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservation_status` (
  `id` int NOT NULL,
  `name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reservation_status`
--

LOCK TABLES `reservation_status` WRITE;
/*!40000 ALTER TABLE `reservation_status` DISABLE KEYS */;
INSERT INTO `reservation_status` VALUES (1,'PendingAssignment'),(2,'Assigned'),(3,'NotAvailable');
/*!40000 ALTER TABLE `reservation_status` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `resident`
--

DROP TABLE IF EXISTS `resident`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `resident` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `resident_user_idx` (`user_id`),
  CONSTRAINT `resident_user` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=297 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resident`
--

LOCK TABLES `resident` WRITE;
/*!40000 ALTER TABLE `resident` DISABLE KEYS */;
/*!40000 ALTER TABLE `resident` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `restaurant_inventory_archive`
--

DROP TABLE IF EXISTS `restaurant_inventory_archive`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `restaurant_inventory_archive` (
  `id` int NOT NULL AUTO_INCREMENT,
  `original_id` varchar(45) NOT NULL,
  `table_name` varchar(45) NOT NULL,
  `record_data_json` text,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `restaurant_inventory_archive`
--

LOCK TABLES `restaurant_inventory_archive` WRITE;
/*!40000 ALTER TABLE `restaurant_inventory_archive` DISABLE KEYS */;
/*!40000 ALTER TABLE `restaurant_inventory_archive` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `restaurant_inventory_product`
--

DROP TABLE IF EXISTS `restaurant_inventory_product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `restaurant_inventory_product` (
  `id` int NOT NULL AUTO_INCREMENT,
  `supplier_id` int DEFAULT NULL,
  `name` varchar(45) NOT NULL,
  `brand` varchar(45) DEFAULT NULL,
  `code` varchar(254) NOT NULL,
  `weight` decimal(10,1) NOT NULL,
  `price` decimal(10,2) NOT NULL,
  `note` varchar(245) DEFAULT NULL,
  `quantity_in_stock` decimal(10,0) DEFAULT NULL,
  `quantity_warning_thershold` decimal(10,0) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=200 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `restaurant_inventory_product`
--

LOCK TABLES `restaurant_inventory_product` WRITE;
/*!40000 ALTER TABLE `restaurant_inventory_product` DISABLE KEYS */;
/*!40000 ALTER TABLE `restaurant_inventory_product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `restaurant_inventory_stock`
--

DROP TABLE IF EXISTS `restaurant_inventory_stock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `restaurant_inventory_stock` (
  `id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `change_in_quantity` decimal(10,2) NOT NULL,
  `quantity` decimal(10,2) NOT NULL,
  `origin` int NOT NULL DEFAULT '1',
  `note` varchar(245) DEFAULT NULL,
  `related_entity` int DEFAULT NULL,
  `created` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1017 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `restaurant_inventory_stock`
--

LOCK TABLES `restaurant_inventory_stock` WRITE;
/*!40000 ALTER TABLE `restaurant_inventory_stock` DISABLE KEYS */;
/*!40000 ALTER TABLE `restaurant_inventory_stock` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `restaurant_inventory_supplier`
--

DROP TABLE IF EXISTS `restaurant_inventory_supplier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `restaurant_inventory_supplier` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `email` varchar(320) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=139 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `restaurant_inventory_supplier`
--

LOCK TABLES `restaurant_inventory_supplier` WRITE;
/*!40000 ALTER TABLE `restaurant_inventory_supplier` DISABLE KEYS */;
/*!40000 ALTER TABLE `restaurant_inventory_supplier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `room`
--

DROP TABLE IF EXISTS `room`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `room` (
  `id` int NOT NULL AUTO_INCREMENT,
  `room_number` int DEFAULT NULL,
  `floor` int DEFAULT NULL,
  `room_type_id` int DEFAULT NULL,
  `is_clean_required` bit(1) DEFAULT NULL,
  `note` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `is_cleaning_inspection_required` bit(1) DEFAULT NULL,
  `assigned_house_keeper_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `assigned_house_keeper_id` int DEFAULT NULL,
  `house_keeping_tracking_id` int DEFAULT NULL,
  `assigned_house_keeper_date` datetime DEFAULT NULL,
  `is_cleaning_inspection_date` datetime DEFAULT NULL,
  `last_cleaned` datetime DEFAULT NULL,
  `amenities` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=68 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `room`
--

LOCK TABLES `room` WRITE;
/*!40000 ALTER TABLE `room` DISABLE KEYS */;
/*!40000 ALTER TABLE `room` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `room_bed`
--

DROP TABLE IF EXISTS `room_bed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `room_bed` (
  `id` int NOT NULL AUTO_INCREMENT,
  `room_id` int NOT NULL,
  `room_number` int DEFAULT NULL,
  `bed_id` int NOT NULL,
  `bed_type_id` int DEFAULT NULL,
  `double_bed_partner_id` int DEFAULT NULL,
  `last_checkout` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `room_bed_unq` (`room_id`,`bed_id`),
  KEY `toombed_fk_idx` (`bed_id`),
  CONSTRAINT `roombed_fk` FOREIGN KEY (`room_id`) REFERENCES `room` (`id`),
  CONSTRAINT `toombed_fk` FOREIGN KEY (`bed_id`) REFERENCES `bed` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1097 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `room_bed`
--

LOCK TABLES `room_bed` WRITE;
/*!40000 ALTER TABLE `room_bed` DISABLE KEYS */;
/*!40000 ALTER TABLE `room_bed` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `room_type`
--

DROP TABLE IF EXISTS `room_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `room_type` (
  `id` int NOT NULL AUTO_INCREMENT,
  `type` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `room_type`
--

LOCK TABLES `room_type` WRITE;
/*!40000 ALTER TABLE `room_type` DISABLE KEYS */;
INSERT INTO `room_type` VALUES (1,'Single'),(2,'Dorm');
/*!40000 ALTER TABLE `room_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shift_end`
--

DROP TABLE IF EXISTS `shift_end`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shift_end` (
  `id` int NOT NULL AUTO_INCREMENT,
  `shift_date` datetime NOT NULL,
  `shift_employee_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `shift_employee_id` int DEFAULT NULL,
  `shift_total_cash` decimal(10,0) DEFAULT NULL,
  `shift_total_credit` decimal(10,0) DEFAULT NULL,
  `shift_total_canceled` decimal(10,0) DEFAULT NULL,
  `shift_total` decimal(10,0) DEFAULT NULL,
  `shift_total_checkouts` decimal(10,2) DEFAULT NULL,
  `shift_total_kitchen` decimal(10,2) DEFAULT NULL,
  `shift_total_services` decimal(10,2) DEFAULT NULL,
  `checkouts_cash` decimal(10,2) DEFAULT NULL,
  `checkouts_credit` decimal(10,2) DEFAULT NULL,
  `expenses` decimal(10,2) DEFAULT NULL,
  `incomes` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `shift_date` (`shift_date`)
) ENGINE=InnoDB AUTO_INCREMENT=3027 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shift_end`
--

LOCK TABLES `shift_end` WRITE;
/*!40000 ALTER TABLE `shift_end` DISABLE KEYS */;
/*!40000 ALTER TABLE `shift_end` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shopping_list`
--

DROP TABLE IF EXISTS `shopping_list`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shopping_list` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(145) DEFAULT NULL,
  `created` datetime NOT NULL,
  `updated` datetime DEFAULT NULL,
  `purchased` datetime DEFAULT NULL,
  `supplier_id` int DEFAULT NULL,
  `deleted` datetime DEFAULT NULL,
  `expense_id` int DEFAULT NULL,
  `period` int DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shopping_list`
--

LOCK TABLES `shopping_list` WRITE;
/*!40000 ALTER TABLE `shopping_list` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopping_list` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shopping_list_product`
--

DROP TABLE IF EXISTS `shopping_list_product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shopping_list_product` (
  `id` int NOT NULL AUTO_INCREMENT,
  `shopping_list_id` int DEFAULT NULL,
  `supplier_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `required_amount` decimal(10,2) DEFAULT NULL,
  `inventory_amount` decimal(10,2) DEFAULT NULL,
  `recommended_amount` decimal(10,2) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `note` varchar(245) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `shplistid_idx` (`shopping_list_id`),
  CONSTRAINT `shplistid` FOREIGN KEY (`shopping_list_id`) REFERENCES `shopping_list` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shopping_list_product`
--

LOCK TABLES `shopping_list_product` WRITE;
/*!40000 ALTER TABLE `shopping_list_product` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopping_list_product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staff`
--

DROP TABLE IF EXISTS `staff`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staff` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `email` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `password` varchar(245) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `start_date` datetime DEFAULT NULL,
  `type` int DEFAULT NULL,
  `is_working` bit(1) DEFAULT NULL,
  `phone` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `pin` varchar(4) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `staaff` (`email`,`type`),
  KEY `staff_type_idx` (`type`),
  CONSTRAINT `staff_type` FOREIGN KEY (`type`) REFERENCES `user_type` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=108 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staff`
--

LOCK TABLES `staff` WRITE;
/*!40000 ALTER TABLE `staff` DISABLE KEYS */;
/*!40000 ALTER TABLE `staff` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staff_event`
--

DROP TABLE IF EXISTS `staff_event`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staff_event` (
  `id` int NOT NULL AUTO_INCREMENT,
  `event_date` datetime DEFAULT NULL,
  `staff_id` int DEFAULT NULL,
  `staff_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `guest_id` int DEFAULT NULL,
  `event_type_id` int DEFAULT NULL,
  `event_value` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `user_cativity_indx` (`staff_id`)
) ENGINE=InnoDB AUTO_INCREMENT=10286 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staff_event`
--

LOCK TABLES `staff_event` WRITE;
/*!40000 ALTER TABLE `staff_event` DISABLE KEYS */;
/*!40000 ALTER TABLE `staff_event` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock`
--

DROP TABLE IF EXISTS `stock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock` (
  `id` int NOT NULL AUTO_INCREMENT,
  `menu_item_id` int NOT NULL,
  `menu_item_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `menu_item_number` int DEFAULT NULL,
  `menu_item_category_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `quantity` int NOT NULL,
  `warning_quantity` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mi_fk_unq` (`menu_item_id`),
  KEY `mi_fk_indx` (`menu_item_id`),
  CONSTRAINT `mi_fk` FOREIGN KEY (`menu_item_id`) REFERENCES `menu_item` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=286 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock`
--

LOCK TABLES `stock` WRITE;
/*!40000 ALTER TABLE `stock` DISABLE KEYS */;
/*!40000 ALTER TABLE `stock` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stock_history`
--

DROP TABLE IF EXISTS `stock_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stock_history` (
  `id` int NOT NULL AUTO_INCREMENT,
  `menu_item_id` int NOT NULL,
  `menu_item_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `menu_item_number` int DEFAULT NULL,
  `menu_item_category_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `quantity` int NOT NULL,
  `total` int DEFAULT NULL,
  `timestamp` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=212 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stock_history`
--

LOCK TABLES `stock_history` WRITE;
/*!40000 ALTER TABLE `stock_history` DISABLE KEYS */;
/*!40000 ALTER TABLE `stock_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `id` int NOT NULL AUTO_INCREMENT,
  `passport` varchar(20) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `cidate` datetime NOT NULL,
  `codate` datetime DEFAULT NULL,
  `is_checked_out` bit(1) DEFAULT NULL,
  `pic` varchar(512) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `bed_id` int DEFAULT NULL,
  `sex` bit(1) DEFAULT NULL,
  `email` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `name` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `nationality` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `checked_in_by` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `is_hidden` bit(1) DEFAULT NULL,
  `phone` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `birth_date` datetime DEFAULT NULL,
  `arrival` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `destination` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `profession` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `document_type` int DEFAULT NULL,
  `barcode` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `last_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `is_resident` bit(1) DEFAULT b'0',
  `intended_codate` date DEFAULT NULL,
  `res_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `email_indx` (`email`),
  KEY `user_bed_fk_idx` (`bed_id`),
  KEY `codate_index` (`codate`),
  KEY `cidate_indx` (`cidate`),
  CONSTRAINT `user_bed_fk` FOREIGN KEY (`bed_id`) REFERENCES `bed` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17976 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'1234','2017-09-22 18:50:20',NULL,NULL,NULL,500,NULL,NULL,'system',NULL,NULL,_binary '',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,_binary '\0',NULL,NULL);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_bed`
--

DROP TABLE IF EXISTS `user_bed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_bed` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `bed_id` int DEFAULT NULL,
  `price` decimal(10,0) DEFAULT NULL,
  `comment` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `start_date` datetime DEFAULT NULL,
  `end_date` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `user_bed_user_fk_idx` (`user_id`),
  KEY `user_bed_bed_fl_idx` (`bed_id`),
  CONSTRAINT `user_bed_bed_fl` FOREIGN KEY (`bed_id`) REFERENCES `bed` (`id`),
  CONSTRAINT `user_bed_user_fk` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20912 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_bed`
--

LOCK TABLES `user_bed` WRITE;
/*!40000 ALTER TABLE `user_bed` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_bed` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_discount`
--

DROP TABLE IF EXISTS `user_discount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_discount` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `comment` varchar(145) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `price` int DEFAULT NULL,
  `discount_date` datetime NOT NULL,
  `staff` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `payment_type_id` int DEFAULT NULL,
  `user_name` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `order_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `dis_user_id_idx` (`user_id`),
  CONSTRAINT `dis_user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6813 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_discount`
--

LOCK TABLES `user_discount` WRITE;
/*!40000 ALTER TABLE `user_discount` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_discount` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_prepay`
--

DROP TABLE IF EXISTS `user_prepay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_prepay` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `amount` int DEFAULT NULL,
  `pay_type` int DEFAULT NULL,
  `comment` varchar(1024) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `prepay_date` datetime DEFAULT NULL,
  `staff` varchar(48) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `deposit_credit_card_charge` decimal(10,0) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `prepay_user_idx` (`user_id`),
  CONSTRAINT `prepay_user` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15209 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_prepay`
--

LOCK TABLES `user_prepay` WRITE;
/*!40000 ALTER TABLE `user_prepay` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_prepay` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_type`
--

DROP TABLE IF EXISTS `user_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_type` (
  `id` int NOT NULL AUTO_INCREMENT,
  `type` varchar(45) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_type`
--

LOCK TABLES `user_type` WRITE;
/*!40000 ALTER TABLE `user_type` DISABLE KEYS */;
INSERT INTO `user_type` VALUES (1,'Admin'),(2,'Guest'),(3,'Employee'),(4,'Editor'),(5,'HouseKeeper'),(6,'KitchenManager');
/*!40000 ALTER TABLE `user_type` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-06-15 15:59:40
