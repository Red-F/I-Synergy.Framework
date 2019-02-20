﻿using ISynergy.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISynergy.Filters
{
    public sealed class RequestShouldBeLocalFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.IsLocal())
                context.Result = new ForbidResult();
        }
    }
}