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
	Budget 	INTEGER NOT NULL
);

CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	DepartmentId INTEGER NOT NULL,
	IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);

CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME,
	Make VARCHAR(55) NOT NULL,
	Manufacturer VARCHAR(55) NOT NULL
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
	LastName VARCHAR(55) NOT NULL
);

CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	ProductTypeId INTEGER NOT NULL,
	CustomerId INTEGER NOT NULL,
	Price MONEY NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);


CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber INTEGER NOT NULL,
	[Name] VARCHAR(55) NOT NULL,
	CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER,
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

-- Department Data
INSERT INTO Department ([Name], Budget) VALUES ('Marketing', 7613.63);
INSERT INTO Department ([Name], Budget) VALUES ('Sales', 31221.54);

-- Employee Data
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Maximilianus', 'Lindl', 2, 1);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Garry', 'Levington', 2, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Roxane', 'Stirgess', 2, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Gardener', 'Mournian', 1, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Kassandra', 'Reid', 2, 1);

-- Computer Data
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('1/11/2017', 'GFE-744', 'Zoombeat');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('8/11/2015', 'YRV-483', 'Oyondu');
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('10/30/2013', '6/22/2015', 'LBP-375', 'Eazzy');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('12/26/2015', 'LSU-940', 'Aibox');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('1/14/2019', 'MMA-189', 'Avamba');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('3/19/2018', 'BSC-907', 'Realbuzz');

-- ComputerEmployee Data
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (3, 3, '4/20/2017', '2/2/2016');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (1, 3, '12/26/2016');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (5, 6, '3/6/2015', '12/24/2015');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (1, 1, '1/10/2018');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (5, 6, '1/10/2016');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (3, 2, '12/8/2013');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (3, 3, '2/4/2016');

-- TrainingProgram Data
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES ('York University', '9/12/2018', '4/29/2020', 82);
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES ('Eastern Mediterranean University', '10/27/2018', '2/20/2019', 38);

-- EmployeeTraining Data
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (5, 1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (5, 2);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (3, 1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (5, 1);

-- ProductType Data
INSERT INTO ProductType ([Name]) VALUES ('Groceries');
INSERT INTO ProductType ([Name]) VALUES ('Electronics');
INSERT INTO ProductType ([Name]) VALUES ('Apparel');

-- Customer Data
INSERT INTO Customer (FirstName, LastName) VALUES ('Inès', 'Northeast');
INSERT INTO Customer (FirstName, LastName) VALUES ('Liè', 'Dible');
INSERT INTO Customer (FirstName, LastName) VALUES ('Laurène', 'Hollyman');
INSERT INTO Customer (FirstName, LastName) VALUES ('Maëlle', 'Overill');

-- Product Data
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 2, '$0.61', 'Honey - Liquid', 'ligula suspendisse ornare consequat lectus in est', 25);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 2, '$1.76', 'Truffle Paste', 'tempus sit amet sem fusce consequat nulla', 32);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 3, '$19.28', 'Sauce Bbq Smokey', 'eu mi nulla ac enim in tempor turpis nec', 17);

-- Payment Data
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (62708, 'jcb', 2);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (28845, 'jcb', 3);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (16797, 'diners-club-enroute', 4);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (622854, 'mastercard', 3);

-- Order Data
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (2, 1);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (3, 2);
INSERT INTO [Order] (CustomerId) VALUES (4);

-- OrderProduct Data
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 2);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (1, 1);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (2, 1);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 2);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 1);