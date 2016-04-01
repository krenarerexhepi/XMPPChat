
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Matrix.Xmpp.Client;
using Matrix.Xmpp;

namespace XMPPChatXamarin
{
	[Activity (Label = "ChatActivity")]			
	public class ChatActivity : Activity
	{
		XmppClient logedUserData;
		PresenceManager pm;
		string username;
		EditText edtSendMessage;

		public ChatActivity ()
		{
		}

		public ChatActivity (string username)
		{
			username = username;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Chat);
			String myValue = this.Intent.GetStringExtra ("username");

			// Create your application here
			logedUserData = XmppService.Instance ().getConnection ();

			logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnPresence);
			logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (presenceManager_OnSubscribe);
			logedUserData.OnBeforeSendPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnBeforeSendPresence);
			logedUserData.OnMessage +=	xmppClient_OnMessage;

			Button sendMessage = FindViewById<Button> (Resource.Id.btnSendMessage);
			sendMessage.Click += new EventHandler (send_message_Click);

			edtSendMessage = FindViewById<EditText> (Resource.Id.edtsendmessage); 


		}

		void send_message_Click (object sender, System.EventArgs e)
		{	
			if (!String.IsNullOrEmpty (edtSendMessage.Text)) {

				var msg = new Matrix.Xmpp.Client.Message {
					Type = MessageType.chat,
					To = username + "@" + Constants.XmppDomain,
					Body = edtSendMessage.Text
				};
				logedUserData.Send (msg);
			} else {
				Toast.MakeText (this, "Plotësoni të dhënat!!", ToastLength.Long).Show ();
				return;

			}
		}

		void xmppClient_OnBeforeSendPresence (object sender, PresenceEventArgs e)
		{
			Console.WriteLine ("Before presence : " + e.Presence);
		}

		void xmppClient_OnPresence (object sender, PresenceEventArgs e)
		{
			// we see who is online 
			Console.WriteLine (string.Format ("OnPresence from {0}", e.Presence.From));
			Console.WriteLine (string.Format ("Status {0}", e.Presence.Status));
			Console.WriteLine (string.Format ("Show type {0}", e.Presence.Show));
			Console.WriteLine (string.Format ("Priority {0}", e.Presence.Priority));

		}

		private void xmppClient_OnMessage (object sender, MessageEventArgs e)
		{
			List<string> recived_Message = new List<string> ();

			recived_Message.Add (string.Format ("OnMessage from {0}", e.Message.From));
			recived_Message.Add (string.Format ("Body {0}", e.Message.Body));
			recived_Message.Add (string.Format ("Type {0}", e.Message.Type));

			if (e.Message.Body == null)
				return;

			string text = "";
			RunOnUiThread (delegate {			
				text = GetString (Resource.String.Message_from) + e.Message.From + "\r\n";
				text += e.Message.Body;
				Toast.MakeText (this, text, ToastLength.Long).Show ();
			});

		}

		private void presenceManager_OnSubscribe (object sender, PresenceEventArgs e)
		{
			if (pm != null) {
				pm.ApproveSubscriptionRequest (e.Presence.From);
			}
		}
	}
}

