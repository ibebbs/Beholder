using System;

namespace Lensman.Application.State.Transition
{
    public class ToRecognising : ITransition
    {
        public ToRecognising(Guid asUserId)
        {
            AsUserId = asUserId;
        }

        public Guid AsUserId { get; }
    }
}
