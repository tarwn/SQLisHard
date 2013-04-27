

CREATE TABLE dbo.Orders(
	Id				int IDENTITY(1,1) PRIMARY KEY,
	CustomerId		int NOT NULL,
	OrderTotal		numeric(14,2) NOT NULL DEFAULT (0),
	OrderTime		DateTime2(0) NOT NULL,
	DeliveryTime	DateTime2(0) NOT NULL
);

DECLARE @OneYearAgoIsh DateTime2(0) = DateAdd(MONTH,-13,GETDATE());

CREATE TABLE #Randomness (
	CustomerId		int,
	NumberOfOrders	int
);

INSERT INTO #Randomness
SELECT IC.Id,
NumberOfOrders = 1 + CAST(ROUND((LOG(1 - (ABS(CHECKSUM(NEWID())) % 100000/100000.0)) / -.75),0) as int)
FROM Customers IC;

WITH CustomerOrderCount AS(
	SELECT C.CustomerId,
			N.Number,
			OrdersSeconds = CAST((ABS(CHECKSUM(NEWID())) % 100000/100000.0) * 31536000 as int),
			DeliveryOffset = CAST((ABS(CHECKSUM(NEWID())) % 100000/100000.0) * 259200 + 259200 as int)
	FROM #Randomness C
		INNER JOIN dbo.Numbers N ON N.Number <= C.NumberOfOrders
)
INSERT INTO dbo.Orders(CustomerId, OrderTotal, OrderTime, DeliveryTime)
SELECT COC.CustomerId, 
		(ABS(CHECKSUM(NEWID())) % 100000/100000.0) * 400,
		DATEADD(SECOND, COC.OrdersSeconds, @OneYearAgoIsh),
		DATEADD(SECOND, COC.OrdersSeconds + COC.DeliveryOffset, @OneYearAgoIsh)
FROM CustomerOrderCount COC		

DROP TABLE #Randomness;

/*

SELECT NumOrders, COUNT(*)
FROM
(
SELECT CustomerId, NumOrders = Count(*)
FROM Orders
GROUP BY CustomerId

) N
GROUP BY NumOrders
ORDER BY NumOrders

--TRUNCATE TABLE ORDERS
*/
