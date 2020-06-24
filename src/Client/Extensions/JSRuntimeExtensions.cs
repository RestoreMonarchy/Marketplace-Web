using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Extensions
{
    public static class JSRuntimeExtensions
    {        
        public static async Task ToggleModalAsync(this IJSRuntime jsRuntime, string modalId)
        {
            await jsRuntime.InvokeVoidAsync("ToggleModal", modalId);
        }
    }
}
