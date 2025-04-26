using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuggyController : ControllerBase
    {
        [HttpGet("notfound")] // GET: api/buggy/notfound
        public IActionResult GetNotFoundRequest()
        {
            return NotFound(); // 404 Not Found
        }

        [HttpGet("servererror")] // GET: api/buggy/servererror
        public IActionResult GetServerErrorRequest()
        {
            throw new Exception(); // 500 Internal Server Error
            return Ok();
        }

        [HttpGet("badrequest")] // GET: api/buggy/badrequest
        public IActionResult GetBadRequest()
        {
            return BadRequest(); // 400 Bad Request
        }
        
        [HttpGet("badrequest/{id}")] // GET: api/buggy/badrequest/ahmed
        public IActionResult GetBadRequest(int id) // Validation Error
        {
            return BadRequest(); // 400 Bad Request
        }

        [HttpGet("unauthorized")] // GET: api/buggy/unauthorized
        public IActionResult GetUnauthorizedRequest()
        {
            return Unauthorized(); // 401 Unauthorized
        }
    }
}
