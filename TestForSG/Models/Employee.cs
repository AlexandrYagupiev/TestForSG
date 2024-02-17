using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

namespace TestForSG.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int Department { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int JobTitle { get; set; }

    public virtual Department DepartmentNavigation { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual JobTitle JobTitleNavigation { get; set; } = null!;

    public override string ToString()
    {
        return $"{nameof(Id)}:{Id} {nameof(Department)}:{Department} {nameof(FullName)}:{FullName} {nameof(Login)}:{Login} {nameof(Password)}:{Password} {nameof(JobTitle)}:{JobTitle}";
    }
}
