using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Client.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DeepSpeechAudio");
            Directory.CreateDirectory(outputFolder);
            var outputFilePath = Path.Combine(outputFolder, "audio.wav");

            using (var waveIn = new WaveInEvent())
            using (var writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat))
            {
                waveIn.DataAvailable += (s, a) =>
                {
                    writer.Write(a.Buffer, 0, a.BytesRecorded);
                };

                Console.WriteLine("Press any key to start recording...");
                Console.ReadLine();

                waveIn.StartRecording();

                Console.WriteLine("Recording... \n");
                Console.WriteLine("Press any key to stop recording...");
                Console.ReadLine();

                waveIn.StopRecording();                
            }

            Console.WriteLine($"Audio was written to {outputFilePath}.\nPress any key to continue...");
            Console.ReadLine();

            using (WebClient client = new WebClient())
            {
                var responceArray = client.UploadFile("http://localhost:8000/predict", outputFilePath);
                var text = System.Text.Encoding.UTF8.GetChars(responceArray);
                Console.WriteLine(text);
            }            
            Console.ReadLine();
        }
    }
}
