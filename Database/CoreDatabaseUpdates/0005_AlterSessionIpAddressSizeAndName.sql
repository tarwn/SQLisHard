ALTER TABLE dbo.[Session] DROP COLUMN HostAddress;
ALTER TABLE dbo.[Session] ADD RemoteAddress varchar(30) NOT NULL CONSTRAINT df_Session_RemoteAddress DEFAULT '';