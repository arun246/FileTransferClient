using FileTransferClient.Presentation.Contracts;
using System.Windows.Forms;
using System;
using FileTransferClient.Domain.Models.Connection;
using FileTransferClient.Domain.Models;

public partial class MainForm : Form, IMainView
{
    // Public events for Presenter interaction
    public event EventHandler ConnectClicked;
    public event EventHandler OpenTransferQueueClicked;
    public event EventHandler OpenOtherFormClicked; // Add event for other form
    public event EventHandler<FileItem> EnqueueFileClicked; // Implement missing event

    private Button btnConnect;
    private Button btnOpenQueue;
    private Button btnOpenOtherForm; // Add button for other form
    private TextBox txtServerAddress;
    private TextBox txtPortNumber; // Add TextBox for Port Number
    private TextBox txtUsername;
    private TextBox txtPassword;
    private CheckBox chkFtp; // Add CheckBox for FTP
    private CheckBox chkSftp; // Add CheckBox for SFTP

    public MainForm()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        this.Text = "FileTransferClient";
        this.Width = 800;
        this.Height = 600;

        Label lblServerAddress = new Label
        {
            Text = "Server Address:",
            Location = new System.Drawing.Point(20, 20),
            Width = 100
        };
        this.Controls.Add(lblServerAddress);

        txtServerAddress = new TextBox
        {
            Location = new System.Drawing.Point(140, 20),
            Width = 200
        };
        this.Controls.Add(txtServerAddress);

        Label lblPortNumber = new Label
        {
            Text = "Port Number:",
            Location = new System.Drawing.Point(20, 60),
            Width = 100
        };
        this.Controls.Add(lblPortNumber);

        txtPortNumber = new TextBox
        {
            Location = new System.Drawing.Point(140, 60),
            Width = 200
        };
        this.Controls.Add(txtPortNumber);

        Label lblUsername = new Label
        {
            Text = "Username:",
            Location = new System.Drawing.Point(20, 100),
            Width = 100
        };
        this.Controls.Add(lblUsername);

        txtUsername = new TextBox
        {
            Location = new System.Drawing.Point(140, 100),
            Width = 200
        };
        this.Controls.Add(txtUsername);

        Label lblPassword = new Label
        {
            Text = "Password:",
            Location = new System.Drawing.Point(20, 140),
            Width = 100
        };
        this.Controls.Add(lblPassword);

        txtPassword = new TextBox
        {
            Location = new System.Drawing.Point(140, 140),
            Width = 200,
            PasswordChar = '*'
        };
        this.Controls.Add(txtPassword);

        chkFtp = new CheckBox
        {
            Text = "FTP",
            Location = new System.Drawing.Point(20, 180),
            Width = 50
        };
        chkFtp.CheckedChanged += (s, e) => { if (chkFtp.Checked) chkSftp.Checked = false; };
        this.Controls.Add(chkFtp);

        chkSftp = new CheckBox
        {
            Text = "SFTP",
            Location = new System.Drawing.Point(80, 180),
            Width = 50
        };
        chkSftp.CheckedChanged += (s, e) => { if (chkSftp.Checked) chkFtp.Checked = false; };
        this.Controls.Add(chkSftp);

        btnConnect = new Button
        {
            Text = "Connect",
            Location = new System.Drawing.Point(20, 220),
            Width = 100
        };
        btnConnect.Click += (s, e) => ConnectClicked?.Invoke(this, EventArgs.Empty);
        this.Controls.Add(btnConnect);

        btnOpenQueue = new Button
        {
            Text = "Transfer Queue",
            Location = new System.Drawing.Point(140, 220),
            Width = 120
        };
        btnOpenQueue.Click += (s, e) => OpenTransferQueueClicked?.Invoke(this, EventArgs.Empty);
        this.Controls.Add(btnOpenQueue);

        btnOpenOtherForm = new Button
        {
            Text = "Open Other Form",
            Location = new System.Drawing.Point(280, 220),
            Width = 120
        };
        btnOpenOtherForm.Click += (s, e) => OpenOtherFormClicked?.Invoke(this, EventArgs.Empty);
        this.Controls.Add(btnOpenOtherForm);
    }

    public void ShowMessage(string message, string title, MessageBoxIcon icon)
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
    }

    public string ServerAddress => txtServerAddress.Text;
    public int PortNumber => int.TryParse(txtPortNumber.Text, out var port) ? port : 21; // Default to 21 if invalid
    public string Username => txtUsername.Text;
    public string Password => txtPassword.Text;
    public bool IsFtp => chkFtp.Checked;
    public bool IsSftp => chkSftp.Checked;
}

