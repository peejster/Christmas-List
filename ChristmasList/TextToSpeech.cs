using System;
using System.Diagnostics;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace ChristmasList
{
    public class TextToSpeech
    {
        public static async void Speak(string text)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _Speak(text);
            }
            );
        }

        private static async void _Speak(string text)
        {
            MediaElement mediaElement = new MediaElement();
            SpeechSynthesizer synth = new SpeechSynthesizer();


            foreach (VoiceInformation voice in SpeechSynthesizer.AllVoices)
            {
                Debug.WriteLine(voice.DisplayName + ", " + voice.Description);
            }

            // Initialize a new instance of the SpeechSynthesizer. 
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(text);

            // Send the stream to the media object. 
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();

            mediaElement.Stop();
            synth.Dispose();
        }

    }
}
