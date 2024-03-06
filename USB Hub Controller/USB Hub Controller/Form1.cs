using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace USB_Hub_Controller
{
    public partial class Form1 : Form
    {
        bool control=false; //kontrol değişkeni
        public Form1()
        {
            InitializeComponent();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                control = true;
                ExecuteDevconCommand("enable", "USB\\*"); //aktif etme
            }
            else if (radioButton2.Checked)
            {
                ExecuteDevconCommand("disable", "USB\\*");// devre dışı bırakma
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close(); //İptal tuşu
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (IsUserAdmin() == false)
            {
                //adminlik sorgusu sonuç mesajı
                MessageBox.Show("Uygulamayı yönetici olarak çalıştırmalısınız, aksi takdirde değişiklik yapamazsınız !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        #region devcon.exe işlemleri
        private void ExecuteDevconCommand(string action, string deviceFilter)
        {
            // ProcessStartInfo sınıfını kullanarak yeni bir işlem başlatmak için bilgi oluşturma kısmı
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "\"C:\\Program Files (x86)\\Windows Kits\\10\\Tools\\10.0.22621.0\\x64\\devcon.exe\"",
                Arguments = $"{action} {deviceFilter}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            // Process sınıfını kullanarak yeni bir işlem oluşturma
            using (Process process = new Process { StartInfo = processInfo })
            {
                process.Start();
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                #region bilgilendirme ve hata mesajları
                if (!string.IsNullOrEmpty(output))
                {
                    MessageBox.Show(output, "Bilgilendirme Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    notification.BalloonTipTitle = "Bilgilendirme";
                    if (control == true)
                    {
                        notification.BalloonTipText = "USB HUB'ları aktif edildi.";
                    }
                    else
                    {
                        notification.BalloonTipText = "USB HUB'ları devre dışı bırakıldı.";
                    }
                    notification.Icon = SystemIcons.Information;
                    notification.ShowBalloonTip(3000);
                   
                }
                if (!string.IsNullOrEmpty(error))
                    MessageBox.Show(error, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                #endregion
            }
        }
        #endregion
        #region Adminlik Sorgusu
        [DllImport("shell32")]
        static extern bool IsUserAnAdmin();

        public static bool IsUserAdmin() 
        {
            bool isAdmin = IsUserAnAdmin();
            return isAdmin;
        }
        #endregion 
    }
}
