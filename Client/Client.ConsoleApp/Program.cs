using NAudio.Wave;
using NAudio.Wave.SampleProviders;
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

            var outRate = 8000;
            var chennelsNumber = 1;
            var outFormat = new WaveFormat(outRate, chennelsNumber);

            using (var waveIn = new WaveInEvent())
            using (var writer = new WaveFileWriter(outputFilePath, outFormat))
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

            Console.WriteLine("Resampling started...");
            var resampledFilePath = Path.Combine(outputFolder, "resampled_audio.wav");
            WDLResample(outputFilePath, 16000, resampledFilePath);
            Console.WriteLine("Resampling ended.");

            using (WebClient client = new WebClient())
            {
                var responceArray = client.UploadFile("http://localhost:8000/predict", resampledFilePath);
                var text = System.Text.Encoding.UTF8.GetChars(responceArray);
                Console.WriteLine(text);
            }            
            Console.ReadLine();
        }

        static void MFResample(string inFile, int outRate, string outFile)
        {
            using (var reader = new WaveFileReader(inFile))
            {
                var resampleOutFormat = new WaveFormat(outRate, 1);
                using (var resampler = new MediaFoundationResampler(reader, resampleOutFormat))
                {
                    // resampler.ResamplerQuality = 60;
                    WaveFileWriter.CreateWaveFile(outFile, resampler);
                }
            }
        }

        static void WDLResample(string inFile, int outRate, string outFile)
        {
            using (var reader = new AudioFileReader(inFile))
            {
                var resampler = new WdlResamplingSampleProvider(reader, outRate);
                WaveFileWriter.CreateWaveFile16(outFile, resampler);
            }
        }

        static void ACMResample(string inFile, int outRate, string outFile)
        {
            using (var reader = new WaveFileReader(inFile))
            {
                var outFormat = new WaveFormat(outRate, reader.WaveFormat.Channels);
                using (var resampler = new WaveFormatConversionStream(outFormat, reader))
                {
                    WaveFileWriter.CreateWaveFile(outFile, resampler);
                }
            }
        }
    }
}
