using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Net;
using System.Net.Mail;
using System.Windows;
//using System.Security.Claims;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using Newtonsoft.Json;

namespace SpikeAccountManager
{
    public partial class AccountWindow : Window
    {
        List<string> existingServers = new List<string>();
        List<string> existingServers_fullDetails = new List<string>();
        List<SpikeAccount> accounts = new List<SpikeAccount>();

        bool validationCheckPass = false;

        Uri ServerAddress;
        string email;
        string password;

        string serverName;
        public string restApi;
        public string apitoken;

        public ResponseUser LoginResponse = null;

        public AccountWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DragRectangle.MouseDown += (sender, e) =>
            {
                this.DragMove();
            };

            this.ServerAddress = new Uri(@"https://vavisjon.no/api/");

            // Set event for auto-sign in
            this.Loaded += AccountWindow_LoadedAutoSignIn;

        }

        private string GetSpikeAccountPath()
        {
            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            System.IO.Directory.CreateDirectory(strPath + @"\SpikeSettings");
            strPath = strPath + @"\SpikeSettings\";
            return strPath;
        }

        private string GetLoginTokenPath(string email)
        {
            string strPath = this.GetSpikeAccountPath();
            string fileName = email + ".login.txt";
            return strPath + fileName;
        }

        private void AccountWindow_LoadedAutoSignIn(object sender, RoutedEventArgs e)
        {
            ResponseUser response = default(ResponseUser);
            string strPath = this.GetSpikeAccountPath();
            if (Directory.Exists(strPath) && Directory.EnumerateFiles(strPath, "*.login.txt").Count() > 0)
            {
                foreach (string file in Directory.EnumerateFiles(strPath, "*.login.txt"))
                {
                    string content = File.ReadAllText(file);
                    if(content.Length > 0)
                    {
                        accounts.Add(new SpikeAccount() { ApiToken = content });
                    }
                }

                foreach (SpikeAccount account in accounts)
                {
                    var myUser = new User()
                    {
                        Token = account.ApiToken
                    };
                    response = this.LoginWithUserToken(myUser);
                    if (response != null && response.Success.HasValue && response.Success.Value)
                        break;
                }
            }

            if (response != null && response.Success.HasValue && response.Success.Value)
            {
                this.LoginResponse = response;

                this.DialogResult = true;
                this.Close();
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private string ValidateRegister()
        {
            Debug.WriteLine("validating...");
            string validationErrors = "";

            MailAddress addr = null;
            try
            {
                addr = new System.Net.Mail.MailAddress(this.RegisterEmail.Text);
            }
            catch
            {
                validationErrors += "Invalid email address. \n";
            }

            string firstName = this.RegisterFirstname.Text;
            if (firstName.Length < 1)
                validationErrors += "First name must be submitted. \n";

            string lastName = this.RegisterLastname.Text;
            if (lastName.Length < 1)
                validationErrors += "Last name must be submitted. \n";

            string password = this.RegisterPassword.Password;
            if (password.Length < 8)
                validationErrors += "Password too short (<8). \n";

            if (password != this.RegisterPasswordConfirm.Password)
                validationErrors += "Passwords do not match. \n";

            return validationErrors;
        }

        private string ValidateLogin()
        {
            string validationErrors = "";

            if(this.LoginEmail.Text.Length <= 1)
            {
                validationErrors += "You must enter a valid username or email\n";
            }

            //MailAddress addr = null;
            //try
            //{
            //    addr = new System.Net.Mail.MailAddress(this.LoginEmail.Text);
            //}
            //catch
            //{
            //    validationErrors += "Invalid email address. \n";
            //}

            return validationErrors;
        }

        private void SaveAccountToDisk(string _email, string _apitoken)
        {
            string content =  _apitoken;
            string loginFile = this.GetLoginTokenPath(_email);

            System.IO.StreamWriter file = new System.IO.StreamWriter(loginFile);
            file.Write(content);
            file.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private void AccountListBox_MouseDoubleClick( object sender, MouseButtonEventArgs e )
        //{
        //  this.restApi = this.accounts[ this.AccountListBox.SelectedIndex ].restApi;
        //  this.apitoken = this.accounts[ this.AccountListBox.SelectedIndex ].apiToken;
        //  this.Close();
        //}

        //private void ButonUseSelected_Click( object sender, RoutedEventArgs e )
        //{
        //  if ( !( this.AccountListBox.SelectedIndex != -1 ) )
        //  {
        //    MessageBox.Show( "Please select an account first." );
        //    return;
        //  }
        //  this.restApi = this.accounts[ this.AccountListBox.SelectedIndex ].restApi;
        //  this.apitoken = this.accounts[ this.AccountListBox.SelectedIndex ].apiToken;
        //  this.Close();
        //}

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterButton.IsEnabled = false;
            RegisterButton.Content = "Contacting server...";
            var errs = ValidateRegister();
            if (errs != "")
            {
                MessageBox.Show(errs);
                RegisterButton.IsEnabled = true;
                RegisterButton.Content = "Register";
                return;
            }

            User myUser = new User()
            {
                Email = this.RegisterEmail.Text,
                Company = this.RegisterCompany.Text,
                Password = this.RegisterPassword.Password,
                FirstName = this.RegisterFirstname.Text,
                LastName = this.RegisterLastname.Text
            };

            string rawPingReply = "";
            dynamic parsedReply = null;
            using (var client = new WebClient())
            {
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string uri = ServerAddress.ToString();
                    rawPingReply = client.DownloadString(ServerAddress.ToString());
                    parsedReply = JsonConvert.DeserializeObject(rawPingReply);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to contact " + ServerAddress.ToString());
                    RegisterButton.IsEnabled = true;
                    RegisterButton.Content = "Register";
                    return;
                }
            }

            SpikeApiClient spikeClient = new SpikeApiClient() { BaseUrl = ServerAddress.ToString() };
            try
            {
                var response = spikeClient.UserRegisterAsync(myUser).Result;
                if (response ==  null || response.Success == false)
                {
                    if(response != null)
                    {
                        MessageBox.Show("Failed to register user. " + response.Message);
                    }
                    else
                    {
                        MessageBox.Show("Failed to register user.");
                    }
                    RegisterButton.IsEnabled = true;
                    RegisterButton.Content = "Register";
                    return;
                }

                string successMessage = "Account creation ok. We have sent you an email to validate your account." +
                    " Once validated, you can log in with your username and password.";

                this.SetSuccessMessageLogin(successMessage);
                Dispatcher.BeginInvoke((Action)(() => LoginRegisterTab.SelectedIndex = 0));

                RegisterButton.IsEnabled = true;
                RegisterButton.Content = "Register";
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is SpikeException) // This we know how to handle.
                    {
                        SpikeException ex = x as SpikeException;
                        try
                        {
                            SpikeServerResponse res = SpikeServerResponse.FromJson(ex.Response);
                            this.SetErrorMessageRegister(res.Message);
                        }
                        catch
                        {
                            this.SetErrorMessageRegister(x.Message);
                        }
                        RegisterButton.IsEnabled = true;
                        RegisterButton.Content = "Register";
                        return true;
                    }
                    return false; // Let anything else stop the application.
                });
            }
            catch (Exception err)
            {
                MessageBox.Show("Failed to register user. " + err.InnerException.ToString()); RegisterButton.IsEnabled = true; RegisterButton.Content = "Register"; return;
            }
        }


        private void SetSuccessMessageLogin(string message)
        {
            this.ErrorMessageLogin.Foreground = System.Windows.Media.Brushes.Green;
            this.ErrorMessageLogin.Text = message;
        }
        private void SetErrorMessageLogin(string message)
        {
            this.ErrorMessageLogin.Foreground = System.Windows.Media.Brushes.Red;
            this.ErrorMessageLogin.Text = message;
        }
        private void SetErrorMessageRegister(string message)
        {
            this.ErrorMessageLogin.Foreground = System.Windows.Media.Brushes.Red;
            this.ErrorMessageRegister.Text = message;
        }

        private ResponseUser LoginWithUserToken(User userWithLoginToken)
        {
            this.ErrorMessageLogin.Inlines.Clear();
            try
            {
                ResponseUser response = this.RunLogin(userWithLoginToken);
                return response;
            }
            catch(SpikeException spike)
            {
                // TODO: Check what the error says, if its only an offline error, retry to see if an offline license is available
                if(spike.StatusCode == 404)
                {
                    // TODO: Fix this
                    // Allow the user to proceed anyhow.

                    this.DialogResult = true;
                    this.Close();

                    // Check the content of the token in offline mode
                    //User checkUser = this.ValidateOfflineJwtSecurityToken(userLoginToken, out bool isValid);
                    //if (isValid)
                    //{
                    //    int a = 1;
                    //}
                }
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is SpikeException) // This we know how to handle.
                    {
                        SpikeException ex = x as SpikeException;
                        try
                        {
                            SpikeServerResponse res = SpikeServerResponse.FromJson(ex.Response);
                            this.SetErrorMessageLogin(res.Message);
                            if (res.Response != null)
                            {
                                int a = 1;
                            }
                        }
                        catch
                        {
                            this.SetErrorMessageLogin(x.Message);
                        }
                        LoginButton.IsEnabled = true;
                        LoginButton.Content = "Login";
                        return true;
                    }
                    return false; // Let anything else stop the application.
                });
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            return default(ResponseUser);
        }

        //private void WriteToken()
        //{
        //    const string sec = "ProEMLh5e_qnzdNUQrqdHPgp";
        //    const string sec1 = "ProEMLh5e_qnzdNU";
        //    var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(sec));
        //    var securityKey1 = new SymmetricSecurityKey(Encoding.Default.GetBytes(sec1));

        //    var signingCredentials = new SigningCredentials(
        //        securityKey,
        //        SecurityAlgorithms.HmacSha512);

        //    List<Claim> claims = new List<Claim>()
        //    {
        //        new Claim("sub", "test"),
        //    };
            
        //    var ep = new EncryptingCredentials(
        //        securityKey1,
        //        SecurityAlgorithms.Aes128KW,
        //        SecurityAlgorithms.Aes128CbcHmacSha256);

        //    var handler = new JwtSecurityTokenHandler();

        //    var jwtSecurityToken = handler.CreateJwtSecurityToken(
        //        "issuer",
        //        "Audience",
        //        new ClaimsIdentity(claims),
        //        DateTime.Now,
        //        DateTime.Now.AddHours(1),
        //        DateTime.Now,
        //        signingCredentials,
        //        ep);


        //    string tokenString = handler.WriteToken(jwtSecurityToken);

        //    File.WriteAllText(this.GetSpikeAccountPath() + "tmp.log", tokenString);

        //    // Id someone tries to view the JWT without validating/decrypting the token,
        //    // then no claims are retrieved and the token is safe guarded.
        //    var jwt = new JwtSecurityToken(tokenString);
        //}

        //private User ValidateOfflineJwtSecurityToken(string token, out bool stillValid)
        //{
        //    //this.WriteToken();

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var validationParameters = new TokenValidationParameters
        //    {
        //        ValidIssuers = new string[]
        //        {
        //            "VAVisjon"
        //        },
        //        TokenDecryptionKey = new SymmetricSecurityKey(
        //            Encoding.Default.GetBytes("VjFaa1U$RXhjRE5RVk&G0ldq&k9iV0ZIVW0x_paRVJSZW1GRVRtO12WR6lRWb00y"))
        //    };

        //    SecurityToken validatedToken;
        //    try
        //    {
        //        tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        //    }
        //    catch (Exception)
        //    {
        //        stillValid = false;
        //        return new User();
        //    }

        //    stillValid = validatedToken != null;
        //    return new User();
            
        //}

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;
            LoginButton.Content = "Contacting server...";
            var errs = ValidateLogin();
            if (errs != "")
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Login";
                this.SetErrorMessageLogin(errs);
                //MessageBox.Show(errs);
                return;
            }

            var myUser = new User()
            {
                Email = this.LoginEmail.Text,
                Password = this.LoginPassword.Password,
            };
            
            this.LoginWithEmailAndPassword(myUser);
        }

        private void LoginWithEmailAndPassword(User userWithEmailAndPass)
        {

            this.ErrorMessageLogin.Inlines.Clear();

            try
            {
                ResponseUser response = this.RunLogin(userWithEmailAndPass);
                if (response != null && response.Success.HasValue && response.Success.Value)
                {
                    this.LoginResponse = response;

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (SpikeException err)
            {
                MessageBox.Show(err.Message);
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is SpikeException) // This we know how to handle.
                    {
                        SpikeException ex = x as SpikeException;
                        try
                        {
                            SpikeServerResponse res = SpikeServerResponse.FromJson(ex.Response);
                            this.SetErrorMessageLogin(res.Message);
                            if (res.Response != null)
                            {
                                ResponseError responseError = ResponseError.FromJson(ex.Response);
                                Error error = responseError.Resource;
                                if(error.Link != null && error.LinkText != null)
                                {
                                    Hyperlink link = new Hyperlink();
                                    link.NavigateUri = new Uri(error.Link);
                                    link.Inlines.Add(error.LinkText);
                                    link.RequestNavigate += Hyperlink_RequestNavigate;

                                    LineBreak lb = new LineBreak();
                                    this.ErrorMessageLogin.Inlines.Add(lb);
                                    this.ErrorMessageLogin.Inlines.Add(link);
                                }
                            }
                        }
                        catch
                        {
                            this.SetErrorMessageLogin(x.Message);
                        }
                        LoginButton.IsEnabled = true;
                        LoginButton.Content = "Login";
                        return true;
                    }
                    return false; // Let anything else stop the application.
                });
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private ResponseUser RunLogin(User myUser)
        {
            SpikeApiClient spikeClient = new SpikeApiClient() { BaseUrl = ServerAddress.ToString() };

            string rawPingReply = "";
            dynamic parsedReply = null;
            using (var client = new WebClient())
            {
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string uri = ServerAddress.ToString();
                    rawPingReply = client.DownloadString(uri);
                    parsedReply = JsonConvert.DeserializeObject(rawPingReply);
                }
                catch (Exception ex){
                    throw new SpikeException(
                        "Failed to contact " + ServerAddress.ToString(),
                        404,
                        "Coult not connect",
                        new Dictionary<string, IEnumerable<string>>(),
                        ex
                    );
                }
            }

            var response = spikeClient.UserLoginAsync(myUser).Result;
            if (response == null)
            {
                throw new SpikeException(
                    "Unexpected response from " + ServerAddress.ToString(),
                    404,
                    "Unexpected response",
                    new Dictionary<string, IEnumerable<string>>(),
                    null
                );
            }
            else if (response.Success == false && response.Resource != null)
            {
                MessageBox.Show("Failed to login. " + response.Message);
                return default(ResponseUser);
            }

            var serverName = parsedReply.serverName;
            this.SaveAccountToDisk(response.Resource.Email, response.Resource.Token);

            this.apitoken = response.Resource.Token;

            return response;

        }

        private void LoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;

            // your event handler here
            e.Handled = true;
            this.LoginButton_Click(sender, e);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
