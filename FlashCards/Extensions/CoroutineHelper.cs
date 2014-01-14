using System.Collections.Generic;
using Caliburn.Micro;

namespace FlashCards.Extensions
{
    /// <summary>
    /// A Helper-Class to execute Coroutines from code and not always have to create an <see cref="IEnumerator{T}"/>
    /// </summary>
    public class Run
    {
        /// <summary>
        /// Execute a single coroutine;
        /// </summary>
        /// <param name="coroutine">The <see cref="Caliburn.Micro.IResult"/></param>
        /// <param name="context">The <see cref="Caliburn.Micro.CoroutineExecutionContext"/></param>
        public static void Coroutine(IResult coroutine,
                                     CoroutineExecutionContext context = null)
        {
            Coroutine(new[] {coroutine}, context);
        }

        /// <summary>
        /// Execute a enumeration of coroutines.
        /// </summary>
        /// <param name="coroutines">The <see cref="IEnumerator&lt;IResult&gt;"/></param>
        /// <param name="context">The <see cref="Caliburn.Micro.CoroutineExecutionContext"/></param>
        public static void Coroutine(IEnumerator<IResult> coroutines,
                                     CoroutineExecutionContext context = null)
        {
            if (context == null)
                Caliburn.Micro.Coroutine.BeginExecute(coroutines);
            else
                Caliburn.Micro.Coroutine.BeginExecute(coroutines, context);
        }

        /// <summary>
        /// Execute an enumerable of coroutines
        /// </summary>
        /// <param name="coroutines">The <see cref="IEnumerable&lt;IResult&gt;"/></param>
        /// <param name="context">The <see cref="CoroutineExecutionContext"/></param>
        public static void Coroutine(IEnumerable<IResult> coroutines,
                                     CoroutineExecutionContext context = null)
        {
            Coroutine(coroutines.GetEnumerator(), context);
        }
    }
}