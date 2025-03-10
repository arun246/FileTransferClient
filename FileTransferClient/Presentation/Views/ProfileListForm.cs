using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FileTransferClient.Domain.Models.Connection;

namespace FileTransferClient.Presentation.Views
{
    public partial class ProfileListForm : Form
    {
        public ProfileListForm(List<ServerProfile> profiles)
        {
            InitializeComponent();
            LoadProfiles(profiles);
        }

        private void LoadProfiles(List<ServerProfile> profiles)
        {
            dataGridViewProfiles.DataSource = profiles;
        }
    }
}
