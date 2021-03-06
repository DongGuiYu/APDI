-- --------------------------------------------------------
-- 主機:                           127.0.0.1
-- 伺服器版本:                        10.4.8-MariaDB - mariadb.org binary distribution
-- 伺服器操作系統:                      Win64
-- HeidiSQL 版本:                  10.2.0.5599
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- 傾印 faceid 的資料庫結構
CREATE DATABASE IF NOT EXISTS `faceid` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin */;
USE `faceid`;

-- 傾印  程序 faceid.EventAddOrEdit 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `EventAddOrEdit`(
	IN `_eve_num` INT,
	IN `_id` VARCHAR(50),
	IN `_byid` VARCHAR(50),
	IN `_eve_time` DATE,
	IN `_eve_locl` VARCHAR(50),
	IN `_eve_desc` VARCHAR(50)


)
BEGIN
      IF _eve_num=0 then
      INSERT INTO event_rec (id,byid,eve_time,eve_locl,eve_desc)
      VALUES (_id,_byid,_eve_time,_eve_locl,_eve_desc);
   ELSE
      UPDATE event_rec
      SET
       id=_id,
       byid=_byid,
		 eve_time=_eve_time,
		 eve_locl=_eve_locl,
		 eve_desc=_eve_desc
		WHERE _eve_num=eve_num; 
	END IF;
END//
DELIMITER ;

-- 傾印  程序 faceid.EventDeleteByID 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `EventDeleteByID`(
	IN `_eve_num` INT

)
BEGIN
   DELETE FROM event_rec
   WHERE eve_num=_eve_num;
END//
DELIMITER ;

-- 傾印  程序 faceid.EventViewAll 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `EventViewAll`()
BEGIN
   SELECT *
   FROM event_rec;
END//
DELIMITER ;

-- 傾印  程序 faceid.EventViewByID 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `EventViewByID`(
	IN `_eve_num` INT
)
BEGIN
   SELECT *
   FROM event_rec
   WHERE eve_num=_eve_num;
END//
DELIMITER ;

-- 傾印  表格 faceid.event_rec 結構
CREATE TABLE IF NOT EXISTS `event_rec` (
  `eve_num` int(5) NOT NULL AUTO_INCREMENT,
  `id` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  `byid` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  `eve_time` date NOT NULL,
  `eve_locl` varchar(20) COLLATE utf8mb4_bin NOT NULL,
  `eve_desc` varchar(50) COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`eve_num`),
  KEY `id` (`id`),
  KEY `byid` (`byid`),
  CONSTRAINT `byid` FOREIGN KEY (`byid`) REFERENCES `teach_id` (`id`),
  CONSTRAINT `id` FOREIGN KEY (`id`) REFERENCES `std_id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- 取消選取資料匯出。

-- 傾印  程序 faceid.n 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `n`(
	IN `_id` VARCHAR(50),
	IN `_eve_num` INT,
	IN `_date` DATE
)
BEGIN
   SELECT event_rec.eve_num, std_id.c_sname, teach_id.c_tname, event_rec.eve_time, event_rec.eve_locl, event_rec.eve_desc
   FROM event_rec
   INNER JOIN std_id
   ON event_rec.id = std_id.id
   INNER JOIN teach_id
   ON event_rec.byid = teach_id.id
   WHERE _id = event_rec.id AND event_rec.eve_time >= _date AND event_rec.eve_num > _eve_num
	ORDER BY event_rec.eve_num ASC
	LIMIT 1;
END//
DELIMITER ;

-- 傾印  程序 faceid.Next 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `Next`(
	IN `_id` VARCHAR(50),
	IN `_eve_num` INT
,
	IN `_date` DATE
)
BEGIN
   SELECT *
   FROM event_rec
	-- (
   -- SELECT *
   -- FROM event_rec
   -- WHERE id = _id
   -- 	)
   WHERE eve_num > _eve_num AND id = _id AND eve_time >= _date
   ORDER BY eve_num ASC 
	LIMIT 1;
END//
DELIMITER ;

-- 傾印  程序 faceid.p 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `p`(
	IN `_id` VARCHAR(50),
	IN `_eve_num` INT,
	IN `_date` DATE
)
BEGIN
   SELECT event_rec.eve_num, std_id.c_sname, teach_id.c_tname, event_rec.eve_time, event_rec.eve_locl, event_rec.eve_desc
   FROM event_rec
   INNER JOIN std_id
   ON event_rec.id = std_id.id
   INNER JOIN teach_id
   ON event_rec.byid = teach_id.id
   WHERE _id = event_rec.id AND event_rec.eve_time >= _date AND event_rec.eve_num < _eve_num
	ORDER BY event_rec.eve_num DESC
	LIMIT 1;
END//
DELIMITER ;

-- 傾印  程序 faceid.Previous 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `Previous`(
	IN `_id` VARCHAR(50),
	IN `_eve_num` INT
,
	IN `_date` DATE
)
BEGIN
   SELECT *
   FROM event_rec
	-- (
-- SELECT *
-- FROM event_rec
-- WHERE id = _id
-- 	)
   WHERE eve_num < _eve_num AND id = _id AND eve_time >= _date
   ORDER BY eve_num DESC 
	LIMIT 1;

END//
DELIMITER ;

-- 傾印  表格 faceid.std_id 結構
CREATE TABLE IF NOT EXISTS `std_id` (
  `id` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  `c_sname` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  `passwd` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- 取消選取資料匯出。

-- 傾印  程序 faceid.StudentMessageView 結構
DELIMITER //
CREATE DEFINER=`root`@`localhost` PROCEDURE `StudentMessageView`(
	IN `_id` VARCHAR(50),
	IN `_date` DATE


)
BEGIN
   SELECT event_rec.eve_num, std_id.c_sname, teach_id.c_tname, event_rec.eve_time, event_rec.eve_locl, event_rec.eve_desc
   FROM event_rec
   INNER JOIN std_id
   ON event_rec.id = std_id.id
   INNER JOIN teach_id
   ON event_rec.byid = teach_id.id
   WHERE _id = event_rec.id AND event_rec.eve_time >= _date;
END//
DELIMITER ;

-- 傾印  表格 faceid.teach_id 結構
CREATE TABLE IF NOT EXISTS `teach_id` (
  `id` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  `c_tname` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  `passwd` varchar(10) COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

-- 取消選取資料匯出。

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
