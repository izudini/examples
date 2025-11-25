namespace STAMP_GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonConnectToSTAMP = new Button();
            SuspendLayout();
            //
            // buttonConnectToSTAMP
            //
            buttonConnectToSTAMP.Location = new Point(12, 12);
            buttonConnectToSTAMP.Name = "buttonConnectToSTAMP";
            buttonConnectToSTAMP.Size = new Size(150, 30);
            buttonConnectToSTAMP.TabIndex = 0;
            buttonConnectToSTAMP.Text = "Connect to STAMP";
            buttonConnectToSTAMP.UseVisualStyleBackColor = true;
            buttonConnectToSTAMP.Click += buttonConnectToSTAMP_Click;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonConnectToSTAMP);
            Name = "Form1";
            Text = "STAMP GUI";
            ResumeLayout(false);
        }

        #endregion

        private Button buttonConnectToSTAMP;
    }
}
