
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
CREATE TABLE #Names (Name VARCHAR(50));

-- Girls Names
DECLARE @GirlsNames XML;
SET @GirlsNames = '{{GIRLSNAMES}}';

INSERT INTO #ForeNames(FirstName, Gender)
SELECT N.Item.value('.', 'varchar(50)'), 1
FROM @GirlsNames.nodes('/ns/n') as N(item);


-- Boys Names
DECLARE @BoysNames XML;
SET @BoysNames = '{{BOYSNAMES}}';

INSERT INTO #ForeNames(FirstName, Gender)
SELECT N.Item.value('.', 'varchar(50)'), 0
FROM @BoysNames.nodes('/ns/n') as N(item);

-- Last Names
DECLARE @LastNames XML;
SET @LastNames = '{{LASTNAMES}}';

INSERT INTO #Names(Name)
SELECT N.Item.value('.', 'varchar(50)')
FROM @LastNames.nodes('/ns/n') as N(item);

WITH Names AS (
	SELECT  LastName = N.Name,
			FirstName = FN.FirstName,
			Gender = FN.Gender,
			Sort = ROW_NUMBER() OVER (PARTITION BY N.Name ORDER BY NewId()),
			Cap = ABS(CHECKSUM(NewId())) % 7 + 1
	FROM #Names N
		CROSS JOIN #ForeNames FN
)
INSERT INTO dbo.Customers(FirstName, LastName, Gender)
SELECT FirstName, LastName, Gender
FROM Names
WHERE Sort <= Cap;

DROP TABLE #Names;
DROP TABLE #ForeNames;

