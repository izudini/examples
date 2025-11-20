namespace GUIApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            textBoxConsole = new System.Windows.Forms.TextBox();
            timerUI = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // textBoxConsole
            // 
            textBoxConsole.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            textBoxConsole.BackColor = System.Drawing.Color.Black;
            textBoxConsole.ForeColor = System.Drawing.Color.GreenYellow;
            textBoxConsole.Location = new System.Drawing.Point(292, 359);
            textBoxConsole.Multiline = true;
            textBoxConsole.Name = "textBoxConsole";
            textBoxConsole.Size = new System.Drawing.Size(554, 159);
            textBoxConsole.TabIndex = 0;
            // 
            // timerUI
            // 
            timerUI.Tick += timerUI_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(858, 530);
            Controls.Add(textBoxConsole);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBoxConsole;
        private System.Windows.Forms.Timer timerUI;
    }
}