using System;

namespace ScrapySharp.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var agilityPackBenchMark = new AgilityPackBenchMark();
            var hDocumentBenchMark = new HDocumentBenchMark();

            agilityPackBenchMark.Run();
            Console.WriteLine("AgilityPackBenchMark => Elapsed time: {0} ms", agilityPackBenchMark.TimeElapsed.TotalMilliseconds);
            
            hDocumentBenchMark.Run();
            Console.WriteLine("HDocumentBenchMark => Elapsed time: {0} ms", hDocumentBenchMark.TimeElapsed.TotalMilliseconds);
            
            
            agilityPackBenchMark.Run();
            Console.WriteLine("AgilityPackBenchMark => Elapsed time: {0} ms", agilityPackBenchMark.TimeElapsed.TotalMilliseconds);
            
            hDocumentBenchMark.Run();
            Console.WriteLine("HDocumentBenchMark => Elapsed time: {0} ms", hDocumentBenchMark.TimeElapsed.TotalMilliseconds);
            

            agilityPackBenchMark.Run();
            Console.WriteLine("AgilityPackBenchMark => Elapsed time: {0} ms", agilityPackBenchMark.TimeElapsed.TotalMilliseconds);

            hDocumentBenchMark.Run();
            Console.WriteLine("HDocumentBenchMark => Elapsed time: {0} ms", hDocumentBenchMark.TimeElapsed.TotalMilliseconds);


            Console.WriteLine("Press any key ...");
            Console.ReadKey(true);
        }
    }
}
