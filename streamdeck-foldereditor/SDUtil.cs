using streamdeck_foldereditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace streamdeck_foldereditor
{
    public enum StreamDeckType
    {
        Classic = 0,
        Mini = 1,
        XL = 2,
        Mobile = 3,
        CorsairGKeys = 4,
        StreamDeckPedal = 5,
        CorsairCueSDK = 6,
        StreamDeckPlus = 7,
        SCUFController = 8,
        StreamDeckNeo = 9,
        UNKNOWN = 999
    }

    internal static class SDUtil
    {
        public static StreamDeckType GetStreamDeckTypeFromProfile(ProfileInfo profileInfo)
        {
            switch (profileInfo.Device.Model)
            {
                case "20GAA9901":
                case "20GAA9902":
                    return StreamDeckType.Classic;
                case "20GAT9901":
                case "20GAT9902":
                    return StreamDeckType.XL;
                case "20GAI9901":
                    return StreamDeckType.Mini;
                case "20GBD9901":
                    return StreamDeckType.StreamDeckPlus;
                case "20GBJ9901":
                    return StreamDeckType.StreamDeckNeo;
                case "VSD/WiFi":
                    return StreamDeckType.Mobile;
                default:
                    return StreamDeckType.UNKNOWN;
            }
        }

        public static int GetColumnsForStreamDeckType(StreamDeckType streamDeckType)
        {
            switch (streamDeckType)
            {
                case StreamDeckType.Classic:
                    return 5;
                case StreamDeckType.Mini:
                    return 3;
                case StreamDeckType.CorsairGKeys:
                    return 1;
                case StreamDeckType.Mobile:
                case StreamDeckType.XL:
                    return 8;
                case StreamDeckType.StreamDeckPlus:
                    return 4;
                case StreamDeckType.StreamDeckNeo:
                    return 4;
            }
            return 0;
        }

        public static int GetRowsForStreamDeckType(StreamDeckType streamDeckType)
        {
            switch (streamDeckType)
            {
                case StreamDeckType.Classic:
                    return 3;
                case StreamDeckType.Mini:
                    return 2;
                case StreamDeckType.CorsairGKeys:
                    return 6;
                case StreamDeckType.Mobile:
                case StreamDeckType.XL:
                    return 4;
                case StreamDeckType.StreamDeckPlus:
                    return 2;
                case StreamDeckType.StreamDeckNeo:
                    return 2;
            }
            return 0;
        }

        public static void DisplayKeyLayout(StreamDeckType streamDeckType)
        {
            int rows = GetRowsForStreamDeckType(streamDeckType);
            int cols = GetColumnsForStreamDeckType(streamDeckType);
            Console.WriteLine("---------- STREAM DECK LAYOUT ----------");
            for (int idx = 0; idx < rows; idx++)
            {
                for (int idx2 = 0; idx2 < cols; idx2++)
                {
                    Console.Write($"[{idx2},{idx}]  ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("----------------------------------------");
        }
    }
}
