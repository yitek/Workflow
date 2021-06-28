DROP TABLE wf_activity;
CREATE TABLE wf_activity(
	id CHAR(40) PRIMARY KEY NOT NULL
	,flowId CHAR(40) NOT NULL
	,superId CHAR(40) NOT NULL
	,`name` VARCHAR(50) NOT NULL
	,`version` CHAR(9) NOT NULL
	,pathname VARCHAR(800) NOT NULL
	,fromId CHAR(40) NULL
	,transitionName VARCHAR(50) NULL
	,`status` INT NOT NULL DEFAULT 0
	,inputs TEXT NULL
	,params TEXT NULL
	,variables TEXT NULL
	,results TEXT NULL
	,`state` TEXT NULL
	,actionType VARCHAR(516) NULL
	,createTime DATETIME NOT NULL 
	,creatorId VARCHAR(64) NOT NULL
	,creatorName VARCHAR(64) NOT NULL
	,dealTime DATETIME NULL
	,dealerId VARCHAR(64)
	,dealerName VARCHAR(64)
	,doneTime DATETIME NULL
	,businessId VARCHAR(64) NULL
	,billId VARCHAR(64) NULL
	,taskId VARCHAR(64) NULL
	,suspended INT NOT NULL DEFAULT 0
	,subCount INT NULL
	,isStart INT NOT NULL DEFAULT 0

)