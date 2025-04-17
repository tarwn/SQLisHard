
-- ONLY RUN IF NOT APPLIED BY OLD MIGRATION SYSTEM
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'UpdateTracking') 
BEGIN

CREATE TABLE [Session](
	Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	UserId int NOT NULL,
	HostAddress varchar(15) NOT NULL,
	UserAgent varchar(200) NOT NULL,
	Created DateTime2(0) CONSTRAINT df_Session_Created DEFAULT GETDATE()
);

END
