using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Matrix.Xmpp.Client;
using Matrix.Xmpp;
using Matrix;
using System.Collections.Generic;
using Matrix.Xmpp.Register;
using Matrix.Xmpp.XData;
using Matrix.Xmpp.Sasl;
using System.Threading;

namespace XMPPChatXamarin
{
	[Activity (Label = "Chaty", Icon = "@drawable/leaf", MainLauncher = false)]
	public class MainActivity : Activity
	{
		XmppClient logedUserData;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

			logedUserData = XmppService.Instance ().getConnection ();
			logedUserData.Show = Matrix.Xmpp.Show.chat;

			AddTab (GetString (Resource.String.First_tab_name), Resource.Drawable.account, new ContactFragment (logedUserData));
			AddTab (GetString (Resource.String.Second_tab_name), Resource.Drawable.account, new AddContactFragment (logedUserData));
			AddTab (GetString (Resource.String.Third_tab_name), Resource.Drawable.account, new  ProfileFragment (logedUserData));

			if (bundle != null)
				this.ActionBar.SelectTab (this.ActionBar.GetTabAt (bundle.GetInt ("tab")));
			
		}

	
		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutInt ("tab", this.ActionBar.SelectedNavigationIndex);

			base.OnSaveInstanceState (outState);
		}

		void AddTab (string tabText, int iconResourceId, Fragment view)
		{
			var tab = this.ActionBar.NewTab ();            
			tab.SetText (tabText);
			tab.SetIcon (Resource.Drawable.account);

			// must set event handler before adding tab
			tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
				var fragment = this.FragmentManager.FindFragmentById (Resource.Id.fragmentContainer);
				if (fragment != null)
					e.FragmentTransaction.Remove (fragment);         
				e.FragmentTransaction.Add (Resource.Id.fragmentContainer, view);
			};
			tab.TabUnselected += delegate(object sender, ActionBar.TabEventArgs e) {
				e.FragmentTransaction.Remove (view);
			};

			this.ActionBar.AddTab (tab);
		}
	
	}



}


