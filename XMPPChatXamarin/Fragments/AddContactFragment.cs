
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

namespace XMPPChatXamarin
{
	public class AddContactFragment : Fragment
	{
		XmppClient logedUserData;
		EditText edtAddContact;
		EditText nickName;
		EditText edtgroup;
		PresenceManager pm;

		public AddContactFragment (XmppClient getXmppClient)
		{
			logedUserData = getXmppClient;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (
				           Resource.Layout.TabAddContact, container, false);
			
			Button addContact = view.FindViewById<Button> (Resource.Id.btnAddContactTab);

			addContact.Click += new EventHandler (addContact_click);

			edtAddContact = view.FindViewById<EditText> (Resource.Id.edtAddContactTab);
			edtgroup = view.FindViewById<EditText> (Resource.Id.edtGroup);
			nickName = view.FindViewById<EditText> (Resource.Id.edtNockName);

			return view;
		}

		void addContact_click (object sender, System.EventArgs e)
		{	
			

			if (string.IsNullOrWhiteSpace (edtAddContact.Text)) {	
				showMessage (GetString (Resource.String.username_required));
				edtAddContact.RequestFocus ();
				return;
			}

			showWaiting ();
			logedUserData = XmppService.Instance ().getConnection ();

			var rm = new RosterManager (logedUserData);
			pm = new PresenceManager (logedUserData);

			Jid jid = edtAddContact.Text + "@" + Constants.XmppDomain;
			rm.Add (jid, nickName.Text, edtgroup.Text);
			pm.Subscribe (jid);

			logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnPresence);
			logedUserData.OnPresence += new EventHandler<PresenceEventArgs> (presenceManager_OnSubscribe);
			logedUserData.OnBeforeSendPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnBeforeSendPresence);

			edtAddContact.Text = "";

			hideWaiting ();
		}

		ProgressDialog progressDialog;

		private void showWaiting ()
		{
			progressDialog = ProgressDialog.Show (Activity, GetString (Resource.String.please_wait), GetString (Resource.String.login_in_system), true);
			Activity.RunOnUiThread (() => progressDialog.Show ());
		}

		private void hideWaiting ()
		{
			if (progressDialog != null)
				Activity.RunOnUiThread (() =>	progressDialog.Hide ());
		}

		private void showMessage (string message)
		{
			Activity.RunOnUiThread (() => Toast.MakeText (Activity, message, ToastLength.Long).Show ());
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

			showMessage (GetString (Resource.String.Added_contact_success));
		}

		private void presenceManager_OnSubscribe (object sender, PresenceEventArgs e)
		{
			if (pm != null) {
				pm.ApproveSubscriptionRequest (e.Presence.From);
			}
		}



	}
}



