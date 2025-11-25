namespace STAMP_GUI
{
    public partial class Form1 : Form
    {
        private STAMP_Communicator stampCommunicator;



        public Form1()
        {
            InitializeComponent();
            stampCommunicator = new STAMP_Communicator();

            ConnectedToStamp(false);
        }

        private async void buttonConnectToSTAMP_Click(object? sender, EventArgs e)
        {
            bool success = false;

            try
            {
                buttonConnectToSTAMP.Enabled = false;
                success = await stampCommunicator!.Connect();
            }
            catch (Exception ex)
            {
                success = false;
            }
            finally
            {
                buttonConnectToSTAMP.Enabled = true;
                ConnectedToStamp(success);
            }
        }

        private void ConnectedToStamp(bool success)
        {
            if (success)
            {
                labelSTAMP.BackColor = Color.GreenYellow;
                labelSTAMP.Text = "Connected";
            }
            else
            {
                labelSTAMP.BackColor = Color.PaleVioletRed;
                labelSTAMP.Text = "Disconnected";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up STAMP communicator
                if (stampCommunicator != null)
                {
                    stampCommunicator.Dispose();
                    stampCommunicator = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void buttonStartSTAMP_Click(object sender, EventArgs e)
        {
            stampCommunicator.SendControlCommand(GUIApp.Comm.Command.Start);
        }

        private void buttonStopSTAMP_Click(object sender, EventArgs e)
        {
            stampCommunicator.SendControlCommand(GUIApp.Comm.Command.Stop);
        }

        private void buttonApplyStampStatus_Click(object sender, EventArgs e)
        {
            if (radioSTAMP_Status_init.Checked)
                stampCommunicator.SendSubsystemStatusMessage(GUIApp.Comm.SystemStatus.StatusInitializing);
            else if (radioSTAMP_Status_Normal.Checked)
                stampCommunicator.SendSubsystemStatusMessage(GUIApp.Comm.SystemStatus.StatusNormal);
            else if (radioSTAMP_Status_Degraded.Checked)
                stampCommunicator.SendSubsystemStatusMessage(GUIApp.Comm.SystemStatus.StatusDegraded);
            else if (radioSTAMP_Status_Inoperable.Checked)
                stampCommunicator.SendSubsystemStatusMessage(GUIApp.Comm.SystemStatus.StatusInoprable);
        }
    }
}
