
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

namespace XMPPChatXamarin
{
	public class ProfileFragment : Fragment
	{
		int status;
		Spinner spinner;
		XmppClient logedUserData;
		EditText edtStatus;

		public ProfileFragment (XmppClient getXmppClient)
		{
			logedUserData = getXmppClient;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{	
			base.OnCreateView (inflater, container, savedInstanceState);
			var view = inflater.Inflate (Resource.Layout.TabProfile, container, false);			
			setSpinnerContent (view);
			Button saveStatus = view.FindViewById<Button> (Resource.Id.btnSaveStatus);
			edtStatus = view.FindViewById<EditText> (Resource.Id.edtStatus);

			logedUserData = XmppService.Instance ().getConnection ();
			logedUserData.OnBeforeSendPresence += new EventHandler<PresenceEventArgs> (xmppClient_OnBeforeSendPresence);
			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner1_ItemSelected);
			saveStatus.Click += new EventHandler (btnSaveStatus_Click);

			return view;
		}

		private void setSpinnerContent (View view)
		{
			spinner = view.FindViewById<Spinner> (Resource.Id.spinner1);

			List<String> list = new List<String> ();
			list.Add (GetString (Resource.String.set_status_online));
			list.Add (GetString (Resource.String.set_status_away));
			list.Add (GetString (Resource.String.set_status_busy));
			list.Add (GetString (Resource.String.set_status_onphone));
			list.Add (GetString (Resource.String.set_status_invi));

			ISpinnerAdapter lista = new ArrayAdapter<String> (Activity, Android.Resource.Layout.SimpleDropDownItem1Line, list);
			spinner.Adapter = lista;
		}

		private void spinner1_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			status = e.Position;
		}

		void btnSaveStatus_Click (object sender, System.EventArgs e)
		{	

			// to do: te regullohet statusi , presenca eshte mire 
			logedUserData.Open ();
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

		void xmppClient_OnBeforeSendPresence (object sender, PresenceEventArgs e)
		{
			Console.WriteLine ("Before presence : " + e.Presence);
		}
	}
}

