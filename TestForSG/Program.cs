using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sharprompt;
using System.IO;
using System.Xml.Linq;
using TestForSG.Models;



namespace TestForSG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = Prompt.Input<string>("Укажите путь к файлу");
            var importType = Prompt.Select("Выберете тип импорта", new[] { "подразделение", "сотрудник", "должность" });

            ReadFile(path, importType);
            Console.WriteLine("Считал");
        }

        public static void ReadFile(string pathFile, string importType)
        {
            using (UsersdbContext usersdbContext = new UsersdbContext())
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
                            //employee.Department = ;
                            employee.FullName = lineEmployee[1];
                            employee.Login = lineEmployee[2];
                            employee.Password = lineEmployee[3];
                            //employee.JobTitle = ;
                            usersdbContext.Employees.Add(employee);
                            usersdbContext.SaveChanges();
                        }
                    }
                    streamReaderEmployee.Close();
                }
                else if (importType == "подразделение")
                {
                    StreamReader streamReaderDepartment = new StreamReader(pathFile);
                    var departmentRead = streamReaderDepartment.ReadToEnd().Split(new[] { "\n" }, StringSplitOptions.None);
                    for (int i = 1; i < departmentRead.Length - 1; i++)
                    {
                        var lineEmployee = departmentRead[i].Split("\t");
                        if (lineEmployee != null)
                        {
                            var department = new Department(); 
                            
                            department.Name = lineEmployee[2];
                            department.Phone = lineEmployee[3].Replace(" ","");

                            usersdbContext.Departments.Add(department);
                            usersdbContext.SaveChanges();
                        }
                    }
                    streamReaderDepartment.Close();
                }
            }
        }
    }
}

