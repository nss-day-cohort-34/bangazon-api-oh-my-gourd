SELECT tp.id as TPID, tp.name as TPName, tp.startDate, tp.endDate, tp.maxAttendees, e.id as EID, e.firstName, e.lastName, e.isSupervisor, e.departmentId, d.Name, c.Make, c.Manufacturer, c.IsWorking, c.PurchaseDate, c.Id as CID
                                        FROM TrainingProgram tp LEFT JOIN EmployeeTraining et
                                        ON et.TrainingProgramId = tp.id
                                        Left join Employee e 
                                        on et.EmployeeId = e.id 
										left join Department d
										on e.DepartmentId = d.Id
										left join Computer c
										on c.EmployeeId = e.id

