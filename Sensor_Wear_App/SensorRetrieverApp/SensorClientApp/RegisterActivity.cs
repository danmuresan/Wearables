using System;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Util;

namespace SensorClientApp
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        private EditText m_emailTextBox;
        private EditText m_passwordTextBox;
        private EditText m_repeatPassTextBox;
        private Button m_submitBtn;
        private EditText m_usernameTextBox;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Register);

            m_passwordTextBox = FindViewById<EditText>(Resource.Id.passwordBox);
            m_usernameTextBox = FindViewById<EditText>(Resource.Id.usernameBox);
            m_repeatPassTextBox = FindViewById<EditText>(Resource.Id.repeatPasswordBox);
            m_emailTextBox = FindViewById<EditText>(Resource.Id.emailBox);
            m_submitBtn = FindViewById<Button>(Resource.Id.submitRegistrationBtn);

            m_submitBtn.Click += OnSubmitRegistration;
        }

        private void OnSubmitRegistration(object sender, EventArgs e)
        {
            if (FieldsAreValid())
            {
                try
                {
                    // TODO registration

                    Toast.MakeText(this, "User registered succesfully", ToastLength.Long).Show();
                    Finish();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Something went wrong with the registration", ToastLength.Long).Show();
                    Log.Error("REGISTER", ex.ToString());
                }
            }

        }

        private bool FieldsAreValid()
        {
            // TODO: validation
            return true;
        }
    }
}