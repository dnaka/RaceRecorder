namespace RaceRecorder
{
    partial class RaceRecorder
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RaceRecorder));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.RaceNameTextBox = new System.Windows.Forms.TextBox();
            this.RaceNamelabel = new System.Windows.Forms.Label();
            this.DriverListView = new System.Windows.Forms.DataGridView();
            this.driverEntryLabel = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.DriverNumberTextBox = new System.Windows.Forms.TextBox();
            this.registButton = new System.Windows.Forms.Button();
            this.lapTimer = new System.Windows.Forms.Timer(this.components);
            this.resultViewButton = new System.Windows.Forms.Button();
            this.validateButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.StateLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.DriverListView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RaceNameTextBox
            // 
            resources.ApplyResources(this.RaceNameTextBox, "RaceNameTextBox");
            this.RaceNameTextBox.Name = "RaceNameTextBox";
            // 
            // RaceNamelabel
            // 
            resources.ApplyResources(this.RaceNamelabel, "RaceNamelabel");
            this.RaceNamelabel.Name = "RaceNamelabel";
            // 
            // DriverListView
            // 
            this.DriverListView.AllowUserToResizeRows = false;
            resources.ApplyResources(this.DriverListView, "DriverListView");
            this.DriverListView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DriverListView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.DriverListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DriverListView.DefaultCellStyle = dataGridViewCellStyle6;
            this.DriverListView.MultiSelect = false;
            this.DriverListView.Name = "DriverListView";
            this.DriverListView.RowTemplate.Height = 21;
            // 
            // driverEntryLabel
            // 
            resources.ApplyResources(this.driverEntryLabel, "driverEntryLabel");
            this.driverEntryLabel.Name = "driverEntryLabel";
            // 
            // startButton
            // 
            resources.ApplyResources(this.startButton, "startButton");
            this.startButton.Name = "startButton";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // TimeLabel
            // 
            resources.ApplyResources(this.TimeLabel, "TimeLabel");
            this.TimeLabel.Name = "TimeLabel";
            // 
            // DriverNumberTextBox
            // 
            resources.ApplyResources(this.DriverNumberTextBox, "DriverNumberTextBox");
            this.DriverNumberTextBox.Name = "DriverNumberTextBox";
            this.DriverNumberTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DriverNumberTextBox_KeyPress);
            // 
            // registButton
            // 
            resources.ApplyResources(this.registButton, "registButton");
            this.registButton.Name = "registButton";
            this.registButton.UseVisualStyleBackColor = true;
            this.registButton.Click += new System.EventHandler(this.registButton_Click);
            // 
            // lapTimer
            // 
            this.lapTimer.Interval = 500;
            this.lapTimer.Tick += new System.EventHandler(this.lapTimer_Tick);
            // 
            // resultViewButton
            // 
            resources.ApplyResources(this.resultViewButton, "resultViewButton");
            this.resultViewButton.Name = "resultViewButton";
            this.resultViewButton.UseVisualStyleBackColor = true;
            this.resultViewButton.Click += new System.EventHandler(this.resultViewButton_Click);
            // 
            // validateButton
            // 
            resources.ApplyResources(this.validateButton, "validateButton");
            this.validateButton.Name = "validateButton";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // StateLabel
            // 
            resources.ApplyResources(this.StateLabel, "StateLabel");
            this.StateLabel.Name = "StateLabel";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StateLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // RaceRecorder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.resultViewButton);
            this.Controls.Add(this.registButton);
            this.Controls.Add(this.DriverNumberTextBox);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.driverEntryLabel);
            this.Controls.Add(this.DriverListView);
            this.Controls.Add(this.RaceNamelabel);
            this.Controls.Add(this.RaceNameTextBox);
            this.Name = "RaceRecorder";
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DriverListView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox RaceNameTextBox;
        private System.Windows.Forms.Label RaceNamelabel;
        private System.Windows.Forms.DataGridView DriverListView;
        private System.Windows.Forms.Label driverEntryLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.TextBox DriverNumberTextBox;
        private System.Windows.Forms.Button registButton;
        private System.Windows.Forms.Timer lapTimer;
        private System.Windows.Forms.Button resultViewButton;
        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label StateLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

