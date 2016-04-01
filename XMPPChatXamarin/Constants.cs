using System;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Sasl;
using Matrix;
using System.Collections.Generic;
using Matrix.Xmpp;
using System.Threading;
using Matrix.Xmpp.Register;
using Matrix.Xmpp.Google.Push;

namespace XMPPChatXamarin
{
	public class Constants
	{
		private XmppClient xmppClient = new XmppClient ();
			
		string lic = @"eJxkkN1S8jAQhm/F8ZTRtCBSvlkziq1YqXwFVNSztAk1tE3SNEHK1YuK/yc7
							u/vsu/vOQsRTJmq2ty4LUZ/sk+yglgvzTDT7V7yjfQyxltSmJqR4ZizlEtBX
							ByaWCMNNg11Anzmc29rIkmkMY1IyHKxIYYmRGtBbDeeyVEQ0H4BLsbezAuiD
							QVASXuCaFKw+/ebskG6H3tl2+PPQraLEsGCtuGb+NsNtxz12+h0H0B8EYe2z
							UmKj7XbXroDX+FPfdZ1X/S8AM54JYqxmWE+y0dQPGvPYjprqtkVokA28qDtY
							5Nc5rdxq0BN0apNh2L+h+VF63/zfDFUlSCdZPqjkqBdOmrbbWcTV8arXnyuG
							nMvrBGV+JB5IMmpGy3ncUn6MuF1x6rpDfkOj6YV3gar0afx8piLZImoT5LOr
							sJ3n2gvX94N4nvpP/thbN+OlXmy6hE7uvPoE0JdvQLt34xcB";
		
		private static string domainName = "krenare-pc";
		//private static string hostName = "Krenare-PC";

		private static string hostName = "192.168.0.110";
		//private static string hostName = "192.168.0.105";
		private static int port = 5222;

		public Constants ()
		{
				
		}

		public string getLicense ()
		{
			return lic;
		}

		public static string XmppDomain{ get { return domainName; } }

		public static string XmppHostName{ get { return hostName; } }

		public static int XmppPort{ get { return port; } }

		public void SendMessageToUser (string sentToUsername, string message)
		{			
			xmppClient.Send (new Matrix.Xmpp.Client.Message (new Jid (sentToUsername + "@" + xmppClient.XmppDomain.ToString ()),
				MessageType.chat, message));
		}

	
		private void SendAndRecieveXML ()
		{
			xmppClient.OnReceiveXml += new EventHandler<TextEventArgs> (XmppClientOnReceiveXml);
			xmppClient.OnSendXml += new EventHandler<TextEventArgs> (XmppClientOnSendXml);

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
			Console.WriteLine (debug);

		}

		void xmppClient_OnLogin (object sender, Matrix.EventArgs e)
		{
			Console.WriteLine ("Log in : " + e.State);
		}

		void xmppClient_OnError (object sender, Matrix.ExceptionEventArgs e)
		{
			//Console.WriteLine ("Error: " + e.ToString ());
			string msg = (e != null ? (e.Exception != null ? e.Exception.Message : "") : "");
			Console.WriteLine ("OnError: " + msg);
			xmppClient.Close ();

		}

		void xmppClient_OnBeforeSasl (object sender, SaslEventArgs e)
		{
			e.Auto = true;
			e.SaslMechanism = Matrix.Xmpp.Sasl.SaslMechanism.PLAIN;
		}

		void xmppClient_OnAuthError (object sender, SaslEventArgs e)
		{
			Console.WriteLine ("Auth Error: " + e.ToString ());
		}

		void xmppClient_OnSendBody (object sender, Matrix.Net.BodyEventArgs e)
		{
			Console.WriteLine ("SENDBody: " + e.ToString ());
		}

		void xmppClient_OnReceiveBody (object sender, Matrix.Net.BodyEventArgs e)
		{
			Console.WriteLine ("RECVBody: " + e.ToString ());
		}


		private void xmppClient_OnMessage (object sender, MessageEventArgs e)
		{
			List<string> recived_Message = new List<string> ();

			recived_Message.Add (string.Format ("OnMessage from {0}", e.Message.From));
			recived_Message.Add (string.Format ("Body {0}", e.Message.Body));
			recived_Message.Add (string.Format ("Type {0}", e.Message.Type));

			if (e.Message.Body == null)
				return;

			/*RunOnUiThread (delegate {			
				text = "Message from: " + e.Message.From + "\r\n";
				text += e.Message.Body;
				//	Android.Widget.Toast.MakeText (this, text, ToastLength.Long).Show ();
			});
			//	AcceptedMessage = new string[]{ text };*/
		}

		private void xmppClient_OnRegisterInformation (object sender, RegisterEventArgs e)
		{
			e.Register.RemoveAll ();
			e.Register.Username = xmppClient.Username;
			e.Register.Password = xmppClient.Password;
		}

		private void xmppClient_OnRegister (object sender, Matrix.EventArgs e)
		{
			// registration was successful
		}

		private void xmppClient_OnRegisterError (object sender, IqEventArgs e)
		{
			Console.Write ("Error registration" + e.Iq);
			// registration failed.
			xmppClient.Close ();
		}

	
		public void UpdatePresence ()
		{
			xmppClient.SendPresence (Matrix.Xmpp.Show.away, "Away");
		}

		/*
		 * 
		 * 
		*/
	}
}

