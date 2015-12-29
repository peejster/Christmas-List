using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChristmasList
{

    public sealed partial class MainPage : Page
    {
        private SpeechRecognizer itemRecognizer;

        private SpeechRecognizer nameRecognizer;

        // Define constraint list
        IEnumerable<string> listOfNames = new List<string>() { "Prasantha", "Michele", "Lotus" };

        // Keep track of whether the recognizer is currently listening for user input
        private bool isListening = false;

        // Keep track of who is making the request
        private string requestor = null;

        // Keep track of all the requested items
        private List<string> requestedItems = new List<string>();

        public MainPage()
        {
            this.InitializeComponent();

            Unloaded += MainPage_Unloaded;

            initializeSpeechRecognizer();
        }

        // Initialize Speech Recognizer
        private async void initializeSpeechRecognizer()
        {
            // Initialize name recognizer
            nameRecognizer = new SpeechRecognizer();

            // Create list constraint
            SpeechRecognitionListConstraint listConstraint = new SpeechRecognitionListConstraint(listOfNames);

            // Add list constraint and compile
            nameRecognizer.Constraints.Add(listConstraint);
            SpeechRecognitionCompilationResult nameResult = await nameRecognizer.CompileConstraintsAsync();

            if (nameResult.Status != SpeechRecognitionResultStatus.Success)
            {
                listenButton.IsEnabled = false;
                return;
            }

            // Initialize item recognizer
            itemRecognizer = new SpeechRecognizer();

            // Create topic constraint
            SpeechRecognitionTopicConstraint topicConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "Development");

            // Add topic constraint and compile
            itemRecognizer.Constraints.Add(topicConstraint);
            SpeechRecognitionCompilationResult itemresult = await itemRecognizer.CompileConstraintsAsync();

            if (itemresult.Status != SpeechRecognitionResultStatus.Success)
            {
                listenButton.IsEnabled = false;
                return;
            }
        }

        // Start listening when the button is clicked
        void listenButton_Click(object sender, RoutedEventArgs e)
        {
            TextToSpeech.Speak("What is your name?");
            Task.Delay(4000).Wait();

            RecognizeName();
        }

        // Start the recognizer to listen for the requestor's name
        private void RecognizeName()
        {
            if (isListening == false)
            {
                isListening = true;

                var nameRecognition = nameRecognizer.RecognizeAsync();
                nameRecognition.Completed += this.NameRecognitionCompletedHandler;
            }

        }

        // Start the recognizer to listen for christmas list items
        private void RecognizeChristmasListItem()
        {
            if (isListening == false)
            {
                isListening = true;

                var itemRecognition = itemRecognizer.RecognizeAsync();
                itemRecognition.Completed += this.ItemRecognitionCompletedHandler;
            }
        }

        // Event handler when name recognition is completed
        private void NameRecognitionCompletedHandler(IAsyncOperation<SpeechRecognitionResult> asyncInfo, AsyncStatus asyncStatus)
        {
            isListening = false;

            var results = asyncInfo.GetResults();
            if (results.Status != SpeechRecognitionResultStatus.Success)
            {
                TextToSpeech.Speak("Sorry, I wasn't able to hear you. Try again later.");
                return;
            }

            if (results.Confidence == SpeechRecognitionConfidence.High | results.Confidence == SpeechRecognitionConfidence.Medium)
            {
                requestor = results.Text;
                if (requestor == listOfNames.First())
                {
                    if (requestedItems.Count == 0)
                    {
                        TextToSpeech.Speak("Hello" + requestor + "\n" + "The list is empty.");
                    }
                    else
                    {
                        TextToSpeech.Speak("Hello" + requestor + "\n" + "Here is the list");
                        foreach (string item in requestedItems)
                        {
                            Task.Delay(4000).Wait();
                            TextToSpeech.Speak(item);
                        }
                    }
                }
                else
                {
                    TextToSpeech.Speak("Hello" + requestor + "\n" + "What would you like for Christmas?");
                    Task.Delay(4000).Wait();
                    RecognizeChristmasListItem();
                }
            }
            else
            {
                TextToSpeech.Speak("Sorry, I do not recognize you.");
            }

        }

        // Event handler when item recognition is completed
        private void ItemRecognitionCompletedHandler(IAsyncOperation<SpeechRecognitionResult> asyncInfo, AsyncStatus asyncStatus)
        {
            isListening = false;

            var results = asyncInfo.GetResults();
            if (results.Status != SpeechRecognitionResultStatus.Success)
            {
                TextToSpeech.Speak("Sorry, I wasn't able to hear you. Try again later.");
                return;
            }

            if (results.Confidence == SpeechRecognitionConfidence.High | results.Confidence == SpeechRecognitionConfidence.Medium)
            {
                requestedItems.Add(requestor + " wants " + results.Text);
                TextToSpeech.Speak(results.Text + "\n" + "Got it. I have added it your Christmas list" + requestor);
            }
            else
            {
                TextToSpeech.Speak("Sorry, I did not get that.");
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.itemRecognizer != null)
            {
                this.itemRecognizer.Dispose();
                this.itemRecognizer = null;
            }
        }
    }
}
