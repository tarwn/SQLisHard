
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES T WHERE T.TABLE_NAME = 'Customers' AND T.TABLE_SCHEMA = 'dbo')
DROP TABLE dbo.Customers;

-- 'Customers' sample table
CREATE TABLE dbo.Customers (
	Id		int IDENTITY(1,1) PRIMARY KEY,
	FirstName	varchar(50),
	LastName	varchar(50),
	Gender		bit
);


CREATE TABLE #ForeNames (FirstName VARCHAR(50), Gender BIT);
CREATE TABLE #Name (Name VARCHAR(50));

BULK INSERT #Name FROM "%PATH%girlsforenames.txt" WITH (ROWTERMINATOR='\n');
INSERT INTO #ForeNames SELECT [Name], 1 FROM #Name;
TRUNCATE TABLE #Name;

BULK INSERT #Name FROM "%PATH%boysforenames.txt" WITH (ROWTERMINATOR='\n');
INSERT INTO #ForeNames SELECT [Name], 0 FROM #Name;
TRUNCATE TABLE #Name;

BULK INSERT #Name FROM "%PATH%surnames.txt" WITH (ROWTERMINATOR='\n');
 

WITH Names AS (
	SELECT  LastName = N.Name,
			FirstName = FN.FirstName,
			Gender = FN.Gender,
			Sort = ROW_NUMBER() OVER (PARTITION BY N.Name ORDER BY NewId()),
			Cap = ABS(CHECKSUM(NewId())) % 7 + 1
	FROM #Name N
		CROSS JOIN #ForeNames FN
)
INSERT INTO dbo.Customers(FirstName, LastName, Gender)
SELECT FirstName, LastName, Gender
FROM Names
WHERE Sort <= Cap;

DROP TABLE #Name;
DROP TABLE #ForeNames;

