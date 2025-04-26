using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ErrorModels
{
    public class ValidationErrorResponse
    {
        public ValidationErrorResponse(IEnumerable<ValidationError> errors)
        {
            Errors = errors;
        }

        public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;
        public string ErrorMessage { get; set; } = "Validation Errors";
        public IEnumerable<ValidationError> Errors { get; set; }
    }
}
