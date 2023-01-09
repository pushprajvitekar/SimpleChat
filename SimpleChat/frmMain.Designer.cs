namespace SimpleChat
{
    partial class frmMain
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
            this.txtMyIp = new System.Windows.Forms.TextBox();
            this.txtMyPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFriendPort = new System.Windows.Forms.TextBox();
            this.txtFriendIp = new System.Windows.Forms.TextBox();
            this.lstBoxMessages = new System.Windows.Forms.ListBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnInitNetwork = new System.Windows.Forms.Button();
            this.lstboxChat = new System.Windows.Forms.ListBox();
            this.lstBoxPeers = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblMyNodeId = new System.Windows.Forms.Label();
            this.lblFriendNodeId = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtMyIp
            // 
            this.txtMyIp.Location = new System.Drawing.Point(173, 49);
            this.txtMyIp.Name = "txtMyIp";
            this.txtMyIp.Size = new System.Drawing.Size(100, 23);
            this.txtMyIp.TabIndex = 0;
            // 
            // txtMyPort
            // 
            this.txtMyPort.Location = new System.Drawing.Point(366, 49);
            this.txtMyPort.Name = "txtMyPort";
            this.txtMyPort.Size = new System.Drawing.Size(58, 23);
            this.txtMyPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(114, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "My IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(295, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "My Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(295, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Friend Port";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(114, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Friend IP";
            // 
            // txtFriendPort
            // 
            this.txtFriendPort.Location = new System.Drawing.Point(366, 79);
            this.txtFriendPort.Name = "txtFriendPort";
            this.txtFriendPort.Size = new System.Drawing.Size(58, 23);
            this.txtFriendPort.TabIndex = 5;
            // 
            // txtFriendIp
            // 
            this.txtFriendIp.Location = new System.Drawing.Point(173, 79);
            this.txtFriendIp.Name = "txtFriendIp";
            this.txtFriendIp.Size = new System.Drawing.Size(100, 23);
            this.txtFriendIp.TabIndex = 4;
            // 
            // lstBoxMessages
            // 
            this.lstBoxMessages.FormattingEnabled = true;
            this.lstBoxMessages.HorizontalScrollbar = true;
            this.lstBoxMessages.ItemHeight = 15;
            this.lstBoxMessages.Location = new System.Drawing.Point(430, 25);
            this.lstBoxMessages.Name = "lstBoxMessages";
            this.lstBoxMessages.Size = new System.Drawing.Size(387, 94);
            this.lstBoxMessages.TabIndex = 8;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(12, 291);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(333, 50);
            this.txtMessage.TabIndex = 9;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(349, 318);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnInitNetwork
            // 
            this.btnInitNetwork.Location = new System.Drawing.Point(12, 3);
            this.btnInitNetwork.Name = "btnInitNetwork";
            this.btnInitNetwork.Size = new System.Drawing.Size(161, 23);
            this.btnInitNetwork.TabIndex = 12;
            this.btnInitNetwork.Text = "Connect";
            this.btnInitNetwork.UseVisualStyleBackColor = true;
            this.btnInitNetwork.Click += new System.EventHandler(this.btnInitNetwork_Click);
            // 
            // lstboxChat
            // 
            this.lstboxChat.FormattingEnabled = true;
            this.lstboxChat.ItemHeight = 15;
            this.lstboxChat.Location = new System.Drawing.Point(12, 142);
            this.lstboxChat.Name = "lstboxChat";
            this.lstboxChat.Size = new System.Drawing.Size(412, 139);
            this.lstboxChat.TabIndex = 13;
            // 
            // lstBoxPeers
            // 
            this.lstBoxPeers.DisplayMember = "NodeId";
            this.lstBoxPeers.FormattingEnabled = true;
            this.lstBoxPeers.ItemHeight = 15;
            this.lstBoxPeers.Location = new System.Drawing.Point(430, 142);
            this.lstBoxPeers.Name = "lstBoxPeers";
            this.lstBoxPeers.Size = new System.Drawing.Size(372, 199);
            this.lstBoxPeers.TabIndex = 14;
            this.lstBoxPeers.SelectedIndexChanged += new System.EventHandler(this.lstBoxPeers_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(430, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "Log";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(430, 124);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 15);
            this.label6.TabIndex = 17;
            this.label6.Text = "Friends";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 15);
            this.label7.TabIndex = 18;
            this.label7.Text = "Chat History";
            // 
            // lblMyNodeId
            // 
            this.lblMyNodeId.AutoSize = true;
            this.lblMyNodeId.Location = new System.Drawing.Point(12, 49);
            this.lblMyNodeId.Name = "lblMyNodeId";
            this.lblMyNodeId.Size = new System.Drawing.Size(66, 15);
            this.lblMyNodeId.TabIndex = 19;
            this.lblMyNodeId.Text = "My NodeId";
            // 
            // lblFriendNodeId
            // 
            this.lblFriendNodeId.AutoSize = true;
            this.lblFriendNodeId.Location = new System.Drawing.Point(12, 81);
            this.lblFriendNodeId.Name = "lblFriendNodeId";
            this.lblFriendNodeId.Size = new System.Drawing.Size(49, 15);
            this.lblFriendNodeId.TabIndex = 20;
            this.lblFriendNodeId.Text = " NodeId";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 375);
            this.Controls.Add(this.lblFriendNodeId);
            this.Controls.Add(this.lblMyNodeId);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lstBoxPeers);
            this.Controls.Add(this.lstboxChat);
            this.Controls.Add(this.btnInitNetwork);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lstBoxMessages);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtFriendPort);
            this.Controls.Add(this.txtFriendIp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMyPort);
            this.Controls.Add(this.txtMyIp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Simple Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtMyIp;
        private TextBox txtMyPort;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txtFriendPort;
        private TextBox txtFriendIp;
        private ListBox lstBoxMessages;
        private TextBox txtMessage;
        private Button btnSend;
        private Button btnInitNetwork;
        private ListBox lstboxChat;
        private ListBox lstBoxPeers;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label lblMyNodeId;
        private Label lblFriendNodeId;
    }
}