using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noesis.Javascript;
using ScrapySharp.JavaScript.Dom;

namespace ScrapySharp.JavaScript
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var context = new JavascriptContext())
            {

                // Setting external parameters for the context
                context.SetParameter("console", new SystemConsole());
                //context.SetParameter("message", "Hello World !");
                //context.SetParameter("number", 1);

                context.SetParameter("alert", new Action<string>(Alert));
                context.SetParameter("print", new Action<string>(Alert));

//                // Script
//                string script = @"
//        var i;
//        for (i = 0; i < 5; i++)
//            console.Print(message + ' (' + i + ')');
//        number += i;
//
//        alert('popopo');
//    ";

                var document = new Document();

                context.Run(File.ReadAllText("EmbeddedScripts/InitMockings.js"));

                //context.SetParameter("window", new Window(document));
                var window = new Window(document);
                context.SetParameter("window.W", window);
                context.SetParameter("document", document);
                
                // Running the script
                //context.Run(File.ReadAllText("EmbeddedScripts/jquery-1.9.1.min.js"));
                //context.Run(File.ReadAllText("EmbeddedScripts/jquery-1.9.1.js"));
                context.Run(File.ReadAllText("EmbeddedScripts/JavaScript1.js"));


                var e = context.GetParameter("e");
                var div = context.GetParameter("div");
                var windowjQuery = context.GetParameter("window.jQuery");


                var windowjQuery2 = context.GetParameter("window2.jQuery");

                var w3 = context.Run("window.jQuery");

                // Getting a parameter
                //Console.WriteLine("number: " + context.GetParameter("number"));
            }


            Console.WriteLine("Press any key ...");
            Console.ReadKey(true);
        }

        static void Alert(string iString)
        {
            Console.WriteLine(iString);
        }
    }

    public class SystemConsole
    {


        public void Print(string iString)
        {
            Console.WriteLine(iString);
        }
    }
}
