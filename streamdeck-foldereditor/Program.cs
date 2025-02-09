using streamdeck_foldereditor.Models;
using System;
using System.IO;
using System.Linq;

namespace streamdeck_foldereditor
{
    class Program
    {
        private const string VERSION = "2.0";
        private static readonly ProfilesExplorer pe = new ProfilesExplorer();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Stream Deck 'Folder-Back' Editor v{VERSION} by BarRaider\r\nView my other projects on: https://BarRaider.com\r\n\r\nDISCLAIMER: This may damage your profiles! Use with caution and under your own risk.\r\nTo make sure I stress this enough - this usually works but you MAY loose the profile - make a backup by EXPORTING the profile before running this app...");
                Console.WriteLine("MAKE SURE YOU'VE QUIT STREAM DECK APP AND IT'S FULLY SHUT DOWN and then Press any key to start...");
                Console.ReadKey();
                var profileInfo = GetProfileToEdit();
                if (profileInfo == null)
                {
                    return;
                }

                int pageNum = 0;
                if (profileInfo.Pages.Pages.Count > 1)
                {
                    var receivedPageNum = GetRequiredPageNum(profileInfo);
                    if (receivedPageNum == null || !receivedPageNum.HasValue)
                    {
                        Console.WriteLine("A valid page number is required.");
                    }
                    pageNum = receivedPageNum.Value;
                }
                var folderInfo = GetFolderLocationToEdit(profileInfo, pageNum);
                if (folderInfo == null || String.IsNullOrWhiteSpace(folderInfo.Item1) || String.IsNullOrWhiteSpace(folderInfo.Item2))
                {
                    return;
                }
                
                ReAssignFolderBack(profileInfo, folderInfo.Item1, folderInfo.Item2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GENERAL ERROR: " + ex);
                Console.WriteLine("\r\n*** For help go to https://BarRaider.com ***");
            }
        }

        private static ProfileInfo GetProfileToEdit()
        {
            int? profileNum;
            int idx = 1;
            
            var profiles = pe.GetProfiles().Where(p => p.Version == "2.0").OrderBy(p => p.Name).ToList();

            if (profiles == null || profiles.Count == 0)
            {
                Console.WriteLine("Could not find profiles in folder");
                return null;
            }

            foreach (var profile in profiles)
            {
                Console.WriteLine($"[{idx}] {profile.Name} ({profile.Pages.Pages.Count} pages)");
                idx++;
            }

            Console.Write("Enter the number of the profile to edit: ");
            profileNum = SanitizeNumericInput(idx - 1);
            if (profileNum == null || !profileNum.HasValue)
            {
                return null;
            }
           
            return profiles[profileNum.Value - 1];
        }

        private static int? GetRequiredPageNum(ProfileInfo profileInfo)
        {
            Console.Write($"This profile has {profileInfo.Pages.Pages.Count} pages. Enter the number of the page that has the folder: ");
            int? pageNum = SanitizeNumericInput(profileInfo.Pages.Pages.Count);
            if (pageNum == null || !pageNum.HasValue)
            {
                return null;
            }

            return pageNum - 1;
        }

        
        private static Tuple<string, string> GetFolderLocationToEdit(ProfileInfo profileInfo, int pageNum)
        {
            int idx = 1;
            int? folderNum;
            var pageFolderLocations = pe.FindProfileFolderActions(profileInfo, pageNum);

            if (pageFolderLocations == null || pageFolderLocations.Count == 0)
            {
                Console.WriteLine("Profile does not have any top-level folders");
                return null;
            }

            var streamDeckType = SDUtil.GetStreamDeckTypeFromProfile(profileInfo);

            if (streamDeckType == StreamDeckType.CorsairGKeys || streamDeckType == StreamDeckType.CorsairCueSDK || streamDeckType == StreamDeckType.StreamDeckPedal)
            {
                Console.WriteLine("Folders are not supported on this device.");
                return null;
            }

            if (streamDeckType == StreamDeckType.UNKNOWN)
            {
                Console.WriteLine($"**** WARNING: Unrecognized Stream Deck type [{profileInfo?.Device?.Model}] - It is NOT RECOMMENDED to continue ****");
            }

            SDUtil.DisplayKeyLayout(streamDeckType);

            Console.WriteLine("\r\nFolders in profile:");
            foreach (var folder in pageFolderLocations)
            {
                Console.WriteLine($"[{idx}]   Location: {folder.FolderLocation}");
                idx++;
            }

            Console.WriteLine("The key location is the physical location of the folder on the Stream Deck.\r\nSo 0,0 is the top left key. Only actual folders are shown above.");
            Console.Write("Enter the number (NUMBER in the square brackets NOT the location) of the folder to edit: ");

            folderNum = SanitizeNumericInput(idx - 1);
            if (folderNum == null || !folderNum.HasValue)
            {
                return null;
            }
            string pageDirectory = Path.Combine(profileInfo.FullPath, ProfilesExplorer.PAGE_FOLDER_INTERNAL_SUFFIX, pageFolderLocations[folderNum.Value - 1].FolderProfilePath);
            if (!Directory.Exists(pageDirectory))
            {
                pageDirectory += "Z";

                if (!Directory.Exists(pageDirectory))
                {
                    Console.WriteLine("Page directory not found: " + pageDirectory);
                    return null;
                }
            }

            return new Tuple<string, string>(pageDirectory, pageFolderLocations[folderNum.Value - 1].FolderLocation);
        }
                
        
        private static void ReAssignFolderBack(ProfileInfo profileInfo, String pagePath, String folderLocation)
        {
            var streamDeckType = SDUtil.GetStreamDeckTypeFromProfile(profileInfo);
            SDUtil.DisplayKeyLayout(streamDeckType);
            int maxCols = SDUtil.GetColumnsForStreamDeckType(streamDeckType);
            int maxRows = SDUtil.GetRowsForStreamDeckType(streamDeckType);

            Console.WriteLine($"Moving the back location for the folder in location: {folderLocation}");
            Console.WriteLine("Choose where you would like the Back button to move to. If that position is already used, it will be moved to the Top-Left (0,0) position\r\n");
            Console.Write($"Enter the Column to put the back folder on [0-{maxCols - 1}]:");
            int? col = SanitizeNumericInput(maxCols);

            if (col == null || !col.HasValue)
            {
                return;
            }

            Console.Write($"Enter the Row to put the back folder on [0-{maxRows - 1}]:");
            int? row = SanitizeNumericInput(maxRows);

            if (row == null || !row.HasValue)
            {
                return;
            }
            pe.MoveFolderBackLocation(profileInfo, pagePath, folderLocation, $"{col.Value},{row.Value}");
        }
        

        private static int? SanitizeNumericInput(int maxNum)
        {
            string result = Console.ReadLine();
            if (!Int32.TryParse(result, out int numeric))
            {
                Console.WriteLine("Invalid input! Number expected");
                return null;
            }

            if (numeric > maxNum)
            {
                Console.WriteLine("Invalid input! Number too high");
                return null;
            }

            return numeric;
        }
    }
}
