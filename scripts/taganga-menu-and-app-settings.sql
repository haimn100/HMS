LOCK TABLES `menu_category` WRITE;
/*!40000 ALTER TABLE `menu_category` DISABLE KEYS */;
INSERT INTO `menu_category` VALUES (1,'DESAYUNOS',1,NULL,1,''),(2,'TOSTADAS',2,NULL,1,''),(3,'SANDWICH',3,NULL,1,''),(4,'BEBIDAS',4,NULL,1,''),(5,'BEBIDAS CALIENTES',5,NULL,1,''),(6,'LICORES',6,NULL,1,''),(7,'ENSALADAS',7,NULL,1,''),(8,'PLATOS PRINCIPALES',8,NULL,1,''),(9,'ESPAGUETI',9,NULL,1,''),(10,'ESPECIALES DE LA CASA',10,NULL,1,''),(11,'HUMMUS',11,NULL,1,''),(12,'POSTRES',12,NULL,1,''),(13,'VIAJES',13,NULL,1,''),(14,'VITRINA',14,NULL,1,''),(15,'EXTRA',15,NULL,1,'');
/*!40000 ALTER TABLE `menu_category` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `menu_item` WRITE;
/*!40000 ALTER TABLE `menu_item` DISABLE KEYS */;
INSERT INTO `menu_item` VALUES (1,'Desayuno de la casa',1,1,1,14000,''),(2,'tostado de queso',7,2,1,14000,''),(3,'Desayuno israeli ',2,1,1,14000,''),(4,'desayuno frances',3,1,1,14000,''),(5,'tostada bulgara',8,2,1,14000,''),(6,'SHAKSHUKA DE LA CASA',4,1,1,14000,''),(7,'CORNFLEKS',5,1,1,10000,''),(8,'MUSLI',6,1,1,14000,''),(9,'TOSTADA DE ATUN',9,2,1,16000,''),(10,'SANDWICH HOMELET',10,3,1,14000,''),(11,'SANDWICH MOZARELLA',11,3,1,14000,''),(12,'SANDWICH AGUACATE',12,3,1,15000,''),(13,'SANDWICH SABIH',13,3,1,15000,''),(14,'SANDWICH TUNISAI',14,3,1,15000,''),(15,'AGUA',100,4,1,3000,''),(20,'GASEOSA',101,4,1,4000,''),(21,'JUGOS DE BOTELLAS',102,4,1,4000,''),(22,'TE HELADO',103,4,1,4000,''),(23,'GATORADE',104,4,1,4000,''),(29,'JUGO NATURAL EN AGUA',105,4,1,4000,''),(30,'JUGO NATURAL EN LECHE',106,4,1,5000,''),(31,'TE',107,5,1,2000,''),(32,'CAFE',108,5,1,3000,''),(33,'NESCAFE',109,5,1,3000,''),(34,'CAFE CON LECHE FRIO/CALIENTE',110,5,1,4000,''),(35,'CHOCOLATE FRIO/CALIENTE',111,5,1,4000,''),(36,'AGUILA',112,6,1,5000,''),(37,'AGUILA LIGTH',113,6,1,5000,''),(38,'CLUB COLOMBIA',114,6,1,5000,''),(39,'CORONA',115,6,1,8000,''),(40,'HEINEKEN',116,6,1,8000,''),(41,'RED BULL',117,6,1,10000,''),(42,'SHOT ',118,6,1,10000,''),(43,'CUBA LIBRE',119,6,1,10000,''),(44,'WHISKEY CON COCA COLA',120,6,1,10000,''),(45,'VASO DE VODKA CON NARANJA',121,6,1,10000,''),(46,'VASO DE VODKA CON RED BULL',122,6,1,15000,''),(47,'BOTELLA DE AGUARDIENTE',123,6,1,60000,''),(48,'BOTELLA DE RON',124,6,1,60000,''),(49,'BOTELLA DE TEQUILA',125,6,1,80000,''),(50,'BOTELLA DE VODKA',126,6,1,80000,''),(51,'ENSALADA ISRAELI',15,7,1,14000,''),(52,'ENSALADA GRIEGA',16,7,1,16000,''),(53,'ENSALADA DE ATUN',17,7,1,16000,''),(54,'ENSALADA JERUSALEM',18,7,1,16000,''),(55,'ENSALADA DE POLLO',19,7,1,17000,''),(56,'PECHUGA DE POLLO',20,8,1,17000,''),(57,'MILANESA',21,8,1,17000,''),(58,'PARGUIT',22,8,1,17000,''),(59,'KABAB',23,8,1,17000,''),(60,'ALBONDIGAS CON SALSA DE TOMATE',24,8,1,17000,''),(61,'SALMON ',25,8,1,25000,''),(62,'ESPAGUETI NAPOLITANA',26,9,1,16000,''),(63,'ESPAGUETI ALFREDO',27,9,1,17000,''),(64,'ESPAGUETI BOLONESA',28,9,1,17000,''),(65,'SALTEADO DE POLLO',29,10,1,17000,''),(66,'HAMBURGUESA',30,10,1,17000,''),(67,'TORTILLA DE CARNE',31,10,1,17000,''),(68,'BUREKAS',32,10,1,16000,''),(69,'MALAWAH',33,10,1,16000,''),(70,'MALAWAH PIZZA',34,10,1,18000,''),(71,'TORTILLA DE QUESO',35,10,1,16000,''),(72,'PLATO DE PAPAS FRITAS',36,10,1,10000,''),(73,'HUMMUS DE LA CASA',37,11,1,14000,''),(74,'HUMMUS THINA/CHAMPINONES',38,11,1,16000,''),(75,'HUMMUS CARNE/POLLO',39,11,1,17000,''),(76,'MALTEADA DE OREO ',40,12,1,8000,''),(77,'MALTEADA DE CAFE',41,12,1,8000,''),(78,'MALTEADA DE KINDER BUENO',42,12,1,13000,''),(79,'ENSALADA DE FRUTAS',43,12,1,11000,''),(80,'TOSTADA CON NUTELLA',44,12,1,12000,''),(81,'BANANA SPLIT',45,12,1,14000,''),(82,'PANQUEQUES',46,12,1,14000,''),(83,'MINKA ',121,13,1,70000,''),(84,'MINKA TOUR',122,13,1,90000,''),(85,'CARTAGENA',123,13,1,50000,''),(86,'SHAMPOO',123,14,1,2000,''),(87,'CEPILLO DE DIENTES',124,14,1,3000,''),(88,'CREMA DENTAS',125,14,1,3000,''),(89,'DESODORANTE',126,14,1,2000,''),(90,'TAMPONES',127,14,1,1000,''),(91,'PROTECTORES',128,14,1,1000,''),(92,'REPELENTE',129,14,1,5000,''),(98,'PANOS HUMEDOS',130,14,1,6000,''),(99,'GRINDER',131,14,1,25000,''),(100,'RIZLA',132,14,1,10000,''),(101,'FILTROS',133,14,1,5000,''),(102,'CONDONES',134,14,1,5000,''),(103,'PIPA',135,14,1,10000,''),(104,'TOALLAS',136,14,1,25000,''),(105,'PAREO',137,14,1,50000,''),(106,'CAMISAS',138,14,1,20000,''),(107,'CLIPPER',139,14,1,4000,''),(108,'MARLBORO',140,14,1,8000,''),(109,'MANI',141,14,1,2000,''),(110,'CHICLET',142,14,1,1000,''),(111,'HELADO',143,14,1,4000,''),(112,'DORITOS',144,14,1,3000,''),(113,'BOMBON',145,14,1,1000,''),(114,'KINDER BUENO',146,14,1,5000,''),(115,'Queso TOSTADA',1,15,1,0,''),(116,'queso',2,15,1,0,''),(117,'VERDURAS',3,15,1,0,''),(118,'POLLO',4,15,1,0,''),(119,'ENSALADA',5,15,1,3000,''),(120,'ascaite de oliva',6,15,1,0,''),(121,'NOCHE',7,15,1,35000,''),(122,'OREO',147,14,1,2000,'');
/*!40000 ALTER TABLE `menu_item` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES (1,'chocolate'),(2,'zucaritas'),(3,'banana y miel'),(4,'banana'),(5,'sandia'),(6,'melon'),(7,'pine'),(8,'fresa'),(9,'dos huevos'),(10,'queso filli'),(11,'ensalada'),(12,'pan tostado '),(13,'pan pita'),(14,'hummos'),(15,'tajini'),(17,'pan bagget'),(18,' huevos con Especias'),(19,'pan frances'),(20,'miel'),(21,'yogurth'),(22,'granola'),(23,'nutella'),(24,'frutas'),(25,'tomate'),(26,'cebolla'),(27,'pepino'),(28,'Zanahoria'),(29,'pimenton rojo'),(30,'huevos homelet'),(31,'huevos revuelto'),(32,'huevos ojos'),(33,' pan bimbo'),(34,'picnte'),(35,'mayonesa'),(36,'catsop'),(37,'berenjena frita'),(38,'bulgara'),(39,'champinones'),(40,'maiz'),(41,'queso'),(42,'EN EL HOMELET'),(43,'ex cebolla'),(44,'ex tomate'),(45,'EN LA ENSALADA'),(46,'homelet'),(47,'EN EL HUEVO'),(49,'salsa pizza'),(50,'huevo doro'),(51,'cilantro'),(52,'atun'),(53,'limon'),(54,'lechoga'),(55,'aguacate'),(57,'queso feta'),(58,'aceitunas negras'),(59,'aceitunas verde'),(60,'aceite de oliva'),(61,'garbanzo caliente'),(62,'nachos'),(63,'pechuga de pollo '),(64,'salsa teriaki'),(65,'EN LA TOSTADA'),(66,'arroz'),(67,'papas a la francesa'),(68,'pure de papa'),(69,'KOSHER'),(70,'cebolla cocida'),(71,'EXTRA'),(72,'kabab'),(73,'falfel'),(74,'hummos chico'),(75,'salsa de tomate'),(76,'rull'),(77,'kinder bueno'),(78,'papa'),(79,'EXTRA EN LA BUREKAS'),(80,'pepenios'),(81,'extra queso'),(82,'pimento picnte'),(83,'pasta'),(84,'repollo'),(85,'extra pollo'),(86,'extra carne'),(87,'extra pasta'),(88,'extra verdoras'),(89,'extra arroz'),(90,'extra papas fritas'),(91,'carne'),(92,'carne de hamburgesa'),(93,'EN LA HAMBURGESA'),(94,'EXTRA EN LA HAMBURGESA'),(95,'tortilla de carne'),(96,'milanesa'),(97,'kabab'),(98,'shawarma'),(99,'pollo'),(100,'verduras'),(101,'EN LA VERDURAS'),(102,'pasta napolitana'),(103,'pasta rose'),(104,'pasta pesto'),(105,'pasta alfredo'),(106,'pasta bolonesa'),(107,'albahaca'),(108,'crema de leche'),(109,'pesto'),(110,'carne molida'),(111,'sin'),(112,'malteada de oreo'),(113,'malteada de kinder bueno'),(114,'ensalada de frutas'),(115,'tostada de nutella'),(116,'banana split'),(117,'pancake'),(118,'banana'),(119,'fresa'),(120,'melon'),(121,'sandilla'),(122,'pine'),(123,'mango'),(124,'banana frita'),(125,'helado'),(126,'nutella'),(127,'oreo'),(128,'agua'),(129,'agua'),(130,'gaseosa'),(131,'juegos en botella'),(132,'te helado'),(133,'jugo natural'),(134,'jugo natural en agua'),(135,'jugos en botalla'),(136,'jogo natural en leche'),(137,'te'),(138,'nescafe'),(139,'chocolate'),(140,'cafe frio'),(141,'cafe caliente'),(142,'chocolate frio'),(143,'chocolate caliente'),(144,'con'),(145,'CON'),(146,'redbull'),(147,'couscous'),(148,'golash '),(149,'Fish & Chips'),(150,' bulas de carne '),(151,'corona'),(152,'grey goose'),(153,'smirnoff'),(154,'Heineken '),(155,'sigaros'),(156,'bisli'),(157,'bamba chica'),(158,'bamba grnde'),(159,'kamagra'),(160,'redbull'),(161,'absolut'),(162,'black level'),(163,'mango'),(164,'pargit'),(165,'pechoga de pollo con verdoras'),(166,'soda'),(168,'Queso Crema'),(169,'Mozarella'),(170,'thina'),(171,'SANDWICH'),(172,'ESPAGUETI'),(173,'ARROZ'),(174,'PAN FRANCES'),(175,'pina'),(176,'naranja'),(177,'maracoya'),(178,'frio'),(179,'caliente'),(180,'normal'),(181,'VODKA'),(182,'TEQUILA'),(183,'AGUARDIENTE'),(184,'WISKEY'),(185,'RON'),(186,'PITA');
/*!40000 ALTER TABLE `ingredient` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `menu_item_ingredient` WRITE;
/*!40000 ALTER TABLE `menu_item_ingredient` DISABLE KEYS */;
INSERT INTO `menu_item_ingredient` VALUES (1,1,12,0,'Pan',1,''),(2,1,19,0,'Pan',1,''),(3,1,168,0,'Queso',2,''),(4,1,169,0,'Queso',2,''),(5,1,38,0,'Queso',2,''),(6,1,46,0,'hoevos',3,'\0'),(7,1,32,0,'hoevos',3,'\0'),(8,1,31,0,'hoevos',3,'\0'),(9,3,12,0,'pan',1,'\0'),(10,3,19,0,'pan',1,'\0'),(11,3,168,0,'queso',2,'\0'),(12,3,169,0,'queso',2,'\0'),(13,3,38,0,'queso',2,'\0'),(14,3,46,0,'huevo',3,'\0'),(15,3,32,0,'huevo',3,'\0'),(16,3,31,0,'huevo',3,'\0'),(18,6,12,0,'pan',1,'\0'),(19,6,19,0,'pan',1,'\0'),(20,6,14,3000,'extra',2,'\0'),(21,6,168,3000,'extra',2,'\0'),(22,6,170,4000,'extra',2,'\0'),(23,8,20,0,'extra ',1,'\0'),(30,115,169,1000,'Queso',1,'\0'),(31,115,168,1000,'Queso',1,'\0'),(32,115,38,1000,'Queso',1,'\0'),(33,116,169,2000,'queso ',1,'\0'),(34,116,168,2000,'queso ',1,'\0'),(35,116,38,2000,'queso ',1,'\0'),(36,117,25,1000,'VERDURAS',1,'\0'),(37,117,26,1000,'VERDURAS',1,'\0'),(38,117,29,1000,'VERDURAS',1,'\0'),(39,117,39,1000,'VERDURAS',1,'\0'),(40,117,59,1000,'VERDURAS',1,'\0'),(41,117,52,2000,'VERDURAS',1,'\0'),(42,117,55,2000,'VERDURAS',1,'\0'),(43,117,50,2000,'VERDURAS',1,'\0'),(44,117,37,2000,'VERDURAS',1,'\0'),(45,118,63,8000,'POLLO',1,'\0'),(46,118,96,8000,'POLLO',1,'\0'),(47,60,67,0,'CON',1,'\0'),(48,60,66,0,'CON',1,'\0'),(49,60,68,0,'CON',1,'\0'),(50,60,171,0,'CON',1,'\0'),(52,57,67,0,'CON',1,'\0'),(53,57,66,0,'CON',1,'\0'),(54,57,68,0,'CON',1,'\0'),(55,57,171,0,'CON',1,'\0'),(56,59,67,0,'CON',1,'\0'),(57,59,66,0,'CON',1,'\0'),(58,59,68,0,'CON',1,'\0'),(59,59,171,0,'CON',1,'\0'),(60,56,67,0,'CON',1,'\0'),(61,56,66,0,'CON',1,'\0'),(62,56,68,0,'CON',1,'\0'),(63,56,171,0,'CON',1,'\0'),(64,61,67,0,'CON',1,'\0'),(65,61,66,0,'CON',1,'\0'),(66,61,68,0,'CON',1,'\0'),(67,61,171,0,'CON',1,'\0'),(68,58,67,0,'CON',1,'\0'),(69,58,66,0,'CON',1,'\0'),(70,58,68,0,'CON',1,'\0'),(71,58,171,0,'CON',1,'\0'),(74,65,172,0,'CON',1,'\0'),(75,65,66,0,'CON',1,'\0'),(76,65,19,0,'CON',1,'\0'),(77,70,59,1000,'EXTRA ',1,'\0'),(78,70,25,1000,'EXTRA ',1,'\0'),(79,70,26,1000,'EXTRA ',1,'\0'),(80,70,39,1000,'EXTRA ',1,'\0'),(81,70,50,2000,'EXTRA ',1,'\0'),(82,29,175,0,'frutas',1,'\0'),(83,29,123,0,'frutas',1,'\0'),(84,29,6,0,'frutas',1,'\0'),(85,29,4,0,'frutas',1,'\0'),(86,29,176,0,'frutas',1,'\0'),(87,29,177,0,'frutas',1,'\0'),(88,29,8,0,'frutas',1,'\0'),(89,30,175,0,'frutas',1,'\0'),(90,30,123,0,'frutas',1,'\0'),(91,30,6,0,'frutas',1,'\0'),(92,30,4,0,'frutas',1,'\0'),(93,30,176,0,'frutas',1,'\0'),(94,30,177,0,'frutas',1,'\0'),(95,30,8,0,'frutas',1,'\0'),(96,75,19,0,'pan',1,'\0'),(97,75,13,0,'pan',1,'\0'),(98,73,19,0,'pan',1,'\0'),(99,73,13,0,'pan',1,'\0'),(100,74,19,0,'pan',1,'\0'),(101,74,13,0,'pan',1,'\0'),(102,77,4,1000,'extra',1,'\0'),(103,77,77,5000,'extra',1,'\0'),(104,76,4,1000,'extra',1,'\0'),(105,76,77,5000,'extra',1,'\0'),(106,82,77,5000,'extra',1,'\0'),(107,82,8,1000,'extra',1,'\0'),(108,82,127,3000,'extra',1,'\0'),(110,34,178,0,'escoger',1,''),(111,34,179,0,'escoger',1,''),(112,35,178,0,'escoger',1,''),(113,35,179,0,'escoger',1,''),(114,7,1,0,'escoger',1,'\0'),(115,42,181,10000,'TIPO',1,'\0'),(116,42,182,10000,'TIPO',1,'\0'),(117,42,185,10000,'TIPO',1,'\0'),(118,42,183,10000,'TIPO',1,'\0'),(119,42,184,10000,'TIPO',1,'\0'),(120,1,186,0,'Pan',1,'');
/*!40000 ALTER TABLE `menu_item_ingredient` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `app_settings` WRITE;
/*!40000 ALTER TABLE `app_settings` DISABLE KEYS */;
INSERT INTO `app_settings` VALUES (1,'','','cp','383','HB','es','47001','19560','');
/*!40000 ALTER TABLE `app_settings` ENABLE KEYS */;
UNLOCK TABLES;
