using Microsoft.AspNetCore.Mvc;

namespace BuilderERP.Web.Extensions;

public static class ControllerExtensions
{
    public static bool IsAjaxRequest(this Controller controller)
    {
        return controller.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}
