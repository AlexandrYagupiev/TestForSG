using NUnit.Framework;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using TestForSG;

namespace TestProjectTestForSG
{
    public class Tests
    {
        private readonly string connectionString = "Host=localhost;Port=5432;Database=usersdb;User Id=postgres;Password=1511";
        private readonly bool debug = true;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CountCorrect()
        {
            
            var businessLogic = new BusinessLogic(connectionString, debug);
            businessLogic.ReadFile("C:\\Users\\Александр\\Downloads\\departments.tsv", "подразделение");
            using (UsersdbContext usersdbContext = new UsersdbContext(connectionString, false))
            {
                var departmentCount =  usersdbContext.Departments.ToList().Count();
                Assert.AreEqual(6, departmentCount);
            }
       }
    }
}