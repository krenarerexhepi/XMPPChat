
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
using Matrix.Xmpp.Sasl;
using Matrix;
using Matrix.Xmpp.Roster;
using Matrix.Xmpp.Privacy;
using Android.Views.InputMethods;
using System.Net.Sockets;

namespace XMPPChatXamarin
{
	[Activity (Label = "Chaty", MainLauncher = true, Icon = "@drawable/leaf")]			
	public class LogInActivity : Activity
	{
		Constants con = new Constants ();
		XmppClient xmppClient;
		TextView txtUsername;
		TextView txtPassword;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Create your application here
			SetContentView (Resource.Layout.LogIn);

			//this license we need to get to use matrix
			Matrix.License.LicenseManager.SetLicense (con.getLicense ());
		
			Button logIn = FindViewById<Button> (Resource.Id.btnLogIn);
			Button register = FindViewById<Button> (Resource.Id.btnRegister);

			logIn.Click += new EventHandler (btnLogIn_Click);
			register.Click += new EventHandler (btnRegister_Click);

			txtUsername = FindViewById<TextView> (Resource.Id.txtUsername);
			txtPassword = FindViewById<TextView> (Resource.Id.txtPassword);

			txtPassword.EditorAction += (sender, args) => {
				if (args.ActionId == ImeAction.Done) {
					register.PerformClick ();
					args.Handled = true;
				}
			};

		}

		#region  button events


		void btnLogIn_Click (object sender, System.EventArgs e)
		{
	
			if (string.IsNullOrWhiteSpace (txtUsername.Text)) {	
				showMessage (GetString (Resource.String.username_required));
				txtUsername.RequestFocus ();
				return;
			}

			if (string.IsNullOrWhiteSpace (txtPassword.Text)) {	
				showMessage (GetString (Resource.String.password_required));
				txtPassword.RequestFocus ();
				return;
			}

			xmppClient = new XmppClient ();

			xmppClient.SetUsername (txtUsername.Text);
			xmppClient.SetXmppDomain (Constants.XmppDomain);
			xmppClient.Password = txtPassword.Text;
			xmppClient.Hostname = Constants.XmppHostName;
			xmppClient.Port = Constants.XmppPort;

			xmppClient.Status = GetString (Resource.String.status_online);
			xmppClient.Show = Matrix.Xmpp.Show.chat;

					
			xmppClient.OnAuthError += new EventHandler<SaslEventArgs> (xmppClient_OnAuthError);
			xmppClient.OnError += new EventHandler<ExceptionEventArgs> (xmppClient_OnError);
			xmppClient.OnLogin += new EventHandler<Matrix.EventArgs> (xmppClient_OnLogin);
			xmppClient.OnBeforeSasl += new EventHandler<SaslEventArgs> (xmppClient_OnBeforeSasl);
			xmppClient.OnReceiveBody += new EventHandler<Matrix.Net.BodyEventArgs> (xmppClient_OnReceiveBody);
			xmppClient.OnSendBody += new EventHandler<Matrix.Net.BodyEventArgs> (xmppClient_OnSendBody);
		

			showWaiting ();
			xmppClient.Open ();
			XmppService.Instance ().InstanceService (xmppClient);

			InputMethodManager inputManager = (InputMethodManager)this.GetSystemService (Context.InputMethodService);
			inputManager.HideSoftInputFromWindow (this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
		}

		void btnRegister_Click (object sender, System.EventArgs ea)
		{
			StartActivity (typeof(RegisterActivity));
		}

		#endregion

		public void xmppClient_OnAuthError (object sender, SaslEventArgs e)
		{
			hideWaiting ();
			if (e.Failure is Failure) {
				var ex = (Failure)e.Failure;
				if (ex != null && Convert.ToString (ex.Condition) == GetString (Resource.String.not_autorized)) {
					showMessage (GetString (Resource.String.error_logIn_username));
					return;
				}
			}

			Console.WriteLine (GetString (Resource.String.on_error) + e.ToString ());
			xmppClient.Close ();
		}


		void xmppClient_OnSendBody (object sender, Matrix.Net.BodyEventArgs e)
		{
			Console.WriteLine ("SENDBody: " + e.ToString ());
		}

		void xmppClient_OnReceiveBody (object sender, Matrix.Net.BodyEventArgs e)
		{
			Console.WriteLine ("RECVBody: " + e.ToString ());
		}


		void xmppClient_OnLogin (object sender, Matrix.EventArgs e)
		{  
			hideWaiting ();
			StartActivity (typeof(MainActivity)); 
			//txtPassword.Text = "";
			//txtUsername.Text = "";
		}

		void xmppClient_OnError (object sender, Matrix.ExceptionEventArgs e)
		{
			hideWaiting ();
			if (e.Exception is SocketException) {
				var ex = (SocketException)e.Exception;
				if (ex != null && ex.ErrorCode == 11001) {
					showMessage (GetString (Resource.String.server_no_connection));
					return;
				} else if (ex != null && ex.ErrorCode == 10060) {
					showMessage (GetString (Resource.String.error_time_out));
					return;
				}
			}
			string msg = (e != null ? (e.Exception != null ? e.Exception.Message : "") : "");
			Console.WriteLine (GetString (Resource.String.on_error) + msg);
		}

		void xmppClient_OnBeforeSasl (object sender, SaslEventArgs e)
		{
			e.Auto = true;
			e.SaslMechanism = Matrix.Xmpp.Sasl.SaslMechanism.PLAIN;
		}

		private void showMessage (string message)
		{
			RunOnUiThread (() => Toast.MakeText (this, message, ToastLength.Long).Show ());
		}

		ProgressDialog progressDialog;

		private void showWaiting ()
		{
			progressDialog = ProgressDialog.Show (this, GetString (Resource.String.please_wait), GetString (Resource.String.login_in_system), true);
			RunOnUiThread (() => progressDialog.Show ());
		}

		private void hideWaiting ()
		{
			if (progressDialog != null)
				RunOnUiThread (() =>	progressDialog.Hide ());
		}
	}
}
	
