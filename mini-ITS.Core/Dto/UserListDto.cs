using System;

namespace mini_ITS.Core.Models
{
    public class UserListDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }

        public UserListDto() { }

        public UserListDto(Guid id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}
