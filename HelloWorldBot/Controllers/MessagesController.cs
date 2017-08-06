using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Text;
using System.Collections.Generic;

namespace HelloWorldBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            bool boolAskedForUserName = false;
            string strUserName = "";
            if (activity.Type == ActivityTypes.Message)
            {
                // Get any saved values
                StateClient sc = activity.GetStateClient();
                BotData userData = sc.BotState.GetPrivateConversationData(
                    activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                boolAskedForUserName = userData.GetProperty<bool>("AskedForUserName");
                strUserName = userData.GetProperty<string>("UserName") ?? "";
                // Create text for a reply message   
                StringBuilder strReplyMessage = new StringBuilder();
                if (boolAskedForUserName == false) // Never asked for name
                {
                    strReplyMessage.Append($"Hello, I am Chat Bot");
                    strReplyMessage.Append($"\n");
                    strReplyMessage.Append($"You can say anything");
                    strReplyMessage.Append($"\n");
                    strReplyMessage.Append($"to me and I will repeat it back");
                    strReplyMessage.Append($"\n\n");
                    strReplyMessage.Append($"So, what is your name?");
                    // Set BotUserData
                    userData.SetProperty<bool>("AskedForUserName", true);
                }
                else // Have asked for name
                {
                    if (strUserName == "") // Name was never provided
                    {
                        // If we have asked for a username but it has not been set
                        // the current response is the user name
                        strReplyMessage.Append($"Hello **{activity.Text}!**");
                        // Set BotUserData
                        userData.SetProperty<string>("UserName", activity.Text);
                    }
                    else // Name was provided
                    {
                        strReplyMessage.Append($"**{strUserName}**, You said: {activity.Text}");
                    }
                }
                // Save BotUserData
                sc.BotState.SetPrivateConversationData(
                    activity.ChannelId, activity.Conversation.Id, activity.From.Id, userData);
                // Create a reply message
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity replyMessage = activity.CreateReply(strReplyMessage.ToString());

                if (activity.Text == "adil")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent-cdg2-1.xx.fbcdn.net/v/t1.0-9/18582305_1472101472854297_6702920825411222930_n.jpg?oh=ddb234e3ff5e5abc4ec9498b92e95173&oe=59EACA30",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                    replyMessage.Speak = "This is the text that will be spoken.";
                    replyMessage.InputHint = InputHints.AcceptingInput;
                }
                else if(activity.Text == "abdul karim")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent-cdg2-1.xx.fbcdn.net/v/t1.0-9/18698133_1828537310493504_577993740993653688_n.jpg?oh=becdfc1a250e8abad47cc975517b61e2&oe=59FC2DBC",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                }
                else if(activity.Text == "abdul hanan")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent-cdg2-1.xx.fbcdn.net/v/t1.0-9/14369870_683436315146860_453917732903214659_n.jpg?oh=aef253fdae006076b361fcd60836bc8c&oe=5A27FCF1",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                }
                else if(activity.Text == "fasih haider")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent-cdg2-1.xx.fbcdn.net/v/t1.0-9/15781407_1053655681412609_5793125683587162591_n.jpg?oh=2099fef0e1d72a2963d4aa2055ac9ce0&oe=5A264125",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                }
                else if(activity.Text == "shaheryar imtiaz")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent-cdg2-1.xx.fbcdn.net/v/t1.0-9/14485125_530572040470935_4023836458597440650_n.jpg?oh=1705bf2b82143e039cec73f0a7f83e76&oe=59F69C45",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                }
                await connector.Conversations.ReplyToActivityAsync(replyMessage);
            }
            else
            {
                Activity replyMessage = HandleSystemMessage(activity);
                if (replyMessage != null)
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    await connector.Conversations.ReplyToActivityAsync(replyMessage);
                }
            }
            // Return response
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Get BotUserData
                StateClient sc = message.GetStateClient();
                BotData userData = sc.BotState.GetPrivateConversationData(
                    message.ChannelId, message.Conversation.Id, message.From.Id);
                // Set BotUserData
                userData.SetProperty<string>("UserName", "");
                userData.SetProperty<bool>("AskedForUserName", false);
                // Save BotUserData
                sc.BotState.SetPrivateConversationData(
                    message.ChannelId, message.Conversation.Id, message.From.Id, userData);
                // Create a reply message
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                Activity replyMessage = message.CreateReply("Personal data has been deleted.");
                return replyMessage;
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}