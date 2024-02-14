using Sharprompt;
using TestForSG.Models;



namespace TestForSG
{
    internal class Program
    {
        static void Main(string[] args)
        {           
            var path = Prompt.Input<string>("Укажите путь к файлу");
            //var importType = Prompt.Select("Выберете тип импорта", new[] { "подразделение", "сотрудник", "должность" });
            String line;
            using (UsersdbContext usersdbContext = new UsersdbContext())
            {
                StreamReader sr = new StreamReader(path);
                line = sr.ReadLine();
                for (int i=0; i<line.Length;i++)
                {
                        line = sr.ReadLine();
                        var jobTitle = new JobTitle();
                        jobTitle.Name = line;
                        usersdbContext.JobTitles.Add(jobTitle);
                        usersdbContext.SaveChanges();
                }               
                sr.Close();
            }
        }

              
    }
}
