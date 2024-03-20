using AutoMapper;
using HRLeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HRLeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HRLeaveManagement.Application.Responses;
using HRLeaveManagement.Domain;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using HRLeaveManagement.Application.Contracts.Persistence;
using HRLeaveManagement.Application.Contracts.Infrastructure;
using HRLeaveManagement.Application.Models;

namespace HRLeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, BaseCommandResponse>
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public CreateLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository,
                                                IMapper mapper,
                                                IEmailSender emailSender,
                                                ILeaveTypeRepository leaveTypeRepository = null)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _mapper = mapper;
            _emailSender = emailSender;
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<BaseCommandResponse> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
        {
            var response         = new BaseCommandResponse();
            var validator        = new CreateLeaveRequestDtoValidator(_leaveTypeRepository);
            var validationResult = await validator.ValidateAsync(request.CreateLeaveRequestDto);
            if (!validationResult.IsValid)
            {
                response.Success = false;
                response.Message = "LeaveRequest creation failed.";
                response.Errors  = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return response;
            }

            var leaveRequest = _mapper.Map<LeaveRequest>(request.CreateLeaveRequestDto);
            leaveRequest     = await _leaveRequestRepository.Add(leaveRequest);
            response.Message = "LeaveRequest creation successful.";
            response.Id      = leaveRequest.Id;
            SendEmail(request);
            return response;
        }

        public async void SendEmail(CreateLeaveRequestCommand request)
        {
            var leaveRequest = request.CreateLeaveRequestDto;
            var email = new Email
            {
                To      = "employee@org.com",
                Body    = $"Your leave request for {leaveRequest.StartDate:D} to {leaveRequest.EndDate:D} has been submitted successfully.",
                Subject = "Leave Request Submitted"
            };
            try
            {
                await _emailSender.SendEmailAsync(email);
            }
            catch(Exception e)
            {
                /// Log error
            }
        }
    }
}
