USE MASTER
GO

IF NOT EXISTS (
    SELECT [name]
    FROM sys.databases
    WHERE [name] = N'BangazonAPITest'
)
CREATE DATABASE BangazonAPITest
GO

USE BangazonAPITest
GO

DROP TABLE IF EXISTS OrderProduct;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS PaymentType;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS Customer;
DROP TABLE IF EXISTS ProductType;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS ComputerEmployee;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS Department;

CREATE TABLE Department (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Budget 	MONEY NOT NULL,
);

CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	IsSupervisor BIT NOT NULL,
	DepartmentId INTEGER NOT NULL,
	CONSTRAINT FK_DepartmentEmployee FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);


CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME,
	Make VARCHAR(55) NOT NULL,
	Manufacturer VARCHAR(55) NOT NULL,
	EmployeeId INTEGER,
	IsWorking BIT NOT NULL,
	CONSTRAINT FK_Computer_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id)
);

CREATE TABLE ComputerEmployee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	ComputerId INTEGER NOT NULL,
	AssignDate DATETIME NOT NULL,
	UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);


CREATE TABLE TrainingProgram (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(255) NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MaxAttendees INTEGER NOT NULL
);

CREATE TABLE EmployeeTraining (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

CREATE TABLE ProductType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL
);

CREATE TABLE Customer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	CreationDate DATETIME NOT NULL,
	LastActiveDate DATETIME NOT NULL
);

CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	ProductTypeId INTEGER NOT NULL,
	SellerId INTEGER NOT NULL,
	Price MONEY NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(SellerId) REFERENCES Customer(Id)
);


CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber VARCHAR(55) NOT NULL,
	[Name] VARCHAR(55) NOT NULL,
	CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER,
	Total MONEY,
	IsCompleted BIT NOT NULL,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);

CREATE TABLE OrderProduct (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);

INSERT INTO Department ([Name], Budget)
VALUES ('C#', 1000000);
INSERT INTO Department ([Name], Budget)
VALUES ('Human Resources', 500000);

INSERT INTO EMPLOYEE (FirstName, LastName, DepartmentId, IsSupervisor) 
VALUES ('Andy', 'Collins', 1, 1);
INSERT INTO EMPLOYEE (FirstName, LastName, DepartmentId, IsSupervisor) 
VALUES ('Steve', 'Brownlee', 2,1);

INSERT INTO COMPUTER (PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId, IsWorking) 
Values('2019-11-5',null,'MacBook Pro','Macintosh', 1, 1);
INSERT INTO COMPUTER (PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId, IsWorking) 
Values('2019-10-5',null,'E500','Gateway', 2, 1);

INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate)
VALUES (1,1,'2019-11-5',null);
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate)
VALUES (2,2,'2019-10-5',null);

INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [CreationDate], [LastActiveDate]) VALUES ('Bobby', 'Brady', '2019-11-05', '2019-11-05')
INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [CreationDate], [LastActiveDate]) VALUES ('Joe', 'Snyder', '2019-11-04', '2019-11-05')
INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [CreationDate], [LastActiveDate]) VALUES ('Jacquelyn', 'McCray', '2019-11-03', '2019-11-05')

INSERT INTO [dbo].[ProductType] ([Name]) VALUES ('Cleaning Supplies')
INSERT INTO [dbo].[ProductType] ([Name]) VALUES ('Electronics')

INSERT INTO [dbo].[PaymentType] ([AcctNumber], [Name], [CustomerId]) VALUES ('12345-67890', 'Visa', 1)
INSERT INTO [dbo].[PaymentType] ([AcctNumber], [Name], [CustomerId]) VALUES ('09876-54432', 'Amex', 2)
INSERT INTO [dbo].[PaymentType] ([AcctNumber], [Name], [CustomerId]) VALUES ('09876-54432', 'Mastercard', 3)

INSERT INTO [dbo].[Product] ([ProductTypeId], [SellerId], [Price], [Title], [Description], [Quantity]) VALUES (1, 1, CAST(200.5500 AS Money), 'Trash Bags', 'Bags for your trash...', 50)
INSERT INTO [dbo].[Product] ([ProductTypeId], [SellerId], [Price], [Title], [Description], [Quantity]) VALUES (2, 2, CAST(1000.0000 AS Money), 'Samsung TV', 'It''s 4k!', 4)

INSERT INTO [dbo].[Order] ([CustomerId], [PaymentTypeId], [Total], [IsCompleted]) VALUES (3, 3, CAST(200.0000 AS Money), 0)
INSERT INTO [dbo].[Order] ([CustomerId], [PaymentTypeId], [Total], [IsCompleted]) VALUES (3, 3, CAST(1000.0000 AS Money), 1)

INSERT INTO [dbo].[OrderProduct] ([OrderId], [ProductId]) VALUES (1, 1)
INSERT INTO [dbo].[OrderProduct] ([OrderId], [ProductId]) VALUES (2, 2)

INSERT INTO [dbo].[TrainingProgram] ([Name], [StartDate], [EndDate], [MaxAttendees]) VALUES ('Phishing Attacks', '2019-12-10', '2019-12-23', 20)
INSERT INTO [dbo].[TrainingProgram] ([Name], [StartDate], [EndDate], [MaxAttendees]) VALUES ('SQL Queries', '2019-11-11', '2019-01-06', 40)

INSERT INTO [dbo].[EmployeeTraining] ([EmployeeId], [TrainingProgramId]) VALUES (2, 1)
INSERT INTO [dbo].[EmployeeTraining] ([EmployeeId], [TrainingProgramId]) VALUES (1, 2)