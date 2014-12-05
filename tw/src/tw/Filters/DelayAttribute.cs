// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace MvcSample.Filters
{
  public class DelayAttribute : ActionFilterAttribute
  {
    public DelayAttribute(int milliseconds)
    {
      Delay = milliseconds;
      Rand = new Random();
    }

    public int Delay { get; private set; }
    public Random Rand { get; private set; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      if (context.HttpContext.Request.Method == "GET")
      {
        // slow down incoming GET requests
        await Task.Delay(Rand.Next(1, Delay));
      }

      var executedContext = await next();

      if (executedContext.Result is ViewResult)
      {
        // slow down outgoing view results
        await Task.Delay(Rand.Next(0, Delay));
      }
    }
  }
}