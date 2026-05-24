using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KresApp.Domain.Entities;

namespace KresApp.Application.Interfaces;

public interface IMeetingRequestRepository
{
    Task<List<MeetingRequest>> GetAllAsync();
    Task<List<MeetingRequest>> GetByParentIdAsync(Guid parentId);
    Task<List<MeetingRequest>> GetByTeacherIdAsync(Guid teacherId);
    Task<List<MeetingRequest>> GetByChildIdAsync(Guid childId);
    Task<MeetingRequest?> GetByIdAsync(Guid id);
    Task AddAsync(MeetingRequest request);
    Task UpdateAsync(MeetingRequest request);
}
