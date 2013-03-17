namespace Company.VSScripts
{
    partial class EditScriptDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this._commandText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._nameText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._stdoutCombo = new System.Windows.Forms.ComboBox();
            this._stderrCombo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._stdinCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Command";
            // 
            // _commandText
            // 
            this._commandText.Location = new System.Drawing.Point(15, 25);
            this._commandText.Name = "_commandText";
            this._commandText.Size = new System.Drawing.Size(309, 20);
            this._commandText.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name";
            // 
            // _nameText
            // 
            this._nameText.Location = new System.Drawing.Point(15, 78);
            this._nameText.Name = "_nameText";
            this._nameText.Size = new System.Drawing.Size(309, 20);
            this._nameText.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "stdout";
            // 
            // _stdoutCombo
            // 
            this._stdoutCombo.FormattingEnabled = true;
            this._stdoutCombo.Location = new System.Drawing.Point(155, 119);
            this._stdoutCombo.Name = "_stdoutCombo";
            this._stdoutCombo.Size = new System.Drawing.Size(169, 21);
            this._stdoutCombo.TabIndex = 7;
            // 
            // _stderrCombo
            // 
            this._stderrCombo.FormattingEnabled = true;
            this._stderrCombo.Location = new System.Drawing.Point(155, 146);
            this._stderrCombo.Name = "_stderrCombo";
            this._stderrCombo.Size = new System.Drawing.Size(169, 21);
            this._stderrCombo.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "stderr";
            // 
            // _stdinCombo
            // 
            this._stdinCombo.FormattingEnabled = true;
            this._stdinCombo.Location = new System.Drawing.Point(155, 184);
            this._stdinCombo.Name = "_stdinCombo";
            this._stdinCombo.Size = new System.Drawing.Size(169, 21);
            this._stdinCombo.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "stdin";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(249, 260);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 12;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _okButton
            // 
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Location = new System.Drawing.Point(168, 260);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 13;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // EditScriptDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 295);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._stdinCombo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._stderrCombo);
            this.Controls.Add(this._stdoutCombo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._nameText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._commandText);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditScriptDialog";
            this.Text = "EditScriptDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _commandText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _nameText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _stdoutCombo;
        private System.Windows.Forms.ComboBox _stderrCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox _stdinCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
    }
}