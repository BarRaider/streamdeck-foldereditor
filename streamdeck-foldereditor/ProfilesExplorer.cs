using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using streamdeck_foldereditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace streamdeck_foldereditor
{
    internal class ProfilesExplorer
    {
        private const string MANINFEST_FILE = "manifest.json";
        private const string WIN_PROFILE_LOCATION = @"Elgato\StreamDeck\ProfilesV2";
        private const string MAC_PROFILE_LOCATION = @"~/Library/Application Support/com.elgato.StreamDeck/ProfilesV2";
        private const string FOLDER_UUID = "com.elgato.streamdeck.profile.openchild";
        public const string PAGE_FOLDER_INTERNAL_SUFFIX = "Profiles";
        private const string ACTION_SETTINGS_FOLDER_UUID = "ProfileUUID";
        private const string FOLDER_PLUGIN_SUFFIX = ".sdProfile";

        public List<ProfileInfo> GetProfiles()
        {
            string directory = GetProfileDir();

            List<ProfileInfo> profiles = new List<ProfileInfo>();
            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Directory not found: " + directory);
                throw new DirectoryNotFoundException("Directory not found: " + directory);
            }

            foreach (var folder in Directory.EnumerateDirectories(directory))
            {
                try
                {
                    ProfileInfo profile = FindProfile(folder);
                    if (profile != null)
                    {
                        profile.FullPath = folder;
                        profiles.Add(profile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Profile Error: {ex}\r\nFolder: {folder}");
                }
            }

            return profiles;
        }

        private ProfileInfo FindProfile(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            string manifestFile = Path.Combine(directory, MANINFEST_FILE);
            if (!File.Exists(manifestFile))
            {
                return null;
            }

            JObject manifest = JObject.Parse(File.ReadAllText(manifestFile));
            return manifest.ToObject<ProfileInfo>();
        }

        private Page FindPage(string directory, string pageUUID)
        {
            if (!Directory.Exists(directory))
            {
                return null;
            }

            string pageDirectory = Path.Combine(directory, PAGE_FOLDER_INTERNAL_SUFFIX, ExtractPageDirectory(pageUUID));
            if (!Directory.Exists(pageDirectory))
            {
                pageDirectory += "Z";

                if (!Directory.Exists(pageDirectory))
                {
                    Console.WriteLine("Page directory not found: " + pageDirectory);
                    return null;
                }
            }

            string manifestFile = Path.Combine(pageDirectory, MANINFEST_FILE);
            if (!File.Exists(manifestFile))
            {
                return null;
            }

            JObject manifest = JObject.Parse(File.ReadAllText(manifestFile));
            Page page = manifest.ToObject<Page>();
            page.FullPath = pageDirectory;

            return page;
        }

        private string ExtractPageDirectory(string pageUUID)
        {
            pageUUID = pageUUID.Replace("-", "");
            byte[] hexBytes = new byte[16];
            int byteCounter = 0;
            for (int idx = 0; idx < pageUUID.Length; idx += 2)
            {
                if (idx + 2 > pageUUID.Length)
                {
                    break;
                }
                var val = pageUUID.Substring(idx, 2);
                byte b = Convert.ToByte(val, 16);
                hexBytes[byteCounter] = b;
                byteCounter++;

            }
            return FolderConverter.ToBase32String(hexBytes);
        }

        public List<PageFolderLocations> FindProfileFolderActions(ProfileInfo profileInfo, int pageNum)
        {
            if (profileInfo.Pages.Pages.Count <= pageNum)
            {
                Console.WriteLine("Invalid page number");
                return null;
            }

            string pageUUID = profileInfo.Pages.Pages[pageNum];
            Page page = FindPage(profileInfo.FullPath, pageUUID);

            if (page == null)
            {
                Console.WriteLine("Failed to parse page");
                return null;
            }

            PageInfo pageInfo = page.Controllers.Where(c => c.Type == "Keypad").FirstOrDefault();
            if (pageInfo == null)
            {
                Console.WriteLine("Failed to parse page manifest");
                return null;
            }

            List<PageFolderLocations> pageFolderLocations = new List<PageFolderLocations>();

            foreach (var key in pageInfo.Actions.Keys)
            {
                if (pageInfo.Actions[key].UUID == FOLDER_UUID)
                {
                    string folderProfilePath = ExtractPageDirectory(pageInfo.Actions[key].Settings[ACTION_SETTINGS_FOLDER_UUID].Value<String>());
                    pageFolderLocations.Add(new PageFolderLocations(key, folderProfilePath));
                }
            }
            return pageFolderLocations;
        }
        

        
        public void MoveFolderBackLocation(ProfileInfo profileInfo, string pagePath, string folderLocation, string newLocation)
        {
            string manifestFile = Path.Combine(pagePath , MANINFEST_FILE);
            if (!File.Exists(manifestFile))
            {
                throw new FileNotFoundException(manifestFile);
            }

            JObject manifest = JObject.Parse(File.ReadAllText(manifestFile));
            bool found = false;
            foreach (var controller in manifest["Controllers"])
            {
                if (controller["Type"].Value<String>() != "Keypad")
                {
                    continue;
                }

                if (controller["Actions"][newLocation] == null) // New location doesn't have an existing key
                {
                    controller["Actions"][newLocation] = controller["Actions"]["0,0"];
                    controller["Actions"]["0,0"]?.Parent?.Remove();
                    found = true;
                }
                else // Does have an existing key, replace them
                {
                    var currPos = controller["Actions"][newLocation];
                    controller["Actions"][newLocation] = controller["Actions"]["0,0"];
                    controller["Actions"]["0,0"] = currPos;
                    found = true;
                }
            }

            if (found)
            {
                File.WriteAllText(manifestFile, manifest.ToString());
                Console.WriteLine("Done! Please restart the Stream Deck app to see the changes");
            }
            else
            {
                Console.WriteLine("Error parsing manifest file. Could not move folder.");
            }
        }
       

        private string GetProfileDir()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), WIN_PROFILE_LOCATION);
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return MAC_PROFILE_LOCATION; // Not tested
            }
            else
            {
                return null;
            }
        }
    }
}
