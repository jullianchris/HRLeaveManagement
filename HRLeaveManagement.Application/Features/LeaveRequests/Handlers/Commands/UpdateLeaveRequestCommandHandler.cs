using AutoMapper;
using HRLeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HRLeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HRLeaveManagement.Application.Responses;
using HRLeaveManagement.Domain;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using System.Threading;
using HRLeaveManagement.Application.Contracts.Persistence;

namespace HRLeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    public class UpdateLeaveRequestCommandHandler : IRequestHandler<UpdateLeaveRequestCommand, BaseCommandResponse>
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IMapper _mapper;

        public UpdateLeaveRequestCommandHandler(ILeaveTypeRepository leaveTypeRepository,
                                                IMapper mapper,
                                                ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<BaseCommandResponse> Handle(UpdateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new UpdateLeaveRequestDtoValidator(_leaveTypeRepository);
            var approvalValidator = new ChangeLeaveRequestApprovalValidator(_leaveRequestRepository);
            ValidationResult validationResult;
            if (!ReferenceEquals(request.ChangeLeaveRequestApprovalDto, null))
                validationResult = await approvalValidator.ValidateAsync(request.ChangeLeaveRequestApprovalDto);
            else 
                validationResult = await validator.ValidateAsync(request.UpdateLeaveRequestDto);

            if (!validationResult.IsValid)
            {
                response.Success = false;
                response.Message = "LeaveRequest update failed.";
                response.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return response;
            }

            LeaveRequest leaveRequest = await _leaveRequestRepository.Get(request.Id);
            if (!ReferenceEquals(request.ChangeLeaveRequestApprovalDto, null))
            {
               await _leaveRequestRepository.ChangeApprovalStatus(leaveRequest, request.ChangeLeaveRequestApprovalDto.Approved);
            } else if (!ReferenceEquals(request.UpdateLeaveRequestDto, null))
            {
                _mapper.Map(request.UpdateLeaveRequestDto, leaveRequest);
                await _leaveRequestRepository.Update(leaveRequest);
            }

            response.Message = "LeaveRequest update successful.";
            return response;
        }
    }
}
