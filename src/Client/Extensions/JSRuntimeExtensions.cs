using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Extensions
{
    public static class JSRuntimeExtensions
    {
        public static void ToggleModal(this IJSRuntime jsRuntime, string modalId)
        {
            jsRuntime.InvokeVoidAsync("ToggleModal", modalId);
        }
    }
}
