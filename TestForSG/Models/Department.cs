using System;
using System.Collections.Generic;

namespace TestForSG.Models;

public partial class Department
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int? ManagerId { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Department> InverseParent { get; set; } = new List<Department>();

    public virtual Employee? Manager { get; set; } = null!;

    public virtual Department? Parent { get; set; } = null!;

    public override string ToString()
    {
        return $"{nameof(Id)}:{Id} {nameof(ParentId)}:{ParentId} {nameof(ManagerId)}:{ManagerId} {nameof(Name)}:{Name} {nameof(Phone)}:{Phone}";
    }
}
