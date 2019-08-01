using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.Media;
using Android.Widget;
using Xamarin.Forms;

namespace AudioRecorderSample
{
    
	public partial class MainPage : ContentPage
    {
        MediaPlayer player;
        MediaRecorder recorder;
        string filePath = "";

        bool isTimerRunning  = false;
        int  seconds         = 0, minutes = 0;
        public MainPage()
        {
            InitializeComponent();            
        }

        //void Finish_Playing(object sender, EventArgs e)
        //{
        //    bntRecord.IsEnabled = true;
        //    bntRecord.BackgroundColor = Color.FromHex("#7cbb45");
        //    bntPlay.IsEnabled = true;
        //    bntPlay.BackgroundColor = Color.FromHex("#7cbb45");
        //    bntStop.IsEnabled = false;
        //    bntStop.BackgroundColor = Color.Silver;
        //    lblSeconds.Text = "00";
        //    lblMinutes.Text = "00";
        //}

        async void Send_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (WebClient client = new WebClient())
                {

                    var responceArray = client.UploadFile("http://195.70.203.43:8777/predict", filePath);
                    var text = new string(System.Text.Encoding.UTF8.GetChars(responceArray));
                    await DisplayAlert("Результат распознавания:", text, "Ок");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
        async void Record_Clicked(object sender, EventArgs e)
        {
            seconds = 0;
            minutes = 0;
            isTimerRunning = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                seconds++;
                   
                if (seconds.ToString().Length == 1)
                {
                    lblSeconds.Text = "0" + seconds.ToString();
                }
                else
                {
                    lblSeconds.Text = seconds.ToString();
                }
                if (seconds == 60)
                {
                    minutes++;
                    seconds = 0;

                    if (minutes.ToString().Length == 1)
                    {
                        lblMinutes.Text = "0" + minutes.ToString();
                    }
                    else
                    {
                        lblMinutes.Text = minutes.ToString();
                    }

                    lblSeconds.Text = "00";
                }
                return isTimerRunning;
            });

            //
            filePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath.ToString() + "/" + Guid.NewGuid().ToString("N") + "_audio.3gp";
            SetupMediaRecorder();
            try
            {
                recorder.Prepare();
                recorder.Start();
            }
            catch (Exception ex)
            {
                
            }
            recordButton.IsEnabled = false;
            recordButton.BackgroundColor = Color.Silver;
            playButton.IsEnabled = false;
            playButton.BackgroundColor = Color.Silver;
            stopRecordButton.IsEnabled = true;
            stopRecordButton.BackgroundColor = Color.FromHex("#7cbb45");
        }

        public void SetupMediaRecorder()
        {
            recorder = new MediaRecorder();
            recorder.SetAudioSource(AudioSource.Mic);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            recorder.SetOutputFile(filePath);
        }

        async void Stop_Clicked(object sender, EventArgs e)
        {
            StopRecording();
            recorder.Stop();            
        } 

        void StopRecording()
        {
            isTimerRunning = false;
            recordButton.IsEnabled = true;
            recordButton.BackgroundColor = Color.FromHex("#7cbb45");
            playButton.IsEnabled = true;
            playButton.BackgroundColor = Color.FromHex("#7cbb45");
            stopRecordButton.IsEnabled = false;
            stopRecordButton.BackgroundColor = Color.Silver;
            lblSeconds.Text = "00";
            lblMinutes.Text = "00";
        }
        void Play_Clicked(object sender, EventArgs e)
        {
            lblSeconds.Text = "00";
            lblMinutes.Text = "00";
            try
            {
                player = new MediaPlayer();
                try
                {
                    player.SetDataSource(filePath);
                    player.Prepare();
                }
                catch (Exception ex)
                {
                    
                }

                player.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }
          
    }
     
}
