-- SQL script that creates a trigger file on the Computer table -> whenever the Computer table is updated,
-- An entry is made in ComputerEmployees with the ComputerId, EmployeeId from the Computer table entry and
-- uses the current date as the assign date.

--CREATE TRIGGER ComputerEmployeeTrigger ON Computer
--AFTER UPDATE
--AS
--BEGIN
--	SET NOCOUNT ON;
--	INSERT INTO ComputerEmployee(ComputerId, EmployeeId, AssignDate)
--	SELECT c.Id, c.EmployeeId, GETDATE() FROM Computer c
--END


-- If you want to test:
-- 1. Create an employee that doesn't have a computer
-- 2. Create a computer with EmployeeId of null
-- 3. Update the computer to 
-- Create an employee that doesn't have a computer, then create a computer and assign it to the new employee, then check to see if a record was created in ComputerEmployee
-- Template Insert Into Employee, Computer, and Update computer queries below

--INSERT INTO EMPLOYEE (FirstName, LastName, DepartmentId, IsSupervisor) 
--VALUES ('Bryan', 'Nilsen', 2, 0);

--INSERT INTO COMPUTER (PurchaseDate, DecomissionDate, Make, Manufacturer, EmployeeId, IsWorking) 
--Values('2019-11-7',null,'MacBook Amateur','Macintosh', null, 1);

--UPDATE Computer SET EmployeeId = 4
--WHERE Id = 4;



