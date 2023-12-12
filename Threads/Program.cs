using System.Diagnostics;
using System.Threading;

namespace Threads
{
	internal class Program
	{
		class Range
		{
			public long Start { get; set; }
			public long End { get; set; }

			public int ThreadNum { get; set; }
		}

		const int ThreadCount = 30;
		const long Max = (long)3E09;

		static long[] partialSums = new long[ThreadCount];

		static void PartialSum(object? delta) // object? delta = Range {start, end}
		{
			Range? range = delta as Range;
			if (range == null)
			{
				return;
			}

			long partialSum = 0;
			for (long i = range.Start; i < range.End; i++)
			{
				partialSum += i;
			}

			partialSums[range.ThreadNum] = partialSum;
		}


		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			Thread[] threads = new Thread[ThreadCount];
			for (int i = 0; i < ThreadCount; i++)
			{
				Range range = new Range
				{
					Start = (Max / ThreadCount) * i,
					End = (Max / ThreadCount) * (i + 1),
					ThreadNum = i
				};

				threads[i] = new Thread(PartialSum);
				threads[i].Start(range);
			}

			//while (threads.Any(t => t.ThreadState == System.Threading.ThreadState.Running)) ; // option 1

			threads
				.ToList()
				.ForEach(t => t.Join());

			long totalSum = partialSums.Sum();

			sw.Stop();

			long mutithread = sw.ElapsedMilliseconds;

			sw.Reset();
			sw.Start();

			long sum = 0;
			for (long i = 0; i < Max; i++)
			{
				sum += i;
			}
			sw.Stop();

			long singlethread = sw.ElapsedMilliseconds;

			Console.WriteLine($"singleThread = {singlethread}; multiThread = {mutithread}");
		}
	}
}
