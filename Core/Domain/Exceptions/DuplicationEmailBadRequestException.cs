using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class DuplicationEmailBadRequestException(string email) : BadRequestException($"There is already a user with this email {email}")
    {
    }
}
