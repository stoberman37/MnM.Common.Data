using System;

namespace MnM.Common.Data
{


	public class RetryStrategyByCount : RetryStrategyBase<Exception>
	{
		public Func<RetryStrategyBase<Exception>, Exception, bool> RetryByCount = (s, e) => s.RetryCount <= s.MaxRetryCount;

		public RetryStrategyByCount() : base((s, ex) => s.RetryCount <= s.MaxRetryCount)
		{
		}

		public RetryStrategyByCount(int maxRetryCount) : base((s, ex) => s.RetryCount <= s.MaxRetryCount, maxRetryCount)
		{
		}
	}
}