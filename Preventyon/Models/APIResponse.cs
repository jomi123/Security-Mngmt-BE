using System;
using System.Collections.Generic;
using System.Net;

namespace RequestDemoMinimal.models
{
    public class APIResponse
    {
        public APIResponse()
        {
            var ErrorMessages = new List<string>();

        }

        // change to pascal case
        public bool isSuccess { get; set; }
        public Object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }

    }
}
