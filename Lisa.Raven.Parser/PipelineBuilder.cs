using System;

namespace Lisa.Raven.Parser
{
	public static class PipelineBuilder
	{
		public static PipelineBuilder<TIn, TOut> Start<TIn, TOut>(IPipe<TIn, TOut> pipe)
		{
			return new PipelineBuilder<TIn, TOut>(pipe.Process);
		}
	}

	public class PipelineBuilder<TIn, TOut>
	{
		private readonly Func<TIn, TOut> _func;

		internal PipelineBuilder(Func<TIn, TOut> func)
		{
			_func = func;
		}

		public PipelineBuilder<TIn, TNextOut> Chain<TNextOut>(IPipe<TOut, TNextOut> pipe)
		{
			return new PipelineBuilder<TIn, TNextOut>(i => pipe.Process(_func(i)));
		}

		public Func<TIn, TNextOut> End<TNextOut>(IPipe<TOut, TNextOut> pipe)
		{
			return i => pipe.Process(_func(i));
		}
	}
}