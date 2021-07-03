using System;

namespace mini_ITS.Core.Models
{
    public class UserList
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }

        public UserList() { }

        public UserList(Guid id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}
