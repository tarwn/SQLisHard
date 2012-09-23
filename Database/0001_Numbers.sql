USE SQLisHard_Sample;

-- http://sqlblog.com/blogs/adam_machanic/archive/2006/07/12/you-require-a-numbers-table.aspx
CREATE TABLE Numbers
(
	Number INT NOT NULL,
	CONSTRAINT PK_Numbers 
		PRIMARY KEY CLUSTERED (Number)
		WITH FILLFACTOR = 100
)

INSERT INTO Numbers
SELECT
	(a.Number * 256) + b.Number AS Number
FROM 
	(
		SELECT number
		FROM master..spt_values
		WHERE 
			type = 'P'
			AND number <= 255
	) a (Number),
	(
		SELECT number
		FROM master..spt_values
		WHERE 
			type = 'P'
			AND number <= 255
	) b (Number);