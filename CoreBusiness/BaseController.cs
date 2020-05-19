using Microsoft.AspNetCore.Mvc;
using my8ViewObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBusiness
{
    public class BaseController : Controller
    {
        public readonly CurrentProcess _process;
        public BaseController(CurrentProcess process)
        {
            _process = process;
        }
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public IActionResult ToResponse()
        {
            var model = new ResponseJsonModel();
            bool hasError = _checkHasError(model);
            return Json(model, _jsonSerializerSettings);
        }
        public IActionResult ToResponse<T>(T data) where T : class
        {
            var model = new ResponseJsonModel<T>();
            if (!_checkHasError(model))
                model.data = data;

            return Json(model, _jsonSerializerSettings);
        }
        public IActionResult ToResponse(bool isSuccess)
        {
            var model = new ResponseJsonModel();

            if (!_checkHasError(model))
                model.success = isSuccess;

            return Json(model, _jsonSerializerSettings);
        }
        private bool _checkHasError(ResponseJsonModel model)
        {
            var hasError = _process.HasError;
            if (hasError)
            {
                var errorMessage = _process.ToError();

                model.error = new ErrorJsonModel()
                {
                    code = errorMessage.Message,
                    message = "error",
                    trace_keys = errorMessage.TraceKeys
                };
            }
            model.success = !hasError;
            return hasError;
        }

    }
}
