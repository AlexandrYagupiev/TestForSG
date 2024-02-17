using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestForSG.Models;

namespace TestForSG
{
    public class BusinessLogic
    {
        private readonly string connectionString;
        private readonly bool debug;
        private object departmentUpdate;

        public BusinessLogic(string connectionString, bool debug)
        {
            this.connectionString = connectionString;
            this.debug = debug;
        }

        public void OutputCurrentDataStructure(int id)
        {
            using (UsersdbContext usersdbContext = new UsersdbContext(connectionString, debug))
            {
                if (id == 0)
                {
                    var department = usersdbContext.Departments;
                    var employee = usersdbContext.Employees;
                    var jobTitle = usersdbContext.JobTitles;
                    Console.WriteLine("Таблица Departments");
                    foreach (var item in department)
                    {
                        Console.WriteLine(item);
                    }
                    Console.WriteLine("Таблица Employees");
                    foreach (var item in employee)
                    {
                        Console.WriteLine(item);
                    }
                    Console.WriteLine("Таблица JobTitles");
                    foreach (var item in jobTitle)
                    {
                        Console.WriteLine(item);
                    }
                }
            }
        }




        public void ReadFile(string pathFile, string importType)
        {
            using (UsersdbContext usersdbContext = new UsersdbContext(connectionString, debug))
            {
                if (importType == "должность")
                {
                    using (var streamReaderJobTitle = new StreamReader(pathFile))
                    {
                        var idJobTitle = 1;
                        var tsvReader = new TsvReader(streamReaderJobTitle);
                        var jobTitleRecord = tsvReader.ReadNextRecord();
                        while (jobTitleRecord != null)
                        {
                            var nameJobTitle = usersdbContext.JobTitles.FirstOrDefault(t => t.Name == jobTitleRecord[0]);
                            if (nameJobTitle != null)
                            {
                                Console.WriteLine($"В базе уже существует должность:{nameJobTitle.Name}");
                            }
                            else
                            {
                                var jobTitle = new JobTitle();
                                jobTitle.Id = idJobTitle;
                                jobTitle.Name = jobTitleRecord[0];
                                usersdbContext.JobTitles.Add(jobTitle);
                                usersdbContext.SaveChanges();
                            }
                            idJobTitle++;
                            jobTitleRecord = tsvReader.ReadNextRecord();
                        }
                    }
                }
                else if (importType == "сотрудник")
                {
                    using (var streamReaderEmployee = new StreamReader(pathFile))
                    {
                       
                        var idEmployee = 1;
                        var tsvReader = new TsvReader(streamReaderEmployee);
                        var employeeRecord = tsvReader.ReadNextRecord();
                        while (employeeRecord != null)
                        {
                            var employee = new Employee();
                            if (!string.IsNullOrEmpty(employeeRecord[1]))
                            {                 
                                var employeeUpdate = usersdbContext.Employees.FirstOrDefault(t => t.FullName == employeeRecord[1]);
                                if (employeeUpdate != null)
                                {
                                    employeeUpdate.Login = employeeRecord[2];
                                    employeeUpdate.Password = employeeRecord[3];
                                    var jobTitle = usersdbContext.JobTitles.FirstOrDefault(t => t.Name == employeeRecord[4]);
                                    if (jobTitle != null)
                                    {
                                        employeeUpdate.JobTitle = usersdbContext.JobTitles.FirstOrDefault(t => t.Name == employeeRecord[4]).Id;
                                    }
                                    var department = usersdbContext.Departments.FirstOrDefault(t => t.Name == employeeRecord[0]);
                                    if (department != null)
                                    {
                                        employeeUpdate.Department = department.Id;
                                    }
                                    usersdbContext.Employees.Update(employeeUpdate);
                                    usersdbContext.SaveChanges();
                                }
                                else
                                {                                  
                                    employee.Id = idEmployee;
                                    var department = usersdbContext.Departments.FirstOrDefault(t => t.Name == employeeRecord[0]);
                                    if (department != null)
                                    {
                                        employee.Department = department.Id;
                                    }
                                    employee.FullName = employeeRecord[1];
                                    employee.Login = employeeRecord[2];
                                    employee.Password = employeeRecord[3];
                                    var jobTitle = usersdbContext.JobTitles.FirstOrDefault(t => t.Name == employeeRecord[4]);
                                    if (jobTitle != null)
                                    {
                                        employee.JobTitle = jobTitle.Id;
                                    }
                                    usersdbContext.Employees.Add(employee);
                                    usersdbContext.SaveChanges();
                                }
                                idEmployee++;
                            }
                            employeeRecord = tsvReader.ReadNextRecord();
                        }
                    }
                }
                else if (importType == "подразделение")
                {
                    using (var streamReaderDepartment = new StreamReader(pathFile))
                    {
                        var idDepartment = 1;
                        var roots = new List<Department>();
                        var notUsedNodes = new List<Department>();
                        var tsvReader = new TsvReader(streamReaderDepartment);
                        var departmentRecord = tsvReader.ReadNextRecord();
                        while (departmentRecord != null)
                        {
                            var parentDepartmentName = departmentRecord[1];
                            var name = departmentRecord[0];
                            var department = new Department();
                            var departmentUpdate = usersdbContext.Departments.FirstOrDefault(t => t.Name == name);
                            if (departmentUpdate!=null)
                            {
                                departmentUpdate.ManagerId = usersdbContext.Employees.FirstOrDefault(t => t.FullName == departmentRecord[2]).Id;
                                departmentUpdate.Phone = departmentRecord[3].Replace(" ", "");
                                usersdbContext.Departments.Update(departmentUpdate);
                                usersdbContext.SaveChanges();
                            }
                            else
                            {
                                department.Id = idDepartment;
                                department.Name = name;
                                var manager = usersdbContext.Employees.FirstOrDefault(t => t.FullName == departmentRecord[2]);
                                if (manager != null)
                                {
                                    department.Manager = manager;
                                }
                                department.Phone = departmentRecord[3].Replace(" ", "");
                                if (string.IsNullOrEmpty(parentDepartmentName))
                                {
                                    roots.Add(department);
                                    usersdbContext.Departments.Add(department);
                                    usersdbContext.SaveChanges();
                                }
                                else
                                {
                                    var rootNode = roots.FirstOrDefault(t => t.Name == parentDepartmentName);
                                    if (rootNode != null)
                                    {
                                        department.Parent = rootNode;
                                        roots.Add(department);
                                        usersdbContext.Departments.Add(department);
                                        usersdbContext.SaveChanges();
                                        var withParent = notUsedNodes.Where(t => t.Parent.Name == department.Name).ToList();
                                        foreach (var node in withParent)
                                        {
                                            notUsedNodes.Remove(node);
                                            node.Parent = department;
                                            usersdbContext.Departments.Add(node);
                                            usersdbContext.SaveChanges();
                                            roots.Add(node);
                                        }
                                    }
                                    else
                                    {
                                        department.Parent = new Department();
                                        department.Parent.Name = parentDepartmentName;
                                        notUsedNodes.Add(department);
                                    }
                                }
                            }
                            idDepartment++;
                            departmentRecord = tsvReader.ReadNextRecord();
                        }
                    }
                }
            }
        }
    }
}
