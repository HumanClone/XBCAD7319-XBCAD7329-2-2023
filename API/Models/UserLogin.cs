using System;
using System.Collections.Generic;

namespace api.Models;

public partial class UserLogin
{
    public string Email { get; set; }

    public string Password { get; set; }
}
