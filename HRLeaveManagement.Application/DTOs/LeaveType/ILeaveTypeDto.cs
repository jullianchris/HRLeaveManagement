namespace HRLeaveManagement.Application.DTOs.LeaveType
{
    public interface ILeaveTypeDto
    {
        public string LeaveName { get; set; }
        public int DefaultDays { get; set; }
    }
}
