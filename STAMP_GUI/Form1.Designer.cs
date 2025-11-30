namespace STAMP_GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonConnectToSTAMP = new Button();
            groupSTAMP = new GroupBox();
            buttonStopSTAMP = new Button();
            buttonStartSTAMP = new Button();
            groupSTAMP_BIT = new GroupBox();
            groupSTAMP_Status = new GroupBox();
            buttonApplyStampStatus = new Button();
            radioSTAMP_Status_Inoperable = new RadioButton();
            radioSTAMP_Status_Degraded = new RadioButton();
            radioSTAMP_Status_Normal = new RadioButton();
            radioSTAMP_Status_init = new RadioButton();
            labelSTAMP = new Label();
            groupAIECS = new GroupBox();
            buttonSendZeroizeRequest = new Button();
            buttonAIECS_RequestBIT = new Button();
            buttonStopAIECS = new Button();
            buttonStartAIECS = new Button();
            buttonConnectAIECS = new Button();
            label1 = new Label();
            groupSTAMP.SuspendLayout();
            groupSTAMP_Status.SuspendLayout();
            groupAIECS.SuspendLayout();
            SuspendLayout();
            // 
            // buttonConnectToSTAMP
            // 
            buttonConnectToSTAMP.Location = new Point(52, 22);
            buttonConnectToSTAMP.Name = "buttonConnectToSTAMP";
            buttonConnectToSTAMP.Size = new Size(150, 30);
            buttonConnectToSTAMP.TabIndex = 0;
            buttonConnectToSTAMP.Text = "Connect";
            buttonConnectToSTAMP.UseVisualStyleBackColor = true;
            buttonConnectToSTAMP.Click += buttonConnectToSTAMP_Click;
            // 
            // groupSTAMP
            // 
            groupSTAMP.Controls.Add(buttonStopSTAMP);
            groupSTAMP.Controls.Add(buttonStartSTAMP);
            groupSTAMP.Controls.Add(groupSTAMP_BIT);
            groupSTAMP.Controls.Add(groupSTAMP_Status);
            groupSTAMP.Controls.Add(labelSTAMP);
            groupSTAMP.Controls.Add(buttonConnectToSTAMP);
            groupSTAMP.Location = new Point(12, 12);
            groupSTAMP.Name = "groupSTAMP";
            groupSTAMP.Size = new Size(282, 558);
            groupSTAMP.TabIndex = 1;
            groupSTAMP.TabStop = false;
            groupSTAMP.Text = "STAMP";
            // 
            // buttonStopSTAMP
            // 
            buttonStopSTAMP.Location = new Point(142, 58);
            buttonStopSTAMP.Name = "buttonStopSTAMP";
            buttonStopSTAMP.Size = new Size(75, 48);
            buttonStopSTAMP.TabIndex = 5;
            buttonStopSTAMP.Text = "Stop STAMP";
            buttonStopSTAMP.UseVisualStyleBackColor = true;
            buttonStopSTAMP.Click += buttonStopSTAMP_Click;
            // 
            // buttonStartSTAMP
            // 
            buttonStartSTAMP.Location = new Point(30, 58);
            buttonStartSTAMP.Name = "buttonStartSTAMP";
            buttonStartSTAMP.Size = new Size(75, 48);
            buttonStartSTAMP.TabIndex = 4;
            buttonStartSTAMP.Text = "Start STAMP";
            buttonStartSTAMP.UseVisualStyleBackColor = true;
            buttonStartSTAMP.Click += buttonStartSTAMP_Click;
            // 
            // groupSTAMP_BIT
            // 
            groupSTAMP_BIT.Location = new Point(12, 260);
            groupSTAMP_BIT.Name = "groupSTAMP_BIT";
            groupSTAMP_BIT.Size = new Size(264, 163);
            groupSTAMP_BIT.TabIndex = 3;
            groupSTAMP_BIT.TabStop = false;
            groupSTAMP_BIT.Text = "BIT Response";
            // 
            // groupSTAMP_Status
            // 
            groupSTAMP_Status.Controls.Add(buttonApplyStampStatus);
            groupSTAMP_Status.Controls.Add(radioSTAMP_Status_Inoperable);
            groupSTAMP_Status.Controls.Add(radioSTAMP_Status_Degraded);
            groupSTAMP_Status.Controls.Add(radioSTAMP_Status_Normal);
            groupSTAMP_Status.Controls.Add(radioSTAMP_Status_init);
            groupSTAMP_Status.Location = new Point(12, 128);
            groupSTAMP_Status.Name = "groupSTAMP_Status";
            groupSTAMP_Status.Size = new Size(244, 123);
            groupSTAMP_Status.TabIndex = 2;
            groupSTAMP_Status.TabStop = false;
            groupSTAMP_Status.Text = "Subsystem Status";
            // 
            // buttonApplyStampStatus
            // 
            buttonApplyStampStatus.Location = new Point(130, 46);
            buttonApplyStampStatus.Name = "buttonApplyStampStatus";
            buttonApplyStampStatus.Size = new Size(75, 43);
            buttonApplyStampStatus.TabIndex = 4;
            buttonApplyStampStatus.Text = "Apply";
            buttonApplyStampStatus.UseVisualStyleBackColor = true;
            buttonApplyStampStatus.Click += buttonApplyStampStatus_Click;
            // 
            // radioSTAMP_Status_Inoperable
            // 
            radioSTAMP_Status_Inoperable.AutoSize = true;
            radioSTAMP_Status_Inoperable.Location = new Point(14, 92);
            radioSTAMP_Status_Inoperable.Name = "radioSTAMP_Status_Inoperable";
            radioSTAMP_Status_Inoperable.Size = new Size(81, 19);
            radioSTAMP_Status_Inoperable.TabIndex = 3;
            radioSTAMP_Status_Inoperable.Text = "Inoperable";
            radioSTAMP_Status_Inoperable.UseVisualStyleBackColor = true;
            // 
            // radioSTAMP_Status_Degraded
            // 
            radioSTAMP_Status_Degraded.AutoSize = true;
            radioSTAMP_Status_Degraded.Location = new Point(14, 70);
            radioSTAMP_Status_Degraded.Name = "radioSTAMP_Status_Degraded";
            radioSTAMP_Status_Degraded.Size = new Size(76, 19);
            radioSTAMP_Status_Degraded.TabIndex = 2;
            radioSTAMP_Status_Degraded.Text = "Degraded";
            radioSTAMP_Status_Degraded.UseVisualStyleBackColor = true;
            // 
            // radioSTAMP_Status_Normal
            // 
            radioSTAMP_Status_Normal.AutoSize = true;
            radioSTAMP_Status_Normal.Location = new Point(14, 48);
            radioSTAMP_Status_Normal.Name = "radioSTAMP_Status_Normal";
            radioSTAMP_Status_Normal.Size = new Size(65, 19);
            radioSTAMP_Status_Normal.TabIndex = 1;
            radioSTAMP_Status_Normal.Text = "Normal";
            radioSTAMP_Status_Normal.UseVisualStyleBackColor = true;
            // 
            // radioSTAMP_Status_init
            // 
            radioSTAMP_Status_init.AutoSize = true;
            radioSTAMP_Status_init.Checked = true;
            radioSTAMP_Status_init.Location = new Point(14, 26);
            radioSTAMP_Status_init.Name = "radioSTAMP_Status_init";
            radioSTAMP_Status_init.Size = new Size(79, 19);
            radioSTAMP_Status_init.TabIndex = 0;
            radioSTAMP_Status_init.TabStop = true;
            radioSTAMP_Status_init.Text = "Initializing";
            radioSTAMP_Status_init.UseVisualStyleBackColor = true;
            // 
            // labelSTAMP
            // 
            labelSTAMP.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelSTAMP.BackColor = Color.Gray;
            labelSTAMP.Location = new Point(12, 536);
            labelSTAMP.Name = "labelSTAMP";
            labelSTAMP.Size = new Size(264, 16);
            labelSTAMP.TabIndex = 1;
            labelSTAMP.Text = "Disconnected";
            labelSTAMP.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupAIECS
            // 
            groupAIECS.Controls.Add(buttonSendZeroizeRequest);
            groupAIECS.Controls.Add(buttonAIECS_RequestBIT);
            groupAIECS.Controls.Add(buttonStopAIECS);
            groupAIECS.Controls.Add(buttonStartAIECS);
            groupAIECS.Controls.Add(buttonConnectAIECS);
            groupAIECS.Controls.Add(label1);
            groupAIECS.Location = new Point(314, 21);
            groupAIECS.Name = "groupAIECS";
            groupAIECS.Size = new Size(288, 548);
            groupAIECS.TabIndex = 2;
            groupAIECS.TabStop = false;
            groupAIECS.Text = "AIECS";
            // 
            // buttonSendZeroizeRequest
            // 
            buttonSendZeroizeRequest.Location = new Point(19, 180);
            buttonSendZeroizeRequest.Name = "buttonSendZeroizeRequest";
            buttonSendZeroizeRequest.Size = new Size(242, 32);
            buttonSendZeroizeRequest.TabIndex = 9;
            buttonSendZeroizeRequest.Text = "Send ZEROIZE Request";
            buttonSendZeroizeRequest.UseVisualStyleBackColor = true;
            // 
            // buttonAIECS_RequestBIT
            // 
            buttonAIECS_RequestBIT.Location = new Point(19, 142);
            buttonAIECS_RequestBIT.Name = "buttonAIECS_RequestBIT";
            buttonAIECS_RequestBIT.Size = new Size(242, 32);
            buttonAIECS_RequestBIT.TabIndex = 5;
            buttonAIECS_RequestBIT.Text = "Send BIT Request";
            buttonAIECS_RequestBIT.UseVisualStyleBackColor = true;
            // 
            // buttonStopAIECS
            // 
            buttonStopAIECS.Location = new Point(153, 58);
            buttonStopAIECS.Name = "buttonStopAIECS";
            buttonStopAIECS.Size = new Size(75, 48);
            buttonStopAIECS.TabIndex = 8;
            buttonStopAIECS.Text = "Stop AIECS Sim";
            buttonStopAIECS.UseVisualStyleBackColor = true;
            // 
            // buttonStartAIECS
            // 
            buttonStartAIECS.Location = new Point(41, 58);
            buttonStartAIECS.Name = "buttonStartAIECS";
            buttonStartAIECS.Size = new Size(75, 48);
            buttonStartAIECS.TabIndex = 7;
            buttonStartAIECS.Text = "Start AIECS Sim";
            buttonStartAIECS.UseVisualStyleBackColor = true;
            // 
            // buttonConnectAIECS
            // 
            buttonConnectAIECS.Location = new Point(63, 22);
            buttonConnectAIECS.Name = "buttonConnectAIECS";
            buttonConnectAIECS.Size = new Size(150, 30);
            buttonConnectAIECS.TabIndex = 6;
            buttonConnectAIECS.Text = "Connect";
            buttonConnectAIECS.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.BackColor = Color.Gray;
            label1.Location = new Point(6, 529);
            label1.Name = "label1";
            label1.Size = new Size(276, 16);
            label1.TabIndex = 2;
            label1.Text = "Disconnected";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 604);
            Controls.Add(groupAIECS);
            Controls.Add(groupSTAMP);
            Name = "Form1";
            Text = "STAMP GUI";
            groupSTAMP.ResumeLayout(false);
            groupSTAMP_Status.ResumeLayout(false);
            groupSTAMP_Status.PerformLayout();
            groupAIECS.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button buttonConnectToSTAMP;
        private GroupBox groupSTAMP;
        private Label labelSTAMP;
        private GroupBox groupSTAMP_Status;
        private RadioButton radioSTAMP_Status_Inoperable;
        private RadioButton radioSTAMP_Status_Degraded;
        private RadioButton radioSTAMP_Status_Normal;
        private RadioButton radioSTAMP_Status_init;
        private Button buttonApplyStampStatus;
        private GroupBox groupSTAMP_BIT;
        private Button buttonStopSTAMP;
        private Button buttonStartSTAMP;
        private GroupBox groupAIECS;
        private Label label1;
        private Button buttonSendZeroizeRequest;
        private Button buttonAIECS_RequestBIT;
        private Button buttonStopAIECS;
        private Button buttonStartAIECS;
        private Button buttonConnectAIECS;
    }
}
