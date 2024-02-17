using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sharprompt;
using System.Configuration;
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
            var BusinessLogic = new BusinessLogic(ConfigurationManager.ConnectionStrings["NpgsqlСonnection"].ConnectionString,false);
            try
            {
                BusinessLogic.ReadFile(path, importType);
                Console.WriteLine("Файл успешно считан");
                var id = Prompt.Input<int>("Укажите id подразделения");
                BusinessLogic.OutputCurrentDataStructure(id);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            
        }


    }
}

