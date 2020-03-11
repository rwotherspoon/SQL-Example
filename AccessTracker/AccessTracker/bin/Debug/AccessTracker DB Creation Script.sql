--Check if DB exists, drop and create
USE master
IF EXISTS (select * from master.sys.databases where name ='AccessTracker')
	BEGIN
		ALTER DATABASE AccessTracker SET SINGLE_USER WITH ROLLBACK IMMEDIATE
		DROP DATABASE IF EXISTS AccessTracker
	END
		CREATE DATABASE AccessTracker

USE AccessTracker
--Create table for current active workers-parsed from WFHUB
CREATE TABLE dbo.tblActiveWorkers
	(
		WorkerID int NOT NULL,
		WorkerFullName nvarchar(50),
		WorkerFirstName nvarchar(50),
		WorkerLastName nvarchar(50),
		ManagerFullName nvarchar(50),
		WorkerCity nvarchar(50),
		WorkerState nvarchar(50),
		VendorName nvarchar(max)
		CONSTRAINT PK_tblActiveWorkers PRIMARY KEY (WorkerID)
	)

--Create table for current access records-parsed from NCIDM
CREATE TABLE dbo.tblAccess
	(
		WorkerID int NOT NULL,
		WorkerLastName nvarchar(50),
		WorkerFirstName nvarchar(50),
		WorkerFullName nvarchar(50),
		BusinessRole nvarchar(100),
		CompanyName nvarchar(50),
		ManagerFullName nvarchar(50),
		WorkerCompanyType nvarchar(50),
		WorkerAccessType nvarchar(50),
		WorkerRegion nvarchar(8),
		WorkerCity nvarchar (25),
		WorkerState nvarchar (8)
	)

ALTER TABLE dbo.tblAccess  WITH CHECK ADD CONSTRAINT FK_TO_tblActiveWorkers_PK FOREIGN KEY(WorkerID)
	REFERENCES dbo.tblActiveWorkers(WorkerID)
ALTER TABLE dbo.tblAccess CHECK CONSTRAINT FK_TO_tblActiveWorkers_PK

--Create table for current training records-parsed from NCIDM
CREATE TABLE dbo.tblTraining
	(
		WorkerID int NOT NULL,
		TrainingExpirationDate datetime,
		WorkerFullName nvarchar(50),
		ManagerFullName nvarchar(50),
		WorkerDepartment nvarchar(50),
		TrainingClass nvarchar(50),
		BusinessRole nvarchar(max),
		ComplianceMonitor nvarchar(max)
	)

ALTER TABLE dbo.tblTraining  WITH NOCHECK ADD  CONSTRAINT FK_tblTraining_tblActiveWorkers FOREIGN KEY(WorkerID)
	REFERENCES dbo.tblActiveWorkers (WorkerID)
	NOT FOR REPLICATION 

ALTER TABLE dbo.tblTraining NOCHECK CONSTRAINT FK_tblTraining_tblActiveWorkers

--Create table for current worker records based on worker city (Duke only)
CREATE TABLE dbo.tblCityRegion
	(
		City nvarchar (max),
		Region nvarchar (max)
	)
	INSERT INTO dbo.tblCityRegion (City, Region)
		VALUES
			('Plainfield', 'MW'),
			('Cincinnati', 'MW'),
			('Milford', 'MW'),
			('Charlotte', 'CW'),
			('Vincennes', 'MW'),
			('MONROE', 'CW'),
			('RALEIGH', 'CE'),
			('KERNERSVILLE', 'CW'),
			('ST. PETERSBURG', 'FL'),
			('SHELBYVILLE', 'MW'),
			('TERRE HAUTE', 'MW'),
			('KOKOMO', 'MW'),
			('COLUMBUS', 'MW')

--Create table for current worker region based on worker city (non-Duke)
CREATE TABLE dbo.tblMgrRegion
	(
		MgrName nvarchar (max),
		Region nvarchar (max)
	)
	INSERT INTO dbo.tblMgrRegion
		(MgrName, Region)
		VALUES
			('Bunnell, Kimberly Jean', 'CE'),
			('Cecil, Jane S', 'CW'),
			('Gaddy, Rodney E', 'CW'),
			('Gilb, Christopher L', 'MW'),
			('Hilburn, Robin	CW', 'CW'),
			('Ingle, Darrel R', 'MW'),
			('Lanham, Misty Varner', 'CW'),
			('Ledford, Burt', 'MW'),
			('Pratt, Shannon Rhoda', 'CW'),
			('Rathburn, Eric Daniel', 'FL'),
			('Rodgers, Andy', 'CW'),
			('Saboorian, K Calvin', 'CW'),
			('Stenzler, Keith Alan', 'CW'),
			('Thigpen, David J', 'CW'),
			('Veit, Robert S', 'CE'),
			('Waldrop, Dawn M', 'CW')

--Change owner/access
ALTER DATABASE AccessTracker set TRUSTWORTHY ON; 
EXEC dbo.sp_changedbowner @loginame = N'sa', @map = false 

--Run SPs to show advanced options, enable clr and reconfigure
EXEC sp_configure 'show advanced options', 1;
	RECONFIGURE

EXEC sp_configure 'clr enabled', 1;
	RECONFIGURE

--Set DB as read/write
USE master
	ALTER DATABASE AccessTracker SET READ_WRITE