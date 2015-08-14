using ServiceCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;

namespace CortanaBackgroundTask
{
    public sealed class VoiceCommandService : IBackgroundTask
    {
        VoiceCommandServiceConnection voiceServiceConnection;
        BackgroundTaskDeferral serviceDeferral;
        ResourceMap cortanaResourceMap;
        ResourceContext cortanaContext;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var client = this.Client;

            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCancelled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            cortanaResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            cortanaContext = ResourceContext.GetForViewIndependentUse();

            if (triggerDetails != null && triggerDetails.Name == "VoiceCommandService")
            {
                try
                {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    // Depending on the operation (defined in the VoiceCommands.xml file)
                    // perform the appropriate command.
                    switch (voiceCommand.CommandName)
                    {
                        case "turnOnItem":
                            var onTarget = voiceCommand.Properties["target"][0];
                            await SendCompletionMessageForOnOff(client, onTarget, true);
                            break;
                        case "turnOffItem":
                            var offTarget = voiceCommand.Properties["target"][0];
                            await SendCompletionMessageForOnOff(client, offTarget, false);
                            break;
                        default:
                            // As with app activation VCDs, we need to handle the possibility that
                            // an app update may remove a voice command that is still registered.
                            // This can happen if the user hasn't run an app since an update.
                            //LaunchAppInForeground();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
                }
            }
        }

        private async Task SendCompletionMessageForOnOff(RelayNodeClient client, string target, bool turnItOn)
        {
            var userMessage = new VoiceCommandUserMessage();

            if (!client.IsReady)
            {
                string noController = string.Format(
                                       cortanaResourceMap.GetValue("noControllerFound", cortanaContext).ValueAsString,
                                       target);
                userMessage.DisplayMessage = noController;
                userMessage.SpokenMessage = noController;
            }
            else
            {
                int relayId = 0;
                switch (target)
                {
                    default:
                    case "light":
                    case "lamp":
                        relayId = 0;
                        break;
                    case "fan":
                        relayId = 1;
                        break;
                }

                if (turnItOn)
                {
                    client.SetRelay(relayId, true);
                    string turnedOn = string.Format(
                                           cortanaResourceMap.GetValue("turnedOnMessage", cortanaContext).ValueAsString,
                                           target);
                    userMessage.DisplayMessage = turnedOn;
                    userMessage.SpokenMessage = turnedOn;
                }
                else
                {
                    client.SetRelay(relayId, false);
                    string turnedOn = string.Format(
                                           cortanaResourceMap.GetValue("turnedOnMessage", cortanaContext).ValueAsString,
                                           target);
                    userMessage.DisplayMessage = turnedOn;
                    userMessage.SpokenMessage = turnedOn;
                }
            }

            //var response = VoiceCommandResponse.CreateResponse(userMessage, destinationsContentTiles);
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }

        private void OnTaskCancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }

        private RelayNodeClient _client;
        private RelayNodeClient Client
        {
            get
            {
                if (_client==null)
                {
                    _client = new RelayNodeClient();
                }
                return _client;
            }
        }
    }
}
