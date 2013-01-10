using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Html.Dom;

namespace ScrapySharp.Benchmarks
{
    public class HDocumentBenchMark : IBenchMark
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public void Run()
        {
            stopwatch.Reset();
            
            var source = File.ReadAllText("Html/Page1.htm");
            int matched = 0;

            for (int i = 0; i < BenchMarksParameters.Iterations; i++)
            {
                stopwatch.Start();
                var html = HDocument.Parse(source);

                var nodes = html.CssSelect("span.login-box").ToArray();
                matched += nodes.Length;

                nodes = html.CssSelect("span#pass-box").ToArray();
                matched += nodes.Length;

                nodes = html.CssSelect("script[type=text/javascript]").ToArray();
                matched += nodes.Length;

                stopwatch.Stop();
                GC.Collect();
            }

            Console.WriteLine("Matched: {0}", matched);

            stopwatch.Stop();
        }

        public TimeSpan TimeElapsed
        {
            get { return stopwatch.Elapsed; }
        }
    }
}