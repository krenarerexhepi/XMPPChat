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
using Matrix;
using Matrix.Xmpp;
using Matrix.Xmpp.Sasl;

namespace XMPPChatXamarin
{
	[Activity (Label = "MainFormActivity")]			
	public class MainFormActivity : Activity
	{
		Constants con = new Constants ();
		String username;
		EditText edtStatus;
		XmppClient logedUserData;
		EditText edtAddContact;
		EditText usernamemessage;
		EditText message;
		PresenceManager pm;
		int status;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.MainForm);
			// Create your application here

			username = Intent.GetStringExtra ("Username");
			edtStatus = FindViewById<EditText> (Resource.Id.edtStatus);
			edtAddContact = FindViewById<EditText> (Resource.Id.edtAddContact);

			usernamemessage = FindViewById<EditText> (Resource.Id.sendMessageToUser);
			message = FindViewById<EditText> (Resource.Id.edtsendmessage);


			Spinner spinner = FindViewById<Spinner> (Resource.Id.spinner1);
			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner1_ItemSelected);

			List<String> list = new List<String> ();
			list.Add ("Online");
			list.Add ("Away");
			list.Add ("On the phone");
			list.Add ("Do not disturb");
			list.Add ("Invisible");
			ISpinnerAdapter lista = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleDropDownItem1Line, list);
			spinner.Adapter = lista;

			Button saveStatus = FindViewById<Button> (Resource.Id.btnSaveStatus);
			saveStatus.Click += new EventHandler (btnSaveStatus_Click);

			Button addContact = FindViewById<Button> (Resource.Id.btnAddContact);
			addContact.Click += new EventHandler (addContact_click);

			Button logOut = FindViewById<Button> (Resource.Id.btnLogOut);
			logOut.Click += new EventHandler (btnLogOut_Click);

			Button sendMessage = FindViewById<Button> (Resource.Id.btnSendMessage);
			sendMessage.Click += new EventHandler (send_message_Click);


			logedUserData = XmppService.Instance ().getConnection ();

			logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnPresence);
			logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (presenceManager_OnSubscribe);
			logedUserData.OnBeforeSendPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnBeforeSendPresence);
			logedUserData.OnMessage +=	xmppClient_OnMessage;

		}


		void send_message_Click (object sender, System.EventArgs e)
		{	
			if (!String.IsNullOrEmpty (usernamemessage.Text) && !String.IsNullOrEmpty (message.Text)) {
				
				var msg = new Matrix.Xmpp.Client.Message {
					Type = MessageType.chat,
					To = usernamemessage.Text + "@" + Constants.XmppDomain,
					Body = message.Text
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

		void btnSaveStatus_Click (object sender, System.EventArgs e)
		{	

			// to do: te regullohet statusi , presenca eshte mire 
			logedUserData = XmppService.Instance ().getConnection ();
			logedUserData.OnBeforeSendPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnBeforeSendPresence);
			if (status == 0) {
				// set status online in client side
				logedUserData.Status = edtStatus.Text;
				logedUserData.SendPresence (Matrix.Xmpp.Show.chat);
			} else if (status == 1) {	
				// set status away in client side
				logedUserData.Status = edtStatus.Text;
				logedUserData.SendPresence (Matrix.Xmpp.Show.away);
			} else if (status == 2) {
				// set status on the phone in client side
				logedUserData.Status = edtStatus.Text;
				logedUserData.SendPresence (Matrix.Xmpp.Show.xa);
			} else if (status == 3) {
				// set status Do not disturb in client side
				logedUserData.Status = edtStatus.Text;
				logedUserData.SendPresence (Matrix.Xmpp.Show.dnd);
			} else if (status == 4) {
				logedUserData.SendUnavailablePresence ();
			}

		}

		private void spinner1_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			status = e.Position;
		}

		void xmppClient_OnPresence (object sender, PresenceEventArgs e)
		{
			// we see who is online 
			Console.WriteLine (string.Format ("OnPresence from {0}", e.Presence.From));
			Console.WriteLine (string.Format ("Status {0}", e.Presence.Status));
			Console.WriteLine (string.Format ("Show type {0}", e.Presence.Show));
			Console.WriteLine (string.Format ("Priority {0}", e.Presence.Priority));

		}

		void addContact_click (object sender, System.EventArgs e)
		{	

			if (!String.IsNullOrEmpty (edtAddContact.Text)) {
				logedUserData = XmppService.Instance ().getConnection ();

				var rm = new RosterManager (logedUserData);
				pm = new PresenceManager (logedUserData);

				Jid jid = edtAddContact.Text + "@" + Constants.XmppDomain;
				rm.Add (jid, "Friends");
				pm.Subscribe (jid);
				logedUserData.SendPresence (Matrix.Xmpp.Show.chat);

				logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnPresence);
				logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (presenceManager_OnSubscribe);
				logedUserData.OnBeforeSendPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnBeforeSendPresence);


				edtAddContact.Text = "";
			} else {
				Toast.MakeText (this, "Plotësoni të dhënat!!", ToastLength.Long).Show ();
				return;

			}

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
				text = "Message from: " + e.Message.From + "\r\n";
				text += e.Message.Body;
				Toast.MakeText (this, text, ToastLength.Long).Show ();
			});

		}

		void btnLogOut_Click (object	sender, System.EventArgs e)
		{
			logedUserData.SendUnavailablePresence ("Gone home");
			StartActivity (typeof(LogInActivity));
			logedUserData.Close ();
		}

		private void presenceManager_OnSubscribe (object sender, PresenceEventArgs e)
		{
			if (pm != null) {
				pm.ApproveSubscriptionRequest (e.Presence.From);
			}
		}


	}


}


