using System;
using System.Collections.Generic;

namespace TimeSheetAuthAPI.Models;

public partial class Employee
{
    public int EmployeeKey { get; set; }

    public string? EmployeeFirstName { get; set; }

    public string? EmployeeLastName { get; set; }

    public string? EmployeeDesignation { get; set; }

    public string? EmployeeEmail { get; set; }

    public string? EmployeeAlternateEmail { get; set; }

    public long? EmployeePhone { get; set; }

    public DateTime? EmployeeHireDate { get; set; }

    public int? EmployeeWorkLocationKey { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDateTime { get; set; }

    public short IsDeleted { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public string? Password { get; set; }

    //public string? Token { get; set; }
}
