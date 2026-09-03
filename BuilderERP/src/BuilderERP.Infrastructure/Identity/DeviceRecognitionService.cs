using System.Security.Cryptography;
using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UAParser;

namespace BuilderERP.Infrastructure.Identity;

public class DeviceRecognitionService : IDeviceRecognitionService
{
    public const string DeviceCookieName = "ERP.DeviceId";

    private static readonly Parser UserAgentParser = Parser.GetDefault();

    private readonly AppDbContext _context;

    public DeviceRecognitionService(AppDbContext context)
    {
        _context = context;
    }

    public string GetOrCreateDeviceId(HttpContext httpContext)
    {
        if (httpContext.Request.Cookies.TryGetValue(DeviceCookieName, out var existingDeviceId) && !string.IsNullOrWhiteSpace(existingDeviceId))
        {
            return existingDeviceId;
        }

        var deviceId = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        httpContext.Response.Cookies.Append(DeviceCookieName, deviceId, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddYears(2),
            IsEssential = true
        });

        return deviceId;
    }

    public async Task<DeviceStatus> CheckDeviceAsync(Guid userId, string deviceId, HttpContext httpContext)
    {
        var device = await _context.UserDevices
            .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);

        if (device is not null)
        {
            return device.Status;
        }

        var isUsersFirstDevice = !await _context.UserDevices.AnyAsync(d => d.UserId == userId);
        var clientInfo = UserAgentParser.Parse(httpContext.Request.Headers.UserAgent.ToString());
        var deviceType = string.IsNullOrWhiteSpace(clientInfo.Device.Family) || clientInfo.Device.Family == "Other"
            ? "Desktop"
            : clientInfo.Device.Family;

        var newDevice = new UserDevice
        {
            UserId = userId,
            DeviceId = deviceId,
            DeviceType = deviceType,
            Browser = clientInfo.UA.Family,
            OperatingSystem = clientInfo.OS.Family,
            DeviceName = $"{clientInfo.OS.Family} - {clientInfo.UA.Family}",
            IPAddress = GetClientIp(httpContext),
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            RequestedDate = DateTime.UtcNow,
            Status = isUsersFirstDevice ? DeviceStatus.Approved : DeviceStatus.Pending
        };

        if (isUsersFirstDevice)
        {
            newDevice.ApprovedDate = DateTime.UtcNow;
        }

        _context.UserDevices.Add(newDevice);
        await _context.SaveChangesAsync();

        return newDevice.Status;
    }

    public async Task RecordSuccessfulLoginAsync(Guid userId, string deviceId, HttpContext httpContext)
    {
        var device = await _context.UserDevices
            .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);

        if (device is null)
        {
            return;
        }

        device.LastLoginDate = DateTime.UtcNow;
        device.LastIPAddress = GetClientIp(httpContext);
        await _context.SaveChangesAsync();
    }

    private static string? GetClientIp(HttpContext httpContext) => httpContext.Connection.RemoteIpAddress?.ToString();
}
