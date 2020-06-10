using System;

namespace Lensman.Login.Event
{
    public class SelectedUser
    {
        public SelectedUser(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
