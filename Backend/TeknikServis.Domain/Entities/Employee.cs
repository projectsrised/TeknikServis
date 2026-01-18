using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class Employee : BranchEntity
{
    public Guid UserId { get; set; }
    public string TcNo { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? Position { get; set; }
    public decimal BaseSalary { get; set; }
    public string? IBAN { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public int AnnualLeaveBalance { get; set; } = 14;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual ICollection<EmployeeSalary> Salaries { get; set; } = new List<EmployeeSalary>();
    public virtual ICollection<EmployeeAdvance> Advances { get; set; } = new List<EmployeeAdvance>();
    public virtual ICollection<EmployeeLeave> Leaves { get; set; } = new List<EmployeeLeave>();
    public virtual ICollection<EmployeeOvertime> Overtimes { get; set; } = new List<EmployeeOvertime>();
}

public class EmployeeSalary : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal NetSalary { get; set; }
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidAt { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual Employee Employee { get; set; } = null!;
}

public class EmployeeAdvance : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public bool IsApproved { get; set; } = false;
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidAt { get; set; }
    public bool IsDeducted { get; set; } = false;
    public int? DeductionMonth { get; set; }
    public int? DeductionYear { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual Employee Employee { get; set; } = null!;
}

public class EmployeeLeave : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual Employee Employee { get; set; } = null!;
}

public class EmployeeOvertime : TenantEntity
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual Employee Employee { get; set; } = null!;
}
