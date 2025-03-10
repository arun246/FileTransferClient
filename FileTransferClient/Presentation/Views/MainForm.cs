using FileTransferClient.Presentation.Contracts;
using System.Windows.Forms;
using System;
using FileTransferClient.Domain.Models.Connection;
using FileTransferClient.Domain.Models;
using FileTransferClient.Infrastructure.Logging;
using FileTransferClient.Presentation.Controls.FileBrowser;
using FileTransferClient.Presentation.Controls.TransferQueue;
using FileTransferClient.Domain.Models.Transfer;
using FileTransferClient.Application.Factories;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using FileTransferClient.Infrastructure.FileTransfer;

public partial class MainForm : Form, IMainView
{
    // Public events for Presenter interaction
    public event EventHandler ConnectClicked;
    public event EventHandler OpenTransferQueueClicked;
    public event EventHandler<FileItem> EnqueueFileClicked;
    public event EventHandler ViewProfilesClicked;

    private Button btnConnect;
    private Button btnOpenQueue;
    private Button btnViewProfiles;
    private Button btnCreateFile;
    private TextBox txtServerAddress;
    private TextBox txtPortNumber;
    private TextBox txtUsername;
    private TextBox txtPassword;
    private CheckBox chkFtp;
    private CheckBox chkSftp;
    private ComboBox cmbEncryption;
    private TextBox txtLog;
    private FileBrowserControl fileBrowserControl;
    private TransferItemControl transferItemControl;

    public MainForm()
    {
        SetupUI();
        EventLogger.Initialize(AppendLog);
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

        Label lblEncryption = new Label
        {
            Text = "Encryption:",
            Location = new System.Drawing.Point(20, 220),
            Width = 100
        };
        this.Controls.Add(lblEncryption);

        cmbEncryption = new ComboBox
        {
            Location = new System.Drawing.Point(140, 220),
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbEncryption.Items.AddRange(new string[] { "None", "Explicit", "Implicit" });
        cmbEncryption.SelectedIndex = 0;
        this.Controls.Add(cmbEncryption);

        btnConnect = new Button
        {
            Text = "Connect",
            Location = new System.Drawing.Point(20, 260),
            Width = 100
        };
        btnConnect.Click += btnConnect_Click;
        this.Controls.Add(btnConnect);

        btnOpenQueue = new Button
        {
            Text = "Transfer Queue",
            Location = new System.Drawing.Point(140, 260),
            Width = 120
        };
        btnOpenQueue.Click += (s, e) => OpenTransferQueueClicked?.Invoke(this, EventArgs.Empty);
        this.Controls.Add(btnOpenQueue);

        btnViewProfiles = new Button
        {
            Text = "View Profiles",
            Location = new System.Drawing.Point(280, 260),
            Width = 120
        };
        btnViewProfiles.Click += (s, e) => ViewProfilesClicked?.Invoke(this, EventArgs.Empty);
        this.Controls.Add(btnViewProfiles);

        btnCreateFile = new Button
        {
            Text = "Create File",
            Location = new System.Drawing.Point(420, 260),
            Width = 100
        };
        btnCreateFile.Click += btnCreateFile_Click;
        this.Controls.Add(btnCreateFile);

        txtLog = new TextBox
        {
            Location = new System.Drawing.Point(20, 300),
            Width = 740,
            Height = 100,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical
        };
        this.Controls.Add(txtLog);

        fileBrowserControl = new FileBrowserControl
        {
            Location = new System.Drawing.Point(20, 410),
            Width = 740,
            Height = 200
        };
        this.Controls.Add(fileBrowserControl);

        transferItemControl = new TransferItemControl(new TransferJob("example.txt", "destinationPath", TransferPriority.Normal, new ServerProfile("host", 21, "username", "password")))
        {
            Location = new System.Drawing.Point(20, 620)
        };
        this.Controls.Add(transferItemControl);
    }

    private void AppendLog(string message)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(new Action(() => txtLog.AppendText(message + Environment.NewLine)));
        }
        else
        {
            txtLog.AppendText(message + Environment.NewLine);
        }
    }

    private void SaveEncryptionSetting(string encryption)
    {
        var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        var json = File.ReadAllText(configFilePath);
        var jsonObj = JObject.Parse(json);
        jsonObj["EncryptionSettings"]["EncryptionType"] = encryption;
        File.WriteAllText(configFilePath, jsonObj.ToString());
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        SaveEncryptionSetting(cmbEncryption.SelectedItem.ToString());
        await LoadFtpDirectoryAsync("/"); // Load the root directory or any specific directory
        ConnectClicked?.Invoke(this, EventArgs.Empty);
    }

    private async void btnCreateFile_Click(object sender, EventArgs e)
    {
        var directoryPath = fileBrowserControl.GetCurrentDirectoryPath(); // Use the new method to get the current directory
        var newFileName = "NewFile.txt"; // You can prompt the user for a file name if needed
        var newFilePath = Path.Combine(directoryPath, newFileName);

        try
        {
            var ftpStrategy = new FtpStrategy(cmbEncryption.SelectedItem.ToString());
            var serverProfile = new ServerProfile(txtServerAddress.Text, int.Parse(txtPortNumber.Text), txtUsername.Text, txtPassword.Text);

            await ftpStrategy.CreateFileAsync(serverProfile, newFilePath);

            await LoadFtpDirectoryAsync(directoryPath); // Refresh the directory listing
            ShowMessage("File created successfully.", "Success", MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowMessage("Failed to create file: " + ex.Message, "Error", MessageBoxIcon.Error);
        }
    }

    private async Task LoadFtpDirectoryAsync(string directoryPath)
    {
        var ftpStrategy = new FtpStrategy(cmbEncryption.SelectedItem.ToString());
        var serverProfile = new ServerProfile(txtServerAddress.Text, int.Parse(txtPortNumber.Text), txtUsername.Text, txtPassword.Text);

        try
        {
            var fileItems = await ftpStrategy.ListDirectoryAsync(serverProfile, directoryPath);
            fileBrowserControl.DisplayFiles(fileItems);
        }
        catch (Exception ex)
        {
            ShowMessage("Failed to load directory contents: " + ex.Message, "Error", MessageBoxIcon.Error);
        }
    }

    public void ShowMessage(string message, string title, MessageBoxIcon icon)
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
    }

    public string ServerAddress => txtServerAddress.Text;
    public int PortNumber => int.TryParse(txtPortNumber.Text, out var port) ? port : 21;
    public string Username => txtUsername.Text;
    public string Password => txtPassword.Text;
    public bool IsFtp => chkFtp.Checked;
    public bool IsSftp => chkSftp.Checked;
    public string Encryption => cmbEncryption.SelectedItem.ToString();
}

