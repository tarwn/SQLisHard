
-- ONLY RUN IF NOT APPLIED BY OLD MIGRATION SYSTEM
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'UpdateTracking') 
BEGIN

ALTER TABLE dbo.[Session] DROP COLUMN HostAddress;
ALTER TABLE dbo.[Session] ADD RemoteAddress varchar(30) NOT NULL CONSTRAINT df_Session_RemoteAddress DEFAULT '';

END
