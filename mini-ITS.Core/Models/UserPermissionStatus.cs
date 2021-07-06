using System;
using System.Collections.Generic;
using System.Text;

namespace mini_ITS.Core.Models
{
    public class UserPermissionStatus
    {
        public bool isAdmin { get; set; }
        public bool isManager { get; set; }
        public bool isUser { get; set; }

        public bool isCreater { get; set; }
        public bool isFromDepartment { get; set; }

        public string roleName { get; set; }
    }
}
