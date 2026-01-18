using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public string TcNo { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? Position { get; set; }
    public decimal BaseSalary { get; set; }
    public string? IBAN { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public int AnnualLeaveBalance { get; set; }
    public bool IsActive { get; set; }
    public UserRole Role { get; set; }
    public string RoleName { get; set; } = null!;
}

public class CreateEmployeeRequest
{
    public Guid UserId { get; set; }
    public string TcNo { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }
    public string? Position { get; set; }
    public decimal BaseSalary { get; set; }
    public string? IBAN { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public int AnnualLeaveBalance { get; set; } = 14;
}

public class UpdateEmployeeRequest
{
    public string? Position { get; set; }
    public decimal BaseSalary { get; set; }
    public string? IBAN { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public int AnnualLeaveBalance { get; set; }
}

// Maaş
public class EmployeeSalaryDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
    public string Period { get; set; } = null!; // "Ocak 2024"
    public decimal BaseSalary { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal NetSalary { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentMethodName { get; set; }
    public string? Notes { get; set; }
}

public class CreateSalaryRequest
{
    public Guid EmployeeId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Bonus { get; set; } = 0;
    public decimal Deductions { get; set; } = 0;
    public string? Notes { get; set; }
}

public class PaySalaryRequest
{
    public Guid SalaryId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}

// Avans
public class EmployeeAdvanceDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedByName { get; set; }
    public bool IsApproved { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public bool IsDeducted { get; set; }
    public string? DeductionPeriod { get; set; }
    public string? Notes { get; set; }
}

public class CreateAdvanceRequest
{
    public Guid EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}

public class ApproveAdvanceRequest
{
    public Guid AdvanceId { get; set; }
    public bool Approved { get; set; }
    public string? Notes { get; set; }
}

public class PayAdvanceRequest
{
    public Guid AdvanceId { get; set; }
    public int DeductionMonth { get; set; }
    public int DeductionYear { get; set; }
}

// İzin
public class EmployeeLeaveDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public LeaveType Type { get; set; }
    public string TypeName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public LeaveStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedByName { get; set; }
    public string? Notes { get; set; }
}

public class CreateLeaveRequest
{
    public Guid EmployeeId { get; set; }
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
}

public class ApproveLeaveRequest
{
    public Guid LeaveId { get; set; }
    public bool Approved { get; set; }
    public string? Notes { get; set; }
}

// Ek Mesai
public class EmployeeOvertimeDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedByName { get; set; }
    public string? Notes { get; set; }
}

public class CreateOvertimeRequest
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public decimal HourlyRate { get; set; }
    public string? Notes { get; set; }
}

public class ApproveOvertimeRequest
{
    public Guid OvertimeId { get; set; }
    public bool Approved { get; set; }
    public string? Notes { get; set; }
}
