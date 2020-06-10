namespace Lensman.Application.State.Transition
{
    public class ToRunning : ITransition
    {
        public ToRunning(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
