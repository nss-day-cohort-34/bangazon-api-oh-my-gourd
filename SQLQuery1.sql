Select Count(et.EmployeeId) as EmployeesEnrolled, tp.id, tp.Name from TrainingProgram tp 
left join EmployeeTraining et on tp.Id = et.TrainingProgramId 
Group By tp.Id, tp.StartDate, tp.Name
having tp.Id = 6 and tp.StartDate > '2019-11-7' and Count(et.EmployeeId) = 0


