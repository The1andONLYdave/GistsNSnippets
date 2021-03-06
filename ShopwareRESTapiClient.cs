using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    using System;
    using System.Globalization;
    using System.Net;
    using Newtonsoft.Json;
    using RestSharp;
    using CommandLine;

    internal class Program
    {
        // Consume them
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Values are available here
                if (options.BooleanValue) Console.WriteLine("Filename: {0}", options.TextValue);
            }

            //cmdline args: outfile, user, url, pass, ID(nix=alle orders)


            const string user = "api";
            const string pass = "11223344556677889900abcdefg";
            

            // http://restsharp.org/
            var client =
                new RestClient(@"http://shop.domain.com/api")
                {
                    Authenticator = new DigestAuthenticator(user, pass)
                };

            var request = new RestRequest("orders/", Method.GET);
          //  request.AddUrlSegment("id", "200"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                Console.WriteLine(@"################ ERROR ################");
                Console.WriteLine(response.ErrorException.Message);
            }
            else
            {
                var content = response.Content; // raw content as string

                dynamic json = JsonConvert.DeserializeObject(content);
                Console.WriteLine(json);
            }
            Console.ReadKey();
        }
    }

    public class DigestAuthenticator :
        IAuthenticator
    {
        private readonly string _user;
        private readonly string _pass;

        public DigestAuthenticator(string user, string pass)
        {
            _user = user;
            _pass = pass;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.Credentials = new NetworkCredential(_user, _pass);
        }
    }

    // Define a class to receive parsed values
    internal class Options
    {
        [Option('t', "text", Required = true, HelpText = "text value here")]
        public string TextValue { get; set; }

        [Option('n', "numeric", HelpText = "numeric value here")]
        public double NumericValue { get; set; }

        [Option('b', "bool", HelpText = "on|off switch here")]
        public bool BooleanValue { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }
}
