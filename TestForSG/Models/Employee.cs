using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestForSG.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int Department { get; set; }

    public string FullName { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public int JobTitle { get; set; }
}
