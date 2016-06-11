using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace SensorClientApp
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private EditText m_passwordTextBox;
        private EditText m_usernameTextBox;
        private Button m_loginBtn;
        private Button m_registerBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            m_passwordTextBox = FindViewById<EditText>(Resource.Id.passwordBox);
            m_usernameTextBox = FindViewById<EditText>(Resource.Id.usernameBox);
            m_loginBtn = FindViewById<Button>(Resource.Id.loginBtn);
            m_registerBtn = FindViewById<Button>(Resource.Id.registerBtn);

            m_loginBtn.Click += OnLogin;
            m_registerBtn.Click += OnRegisterClick;
        }

        private void OnRegisterClick(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }

        private void OnLogin(object sender, EventArgs e)
        {
            // check credentials...

            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }
    }
}