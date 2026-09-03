using BuilderERP.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace BuilderERP.Infrastructure.Identity;

public interface IDeviceRecognitionService
{
    string GetOrCreateDeviceId(HttpContext httpContext);

    Task<DeviceStatus> CheckDeviceAsync(Guid userId, string deviceId, HttpContext httpContext);

    Task RecordSuccessfulLoginAsync(Guid userId, string deviceId, HttpContext httpContext);
}
