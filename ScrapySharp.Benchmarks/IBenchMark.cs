using System;

namespace ScrapySharp.Benchmarks
{
    public interface IBenchMark
    {
        void Run();

        TimeSpan TimeElapsed { get; }
    }
}