using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;

namespace JSONrawdataconvert
{
    class Convert
    {

        public class Options
        {
            [Option('p', Required = true, HelpText = "Path of JSON")]
            public string jsonpath { get; set; }

            [Option('d', Required = false, HelpText = "Set custom path")]
            public string path { get; set; }

        }

        //Main
        public static void Main(string[] args)
        {

            var cmdOptions = Parser.Default.ParseArguments<Options>(args);
            cmdOptions.WithParsed(
                options => {
                    Main(options);
                });
        }

        //Main
        public static void Main(Options options)
        {
            if (File.Exists(options.jsonpath))
            {
                if(options.path == null)
                {
                    options.path = returnPath(options.jsonpath);
                }

                if (!options.jsonpath.Equals(options.path))
                {
                    convertCSV(options.jsonpath, options.path);
                }

                callError("Cannot overwrite file");
            }

            else
            {
                callError(String.Format("{0} File does not exist", options.jsonpath));
            }
        }


        //Convert to CSV
        public static void convertCSV(string path, string wpath)
        {
            string content = System.IO.File.ReadAllText(path);
            var csv = new StringBuilder();
            Data dData = JsonConvert.DeserializeObject<Data>(content);

            if (dData == null)
            {
                callError("Improper JSON formatting");
            }

            csv.AppendLine(String.Format("{0}; {1}; {2}; {3}; {4}; {5}; {6};",
                    dData.assayID, dData.assayLotNumber, dData.assayName, dData.firmwareVersion, dData.instrumentName, dData.instrumentUuid, dData.sampleId));

            foreach (object scan in dData.scans)
            {
                Count dCount = JsonConvert.DeserializeObject<Count>(scan.ToString());
                csv.AppendLine((dData.dateTime.Split("T"))[0]);
                csv.AppendLine("00:00:00");
                csv.AppendLine("COUNT");
                csv.AppendLine(dCount.count);
                csv.AppendLine("SYNC");
                csv.AppendLine(getFormatted(dCount.sync));
                csv.AppendLine("AMP");
                csv.AppendLine(getFormatted(dCount.amp));
            }

            try
            {
                File.WriteAllText(wpath, csv.ToString());
                Console.WriteLine("File {0} written", wpath);
            }
            catch(Exception e)
            {
                callError(e.ToString());
            }
            
        }


        //Get formatted line
        public static string getFormatted(int[] list)
        {
            string line = "";
            foreach (int i in list)
            {
                line += (i + ",");
            }
            return line;
        }


        //Errors
        public static void callError(string message)
        {
            Console.WriteLine("ERROR: {0}.", message);
            Console.WriteLine("---");
            Console.WriteLine("USAGE: JSONrawdataconvert.exe [INPUTPATH] [OUTPUTPATH] true|false");
            Console.WriteLine("[INPUTPATH] = Path to JSON file");
            Console.WriteLine("[OUTPUTPATH] = Path and name of CSV to be created ('-s' for same)");
            Console.WriteLine("true|false = Include metadata in CSV file");
            Console.WriteLine("---");
        }


        //Return name
        public static string returnPath(string path)
        {
            string[] wpath = path.Split(".");
            wpath[wpath.Length - 1] = "csv";
            string final = string.Join(".", wpath);
            return final ;
        }
    }
}
