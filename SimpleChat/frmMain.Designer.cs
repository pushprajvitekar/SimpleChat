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
            this.button1btnStartUdpClient = new System.Windows.Forms.Button();
            this.btnSendUdpMessage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtMyIp
            // 
            this.txtMyIp.Location = new System.Drawing.Point(132, 51);
            this.txtMyIp.Name = "txtMyIp";
            this.txtMyIp.Size = new System.Drawing.Size(100, 23);
            this.txtMyIp.TabIndex = 0;
            // 
            // txtMyPort
            // 
            this.txtMyPort.Location = new System.Drawing.Point(318, 51);
            this.txtMyPort.Name = "txtMyPort";
            this.txtMyPort.Size = new System.Drawing.Size(100, 23);
            this.txtMyPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "My IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "My Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(247, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Friend Port";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Friend IP";
            // 
            // txtFriendPort
            // 
            this.txtFriendPort.Location = new System.Drawing.Point(318, 106);
            this.txtFriendPort.Name = "txtFriendPort";
            this.txtFriendPort.Size = new System.Drawing.Size(100, 23);
            this.txtFriendPort.TabIndex = 5;
            // 
            // txtFriendIp
            // 
            this.txtFriendIp.Location = new System.Drawing.Point(132, 106);
            this.txtFriendIp.Name = "txtFriendIp";
            this.txtFriendIp.Size = new System.Drawing.Size(100, 23);
            this.txtFriendIp.TabIndex = 4;
            // 
            // lstBoxMessages
            // 
            this.lstBoxMessages.FormattingEnabled = true;
            this.lstBoxMessages.ItemHeight = 15;
            this.lstBoxMessages.Location = new System.Drawing.Point(65, 140);
            this.lstBoxMessages.Name = "lstBoxMessages";
            this.lstBoxMessages.Size = new System.Drawing.Size(414, 94);
            this.lstBoxMessages.TabIndex = 8;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(64, 240);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(333, 50);
            this.txtMessage.TabIndex = 9;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(403, 257);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnInitNetwork
            // 
            this.btnInitNetwork.Location = new System.Drawing.Point(64, 12);
            this.btnInitNetwork.Name = "btnInitNetwork";
            this.btnInitNetwork.Size = new System.Drawing.Size(161, 23);
            this.btnInitNetwork.TabIndex = 12;
            this.btnInitNetwork.Text = "Enter Network";
            this.btnInitNetwork.UseVisualStyleBackColor = true;
            this.btnInitNetwork.Click += new System.EventHandler(this.btnInitNetwork_Click);
            // 
            // lstboxChat
            // 
            this.lstboxChat.FormattingEnabled = true;
            this.lstboxChat.ItemHeight = 15;
            this.lstboxChat.Location = new System.Drawing.Point(66, 302);
            this.lstboxChat.Name = "lstboxChat";
            this.lstboxChat.Size = new System.Drawing.Size(412, 124);
            this.lstboxChat.TabIndex = 13;
            // 
            // lstBoxPeers
            // 
            this.lstBoxPeers.FormattingEnabled = true;
            this.lstBoxPeers.ItemHeight = 15;
            this.lstBoxPeers.Location = new System.Drawing.Point(585, 25);
            this.lstBoxPeers.Name = "lstBoxPeers";
            this.lstBoxPeers.Size = new System.Drawing.Size(183, 79);
            this.lstBoxPeers.TabIndex = 14;
            this.lstBoxPeers.SelectedIndexChanged += new System.EventHandler(this.lstBoxPeers_SelectedIndexChanged);
            // 
            // button1btnStartUdpClient
            // 
            this.button1btnStartUdpClient.Location = new System.Drawing.Point(307, 14);
            this.button1btnStartUdpClient.Name = "button1btnStartUdpClient";
            this.button1btnStartUdpClient.Size = new System.Drawing.Size(75, 23);
            this.button1btnStartUdpClient.TabIndex = 15;
            this.button1btnStartUdpClient.Text = "Start udp client";
            this.button1btnStartUdpClient.UseVisualStyleBackColor = true;
            this.button1btnStartUdpClient.Click += new System.EventHandler(this.btnStartUdpClient_Click);
            // 
            // btnSendUdpMessage
            // 
            this.btnSendUdpMessage.Location = new System.Drawing.Point(415, 18);
            this.btnSendUdpMessage.Name = "btnSendUdpMessage";
            this.btnSendUdpMessage.Size = new System.Drawing.Size(75, 23);
            this.btnSendUdpMessage.TabIndex = 16;
            this.btnSendUdpMessage.Text = "Send Udp";
            this.btnSendUdpMessage.UseVisualStyleBackColor = true;
            this.btnSendUdpMessage.Click += new System.EventHandler(this.btnSendUdpMessage_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSendUdpMessage);
            this.Controls.Add(this.button1btnStartUdpClient);
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
        private Button button1btnStartUdpClient;
        private Button btnSendUdpMessage;
    }
}