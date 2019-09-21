using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace StockFrontend.Shared
{
    public class FullHeaderBase : ComponentBase 
    {
        [Parameter]
        public EventCallback OnClick { get; set; }

        protected IUriHelper UriHelper { get; set; }

        private string _ticker;

        public string ticker
        {
            get { return _ticker; }
            set
            {
                _ticker = CleanInput(value);
            }
        }

        public void KeyWasPressed(UIKeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                SearchGo();
            }
        }

        public async void SearchGo()
        {
            ValidateTicker();
            await OnClick.InvokeAsync(null);
        }

        public void ValidateTicker()
        {
            //TODO: validation code here
        }

        #region Header Input Aux methods   
        //Cleans any extra $ symbols that could exist in the raw input
        public string CleanInput(string raw)
        {
            string output = raw.Replace("$", "");
            return "$" + output;
        }

        //Call this when user clicks to clean current value and add a "$"
        public void ClearContent()
        {
            _ticker = "$";
        }

        #endregion
        #region Nav Instructions
        public void NavigateHome()
        {
            UriHelper.NavigateTo("/");
        }
        public void NavigateApp()
        {
            UriHelper.NavigateTo("/App");
        }
        public void NavigateAbout()
        {
            UriHelper.NavigateTo("/About");
        }
        #endregion
    }
}
