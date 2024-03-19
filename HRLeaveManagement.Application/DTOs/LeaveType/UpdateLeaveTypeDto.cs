using HRLeaveManagement.Application.DTOs.Common;

namespace HRLeaveManagement.Application.DTOs.LeaveType
{
    public class UpdateLeaveTypeDto : BaseDto, ILeaveTypeDto
    {
        public string LeaveName { get; set; }
        public int DefaultDays { get; set; }
    }
}
