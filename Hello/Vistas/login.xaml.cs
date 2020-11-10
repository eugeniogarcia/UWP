using Hello.Models;
using Hello.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Hello.Vistas
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class login : Page
    {
        private Account _account;

        public login()
        {
            this.InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            SignInFingerprint();
        }


        public async System.Threading.Tasks.Task<string> CheckFingerprintAvailability()
        {
            string returnMessage = "";

            try
            {
                // Check the availability of fingerprint authentication.
                var ucvAvailability = await Windows.Security.Credentials.UI.UserConsentVerifier.CheckAvailabilityAsync();

                switch (ucvAvailability)
                {
                    case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.Available:
                        //returnMessage = "Fingerprint verification is available.";
                        returnMessage = "";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DeviceBusy:
                        returnMessage = "Biometric device is busy.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DeviceNotPresent:
                        returnMessage = "No biometric device found.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DisabledByPolicy:
                        returnMessage = "Biometric verification is disabled by policy.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.NotConfiguredForUser:
                        returnMessage = "The user has no fingerprints registered. Please add a fingerprint to the " +
                                        "fingerprint database and try again.";
                        break;
                    default:
                        returnMessage = "Fingerprints verification is currently unavailable.";
                        break;
                }
            }
            catch (Exception ex)
            {
                returnMessage = "Fingerprint authentication availability check failed: " + ex.ToString();
            }

            return returnMessage;
        }


        private async System.Threading.Tasks.Task<string> RequestConsent(string userMessage)
        {
            string returnMessage;

            if (String.IsNullOrEmpty(userMessage))
            {
                userMessage = "Please provide fingerprint verification.";
            }

            try
            {
                // Request the logged on user's consent via fingerprint swipe.
                var consentResult = await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync(userMessage);

                switch (consentResult)
                {
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                        returnMessage = "Fingerprint verified.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                        returnMessage = "Biometric device is busy.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                        returnMessage = "No biometric device found.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                        returnMessage = "Biometric verification is disabled by policy.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                        returnMessage = "The user has no fingerprints registered. Please add a fingerprint to the " +
                                        "fingerprint database and try again.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                        returnMessage = "There have been too many failed attempts. Fingerprint authentication canceled.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Canceled:
                        returnMessage = "Fingerprint authentication canceled.";
                        break;
                    default:
                        returnMessage = "Fingerprint authentication is currently unavailable.";
                        break;
                }
            }
            catch (Exception ex)
            {
                returnMessage = "Fingerprint authentication failed: " + ex.ToString();
            }

            return returnMessage;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Check Microsoft Passport is setup and available on this machine
            String resp = await CheckFingerprintAvailability();
            if (resp=="")
            {
            }
            else
            {
                // Microsoft Passport is not setup so inform the user
                FigerprintStatus.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 170, 207));
                FigerprintStatusText.Text = resp;
            }
        }

        private async void SignInFingerprint()
        {
            if (AccountHelper.ValidateAccountCredentials(UsernameTextBox.Text))
            {
                // Create and add a new local account
                _account = AccountHelper.AddAccount(UsernameTextBox.Text);
                Debug.WriteLine("Successfully signed in with traditional credentials and created local account instance!");
            }
            else
            {
                if (UsernameTextBox.Text == "")
                {
                    ErrorMessage.Text = await RequestConsent("Probando con huella!!");
                }
                else
                {
                    ErrorMessage.Text = "Invalid Credentials";
                }
            }


        }
    }
}
