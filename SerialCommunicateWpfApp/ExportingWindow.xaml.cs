using System;
using System.Windows;

namespace SerialCommunicateWpfApp
{
    public partial class ExportingWindow : Window
    {
        public Action CancelBtnClicked { get; set; }
        public ExportingWindow()
        {
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        protected virtual void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            CancelBtnClicked();
        }
    }
}
