
-- ONLY RUN IF NOT APPLIED BY OLD MIGRATION SYSTEM
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'UpdateTracking') 
BEGIN

	CREATE TABLE dbo.[User](
		Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
		Name varchar(80) NULL
	);

	CREATE TABLE dbo.[History](
		Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
		UserId int NOT NULL,
		SqlStatement varchar(MAX) NOT NULL,
		Result int NOT NULL,
		CompletesExercise bit,
		Created DateTime2(0) NOT NULL CONSTRAINT df_Created DEFAULT GETDATE()
	);

END
