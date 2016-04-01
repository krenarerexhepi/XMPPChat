
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
using Matrix.Xmpp.Register;
using System.Net.Sockets;
using System.Threading;
using Android.Views.InputMethods;
using System.Net;
using Matrix.Xmpp.Receipts;

namespace XMPPChatXamarin
{
	[Activity (Label = "Register user", Icon = "@drawable/leaf")]			
	public class RegisterActivity : Activity
	{
		ProgressDialog progressDialog;
		Constants con = new Constants ();
		TextView name;
		TextView username;
		TextView password;

		Button register;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Register);

			name = FindViewById<TextView> (Resource.Id.txtName);
			username = FindViewById<TextView> (Resource.Id.txtusername);
			password = FindViewById<TextView> (Resource.Id.txtPassword);

			register = FindViewById<Button> (Resource.Id.btnRegister);
			register.Click += new EventHandler (btnRegister_Click);

			Matrix.License.LicenseManager.SetLicense (con.getLicense ());

			password.EditorAction += (sender, args) => {
				if (args.ActionId == ImeAction.Done) {
					register.PerformClick ();
					args.Handled = true;
				}
			};

		}

		XmppClient xmppClient;

		void btnRegister_Click (object sender, System.EventArgs ea)
		{
			if (string.IsNullOrWhiteSpace (username.Text)) {	
				showMessage (GetString (Resource.String.username_required));
				username.RequestFocus ();
				return;
			}

			if (string.IsNullOrWhiteSpace (password.Text)) {	
				showMessage (GetString (Resource.String.password_required));
				password.RequestFocus ();
				return;
			}
					
			xmppClient = new XmppClient ();

			xmppClient.OnRegister += new EventHandler<Matrix.EventArgs> (xmppClient_OnRegister);
			xmppClient.OnRegisterInformation += new EventHandler<RegisterEventArgs> (xmppClient_OnRegisterInformation);
			xmppClient.OnRegisterError += new EventHandler<Matrix.Xmpp.Client.IqEventArgs> (xmppClient_OnRegisterError);
			xmppClient.OnError += XmppClient_OnError;

			xmppClient.SetUsername (username.Text);
			xmppClient.SetXmppDomain (Constants.XmppDomain);
			xmppClient.Hostname = Constants.XmppHostName;
			xmppClient.Password = password.Text;
			xmppClient.Port = Constants.XmppPort;
			xmppClient.Show = Matrix.Xmpp.Show.chat;
			xmppClient.RegisterNewAccount = true;
		

			// data will saved in instance to use in main form
			XmppService.Instance ().InstanceService (xmppClient);
			showWaiting ();
			xmppClient.Open ();
			
		}

		void XmppClient_OnError (object sender, Matrix.ExceptionEventArgs e)
		{
			hideWaiting ();
			if (e.Exception is SocketException) {
				var ex = (SocketException)e.Exception;
				if (ex != null && ex.ErrorCode == 11001) {
					showMessage (GetString (Resource.String.server_no_connection));
					xmppClient.Close ();
					return;
				} else if (ex != null && ex.ErrorCode == 10060) {
					showMessage (GetString (Resource.String.error_time_out));
					return;
				}
			}
			showMessage (GetString (Resource.String.server_no_accessible));
			Console.WriteLine (e.ToString ());
		}

		private void xmppClient_OnRegisterInformation (object sender, RegisterEventArgs e)
		{
			e.Register.RemoveAll ();
			e.Register.Name = name.Text;
			e.Register.Username = xmppClient.Username;
			e.Register.Password = xmppClient.Password;
		}

		private void xmppClient_OnRegister (object sender, Matrix.EventArgs e)
		{
			// registration was successful
			StartActivity (typeof(MainActivity));
			username.Text = "";
			password.Text = "";
			name.Text = "";
		}

		private void xmppClient_OnRegisterError (object sender, IqEventArgs e)
		{
			hideWaiting ();
			showMessage (GetString (Resource.String.not_registered));
			Console.Write (GetString (Resource.String.error_registration) + e.Iq);

			if (e.Iq is Matrix.Xmpp.Client.Iq) {
				var ex = (Matrix.Xmpp.Client.Error)e.Iq.Error;
				if (ex != null && ex.Condition.ToString () == GetString (Resource.String.conflict)) {
					showMessage (GetString (Resource.String.error_registration_username));
					return;
				}
			}
			Console.WriteLine (e.ToString ());		
		}

		private void showWaiting ()
		{
			progressDialog = ProgressDialog.Show (this, GetString (Resource.String.please_wait), GetString (Resource.String.registration_in_system), true);
			RunOnUiThread (() => progressDialog.Show ());

		}

		private void hideWaiting ()
		{
			if (progressDialog != null)
				RunOnUiThread (() =>	progressDialog.Hide ());
		}

		private void showMessage (string message)
		{
			RunOnUiThread (() => Toast.MakeText (this, message, ToastLength.Short).Show ());
		}
	}

}

