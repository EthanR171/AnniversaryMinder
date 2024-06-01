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
            Console.ForegroundColor = ConsoleColor.DarkCyan;
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
            Console.ResetColor();
        }

        // Prints a line with text
        internal static void WithText(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(VerLine);
            Console.ResetColor();
            Console.Write(text.PadRight(Width - 2));
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(VerLine);
            Console.ResetColor();
        }
    }


    class Program
    {
        // Constants
        public const string SCHEMA_PATH = "../../../../anniversary_schema.json";
        public const string SAMPLE_PATH = "../../../../anniversary.json"; // if no file existed this will be used as default

        public static string? GetUserInput()
        {
            return Console.ReadLine();
        }


        // Displays All Aniversaries homepage assuming json existed
        public static void GenerateHomePage(List<Anniversary> anniversaryList)
        {
            Console.Clear();
            LineOutput.Full(LineOutput.LineType.Top);
            LineOutput.WithText("                   ANNIVERSARY MINDER ~ All Anniversaries");
            LineOutput.Full(LineOutput.LineType.Middle);
            LineOutput.WithText("  Name(s)                                       Date         Type");
            LineOutput.Full(LineOutput.LineType.Middle);

            if (anniversaryList.Count == 0)
            {
                LineOutput.WithText("  There are currently no saved anniversaries.");
            }
            else
            {
                
                for (int i = 0; i < anniversaryList.Count; i++)
                {
                    // the paddings can be changed to alter the output if needed
                    string name = anniversaryList[i].Names.PadRight(43);
                    string date = anniversaryList[i].AnniversaryDate.PadRight(13);
                    string type = anniversaryList[i].AnniversaryType.PadRight(10);

                    LineOutput.WithText($"  {i + 1}. {name}{date}{type}");
                }
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
            Console.Clear();
            Anniversary selectedAnniversary = anniversaryList[index];
            string names = selectedAnniversary.Names;
            string date = selectedAnniversary.AnniversaryDate;
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
            Console.Clear();
            LineOutput.Full(LineOutput.LineType.Top);
            LineOutput.WithText("                   ANNIVERSARY MINDER ~ Edit Selected Anniversary");
            LineOutput.Full(LineOutput.LineType.Bottom);
            Console.WriteLine("KEY-IN NEW values for any field, or PRESS ENTER to accept the current field value...\n");
        }

        public static void GenerateAddNewAnniversaryPage()
        {
            Console.Clear();
            LineOutput.Full(LineOutput.LineType.Top);
            LineOutput.WithText("                   ANNIVERSARY MINDER ~ Add a New Anniversary");
            LineOutput.Full(LineOutput.LineType.Bottom);
            Console.WriteLine("Please key-in values for the following fields...\n");
        }

        public static void GenerateDeleteAnniversaryPage()
        {
            Console.Clear();
            LineOutput.Full(LineOutput.LineType.Top);
            LineOutput.WithText("                   ANNIVERSARY MINDER ~ Delete Selected Anniversary");
            LineOutput.Full(LineOutput.LineType.Bottom);
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
                    List<Anniversary> anniversaryList;
                    bool returnToMainMenu = false;
                    bool expandAnniversary = false;
                    bool addNew = false;
                    bool deleteAnniversary = false;
                    bool editAnniversary = false;
                    bool validAnniversary = false;

                    // ensure file even exists before continuing. create it if needed
                    if (!File.Exists(SAMPLE_PATH))
                    {
                        File.WriteAllText(SAMPLE_PATH, "[]"); // fill it with garbage for now 
                    }

                    // attempt to read sample json data into memory
                    if (ReadFile(SAMPLE_PATH, out string anniversaryJson))
                    {
                        // validate json data against schema
                        if (ValidateAnniversaryJSON(anniversaryJson, anniversarySchema, out IList<string> messages))
                        {
                            // parse json data into a list of objects for user to interact with
                            anniversaryList = DeserializeJSON(anniversaryJson);
                            GenerateHomePage(anniversaryList);
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

                        if (string.IsNullOrEmpty(userCommand))
                        {
                            continue; // just re-display menu options and reprompt user
                        }

                        bool isDigit = userCommand!.All(char.IsDigit); // flag to determine if user selected an anniversary
                        if (isDigit)
                        {
                            expandAnniversary = true;
                        }
                        else // if user is not looking to expand an anniversary they are looking to add, show upcoming or quit
                        {

                            switch (userCommand.ToLower())
                            {
                                case "n":
                                    addNew = true;
                                    break;
                                case "x":
                                    isDone = true;
                                    break;
                                default:
                                    Console.Clear();
                                    break;
                            }
                        }

                        if (addNew)
                        {
                            addNew = false;
                            do
                            {
                                GenerateAddNewAnniversaryPage();
                                string? userAddInput;
                                bool hasAddress = false;
                                int padding = 35;
                                Anniversary anniversaryToAdd = new Anniversary();
                                Address addressToAdd = new Address();

                                Console.Write("Name(s):".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    anniversaryToAdd.Names = userAddInput;
                                }

                                Console.Write("Anniversary Type:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    anniversaryToAdd.AnniversaryType = userAddInput;
                                }

                                Console.Write("Description:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    anniversaryToAdd.Description = userAddInput;
                                }

                                Console.Write("Anniversary Date (yyyy-mm-dd):".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    anniversaryToAdd.AnniversaryDate = userAddInput;
                                }

                                Console.Write("Email:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    anniversaryToAdd.Email = userAddInput;
                                }

                                Console.Write("Phone #:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    anniversaryToAdd.Phone = userAddInput;
                                }

                                Console.Write("Street Address:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    hasAddress = true;
                                    addressToAdd.StreetAddress = userAddInput;
                                }

                                Console.Write("Municipality:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    hasAddress = true;
                                    addressToAdd.Municipality = userAddInput;
                                }

                                Console.Write("Province:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    hasAddress = true;
                                    addressToAdd.Province = userAddInput;
                                }

                                Console.Write("PostalCode:".PadRight(padding));
                                userAddInput = GetUserInput();
                                if (!string.IsNullOrEmpty(userAddInput))
                                {
                                    hasAddress = true;
                                    addressToAdd.PostalCode = userAddInput;
                                }

                                if (hasAddress)
                                {
                                    anniversaryToAdd.Address = addressToAdd;
                                }

                                anniversaryList.Add(anniversaryToAdd);
                                string json_all = JsonConvert.SerializeObject(anniversaryList);
                                if (ValidateAnniversaryJSON(json_all, anniversarySchema, out IList<string> userEditedErrors))
                                {
                                    validAnniversary = true;
                                    // update the jsonfile 
                                    WriteToFile(SAMPLE_PATH, json_all);
                                }
                                else
                                {
                                    anniversaryList.Remove(anniversaryToAdd);
                                    Console.WriteLine($"\nThe following validation errors occured....\n");

                                    // Report validation error messages
                                    foreach (string msg in userEditedErrors)
                                        Console.WriteLine($"\t{msg}");

                                    Console.WriteLine("\nERROR: Invalid anniversary information entered. Press any key to make corrections.");
                                    Console.ReadLine();
                                }


                            } while (!validAnniversary);
                            validAnniversary = false; // reset it just incase
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

                                GenerateSelectedAnniversaryPage(anniversaryList, selectedAnniversaryNumber);
                                Console.Write("Enter command: ");
                                userCommand = GetUserInput(); // at this point it should only be 'E' 'D' or 'M'

                                if (string.IsNullOrEmpty(userCommand))
                                {
                                    continue; // just re-display menu options and reprompt user
                                }

                                switch (userCommand.ToLower())
                                {
                                    case "e":
                                        editAnniversary = true;
                                        break;
                                    case "d":
                                        deleteAnniversary = true;
                                        break;
                                    case "m":
                                        returnToMainMenu = true;
                                        break;
                                    default:
                                        continue; // re-display screen and prompt for user input
                                }

                                if (deleteAnniversary)
                                {
                                    bool goBackOnePage = false;
                                    deleteAnniversary = false;
                                    Anniversary anniversaryToDelete = anniversaryList[selectedAnniversaryNumber];
                                    do
                                    {
                                        string? userDeleteInput; // y or n
                                        GenerateDeleteAnniversaryPage();
                                        Console.Write($"Delete \"{anniversaryToDelete.AnniversaryType}\" anniversary for \"{anniversaryToDelete.Names}\"? (Y/N): ");
                                        userDeleteInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userDeleteInput))
                                        {
                                            switch (userDeleteInput.ToLower())
                                            {
                                                case "y":
                                                    anniversaryList.Remove(anniversaryToDelete);
                                                    string json_all = JsonConvert.SerializeObject(anniversaryList);
                                                    if (ValidateAnniversaryJSON(json_all, anniversarySchema, out IList<string> userEditedErrors))
                                                    {
                                                        validAnniversary = true;
                                                        // update the jsonfile 
                                                        WriteToFile(SAMPLE_PATH, json_all);
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"\nThe following validation errors occured....\n");

                                                        // Report validation error messages
                                                        foreach (string msg in userEditedErrors)
                                                            Console.WriteLine($"\t{msg}");

                                                        Console.WriteLine("\nERROR: Invalid anniversary information entered. Press any key to make corrections.");
                                                        Console.ReadLine();
                                                    }
                                                    returnToMainMenu = true;
                                                    break;
                                                case "n":
                                                    goBackOnePage = true;
                                                    break;
                                            } // end switch
                                        }
                                        if (goBackOnePage)
                                        {
                                            goBackOnePage = false;
                                            break;
                                        }
                                    } while (!validAnniversary);

                                }

                                if (editAnniversary) // user chose to edit an anniversary
                                {
                                    editAnniversary = false;
                                    do
                                    {
                                        string? userEditInput;
                                        bool hasAddress = anniversaryList[selectedAnniversaryNumber].Address != null;
                                        bool createdAddress = false; // will be true the second user enters input that is not null or empty
                                        Console.Clear();
                                        GenerateEditSelectedAnniversaryPage();

                                        /******************************************************************************************  
                                         *                   UPDATE OBJECT PROPERTIES SECTION                                     *
                                         ******************************************************************************************/

                                        Console.Write($"Name(s) \"{anniversaryList[selectedAnniversaryNumber].Names}\": ");
                                        userEditInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userEditInput))
                                        {
                                            anniversaryList[selectedAnniversaryNumber].Names = userEditInput; //update anniversary
                                        }


                                        Console.Write($"Anniversary Type \"{anniversaryList[selectedAnniversaryNumber].AnniversaryType}\": ");
                                        userEditInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userEditInput))
                                        {
                                            anniversaryList[selectedAnniversaryNumber].AnniversaryType = userEditInput; // update type 
                                        }

                                        Console.Write($"Description \"{anniversaryList[selectedAnniversaryNumber].Description}\": ");
                                        userEditInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userEditInput))
                                        {
                                            anniversaryList[selectedAnniversaryNumber].Description = userEditInput; // update desc 
                                        }

                                        Console.Write($"Anniversary Date (yyyy-mm-dd) \"{anniversaryList[selectedAnniversaryNumber].AnniversaryDate}\": ");
                                        userEditInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userEditInput))
                                        {
                                            anniversaryList[selectedAnniversaryNumber].AnniversaryDate = userEditInput; // update date
                                        }

                                        Console.Write($"Email \"{anniversaryList[selectedAnniversaryNumber].Email}\": ");
                                        userEditInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userEditInput))
                                        {
                                            anniversaryList[selectedAnniversaryNumber].Email = userEditInput; // update email 
                                        }

                                        Console.Write($"Phone # \"{anniversaryList[selectedAnniversaryNumber].Phone}\": ");
                                        userEditInput = GetUserInput();
                                        if (!string.IsNullOrEmpty(userEditInput))
                                        {
                                            anniversaryList[selectedAnniversaryNumber].Phone = userEditInput; // update phone 
                                        }


                                        if (hasAddress)
                                        {
                                            Console.Write($"Street Address \"{anniversaryList[selectedAnniversaryNumber].Address!.StreetAddress}\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                anniversaryList[selectedAnniversaryNumber].Address!.StreetAddress = userEditInput; // update street 
                                            }

                                            Console.Write($"Municipality \"{anniversaryList[selectedAnniversaryNumber].Address!.Municipality}\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                anniversaryList[selectedAnniversaryNumber].Address!.Municipality = userEditInput; // update municipality 
                                            }

                                            Console.Write($"Province \"{anniversaryList[selectedAnniversaryNumber].Address!.Province}\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                anniversaryList[selectedAnniversaryNumber].Address!.Province = userEditInput; // update province 
                                            }

                                            Console.Write($"PostalCode \"{anniversaryList[selectedAnniversaryNumber].Address!.PostalCode}\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                anniversaryList[selectedAnniversaryNumber].Address!.PostalCode = userEditInput; // update postal code 
                                            }
                                        }
                                        else // no address field so if user input isn't null, create the Address object for the Anniversary
                                        {
                                            Address newAddress = new Address();

                                            Console.Write($"Street Address \"\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                createdAddress = true;
                                                newAddress.StreetAddress = userEditInput;
                                            }

                                            Console.Write($"Municipality \"\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                createdAddress = true;
                                                newAddress.Municipality = userEditInput;
                                            }

                                            Console.Write($"Province \"\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                createdAddress = true;
                                                newAddress.Province = userEditInput;
                                            }

                                            Console.Write($"PostalCode \"\": ");
                                            userEditInput = GetUserInput();
                                            if (!string.IsNullOrEmpty(userEditInput))
                                            {
                                                createdAddress = true;
                                                newAddress.PostalCode = userEditInput;
                                            }

                                            if (createdAddress)
                                            {
                                                anniversaryList[selectedAnniversaryNumber].Address = newAddress;
                                            }
                                        }

                                        /******************************************************************************************  
                                         *                   VALIDATE NEW JSON CREATED FROM USER EDITED OBJECT                    *
                                         ******************************************************************************************/
                                        string json_all = JsonConvert.SerializeObject(anniversaryList);
                                        if (ValidateAnniversaryJSON(json_all, anniversarySchema, out IList<string> userEditedErrors))
                                        {
                                            validAnniversary = true;
                                            // update the jsonfile 
                                            WriteToFile(SAMPLE_PATH, json_all);
                                        }
                                        else
                                        {
                                            Console.WriteLine($"\nThe following validation errors occured....\n");

                                            // Report validation error messages
                                            foreach (string msg in userEditedErrors)
                                                Console.WriteLine($"\t{msg}");

                                            Console.WriteLine("\nERROR: Invalid anniversary information entered. Press any key to make corrections.");
                                            Console.ReadLine();
                                        }

                                    } while (!validAnniversary); // edit anniversary loop
                                    validAnniversary = false; // reset it just incase
                                    returnToMainMenu = true; // let the user go back to the homepage now
                                } // end edit anniversary if-statement

                            } while (!returnToMainMenu); // selected anniversay loop
                        } // end expand anniversary option
                    } // end read json file into memory

                } while (!isDone); // main menu loop 
            }
            else // failed to read json schema
            {
                Console.WriteLine("ERROR:\tUnable to read the schema file.");
            }

        } // end Main()
    }
}