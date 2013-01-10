using System;

namespace ScrapySharp.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var agilityPackBenchMark = new AgilityPackBenchMark();
            var hDocumentBenchMark = new HDocumentBenchMark();
            var fastHtmlParserBenchMark = new FastHtmlParserBenchMark();

            fastHtmlParserBenchMark.Run();
            Console.WriteLine("FastHtmlParserBenchMark => Elapsed time: {0} ms", fastHtmlParserBenchMark.TimeElapsed.TotalMilliseconds);

            //GC.Collect();

            agilityPackBenchMark.Run();
            Console.WriteLine("AgilityPackBenchMark => Elapsed time: {0} ms", agilityPackBenchMark.TimeElapsed.TotalMilliseconds);

            //GC.Collect();

            hDocumentBenchMark.Run();
            Console.WriteLine("HDocumentBenchMark => Elapsed time: {0} ms", hDocumentBenchMark.TimeElapsed.TotalMilliseconds);

            //GC.Collect();

            

            Console.WriteLine("Press any key ...");
            Console.ReadKey(true);
        }
    }
}
