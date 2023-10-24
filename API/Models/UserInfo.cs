using System;
using System.Collections.Generic;

namespace api.Models;

public partial class UserInfo
{
    public int UserId { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }
}
