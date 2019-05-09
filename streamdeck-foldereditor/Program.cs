﻿using System;

namespace streamdeck_foldereditor
{
    class Program
    {
        private const string VERSION = "1.1";
        private static ProfilesExplorer pe = new ProfilesExplorer();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Stream Deck 'Folder-Back' Editor v{VERSION} by BarRaider\r\nView my other projects on: https://BarRaider.github.io\r\nDISCLAIMER: This may damage your profiles! Use with caution and under your own risk\r\nTo make sure I stress this enough - this usually works but you MAY loose the profile - make a backup by EXPORTING the profile before running this app...");
                var profileInfo = GetProfileToEdit();
                if (profileInfo == null)
                {
                    return;
                }
                var folderInfo = GetFolderLocationToEdit(profileInfo);
                if (String.IsNullOrWhiteSpace(folderInfo))
                {
                    return;
                }
                ReAssignFolderBack(profileInfo, folderInfo);
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("GENERAL ERROR: " + ex);
                Console.WriteLine("\r\n*** For help go to https://BarRaider.github.io ***");
            }
        }

        private static ProfileInfo GetProfileToEdit()
        {
            int? profileNum;
            int idx = 1;
            
            var profiles = pe.GetProfiles();

            if (profiles == null || profiles.Count == 0)
            {
                Console.WriteLine("Could not find profiles in folder");
                return null;
            }

            foreach (var profile in profiles)
            {
                Console.WriteLine($"[{idx}] {profile.Name}");
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

        private static string GetFolderLocationToEdit(ProfileInfo profileInfo)
        {
            int idx = 1;
            int? folderNum;
            var folders = pe.FindProfileFolderActions(profileInfo);

            if (folders == null || folders.Count == 0)
            {
                Console.WriteLine("Profile does not have any top-level folders");
                return null;
            }

            DisplayKeyLayout();

            Console.WriteLine("\r\nFolders in profile:");
            foreach (var location in folders)
            {
                Console.WriteLine($"[{idx}]   Location: {location}");
                idx++;
            }

            Console.WriteLine("The key location is the physical location of the folder on the Stream Deck.\r\nSo 0,0 is the top left key and 4,2 is the bottom right key. Only actual folders are shown above.");
            Console.Write("Enter the number (NUMBER in the square brackets NOT the location) of the folder to edit: ");

            folderNum = SanitizeNumericInput(idx - 1);
            if (folderNum == null || !folderNum.HasValue)
            {
                return null;
            }

            return folders[folderNum.Value - 1];
        }

        private static void DisplayKeyLayout()
        {
            Console.WriteLine("---------- STREAM DECK LAYOUT ----------");
            for (int idx = 0; idx < 3; idx++)
            {
                for (int idx2 = 0; idx2 < 5; idx2++)
                {
                    Console.Write($"[{idx2},{idx}]  ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("----------------------------------------");
        }

        private static void ReAssignFolderBack(ProfileInfo profileInfo, String folderLocation)
        {
            DisplayKeyLayout();
            Console.WriteLine($"Moving the back location for the folder in location: {folderLocation}");
            Console.WriteLine("Choose where you would like the Back button to move to. If that position is already used, it will be moved to the Top-Left (0,0) position\r\n");
            Console.Write("Enter the Column to put the back folder on [0-4]:");
            int? col = SanitizeNumericInput(4);

            if (col == null || !col.HasValue)
            {
                return;
            }

            Console.Write("Enter the Row to put the back folder on [0-2]:");
            int? row = SanitizeNumericInput(2);

            if (row == null || !row.HasValue)
            {
                return;
            }
            pe.MoveFolderBackLocation(profileInfo, folderLocation, $"{col.Value},{row.Value}");
        }

        private static int? SanitizeNumericInput(int maxNum)
        {
            int numeric;
            string result = Console.ReadLine();
            if (!Int32.TryParse(result, out numeric))
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
