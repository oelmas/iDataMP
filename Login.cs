using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using Domain;

namespace MissionPlanner
{
    public sealed partial class Login : Form
    {
        private readonly GOKALPDBEntities _db ;

        public Login()
        {
            InitializeComponent();
            _db = new GOKALPDBEntities();
            Text = string.Empty;
            ControlBox = false;
            DoubleBuffered = true;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private static extern void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private static extern void SendMessage(IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (tbUserName.Text != "Enter the username")
            {
                if (tbPassword.Text != "Enter the password")
                {
                    var user = new UserModel();
                    var username = tbUserName.Text;
                    var password = tbPassword.Text;
                    
                    var hashedPassword = HashedPassword(password);


                    var validLogin = user.LoginUser(username, hashedPassword);
                    if (validLogin)
                    {
                        Hide();
                        var mainForm = new MainV2();
                        mainForm.Show();
                        mainForm.FormClosed += Logout;
                        this.Hide();
                    }
                    else
                    {
                        msgError("Incorrect username or password. Please try again.");
                        //tbPassword.Clear();
                        tbPassword.Text = "Password";
                        tbUserName.Focus();
                        
                    }
                }
                else
                {
                    msgError("Please enter password");
                }
            }
            else
            {
                msgError("Please enter password");
            }
        }

        private static string HashedPassword(string password)
        {
            SHA256 sha = SHA256.Create();

            // convert the input text to a byte array and compute the hash
            byte[] data = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new System.Text.StringBuilder();

            // loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // return the hexadecimal string.   
            var hashedPassword = sBuilder.ToString();
            return hashedPassword;
        }

        private void msgError(string msg)
        {
            lblErrorMessage.ForeColor = Color.Firebrick;
            lblErrorMessage.Text = "     " + msg;
            lblErrorMessage.Visible = true;
        }

        private void panelLoginTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void tbUserName_Enter(object sender, EventArgs e)
        {
            if (tbUserName.Text == "Enter the username")
            {
                tbUserName.Text = "";
                tbUserName.ForeColor = Color.LightGray;
            }
        }

        private void tbUserName_Leave(object sender, EventArgs e)
        {
            if (tbUserName.Text == "")
                //tbUserName.Text = "Enter the username";
                tbUserName.ForeColor = Color.DimGray;

            if (tbUserName.Text != string.Empty &&
                tbPassword.Text != string.Empty && tbPassword.Text != "Enter the password")
                btnSubmit.Enabled = true;

            if (tbUserName.Text == string.Empty)
                msgError("Please enter username!");
        }

        private void Login_Load(object sender, EventArgs e)
        {
            tbUserName.ForeColor = Color.DimGray;
            tbPassword.ForeColor = Color.DimGray;
            tbUserName.Text = "Enter the username";
            tbPassword.Text = "Enter the password";
        }

        private void tbPassword_Enter(object sender, EventArgs e)
        {
            tbPassword.UseSystemPasswordChar = true;
            if (tbPassword.Text == "Enter the password")
            {
                tbPassword.Text = "";
                tbPassword.ForeColor = Color.LightGray;
            }
        }

        private void tbPassword_Leave(object sender, EventArgs e)
        {
            if (tbPassword.Text == "")
            {
                tbPassword.Text = "";
                tbPassword.ForeColor = Color.DimGray;
                tbPassword.UseSystemPasswordChar = false;
            }

            if ((tbPassword.Text != string.Empty &&
                 tbUserName.Text != string.Empty) || tbUserName.Text != "Enter the username")
                btnSubmit.Enabled = true;
            if (tbPassword.Text == string.Empty)
                msgError("Please enter password!");
        }

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf01, 0);
        }

        private void panelLoginLogo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf01, 0);
        }

        private void Logout(object sender, FormClosedEventArgs e)
        {
            tbPassword.Text = "Password";
            tbPassword.UseSystemPasswordChar = false;

            tbUserName.Text = "Username";
            lblErrorMessage.Visible = false;
            this.Show();
            tbUserName.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}