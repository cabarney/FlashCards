using System.Threading.Tasks;
using Caliburn.Micro;

namespace FlashCards.Results
{
    public class DelayResult : ResultBase
    {
        private readonly int _milliseconds;

        public DelayResult(int milliseconds)
        {
            _milliseconds = milliseconds;
        }

        public override async void Execute(CoroutineExecutionContext context)
        {
            await Task.Delay(_milliseconds);

            OnCompleted();
        }
    }
}
