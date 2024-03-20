using AutoMapper;
using HRLeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HRLeaveManagement.Application.Responses;
using HRLeaveManagement.Domain;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using HRLeaveManagement.Application.DTOs.LeaveType.Validators;
using System.Linq;
using HRLeaveManagement.Application.Contracts.Persistence;

namespace HRLeaveManagement.Application.Features.LeaveTypes.Handlers.Commands
{
    public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, BaseCommandResponse>
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper _mapper;

        public CreateLeaveTypeCommandHandler(ILeaveTypeRepository leaveTypeRepository, IMapper mapper)
        {
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
        }

        public async Task<BaseCommandResponse> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            var response         = new BaseCommandResponse();
            var validator        = new CreateLeaveTypeDtoValidator();
            var validationResult = await validator.ValidateAsync(request.CreateLeaveTypeDto);

            if (!validationResult.IsValid)
            {
                response.Success = false;
                response.Message = "LeaveType creation failed.";
                response.Errors  = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return response;
            }

            var leaveType    = _mapper.Map<LeaveType>(request.CreateLeaveTypeDto);
            leaveType        = await _leaveTypeRepository.Add(leaveType);

            response.Id      = leaveType.Id;
            response.Message = "LeaveType creation successful.";

            return response;
        }
    }
}
