
-- This is really designed as a one-time script to help me reset the environments back to zero and statr using the 
-- same mechanism for updating the Sample database that the Core uses

-- Remove the Numbers table
IF EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Numbers') DROP TABLE dbo.Numbers;

-- Remove the Customers table
IF EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Clients') DROP TABLE dbo.Clients;
IF EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Customers') DROP TABLE dbo.Customers;

-- Clear the update tracking table
TRUNCATE TABLE dbo.UpdateTracking;