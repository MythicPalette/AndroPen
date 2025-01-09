namespace AndroPen
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labelPressure = new Label();
            this.pressureCuve1 = new Controls.PressureCurve();
            this.ScreenSelection = new ComboBox();
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.barInputPressure = new Controls.ProgressBar();
            this.label2 = new Label();
            this.label4 = new Label();
            this.label1 = new Label();
            this.label3 = new Label();
            this.labelPrimary = new Label();
            this.labelOrigin = new Label();
            this.labelResolution = new Label();
            this.barOutputPressure = new Controls.ProgressBar();
            this.PortInput = new NumericUpDown();
            this.label5 = new Label();
            this.tableLayoutPanel1.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)this.PortInput ).BeginInit();
            SuspendLayout();
            // 
            // labelPressure
            // 
            this.labelPressure.Dock = DockStyle.Fill;
            this.labelPressure.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.labelPressure.Location = new Point( 3, 100 );
            this.labelPressure.Name = "labelPressure";
            this.labelPressure.Size = new Size( 94, 27 );
            this.labelPressure.TabIndex = 5;
            this.labelPressure.Text = "Output Pressure:";
            // 
            // pressureCuve1
            // 
            this.pressureCuve1.BackColor = Color.FromArgb( 64, 64, 64 );
            this.pressureCuve1.CurveColor = Color.FromArgb( 224, 224, 224 );
            this.pressureCuve1.GridColor = Color.DimGray;
            this.pressureCuve1.Location = new Point( 380, 81 );
            this.pressureCuve1.Name = "pressureCuve1";
            this.pressureCuve1.NodeColor = Color.FromArgb( 255, 128, 128 );
            this.pressureCuve1.Size = new Size( 128, 128 );
            this.pressureCuve1.TabIndex = 9;
            this.pressureCuve1.Text = "pressureCuve1";
            // 
            // ScreenSelection
            // 
            this.ScreenSelection.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.ScreenSelection.FormattingEnabled = true;
            this.ScreenSelection.Location = new Point( 12, 43 );
            this.ScreenSelection.Name = "ScreenSelection";
            this.ScreenSelection.Size = new Size( 302, 33 );
            this.ScreenSelection.TabIndex = 10;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Absolute, 100F ) );
            this.tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100F ) );
            this.tableLayoutPanel1.Controls.Add( this.barInputPressure, 1, 3 );
            this.tableLayoutPanel1.Controls.Add( this.label2, 0, 0 );
            this.tableLayoutPanel1.Controls.Add( this.label4, 0, 3 );
            this.tableLayoutPanel1.Controls.Add( this.label1, 0, 1 );
            this.tableLayoutPanel1.Controls.Add( this.label3, 0, 2 );
            this.tableLayoutPanel1.Controls.Add( this.labelPressure, 0, 4 );
            this.tableLayoutPanel1.Controls.Add( this.labelPrimary, 1, 0 );
            this.tableLayoutPanel1.Controls.Add( this.labelOrigin, 1, 1 );
            this.tableLayoutPanel1.Controls.Add( this.labelResolution, 1, 2 );
            this.tableLayoutPanel1.Controls.Add( this.barOutputPressure, 1, 4 );
            this.tableLayoutPanel1.Location = new Point( 12, 82 );
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.Size = new Size( 362, 127 );
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // barInputPressure
            // 
            this.barInputPressure.BackColor = Color.FromArgb( 64, 64, 64 );
            this.barInputPressure.Dock = DockStyle.Fill;
            this.barInputPressure.ForeColor = Color.Plum;
            this.barInputPressure.Location = new Point( 103, 78 );
            this.barInputPressure.Name = "barInputPressure";
            this.barInputPressure.Size = new Size( 256, 19 );
            this.barInputPressure.TabIndex = 16;
            this.barInputPressure.Text = "progressBar1";
            // 
            // label2
            // 
            this.label2.Dock = DockStyle.Fill;
            this.label2.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.label2.Location = new Point( 3, 0 );
            this.label2.Name = "label2";
            this.label2.Size = new Size( 94, 25 );
            this.label2.TabIndex = 0;
            this.label2.Text = "Primary Screen:";
            // 
            // label4
            // 
            this.label4.Dock = DockStyle.Fill;
            this.label4.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.label4.Location = new Point( 3, 75 );
            this.label4.Name = "label4";
            this.label4.Size = new Size( 94, 25 );
            this.label4.TabIndex = 14;
            this.label4.Text = "Input Pressure:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = DockStyle.Fill;
            this.label1.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.label1.Location = new Point( 3, 25 );
            this.label1.Name = "label1";
            this.label1.Size = new Size( 94, 25 );
            this.label1.TabIndex = 1;
            this.label1.Text = "Origin Point:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = DockStyle.Fill;
            this.label3.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.label3.Location = new Point( 3, 50 );
            this.label3.Name = "label3";
            this.label3.Size = new Size( 94, 25 );
            this.label3.TabIndex = 2;
            this.label3.Text = "Resolution:";
            // 
            // labelPrimary
            // 
            this.labelPrimary.AutoSize = true;
            this.labelPrimary.Dock = DockStyle.Fill;
            this.labelPrimary.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.labelPrimary.Location = new Point( 103, 0 );
            this.labelPrimary.Name = "labelPrimary";
            this.labelPrimary.Size = new Size( 256, 25 );
            this.labelPrimary.TabIndex = 16;
            // 
            // labelOrigin
            // 
            this.labelOrigin.AutoSize = true;
            this.labelOrigin.Dock = DockStyle.Fill;
            this.labelOrigin.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.labelOrigin.Location = new Point( 103, 25 );
            this.labelOrigin.Name = "labelOrigin";
            this.labelOrigin.Size = new Size( 256, 25 );
            this.labelOrigin.TabIndex = 17;
            // 
            // labelResolution
            // 
            this.labelResolution.AutoSize = true;
            this.labelResolution.Dock = DockStyle.Fill;
            this.labelResolution.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.labelResolution.Location = new Point( 103, 50 );
            this.labelResolution.Name = "labelResolution";
            this.labelResolution.Size = new Size( 256, 25 );
            this.labelResolution.TabIndex = 18;
            // 
            // barOutputPressure
            // 
            this.barOutputPressure.BackColor = Color.FromArgb( 64, 64, 64 );
            this.barOutputPressure.Dock = DockStyle.Fill;
            this.barOutputPressure.ForeColor = Color.Plum;
            this.barOutputPressure.Location = new Point( 103, 103 );
            this.barOutputPressure.Name = "barOutputPressure";
            this.barOutputPressure.Size = new Size( 256, 21 );
            this.barOutputPressure.TabIndex = 19;
            this.barOutputPressure.Text = "progressBar1";
            // 
            // PortInput
            // 
            this.PortInput.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.PortInput.Location = new Point( 380, 44 );
            this.PortInput.Maximum = new decimal( new int[] { 65535, 0, 0, 0 } );
            this.PortInput.Name = "PortInput";
            this.PortInput.Size = new Size( 128, 31 );
            this.PortInput.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.label5.Location = new Point( 320, 46 );
            this.label5.Name = "label5";
            this.label5.Size = new Size( 51, 25 );
            this.label5.TabIndex = 15;
            this.label5.Text = "Port";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF( 7F, 15F );
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb( 28, 28, 28 );
            this.ClientSize = new Size( 518, 224 );
            this.Controls.Add( this.label5 );
            this.Controls.Add( this.PortInput );
            this.Controls.Add( this.tableLayoutPanel1 );
            this.Controls.Add( this.ScreenSelection );
            this.Controls.Add( this.pressureCuve1 );
            this.ForeColor = SystemColors.ButtonFace;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Icon = (Icon)resources.GetObject( "$this.Icon" );
            this.Name = "MainForm";
            this.Text = "AndroPen";
            this.tableLayoutPanel1.ResumeLayout( false );
            this.tableLayoutPanel1.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize)this.PortInput ).EndInit();
            ResumeLayout( false );
            PerformLayout();
        }

        #endregion
        private Label labelPressure;
        private Controls.PressureCurve pressureCuve1;
        private ComboBox ScreenSelection;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label2;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label labelPrimary;
        private Label labelOrigin;
        private Label labelResolution;
        private NumericUpDown PortInput;
        private Label label5;
        private Controls.ProgressBar barInputPressure;
        private Controls.ProgressBar barOutputPressure;
    }
}
