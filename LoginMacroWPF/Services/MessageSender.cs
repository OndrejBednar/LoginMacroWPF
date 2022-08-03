using LoginMacroWPF.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LoginMacroWPF.Helpers;
using static LoginMacroWPF.PassManager;

namespace LoginMacroWPF.Services
{
    public class MessageSender
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public static void DoMouseClick()
        {
            //perform click            
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        public static void DoMouseDoubleClick()
        {
            //perform click            
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }


        public static void Login(Summoner summoner, bool isError)
        {
            IntPtr ClientPtr;
            ClientPtr = Process.GetProcessesByName("RiotClientUx").FirstOrDefault().MainWindowHandle;
            var rect = new Rect();
            GetWindowRect(ClientPtr, ref rect);
            SetForegroundWindow(ClientPtr);
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
            SetCursorPos((int)((rect.Right * 0.5) + (rect.Left * 0.5)), (int)((rect.Bottom * 0.3) + (rect.Top * 0.7)));
            DoMouseClick();
            SendKeys.SendWait("{TAB}");
            if (isError)
            {
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{TAB}");
            }
            SendKeys.SendWait($"{summoner.Username}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait($"{Decrypt(summoner.Password)}");
            SendKeys.SendWait("{ENTER}");
        }
        public static void Login(SteamAcc acc)
        {
            IntPtr ClientPtr;
            ClientPtr = Process.GetProcessesByName("Steam").FirstOrDefault().MainWindowHandle;
            var rect = new Rect();
            GetWindowRect(ClientPtr, ref rect);
            SetForegroundWindow(ClientPtr);
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
            SetCursorPos((int)((rect.Right * 0.5) + (rect.Left * 0.5)), (int)((rect.Bottom * 0.3) + (rect.Top * 0.7)));
            DoMouseDoubleClick();
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
            SendKeys.SendWait($"{acc.Username}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait($"{Decrypt(acc.Password)}");
        }
        public static void Startup()
        {
            DateTime controlTime;
            if (Environment.CurrentDirectory == @"C:\Windows\system32")
            {
                return;
            }
            //find if the laucher is already opened
            if (Process.GetProcessesByName("RiotClientUx").FirstOrDefault() == null)
            {
                controlTime = DateTime.Now;
                if (!File.Exists(@"C:\Riot Games\League of Legends\LeagueClient.exe"))
                {
                    MessageBox.Show("Theres no league of legends installed !");
                    return;
                }

                //if not then start it && wait for it to be idle
                Process.Start(@"C:\Riot Games\League of Legends\LeagueClient.exe");
                while (Process.GetProcessesByName("RiotClientUx").FirstOrDefault() == null || Process.GetProcessesByName("RiotClientUx").FirstOrDefault().MainWindowHandle.ToInt32() == 0)
                {
                    if (DateTime.Now - controlTime > TimeSpan.FromSeconds(20))
                    {
                        if (MessageBox.Show("There was an error while launching the game. Please launch the game yourself.") == DialogResult.OK)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            /* attempt of screening client
            //getting right now active(used) window so we can put it back on later
            IntPtr active = GetForegroundWindow();
            SetForegroundWindow(ClientPtr);

            //taking a screenshot to determine where the login is and if the client is loaded
            var image = ScreenCapture.CaptureWindow(ClientPtr);


            SetForegroundWindow(active);
            image.Save(@"C:\snippetsource.jpg", ImageFormat.Jpeg);
            */
        }
    }
}
