using HRLeaveManagement.Domain.Common;

namespace HRLeaveManagement.Domain
{
    public class LeaveType : BaseDomainEntity
    {
        public string LeaveName { get; set; }
        public int DefaultDays { get; set; }
    }
}
