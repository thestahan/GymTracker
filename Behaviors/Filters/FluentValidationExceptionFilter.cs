﻿using FluentValidation;
using GymTracker.ApiResponses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GymTracker.Behaviors.Filters;

public class FluentValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException ex)
        {
            var errorResponse = new ApiValidationErrorResponse
            {
                Errors = ex.Errors.Select(e => e.ErrorMessage).ToList()
            };

            context.Result = new BadRequestObjectResult(errorResponse);

            context.ExceptionHandled = true;
        }
    }
}
