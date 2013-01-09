using System;
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

            for (int i = 0; i < BenchMarksParameters.Iterations; i++)
            {
                var fastHtmlParser = new FastHtmlParser(source);
                var tags = fastHtmlParser.ReadTags().ToList();
                var spans = tags.SelectMany(t => t.Children).Where(t => t.Name == "span").ToArray();

                //var nodes = html.CssSelect("span.login-box").ToArray();
                ////Console.WriteLine("Matched: {0}", nodes.Length);

                //nodes = html.CssSelect("span#pass-box").ToArray();
                ////Console.WriteLine("Matched: {0}", nodes.Length);

                //nodes = html.CssSelect("script[type=text/javascript]").ToArray();
                //Console.WriteLine("Matched: {0}", nodes.Length);

                //html.Descendants("span").ToArray();
            }

            stopwatch.Stop();
        }

        public TimeSpan TimeElapsed
        {
            get { return stopwatch.Elapsed; }
        }
    }
}