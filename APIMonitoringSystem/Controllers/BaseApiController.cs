using Core.CommonModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIMonitoringSystem.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected APIResponse<T> OkResponse<T>(T data, string message = "")
    {
        return APIResponse<T>.SuccessResponse(data, message);
    }

    protected APIResponse<T> FailResponse<T>(string message, params string[] errors)
    {
        return APIResponse<T>.FailureResponse(errors.ToList(), message);
    }

    protected long? GetTenantIdFromClaims()
    {
        var tenantClaim = User.FindFirstValue("tenantId");
        return long.TryParse(tenantClaim, out var tenantId) ? tenantId : null;
    }

    protected long? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
