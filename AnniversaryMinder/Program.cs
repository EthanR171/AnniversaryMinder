using Newtonsoft.Json;          // JsonConvert class
using Newtonsoft.Json.Schema;   // JSchema class 
using Newtonsoft.Json.Linq;     // JObject class

namespace AnniversaryMinder
{
    class Program
    {
        // Constants
        public const string SCHEMA_PATH = "../../../../anniversary_schema.json";
        public const string SAMPLE_PATH = "../../../../anniversary.json";


        // Attempts to read the json file specified by 'path' into the string 'json'
        // Returns 'true' if successful or 'false' if it fails
        private static bool ReadFile(string path, out string json)
        {
            try
            {
                json = File.ReadAllText(path);
                return true;
            }
            catch
            {
                json = "";
                return false;
            }
        }

        // Validates the json data specified by the parameter 'jsonData' against the schema
        // 
        // Returns 'true' if valid or 'false' if invalid
        // Also populates the out parameter 'messages' with validation error messages if invalid
        private static bool ValidateAnniversaryJSON(string jsonData, string jsonSchema, out IList<string> messages)
        {
            JSchema schema = JSchema.Parse(jsonSchema);
            JArray anniversaryArray = JArray.Parse(jsonData);
            messages = new List<string>();
            bool isValid = true;

            foreach (JObject anniversary in anniversaryArray)
            {
                if (!anniversary.IsValid(schema, out IList<string> annivMessages))
                {
                    isValid = false;
                    foreach (var msg in annivMessages)
                    {
                        messages.Add(msg);
                    }
                }
            }

            return isValid;
        } // end ValidateTeamData()

        // either gives us a list populated list of anniversary objects or an empty one
        private static List<Anniversary> DeserializeJSON(string jsonData)
        {
            try
            {
                List<Anniversary> anniversaries = JsonConvert.DeserializeObject<List<Anniversary>>(jsonData) ?? new();
                return anniversaries;
            }
            catch (JsonException) // in theory, we should never end up here based on the null-coalescing operator above
            {
                Console.WriteLine("\nERROR: Failed to convert data in JSON file to an Anniversary List");
                return new List<Anniversary>(); 
            }
        }

        static void Main(string[] args)
        {
            // attempt to read json schema into memory
            if (ReadFile(SCHEMA_PATH, out string jsonSchema))
            {
                // attempt to read sample json data into memory
                if (ReadFile(SAMPLE_PATH, out string jsonData))
                {
                    // validate json data against schema
                    if (ValidateAnniversaryJSON(jsonData, jsonSchema, out IList<string> messages))
                    {
                        // parse json data into a list of objects for user to interact with
                        List<Anniversary> anniversaryList = DeserializeJSON(jsonData);
                        

                    }
                    else // json sample does not follow schema rules
                    {
                        Console.WriteLine($"\nERROR:\tData file is invalid.\n");

                        // Report validation error messages
                        foreach (string msg in messages)
                            Console.WriteLine($"\t{msg}");
                    }
                }
                else // no sample json file exists
                {
                    // create an empty collection of anniversary objects for the user
                    List<Anniversary> anniversaryList = new List<Anniversary>();
                    
                }
            }
            else // failed to read json schema
            {
                Console.WriteLine("ERROR:\tUnable to read the schema file.");
            }

        } // end Main()
    }
}