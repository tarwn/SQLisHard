
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES T WHERE T.TABLE_NAME = 'Customers' AND T.TABLE_SCHEMA = 'dbo')
DROP TABLE dbo.Customers;

-- 'Customers' sample table
CREATE TABLE dbo.Customers (
	Id		int IDENTITY(1,1) PRIMARY KEY,
	FirstName	varchar(50),
	LastName	varchar(50),
	PhoneNumber	varchar(14)		-- using x-xxx-xxx-xxxx format for readability
);


CREATE TABLE #ForeNames (FirstName VARCHAR(50));
CREATE TABLE #Names (Name VARCHAR(50));

-- Girls Names
DECLARE @GirlsNames XML;
SET @GirlsNames = '{{GIRLSNAMES}}';

INSERT INTO #ForeNames(FirstName)
SELECT N.Item.value('.', 'varchar(50)')
FROM @GirlsNames.nodes('/ns/n') as N(item);


-- Boys Names
DECLARE @BoysNames XML;
SET @BoysNames = '{{BOYSNAMES}}';

INSERT INTO #ForeNames(FirstName)
SELECT N.Item.value('.', 'varchar(50)')
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
			Sort = ROW_NUMBER() OVER (PARTITION BY N.Name ORDER BY NewId()),
			Cap = ABS(CHECKSUM(NewId())) % 7 + 1
	FROM #Names N
		CROSS JOIN #ForeNames FN
)
INSERT INTO dbo.Customers(FirstName, LastName, PhoneNumber)
SELECT FirstName, LastName, ''
FROM Names
WHERE Sort <= Cap;

DROP TABLE #Names;
DROP TABLE #ForeNames;

-- Apply some phone numbers w/ hopefully very little collision
WITH PhoneNumbers AS(
	SELECT C.Id,
			AreaCode = RIGHT('000' + CAST(ROW_NUMBER() OVER (ORDER BY NewId()) as VARCHAR), 3),
			Suffix = RIGHT('000' + CAST(ROW_NUMBER() OVER (ORDER BY NewId()) as VARCHAR), 4)
	FROM dbo.Customers C 
)
UPDATE dbo.Customers
SET PhoneNumber = '1-' + AreaCode + '-555-' + Suffix
FROM PhoneNumbers PH 
	INNER JOIN dbo.Customers C ON PH.Id = C.Id;

