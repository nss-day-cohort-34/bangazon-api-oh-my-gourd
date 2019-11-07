--INSERT INTO Department ([Name], Budget, SupervisorId)
--VALUES ('C#', 1000000, null);
--INSERT INTO Department ([Name], Budget, SupervisorId)
--VALUES ('Human Resources', 500000, null);

--INSERT INTO EMPLOYEE (FirstName, LastName, DepartmentId) 
--VALUES ('Andy', 'Collins', 2);
--INSERT INTO EMPLOYEE (FirstName, LastName, DepartmentId) 
--VALUES ('Steve', 'Brownlee', 3);

--INSERT INTO COMPUTER (PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId, IsWorking) 
--Values('2019-11-5',null,'MacBook Pro','Macintosh', 1, 1);
--INSERT INTO COMPUTER (PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId, IsWorking) 
--Values('2019-10-5',null,'E500','Gateway', 2, 1);

--INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate)
--VALUES (1,1,'2019-11-5',null);
--INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate)
--VALUES (2,2,'2019-10-5',null);

--INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [CreationDate], [LastActiveDate]) VALUES ('Bobby', 'Brady', '2019-11-05', '2019-11-05')
--INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [CreationDate], [LastActiveDate]) VALUES ('Joe', 'Snyder', '2019-11-04', '2019-11-05')
--INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [CreationDate], [LastActiveDate]) VALUES ('Jacquelyn', 'McCray', '2019-11-03', '2019-11-05')

--INSERT INTO [dbo].[ProductType] ([Name]) VALUES ('Cleaning Supplies')
--INSERT INTO [dbo].[ProductType] ([Name]) VALUES ('Electronics')

--INSERT INTO [dbo].[PaymentType] ([AcctNumber], [Name], [CustomerId]) VALUES ('12345-67890', 'Visa', 2)
--INSERT INTO [dbo].[PaymentType] ([AcctNumber], [Name], [CustomerId]) VALUES ('09876-54432', 'Amex', 3)

--INSERT INTO [dbo].[Product] ([ProductTypeId], [SellerId], [Price], [Title], [Description], [Quantity]) VALUES (1, 2, CAST(200.5500 AS Money), 'Trash Bags', 'Bags for your trash...', 50)
--INSERT INTO [dbo].[Product] ([ProductTypeId], [SellerId], [Price], [Title], [Description], [Quantity]) VALUES (2, 3, CAST(1000.0000 AS Money), 'Samsung TV', 'It''s 4k!', 4)

--INSERT INTO [dbo].[Order] ([CustomerId], [PaymentTypeId], [Total], [IsCompleted]) VALUES (4, 1, CAST(200.0000 AS Money), 0)
--INSERT INTO [dbo].[Order] ([CustomerId], [PaymentTypeId], [Total], [IsCompleted]) VALUES (5, 2, CAST(1000.0000 AS Money), 1)

--INSERT INTO [dbo].[OrderProduct] ([OrderId], [ProductId]) VALUES (3, 1)
--INSERT INTO [dbo].[OrderProduct] ([OrderId], [ProductId]) VALUES (4, 2)

--INSERT INTO [dbo].[TrainingProgram] ([Name], [StartDate], [EndDate], [MaxAttendees]) VALUES ('Phishing Attacks', '2019-12-10', '2019-12-23', 20)
--INSERT INTO [dbo].[TrainingProgram] ([Name], [StartDate], [EndDate], [MaxAttendees]) VALUES ('SQL Queries', '2019-11-11', '2019-01-06', 40)

--INSERT INTO [dbo].[EmployeeTraining] ([EmployeeId], [TrainingProgramId]) VALUES (2, 1)
--INSERT INTO [dbo].[EmployeeTraining] ([EmployeeId], [TrainingProgramId]) VALUES (1, 2)