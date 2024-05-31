using Newtonsoft.Json;          // JsonConvert class
using Newtonsoft.Json.Schema;   // JSchema class 
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;     // JObject class

namespace AnniversaryMinder
{

    internal class LineOutput
    {
        // Constants & variables for generating menu boxes
        internal enum LineType { Top, Middle, Bottom };
        internal const int Width = 80;
        internal const char HorLine = '\u2500';
        internal const char VerLine = '\u2502';
        internal const char TopLeftL = '\u250C';
        internal const char TopRightL = '\u2510';
        internal const char TopT = '\u252C';
        internal const char LeftT = '\u251C';
        internal const char RightT = '\u2524';
        internal const char BottomLeftL = '\u2514';
        internal const char BottomRightL = '\u2518';

        // Prints a continuous line
        internal static void Full(LineType type, string appendText = "")
        {
            char left = TopLeftL, middle = HorLine, right = TopRightL;

            if (type == LineType.Middle)
            {
                left = LeftT;
                right = RightT;
            }
            else if (type == LineType.Bottom)
            {
                left = BottomLeftL;
                right = BottomRightL;
            }

            Console.Write(left);
            for (int i = 0; i < Width - 2; i++) Console.Write(middle);
            Console.Write(right);
            Console.WriteLine(appendText);
        }

        // Prints a line with text
        internal static void WithText(string text)
        {
            Console.Write(VerLine);
            Console.Write(text.PadRight(Width - 2));
            Console.WriteLine(VerLine);
        }
    }


    class Program
    {
        // Constants
        public const string SCHEMA_PATH = "../../../../anniversary_schema.json";
        public const string SAMPLE_PATH = "../../../../anniversary.json";

        public static string? GetUserInput()
        {
            return Console.ReadLine();
        }


        // Displays All Aniversaries homepage assuming json existed
        public static void GenerateHomePageWithExistingAnniversaries(List<Anniversary> anniversaryList)
        {
            LineOutput.Full(LineOutput.LineType.Top);
            LineOutput.WithText("                   ANNIVERSARY MINDER ~ All Anniversaries");
            LineOutput.Full(LineOutput.LineType.Middle);
            LineOutput.WithText("  Name(s)                                       Date         Type");

            LineOutput.Full(LineOutput.LineType.Middle);
            for (int i = 0; i < anniversaryList.Count; i++)
            {
                // the paddings can be changed to alter the output if needed
                string name = anniversaryList[i].Names.PadRight(43);
                string date = anniversaryList[i].AnniversaryDate.ToString("yyyy-MM-dd").PadRight(13);
                string type = anniversaryList[i].AnniversaryType.PadRight(10);

                LineOutput.WithText($"  {i + 1}. {name}{date}{type}");
            }
            LineOutput.Full(LineOutput.LineType.Middle);
            LineOutput.WithText("  Press # from the above list to select an entry.");
            LineOutput.WithText("  Press N to add a new anniversary.");
            LineOutput.WithText("  Press U to list upcoming anniversaries.");
            LineOutput.WithText("  Press X to quit.");
            LineOutput.Full(LineOutput.LineType.Bottom);
        }

        public static void GenerateSelectedAnniversaryPage(List<Anniversary> anniversaryList, int index)
        {
            // throw an expection if index is larger than size of array later on...
            Anniversary selectedAnniversary = anniversaryList[index];
            string names = selectedAnniversary.Names;
            string date = selectedAnniversary.AnniversaryDate.ToString("yyyy-MM-dd");
            string type = selectedAnniversary.AnniversaryType;
            string? desc = selectedAnniversary.Description ?? "";
            string? email = selectedAnniversary.Email ?? "";
            string? phone = selectedAnniversary.Phone ?? "";
            Address? address = selectedAnniversary.Address;

            // padding for first column of output lines
            string outputLine1 = "  Names: ".PadRight(19);
            string outputLine2 = "  Date: ".PadRight(19);
            string outputLine3 = "  Type: ".PadRight(19);
            string outputLine4 = "  Description: ".PadRight(19);
            string outputLine5 = "  Email: ".PadRight(19);
            string outputLine6 = "  Phone: ".PadRight(19);
            string outputLine7 = "  Address: ".PadRight(19);
            string outputLine8 = "".PadRight(19);

            LineOutput.Full(LineOutput.LineType.Top);
            LineOutput.WithText("                   ANNIVERSARY MINDER ~ Selected Anniversary");
            LineOutput.Full(LineOutput.LineType.Middle);

            LineOutput.WithText($"{outputLine1}{names}");
            LineOutput.WithText($"{outputLine2}{date}");
            LineOutput.WithText($"{outputLine3}{type}");
            LineOutput.WithText($"{outputLine4}{desc}");
            LineOutput.WithText($"{outputLine5}{email}");
            LineOutput.WithText($"{outputLine6}{phone}");

            if (address != null)
            {
                LineOutput.WithText($"{outputLine7}{address.StreetAddress}");
                LineOutput.WithText($"{outputLine8}{address.Municipality}  {address.Province} {address.PostalCode}");
            }
            else
            {
                LineOutput.WithText($"  Address: ");
            }


            LineOutput.Full(LineOutput.LineType.Middle);
            LineOutput.WithText("  Press E to edit this anniversary.");
            LineOutput.WithText("  Press D to delete this anniversary.");
            LineOutput.WithText("  Press M to return to the main menu.");
            LineOutput.Full(LineOutput.LineType.Bottom);
        }

        public static void GenerateEditSelectedAnniversaryPage()
        {
            LineOutput.Full(LineOutput.LineType.Top);
            LineOutput.WithText("                   ANNIVERSARY MINDER ~ Edit Selected Anniversary");
            LineOutput.Full(LineOutput.LineType.Bottom);
            Console.WriteLine("KEY-IN NEW values for any field, or PRESS ENTER to accept the current field value...\n");
        }


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

        private static void WriteToFile(string path, string json)
        {
            try
            {
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to write to the file. Details: {ex.Message}");
            }
        }


        // Validates the json data specified by the parameter 'jsonData' against the schema
        // 
        // Returns 'true' if valid or 'false' if invalid
        // Also populates the out parameter 'validationErrorMessages' with object error messages if invalid
        private static bool ValidateAnniversaryJSON(string jsonData, string jsonSchema, out IList<string> validationErrorMessages)
        {
            JSchema schema = JSchema.Parse(jsonSchema);
            JArray anniversaryArray = JArray.Parse(jsonData);
            validationErrorMessages = new List<string>();
            bool isValid = true;

            foreach (JObject anniversary in anniversaryArray)
            {
                if (!anniversary.IsValid(schema, out IList<string> anniversaryObjectErrorMessages))
                {
                    isValid = false;
                    foreach (var msg in anniversaryObjectErrorMessages)
                    {
                        validationErrorMessages.Add(msg);
                    }
                }
            }

            return isValid;
        } // end ValidateTeamData()

        // either gives us a populated list of anniversary objects or an empty one
        // should only ever be invoked after ValidateAnniversaryJSON has been called
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
            if (ReadFile(SCHEMA_PATH, out string anniversarySchema))
            {
                string? userCommand; // stores what the user wants to do next 
                bool isDone = false;  // tracks when user is finished with the program
                do
                {
                    Console.Clear();
                    List<Anniversary> anniversaryList;
                    bool returnToMainMenu = false;
                    bool expandAnniversary = false;

                    bool editAnniversary = false;
                    bool validEditedAnniversary = false;

                    // attempt to read sample json data into memory
                    if (ReadFile(SAMPLE_PATH, out string anniversaryJson))
                    {
                        // validate json data against schema
                        if (ValidateAnniversaryJSON(anniversaryJson, anniversarySchema, out IList<string> messages))
                        {
                            // parse json data into a list of objects for user to interact with
                            anniversaryList = DeserializeJSON(anniversaryJson);
                            GenerateHomePageWithExistingAnniversaries(anniversaryList);
                        }
                        else // scenario for only the very first json sample ever read in the loop that does not follow schema rules
                        {
                            Console.WriteLine($"\nThe following validation errors occured....\n");

                            // Report validation error messages
                            foreach (string msg in messages)
                                Console.WriteLine($"\t{msg}");

                            break;  // just stop the program since this would have been the uneditied sample read in initialy

                        }

                        // at this point, the anniversaryJson is determined to be in a valid state
                        Console.Write("Enter a command: ");
                        userCommand = GetUserInput();

                        bool isDigit = userCommand!.All(char.IsDigit); // flag to determine if user selected an anniversary
                        if (isDigit)
                        {
                            expandAnniversary = true;
                        }
                        else // if user is not looking to expand an anniversary they are looking to add, show upcoming or quit
                        {

                            switch (userCommand)
                            {
                                case "x":
                                    isDone = true;
                                    break;
                                default:
                                    Console.Clear();
                                    break;
                            }
                        }

                        // handle menu homepage user input selection 
                        if (expandAnniversary)
                        {
                            int selectedAnniversaryNumber = Convert.ToInt32(userCommand) - 1; // minus one to correctly index
                            do
                            {
                                if (selectedAnniversaryNumber < 0 || selectedAnniversaryNumber >= anniversaryList.Count) // index was too large 
                                {
                                    returnToMainMenu = true;
                                    break;
                                }
                                Console.Clear();
                                GenerateSelectedAnniversaryPage(anniversaryList, selectedAnniversaryNumber);
                                Console.Write("Enter command: ");
                                userCommand = GetUserInput(); // at this point it should only be 'E' 'D' or 'M'
                                switch (userCommand)
                                {
                                    case "e":
                                        editAnniversary = true;
                                        break;
                                    case "m":
                                        returnToMainMenu = true;
                                        break;
                                }

                                if (editAnniversary) // user chose to edit an anniversary
                                {
                                    editAnniversary = false;
                                    do
                                    {
                                        string? userEditInput;
                                        Console.Clear();
                                        GenerateEditSelectedAnniversaryPage();

                                        Console.WriteLine($"Name(s) \"{anniversaryList[selectedAnniversaryNumber].Names}\": ");
                                        userEditInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userEditInput))
                                        {
                                            anniversaryList[selectedAnniversaryNumber].Names = userEditInput; //update anniversary
                                        }

                                        string json_all = JsonConvert.SerializeObject(anniversaryList);
                                        if (ValidateAnniversaryJSON(json_all, anniversarySchema, out IList<string> userEditedErrors))
                                        {
                                            validEditedAnniversary = true;
                                            // update the jsonfile 
                                            WriteToFile(SAMPLE_PATH, json_all);
                                        }
                                        else
                                        {
                                            Console.WriteLine($"\nThe following validation errors occured....\n");

                                            // Report validation error messages
                                            foreach (string msg in userEditedErrors)
                                                Console.WriteLine($"\t{msg}");

                                            Console.WriteLine("ERROR: Invalid anniversary information entered. Press any key to make corrections.");
                                            Console.ReadLine();
                                        }

                                    } while (!validEditedAnniversary); // edit anniversary loop
                                } // end edit anniversary if-statement

                                returnToMainMenu = true;

                            } while (!returnToMainMenu); // selected anniversay loop
                        } // end expand anniversary option
                    } // end read json file into memory
                    else // no sample json file exists
                    {
                        // create an empty collection of anniversary objects for the user
                        anniversaryList = new List<Anniversary>();

                    }
                } while (!isDone); // main menu loop 
            }
            else // failed to read json schema
            {
                Console.WriteLine("ERROR:\tUnable to read the schema file.");
            }

        } // end Main()
    }
}