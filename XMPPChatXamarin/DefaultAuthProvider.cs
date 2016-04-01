using System;
using Java.Security;

namespace XMPPChatXamarin
{
	public class DefaultAuthProvider : Object
	{
		//, AuthProvider

		#region implemented abstract members of AuthProvider

		public  void Login (Javax.Security.Auth.Subject subject, Javax.Security.Auth.Callback.ICallbackHandler handler)
		{
			throw new NotImplementedException ();
		}

		public  void Logout ()
		{
			throw new NotImplementedException ();
		}

		public  void SetCallbackHandler (Javax.Security.Auth.Callback.ICallbackHandler handler)
		{
			throw new NotImplementedException ();
		}

		#endregion

		public void authenticate (string username, string password)
		{
			
		}
	}
}

