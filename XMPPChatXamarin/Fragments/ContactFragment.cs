
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Matrix.Xmpp.Client;
using Matrix;
using Matrix.Xmpp.Base;
using Matrix.Xmpp;

namespace XMPPChatXamarin
{
	public class ContactFragment : Fragment
	{
		private static ListView listView;
		XmppClient logedUserData;
		PresenceManager pm;
		List<String> list = new List<String> ();

		public ContactFragment (XmppClient getXmppClient)
		{
			//logedUserData = getXmppClient;
		}

		View view1;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (
				           Resource.Layout.TabContacts, container, false);
			view1 = view;
			logedUserData = XmppService.Instance ().getConnection ();
			var rm = new RosterManager (logedUserData);
			Jid jid = new Jid (logedUserData.Username);
			logedUserData.AutoRoster = true;
			logedUserData.AutoPresence = true;

			logedUserData.OnBeforeSendPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnBeforeSendPresence);
			logedUserData.OnReceiveXml += new EventHandler<TextEventArgs> (XmppClientOnReceiveXml);
			logedUserData.OnSendXml += new EventHandler<TextEventArgs> (XmppClientOnSendXml);

			logedUserData.SendPresence (Show.chat, "Online");
			logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnPresence);
		
			LoadList (view);

			//	listView.ItemClick += OnListItemClick;	
			listView.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) => {
				string selectedFromList = listView.GetItemAtPosition (e.Position).ToString ();
				int s =	selectedFromList.IndexOf ("(");
				string aaa = selectedFromList.Substring (0, s);
				//Console.WriteLine (selectedFromList);
				Intent intent = new Intent (Activity, typeof(ChatActivity));
				intent.PutExtra ("username", aaa);
				StartActivity (intent);
			};
			return view;
		}

		void OnListItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			//string item = listView.GetItemAtPosition [e.Position];


		}

		void LoadList (View view)
		{
			listView = view.FindViewById<ListView> (Resource.Id.listContacts);
			IListAdapter lista = new ArrayAdapter<String> (Activity, Android.Resource.Layout.SimpleListItem1, list);
			Activity.RunOnUiThread (() =>
				listView.Adapter = lista);
		}

		void xmppClient_OnPresence (object sender, PresenceEventArgs e)
		{
			// we see who is online 
			Console.WriteLine (string.Format ("OnPresence from {0}", e.Presence.From));
			Console.WriteLine (string.Format ("Status {0}", e.Presence.Status));
			Console.WriteLine (string.Format ("Show type {0}", e.Presence.Show));
			Console.WriteLine (string.Format ("Priority {0}", e.Presence.Priority));
			list.Clear ();
			if (!string.IsNullOrWhiteSpace (e.Presence.From.User)) {
				string user = e.Presence.From.User;
				string status = e.Presence.Status.ToString ();
				list.Add (user + " (" + status + ")");
				LoadList (view1);
			}

		}

		void XmppClientOnSendXml (object sender, TextEventArgs e)
		{
			AddDebug ("SEND: " + e.Text);
		}

		void XmppClientOnReceiveXml (object sender, TextEventArgs e)
		{
			AddDebug ("RECV: " + e.Text);
		}

		void AddDebug (string debug)
		{
			Console.Write ("XML Recive ore send", debug.ToString ());
			// write the debug data to a file, textbox etc...
		}


		private void presenceManager_OnSubscribe (object sender, PresenceEventArgs e)
		{
			if (pm != null) {
				pm.ApproveSubscriptionRequest (e.Presence.From);
			}
		}

		void xmppClient_OnBeforeSendPresence (object sender, PresenceEventArgs e)
		{
			Console.WriteLine ("Before presence : " + e.Presence);
		}

		private void OnRosterStart (object sender)
		{
			Console.WriteLine ("Roster has started.");
		}

		private void OnRosterItem (object sender, RosterItem item)
		{
			string name = (item.Name);
			string user = (item.Jid.User);
		}

		private void OnRosterEnd (object sender)
		{
			Console.WriteLine ("Roster has ended.");
		}
	
	}
}

