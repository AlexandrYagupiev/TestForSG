using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestForSG.Models;

public partial class Department
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public int ManagerId { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }
}
