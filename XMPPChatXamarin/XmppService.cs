using System;
using Matrix.Xmpp.Client;
using Matrix;

namespace XMPPChatXamarin
{
	/// <summary>
	/// Xmpp service.
	/// This class is singeltone clase that save data of loged user  
	/// </summary>
	public class XmppService
	{
		private static XmppService instance;
		private XmppClient xmppUserList = null;
		private Constants con = new Constants ();

		public XmppService ()
		{
			if (xmppUserList == null) {
				xmppUserList = new XmppClient ();
			}
		}

		public static XmppService Instance ()
		{
			if (instance == null) {
				instance = new XmppService ();
			}
			return instance;
		}

		public XmppClient InstanceService (XmppClient xmp)
		{			
			xmppUserList = xmp;
			return xmppUserList;
		}

		public XmppClient setConnection (string username, string password)
		{
			xmppUserList.SetUsername (username);
			xmppUserList.SetXmppDomain (Constants.XmppDomain);
			xmppUserList.Password = password;
			xmppUserList.Hostname = Constants.XmppHostName;
			xmppUserList.Port = Constants.XmppPort;
			xmppUserList.Show = Matrix.Xmpp.Show.chat;
			return xmppUserList;
		}

		public XmppClient getConnection ()
		{
			return xmppUserList;
		}
	}
}

