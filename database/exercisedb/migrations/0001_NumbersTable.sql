
-- ONLY RUN IF NOT APPLIED BY OLD MIGRATION SYSTEM
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'UpdateTracking') 
BEGIN

	-- http://blogs.msdn.com/b/sqlazure/archive/2010/09/16/10063301.aspx
	CREATE TABLE Numbers
	(
		Number INT NOT NULL,
		CONSTRAINT PK_Numbers 
			PRIMARY KEY CLUSTERED (Number)
	)

	DECLARE @numbers table(number int);
	WITH numbers(number) as
	(
	SELECT 1 AS number
	UNION all
	SELECT number+1 FROM numbers WHERE number<10000
	)
	INSERT INTO @numbers(number)
	SELECT number FROM numbers OPTION(maxrecursion 10000)

	INSERT INTO Numbers(Number)
	SELECT number FROM @numbers

END