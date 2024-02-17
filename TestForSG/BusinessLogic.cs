using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestForSG.Models;

namespace TestForSG
{
    public class BusinessLogic
    {
        private readonly string connectionString;
        private readonly bool debug;

        public BusinessLogic(string connectionString, bool debug)
        {
            this.connectionString = connectionString;
            this.debug = debug;
        }

        public void ReadFile(string pathFile, string importType)
        {
            using (UsersdbContext usersdbContext = new UsersdbContext(connectionString, debug))
            {
                if (importType == "должность")
                {
                    StreamReader streamReaderJobTitle = new StreamReader(pathFile);
                    var jobTitleName = streamReaderJobTitle.ReadToEnd().Split(new[] { "\n" }, StringSplitOptions.None);
                    for (int i = 1; i < jobTitleName.Length - 1; i++)
                    {
                        if (usersdbContext.JobTitles.FirstOrDefault(t => t.Name == jobTitleName[i]) == null)
                        {
                            var jobTitle = new JobTitle();
                            jobTitle.Name = jobTitleName[i];
                            usersdbContext.JobTitles.Add(jobTitle);
                            usersdbContext.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("В базе уже есть такая должность");
                        }
                    }
                    var jobTitles = usersdbContext.JobTitles.ToList();
                    Console.WriteLine("Список объектов:");
                    foreach (var jobTitl in jobTitles)
                    {
                        Console.WriteLine($"{jobTitl.Id}.{jobTitl.Name}");
                    }
                    streamReaderJobTitle.Close();
                }
                else if (importType == "сотрудник")
                {
                    StreamReader streamReaderEmployee = new StreamReader(pathFile);
                    var employeeRead = streamReaderEmployee.ReadToEnd().Split(new[] { "\n" }, StringSplitOptions.None);
                    for (int i = 1; i < employeeRead.Length - 1; i++)
                    {
                        var lineEmployee = employeeRead[i].Split("\t");
                        if (lineEmployee != null)
                        {
                            var employee = new Employee();
                            if (usersdbContext.JobTitles.FirstOrDefault(t => t.Name == lineEmployee[0]) != null)
                            {
                                employee.Department = usersdbContext.Departments.FirstOrDefault(t => t.Name == lineEmployee[0]).Id;
                            }
                            employee.FullName = lineEmployee[1];
                            employee.Login = lineEmployee[2];
                            employee.Password = lineEmployee[3];
                            if (usersdbContext.JobTitles.FirstOrDefault(t => t.Name == lineEmployee[3]) != null)
                            {
                                employee.JobTitle = usersdbContext.JobTitles.FirstOrDefault(t => t.Name == lineEmployee[3]).Id;
                            }
                            usersdbContext.Employees.Add(employee);
                            usersdbContext.SaveChanges();
                        }
                    }
                    streamReaderEmployee.Close();
                }
                else if (importType == "подразделение")
                {
                    using (var stringReaderDepartment = new StreamReader(pathFile))
                    {
                        var idDepartment = 1;
                        var roots = new List<Department>();
                        var notUsedNodes = new List<Department>();
                        var tsvReader = new TsvReader(stringReaderDepartment);
                        var departmentRecord = tsvReader.ReadNextRecord();
                        while (departmentRecord != null)
                        {
                            var parentDepartmentName = departmentRecord[1];
                            var name = departmentRecord[0];
                            var department = new Department();
                            department.Id = idDepartment;
                            department.Name = name;
                            var manager = usersdbContext.Employees.FirstOrDefault(t => t.FullName == departmentRecord[2]);
                            if (manager != null)
                            {
                                department.Manager = manager;
                            }
                            department.Phone = departmentRecord[3].Replace(" ", "");
                            //usersdbContext.Departments.Add(department);
                            //usersdbContext.SaveChanges();                           
                            if (string.IsNullOrEmpty(parentDepartmentName))
                            {
                                //var node = new TreeNode<Department>();
                                //node.Value = department;
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
                                    //notUsedNodes.Add(department);
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
                                    department.Parent.Name= parentDepartmentName;
                                    notUsedNodes.Add(department);
                                    //roots.Add(department);
                                    //usersdbContext.Departments.Add(department);
                                    //usersdbContext.SaveChanges();
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
