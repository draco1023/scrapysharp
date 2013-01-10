using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScrapySharp.Core;

namespace ScrapySharp.Benchmarks
{
    public class FastHtmlParserBenchMark : IBenchMark
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public void Run()
        {
            stopwatch.Reset();
            stopwatch.Start();
            var source = File.ReadAllText("Html/Page1.htm");
            int matched = 0;

            for (int i = 0; i < BenchMarksParameters.Iterations; i++)
            {
                var fastHtmlParser = new FastHtmlParser(source);
                List<Tag> tags = fastHtmlParser.ReadTags();
                var spans = tags.SelectMany(t => t.Children).Where(t => t.Name == "span").ToArray();
                matched += spans.Length;

                GC.Collect(3, GCCollectionMode.Forced);
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