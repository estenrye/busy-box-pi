using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using UARTLogger;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace busy_box_pi_gui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Logger log = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void logWrite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (log == null)
                {
                    log = new Logger();
                    await log.Initialize();
                }
                await log.WriteLog("Hello World!");
            }
            catch (Exception ex)
            {
                this.logOutput.Text = ex.Message;
            }
        }
    }
}
