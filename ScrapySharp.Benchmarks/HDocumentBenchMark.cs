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
            stopwatch.Start();
            var source = File.ReadAllText("Html/Page1.htm");
            var html = HDocument.Parse(source);

            var nodes = html.CssSelect("span.login-box").ToArray();
            Console.WriteLine("Matched: {0}", nodes.Length);

            nodes = html.CssSelect("span#pass-box").ToArray();
            Console.WriteLine("Matched: {0}", nodes.Length);

            nodes = html.CssSelect("script[type=text/javascript]").ToArray();
            Console.WriteLine("Matched: {0}", nodes.Length);

            stopwatch.Stop();
        }

        public TimeSpan TimeElapsed
        {
            get { return stopwatch.Elapsed; }
        }
    }
}