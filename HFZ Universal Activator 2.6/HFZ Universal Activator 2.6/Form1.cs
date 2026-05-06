using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using MobileDevice;
using MobileDevice.Event;
using Renci.SshNet;

namespace HFZ_Universal_Activator_2._6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeDetection();
        }

        Process proceso = new Process();
        public iOSDeviceManager manager = new iOSDeviceManager();
        public iOSDevice currentiOSDevice;
        public SshClient Ssh = new SshClient("127.0.0.1", "root", "alpine");
        public ScpClient Scp = new ScpClient("127.0.0.1", "root", "alpine");

        static string Server_uri = "https://ex3cution3r.com/HFZ";
        public string SheLL(string Command)
        {
            File.WriteAllText("tmp\\shell.cmd", "@echo off\n" + Command);
            proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tmp\\shell.cmd",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                },
            };
            proceso.Start();
            StreamReader reader = proceso.StandardOutput;
            return reader.ReadToEnd();
        }
        public void BoxShow(string Arg, string Caption)
        {
            MessageBox.Show(Arg, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void BoxShowError(string Arg, string Caption)
        {
            MessageBox.Show(Arg, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ListenError(object sender, ListenErrorEventHandlerEventArgs args)
        {
            if (args.ErrorType == MobileDevice.Enumerates.ListenErrorEventType.StartListen)
            {
                string ERROR = args.ErrorMessage;
                Exception e = new Exception(ERROR);
                BoxShowError(e.Message, "ERROR");
            }
        }
        public void InitializeDetection()
        {
            if (SheLL("lib\\curl.exe -s -k " + Server_uri + "/Version.php") != "2.1")
            {
                BoxShowError("This version is deprectaed! Please, try download new version", "ERROR");
                Close();
            }
            SheLL("C:\\Windows\\System32\\TASKKILL /IM iproxy.exe /F");
            string Known = "%USERPROFILE%\\.ssh\\known_hosts";
            if (File.Exists(Known))
            {
                File.Delete(Known);
            }
            string iTunes = "iTunesMobileDevice.dll";
            string X86 = "C:\\Program Files\\Common Files\\Apple\\Mobile Device Support\\" + iTunes + "";
            string X64 = "C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support\\" + iTunes + "";
            if (!Directory.Exists("C:\\Program Files\\Common Files\\Apple\\Mobile Device Support"))
            {
                if (!Directory.Exists("C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support"))
                {
                    BoxShow("Install iTunes or Drivers with 3UTools", "ERROR");
                    Close();
                }
            }
            else
            {
                if (!File.Exists(X86))
                {
                    File.Copy(iTunes, X86);

                }
                if (!File.Exists(X64))
                {
                    File.Copy(iTunes, X64);
                }
                CheckiTunes();
            }
        }

        public void CheckiTunes()
        {
            if (File.Exists("C:\\Program Files\\Common Files\\Apple\\Mobile Device Support\\iTunesMobileDevice.dll"))
            {
                if (File.Exists("C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support\\iTunesMobileDevice.dll"))
                {
                    manager.CommonConnectEvent += CommonConnectDevice; manager.ListenErrorEvent += ListenError; manager.StartListen();
                }
            }

        }

        private void CommonConnectDevice(object sender, DeviceCommonConnectEventArgs args)
        {
            if (args.Message == MobileDevice.Enumerates.ConnectNotificationMessage.Connected)
            {
                currentiOSDevice = args.Device;
                SetData(true);

            }
            if (args.Message == MobileDevice.Enumerates.ConnectNotificationMessage.Disconnected)
            {
                SetData(false);
            }
        }
        public string DeviceInfo(string Info)
        {
            string CMD = "@echo off\nlib\\ideviceinfo.exe | lib\\grep.exe -w " + Info + " | lib\\awk.exe '{printf $NF}'";
            File.WriteAllText("tmp\\Info.cmd", CMD);
            proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tmp\\Info.cmd",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
            };
            proceso.Start();
            StreamReader Information = proceso.StandardOutput;
            string Final = Information.ReadToEnd();
            //  proceso.WaitForExit();
            return Final;
        }

        public void SetData(bool Valor)
        {
            if(Valor != false)
            {
                Invoke((MethodInvoker)(() => saaLabel3.Text = "Connected Device: " + DeviceInfo("ProductType") + " iOS " + DeviceInfo("ProductVersion")));
                Invoke((MethodInvoker)(() => saaLabel4.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel5.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel6.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel8.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel4.Text = "UDID: " + DeviceInfo("UniqueDeviceID")));
                Invoke((MethodInvoker)(() => saaLabel5.Text = "IMEI: " + DeviceInfo("InternationalMobileEquipmentIdentity")));
                Invoke((MethodInvoker)(() => saaLabel6.Text = "SN: " + DeviceInfo("SerialNumber")));
                Invoke((MethodInvoker)(() => saaButton1.Value = "Activate"));
                Invoke((MethodInvoker)(() => saaLabel8.Text = "STATUS: " + DeviceInfo("ActivationState"))); 
            }
            else
            {
                Invoke((MethodInvoker)(() => saaLabel3.Text = "No Device connected"));
                Invoke((MethodInvoker)(() => saaLabel4.Visible = false));
                Invoke((MethodInvoker)(() => saaLabel5.Visible = false));
                Invoke((MethodInvoker)(() => saaLabel6.Visible = false));
                Invoke((MethodInvoker)(() => saaLabel8.Visible = false));
                Invoke((MethodInvoker)(() => saaButton1.Value = "Waiting For Device ..."));
            }
        }

        public void CheckSIMLockCarrier()
        {
            Invoke((MethodInvoker)(() => saaButton2.Enabled = false));
            Invoke((MethodInvoker)(() => saaButton2.Value = "Checking..."));
            if (DeviceInfo("SerialNumber") != "")
            {
                SheLL("lib\\ideviceactivation.exe activate -d -s https://ex3cution3r.com/simlock.php");
                string Ruta = "https://ex3cution3r.com/CarrierResponses/" + DeviceInfo("SerialNumber") + "/response.txt";
                string getResponse = SheLL("lib\\curl.exe -s -k " + Ruta);
                if(getResponse == "Unlocked")
                {
                    BoxShow("SIMLock | Network Status: Unlocked", "CARRIER UNLOCKED");
                }
                else
                {
                    BoxShow("SIMLock | Network Status: " + getResponse, "CARRIER");
                }
            }
            Invoke((MethodInvoker)(() => saaButton2.Value = "SIM-Lock Check"));
            Invoke((MethodInvoker)(() => saaButton2.Enabled = true));

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckSIMLockCarrier();
        }

        private void saaButton2_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        public bool CheckJB()
        {
            SheLL("C:\\Windows\\System32\\TASKKILL.exe /IM iproxy.exe /F");
            SheLL("ERASE %USERPROFILE%\\.ssh\\known_hosts");
            proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "lib\\iproxy.exe",
                    Arguments = "22 44",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                },
            };
            proceso.Start();
            try
            {
                if (!Ssh.IsConnected)
                {
                    Ssh.Connect();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ActivateDevice()
        {
            Invoke((MethodInvoker)(() => saaButton1.Enabled = false));
            Invoke((MethodInvoker)(() => saaButton1.Value = "Activating..."));
            if (DeviceInfo("SerialNumber") == "")
            {
                BoxShowError("Device undetected!", "ERROR");
                Invoke((MethodInvoker)(() => saaButton1.Enabled = true));
                Invoke((MethodInvoker)(() => saaButton1.Value = "Activate"));
            }
            if(CheckJB() == false)
            {
                BoxShowError("Jailbreak Undetected!", "ERROR");
                Invoke((MethodInvoker)(() => saaButton1.Enabled = true));
                Invoke((MethodInvoker)(() => saaButton1.Value = "Activate"));
            }
            SheLL("lib\\ideviceactivation.exe activate -d -s https://ex3cution3r.com/MEID.php");
            string Wildcard = SheLL("lib\\curl.exe -s -k https://ex3cution3r.com/WildcardsTickets/" + DeviceInfo("SerialNumber") + "/Wildcard.der");
            if(Wildcard == "")
            {
                BoxShowError("SerialNumber Unregistered!", "ERROR");
                return;
            }
            string Record = SheLL("lib\\curl.exe -s -k \"https://ex3cution3r.com/MEID.php?activation-record=&serial=" + DeviceInfo("SerialNumber") + "&udid=" + DeviceInfo("UniqueDeviceID") + "&ucid=" + DeviceInfo("UniqueChipID") + "\"");
            if (!Directory.Exists("ActivationFiles"))
            {
                Directory.CreateDirectory("ActivationFiles");
            }
            File.WriteAllText("ActivationFiles\\activation_record.plist", Record);
            string CommCenter = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">\n<plist version=\"1.0\">\n<dict>\n\t<key>ne_config_state</key>\n\t<false/>\n\t<key>imeis</key>\n\t<array>\n\t\t<dict>\n\t\t\t<key>second</key>\n\t\t\t<string>" + DeviceInfo("InternationalMobileEquipmentIdentity") + "</string>\n\t\t\t<key>first</key>\n\t\t\t<string>1:kOne</string>\n\t\t</dict>\n\t</array>\n\t<key>imei_svn</key>\n\t<string>2A</string>\n\t<key>kNextCarrierBundleUpdateCheck</key>\n\t<date>2023-05-22T02:51:55Z</date>\n\t<key>imei</key>\n\t<string>" + DeviceInfo("InternationalMobileEquipmentIdentity") + "</string>\n\t<key>kPostponementTicket</key>\n\t<dict>\n\t\t<key>ActivationTicket</key>\n\t\t<string>" + Wildcard + "</string>\n\t\t<key>ActivityURL</key>\n\t\t<string>https://albert.apple.com/deviceservices/activity</string>\n\t\t<key>PhoneNumberNotificationURL</key>\n\t\t<string>https://albert.apple.com/deviceservices/phoneHome</string>\n\t\t<key>ActivationState</key>\n\t\t<string>Activated</string>\n\t</dict>\n\t<key>meid</key>\n\t<string>" + DeviceInfo("MobileEquipmentIdentifier") + "</string>\n\t<key>activation_gemini_support</key>\n\t<string>1:kFalse</string>\n\t<key>imei_svns</key>\n\t<array>\n\t\t<dict>\n\t\t\t<key>second</key>\n\t\t\t<string>2A</string>\n\t\t\t<key>first</key>\n\t\t\t<string>1:kOne</string>\n\t\t</dict>\n\t</array>\n\t<key>subscriber_account_ids</key>\n\t<array>\n\t\t<dict>\n\t\t\t<key>second</key>\n\t\t\t<string></string>\n\t\t\t<key>first</key>\n\t\t\t<string>1:kOne</string>\n\t\t</dict>\n\t</array>\n</dict>\n</plist>\n";
            File.WriteAllText("ActivationFiles\\com.apple.commcenter.device_specific_nobackup.plist", CommCenter);
            if (!Scp.IsConnected)
            {
                Scp.Connect();
            }
            Scp.Upload(new FileInfo("ActivationFiles\\com.apple.commcenter.device_specific_nobackup.plist"), "/private/var/mobile/Media/Downloads/com.apple.commcenter.device_specific_nobackup.plist");
            Scp.Upload(new FileInfo("ActivationFiles\\activation_record.plist"), "/private/var/mobile/Media/Downloads/activation_record.plist");
            Ssh.CreateCommand("cd /private/var/containers/Data/System/*/Library/internal/../; rm -rf activation_records").Execute();
            Ssh.CreateCommand("cd /private/var/containers/Data/System/*/Library/internal/../; mkdir -m 755 activation_records").Execute();
            Ssh.CreateCommand("cd /private/var/containers/Data/System/*/Library/internal/../; mv -f /private/var/mobile/Media/Downloads/activation_record.plist activation_records/").Execute();
            Ssh.CreateCommand("chflags nouchg /private/var/wireless/Library/Preferences/com.apple.commcenter.device_specific_nobackup.plist").Execute();
            Ssh.CreateCommand("rm /private/var/wireless/Library/Preferences/com.apple.commcenter.device_specific_nobackup.plist").Execute();
            Ssh.CreateCommand("mv -f /private/var/mobile/Media/Downloads/com.apple.commcenter.device_specific_nobackup.plist /private/var/wireless/Library/").Execute();
            Ssh.CreateCommand("cd /private/var/wireless/Library; mv -f com.apple.commcenter.device_specific_nobackup.plist Preferences/").Execute();
            Ssh.CreateCommand("chown _wireless:_wireless /private/var/wireless/Library/Preferences/com.apple.commcenter.device_specific_nobackup.plist").Execute();
            Ssh.CreateCommand("chmod 0600 /private/var/wireless/Library/Preferences/com.apple.commcenter.device_specific_nobackup.plist").Execute();
            Ssh.CreateCommand("chflags uchg /private/var/wireless/Library/Preferences/com.apple.commcenter.device_specific_nobackup.plist").Execute();
            if(guna2CheckBox1.Checked == true)
            {
                Ssh.CreateCommand("chflags nouchg /private/var/mobile/Library/Preferences/com.apple.purplebuddy.plist").Execute();
                Ssh.CreateCommand("rm /private/var/mobile/Library/Preferences/com.apple.purplebuddy.plist").Execute();
                Scp.Upload(new FileInfo("lib\\purplebuddy"), "/private/var/mobile/Media/Downloads/com.apple.purplebuddy.plist");
                Ssh.CreateCommand("mv -f /private/var/mobile/Media/Downloads/com.apple.purplebuddy.plist /private/var/mobile/Library/Preferences/").Execute();
                Ssh.CreateCommand("chown mobile:mobile /private/var/mobile/Library/Preferences/com.apple.purplebuddy.plist").Execute();
                Ssh.CreateCommand("chmod 0600 /private/var/mobile/Library/Preferences/com.apple.purplebuddy.plist").Execute();
                Ssh.CreateCommand("chflags uchg /private/var/mobile/Library/Preferences/com.apple.purplebuddy.plist").Execute();
            }
            if(guna2CheckBox2.Checked == true)
            {
                Ssh.CreateCommand("cd /System/Library && launchctl unload -w -F LaunchDaemons/com.apple.softwareupdateservicesd.plist").Execute();
                Ssh.CreateCommand("cd /System/Library && launchctl unload -w -F LaunchDaemons/com.apple.mobile.softwareupdated.plist").Execute();
                Ssh.CreateCommand("cd /System/Library && launchctl unload -w -F LaunchDaemons/com.apple.OTATaskingAgent.plist").Execute();
            }
            Ssh.CreateCommand("launchctl unload /System/Library/LaunchDaemons && launchctl load /System/Library/LaunchDaemons").Execute();
            Thread.Sleep(7000);
            if(DeviceInfo("ActivationState") != "Unactivated")
            {
                Ssh.CreateCommand("launchctl reboot").Execute();
                BoxShow("Successfully Activated Device!", "MESSAGE");
            }
            else
            {
                BoxShowError("Upss! Error, try again", "ERROR");
            }
            Directory.Delete("ActivationFiles");
            Invoke((MethodInvoker)(() => saaButton1.Enabled = true));
            Invoke((MethodInvoker)(() => saaButton1.Value = "Activate"));
        }

        private void backgroundWorker2_DoWork_1(object sender, DoWorkEventArgs e)
        {
            ActivateDevice();
        }

        private void saaButton1_Click_1(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

        private void saaLabel6_Click(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)(() => Clipboard.SetText(DeviceInfo("SerialNumber"))));
            Invoke((MethodInvoker)(() => saaLabel7.Visible = true));
        }

        private void saaButton3_Click(object sender, EventArgs e)
        {
            SheLL("CD HFZRa1n\n.\\HFZRa1n.exe");
        }
    }
}
