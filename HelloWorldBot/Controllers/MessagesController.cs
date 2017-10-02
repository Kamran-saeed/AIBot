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

                if(activity.Text == "programming")
                {
                    replyMessage.AttachmentLayout = AttachmentLayoutTypes.List;
                    replyMessage.Attachments = new List<Attachment>();

                    Dictionary<string, string> cardContentList = new Dictionary<string, string>();
                    cardContentList.Add("C++ 1", "https://img.wonderhowto.com/img/86/07/63568591614688/0/c-c-programming-for-hackers-part-1-introduction.1280x600.jpg");
                    cardContentList.Add("C++ 2", "https://www.eduonix.com/blog/wp-content/uploads/2016/02/c-17-02.jpg");

                    foreach (KeyValuePair<string, string> cardContent in cardContentList)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: cardContent.Value));

                        List<CardAction> cardButtons = new List<CardAction>();

                        CardAction plButton = new CardAction()
                        {
                            Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                            Type = "openUrl",
                            Title = "WikiPedia Page"
                        };

                        cardButtons.Add(plButton);

                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = $"I'm a thumbnail card about {cardContent.Key}",
                            Subtitle = $"{cardContent.Key} Wikipedia Page",
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyMessage.Attachments.Add(plAttachment);
                    }
                }

                if (activity.Text == "faizan")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent.flhe3-1.fna.fbcdn.net/v/t1.0-9/17351975_1121356034641436_9200264524077866380_n.jpg?oh=7f660947a9624b4d46508857c9bd4407&oe=5A7C9BD1",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                    replyMessage.Speak = "This is the text that will be spoken.";
                    replyMessage.InputHint = InputHints.AcceptingInput;
                }
                else if(activity.Text == "zeeshan")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent.flhe3-1.fna.fbcdn.net/v/t1.0-1/17634668_1195266220596549_4819124406029091190_n.jpg?oh=7d2c152d70420fb85040d83503f9b68c&oe=5A8719AC",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                }
                else if(activity.Text == "salman")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent.flhe3-1.fna.fbcdn.net/v/t1.0-9/20374476_1400090800081877_9126365518091107248_n.jpg?oh=66bd0d481a409745b14105b3eeb9117e&oe=5A5070F6",
                        ContentType = "image/png",
                        Name = "Bender_Rodriguez.png"
                    });
                }
                else if(activity.Text == "kamran")
                {
                    replyMessage.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "https://scontent.flhe3-1.fna.fbcdn.net/v/t1.0-9/17796070_1661165427227260_7279774805161231363_n.jpg?oh=59a6973ed4d03bde74a9933210482526&oe=5A3A93AB",
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