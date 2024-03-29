﻿using AutoMapper;
using HRLeaveManagement.Application.DTOs.LeaveType;
using HRLeaveManagement.Application.Features.LeaveTypes.Requests.Queries;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using HRLeaveManagement.Application.Contracts.Persistence;

namespace HRLeaveManagement.Application.Features.LeaveTypes.Handlers.Queries
{
    public class GetLeaveTypeListRequestHandler : IRequestHandler<GetLeaveTypeListRequest, List<LeaveTypeDto>>
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper _mapper;

        public GetLeaveTypeListRequestHandler(ILeaveTypeRepository leaveTypeRepository, IMapper mapper)
        {
            _leaveTypeRepository = leaveTypeRepository;
            _mapper              = mapper;
        }

        public async Task<List<LeaveTypeDto>> Handle(GetLeaveTypeListRequest request,
            CancellationToken cancellationToken)
        {
            var leaveTypes = await _leaveTypeRepository.GetLeaveTypesWithDetails();
            return _mapper.Map<List<LeaveTypeDto>>(leaveTypes);
        }
    }
}
