using FFMpegCore;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using VideoLibrary;

namespace YTD
{
    public class Downloader
    {
        public void DownloadVideoData(string link)
        {
            List<int> videoQuality = new List<int>();
            var yt = YouTube.Default;
            var videoData = yt.GetAllVideos(link);
            var resolution = videoData.Where(x => x.AdaptiveKind == AdaptiveKind.Video && x.Format == VideoFormat.Mp4).
                Select(x => x.Resolution);
            var target = videoData.FirstOrDefault(x => x.AdaptiveKind == AdaptiveKind.Video);
            foreach (var item in resolution)
            {
                if(!videoQuality.Contains(item))
                    videoQuality.Add(item);
            }
            videoQuality.Sort();
            Console.WriteLine("Video quality options:");
            Console.Write("[ ");
            foreach (var item in videoQuality)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine("]");
            Console.WriteLine("Title: "+ target.Info.Title);
            Console.WriteLine("Format: " + target.Format);
        }

        public void DownloadMP3(string link, string filePath)
        {
            var yt = YouTube.Default;
            var videoTemp = "video.mp4";
            var video = yt.GetVideo(link);
            string uri = video.GetUriAsync().Result;
            Console.WriteLine("Downloading audio...");
            var name = video.FullName.Substring(0, video.FullName.Length - 4);
            var path = filePath + name + ".mp3";
            Download(uri, videoTemp);
            FFMpeg.ExtractAudio(videoTemp, path);
            FileSystem.DeleteFile(videoTemp);
            Console.WriteLine("\nComplited");
        }
        public void DownloadFast(string link, string filePath)
        {
            var yt = YouTube.Default;
            var video = yt.GetVideo(link);
            string uri = video.GetUriAsync().Result;
            string path = filePath + video.FullName;
            Console.WriteLine("Downloading video...");
            Download(uri, path);
            Console.WriteLine("\nComplited");
        }

        public void DownloadVideo(string link, string filePath, string videoQuality)
        {
            var audioTemp = "audio.mp3";
            var videoTemp = "video.mp4";
            var yt = YouTube.Default;
            var video = yt.GetAllVideos(link);
            var targetAudio = video.FirstOrDefault(x => x.AdaptiveKind == AdaptiveKind.Audio);
            var targetVideo = video.FirstOrDefault(x => x.Resolution.ToString() == videoQuality && x.AdaptiveKind == AdaptiveKind.Video);
            string audioUri = targetAudio.GetUriAsync().Result;
            string videoUri = targetVideo.GetUriAsync().Result;
            Console.WriteLine("Downloading Audio...");
            Download(audioUri, audioTemp);
            Console.WriteLine("\nDownloading Video...");
            Download(videoUri, videoTemp);
            Console.WriteLine("\nProcessing. Please wait...");
            FFMpeg.ReplaceAudio(videoTemp, audioTemp, filePath + targetVideo.FullName);
            FileSystem.DeleteFile(audioTemp);
            FileSystem.DeleteFile(videoTemp);
            Console.WriteLine("Complited");

        }
        public static void ProgressBar(double percentage)
        {
            char[] progressBar =
            {
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' ',
                ' ', ' ', ' ', ' ', ' '
            };
            Console.Write("[");
            for (int i = 0; i < percentage / 2; i++)
            {
                progressBar[i] = '#';
            }
            foreach (var item in progressBar)
            {
                Console.Write(item);
            }
            Console.Write("]");
            Console.Write(" " + percentage + "%");

        }

        public static void Download(string uri, string fileName)
        {
            int bytesProcessed = 0;
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;
            
            try
            {
                WebRequest request = WebRequest.Create(uri);
                if(request != null)
                {
                    double totalBytes = 0;
                    var sizeWebRequest = HttpWebRequest.Create(new Uri(uri));
                    sizeWebRequest.Method = "HEAD";

                    using (var webResponse = sizeWebRequest.GetResponse())
                    {
                        var fileSize = webResponse.Headers.Get("Content-Length");
                        totalBytes = Convert.ToDouble(fileSize);
                    }

                    response = request.GetResponse();
                    if (response != null)
                    {
                        remoteStream = response.GetResponseStream();

                        localStream = File.Create(fileName);

                        byte[] buffer = new byte[1024];
                        int bytesRead = 0;
                        var position = Console.GetCursorPosition();
                        do
                        {

                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                            localStream.Write(buffer, 0, bytesRead);
                            bytesProcessed += bytesRead;
                            double bytesIn = double.Parse(bytesProcessed.ToString());
                            double percentage = bytesIn / totalBytes * 100;
                            percentage = Math.Round(percentage,0);    
                            Console.SetCursorPosition(position.Left, position.Top);
                            Console.CursorVisible = false;
                            ProgressBar(percentage);


                        } while (bytesRead > 0);
                        Console.CursorVisible = true;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("There is not file directory like this. Try save the file to anotherone.");
            }
            catch(Exception e)
            {
                Console.WriteLine("Something gone wrong. Try again or contact with us");
            }
            finally
            {
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }
        }
    }
}
