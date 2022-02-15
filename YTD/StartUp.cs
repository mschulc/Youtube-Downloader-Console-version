using System;
using System.IO;

namespace YTD
{
    public static class StartUp
    {
        public static void StartUP()
        {
            Downloader downloader = new Downloader();
            FileReader();
            Console.WriteLine();

            Console.WriteLine("YouTube Downloader MP4/MP3. Version v1.0. Type ytd --h or ytd -help for help.");
            var watchlink = @"https://www.youtube.com/watch?v=";

            var @command = @"";
            var quality = @"";
            var @link = @"";
            var @filePath = @"";
            do 
            {
                Console.Write(">: ");
                @command = Console.ReadLine();
                var commandSub = @"";
                if (command.Length > 6)
                {
                    @commandSub = command.Substring(0, 7);
                }

                if (command.Length >= 51)
                {
                    @link = command.Substring(8, 43);
                }
                if(command.Contains("-f"))
                {
                    int x = command.IndexOf("-f");
                    filePath = command.Substring(x+3);
                }
                if(command.Contains("-p"))
                {
                    int x = command.IndexOf("-f");
                    int y = command.IndexOf("-p");
                    int sub = command.Substring(y).Length;
                    int subL = command.Substring(x+4).Length;
                    filePath = command.Substring(x + 3,subL - sub);
                    quality = command.Substring(y + 3);
                }

                if (command.Equals("ytd --h") || command.Equals("ytd -help"))
                {
                    Console.WriteLine();
                    FileReaderHelp();
                }
                else if (commandSub.Equals("ytd --i"))
                {
                    if(link.StartsWith(watchlink) && link.Length == 43)
                    {
                        downloader.DownloadVideoData(link);
                    }
                    else
                    {
                        Console.WriteLine("Command is wrong. Type ytd --h or ytd -help for help.");
                        continue;
                    }
                }
                else if (commandSub.Equals("ytd --d") && command.Contains("-f"))
                {
                    if (link.StartsWith(watchlink) && link.Length == 43)
                    {
                        downloader.DownloadFast(link, filePath);
                    }
                    else
                    {
                        Console.WriteLine("Command is wrong. Type ytd --h or ytd -help for help.");
                        continue;
                    }
                }
                else if (commandSub.Equals("ytd -dv") && command.Contains("-f") && command.Contains("-p"))
                {
                    if (link.StartsWith(watchlink) && link.Length == 43)
                    {
                        downloader.DownloadVideo(link, filePath, quality);
                    }
                    else
                    {
                        Console.WriteLine("Command is wrong. Type ytd --h or ytd -help for help.");
                        continue;
                    }
                }
                else if (commandSub.Equals("ytd -dm") && command.Contains("-f"))
                {
                    if (link.StartsWith(watchlink) && link.Length == 43)
                    {
                        downloader.DownloadMP3(link, filePath);     
                    }
                    else
                    {
                        Console.WriteLine("Command is wrong. Type ytd --h or ytd -help for help.");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Command is wrong. Type ytd --h or ytd -help for help.");
                }
                
            } while(!command.Equals("ytd --exit"));
        }

        private static void FileReader()
        {
            var text = File.ReadAllText("welcomescreen.txt");
            Console.SetWindowSize(136, 36);
            for (int i = 0; i < text.Length; i++)
            {
                if (i < 1528)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(text[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(text[i]);
                }
            }
        }
        private static void FileReaderHelp()
        {
            var text = File.ReadAllText("help.txt");
            foreach (var item in text)
            {
                Console.Write(item);
            }
            Console.WriteLine();
        }
    }
}
