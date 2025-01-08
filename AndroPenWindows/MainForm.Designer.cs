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
            this.barOutputPressure = new ProgressBar();
            this.labelPressure = new Label();
            this.pressureCuve1 = new Controls.PressureCurve();
            this.ScreenSelection = new ComboBox();
            this.tableLayoutPanel1 = new TableLayoutPanel();
            this.barInputPressure = new ProgressBar();
            this.label2 = new Label();
            this.label4 = new Label();
            this.label1 = new Label();
            this.label3 = new Label();
            this.labelPrimary = new Label();
            this.labelOrigin = new Label();
            this.labelResolution = new Label();
            this.numericUpDown1 = new NumericUpDown();
            this.label5 = new Label();
            this.tableLayoutPanel1.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)this.numericUpDown1 ).BeginInit();
            SuspendLayout();
            // 
            // barOutputPressure
            // 
            this.barOutputPressure.Dock = DockStyle.Fill;
            this.barOutputPressure.Location = new Point( 103, 103 );
            this.barOutputPressure.Maximum = 8096;
            this.barOutputPressure.Name = "barOutputPressure";
            this.barOutputPressure.Size = new Size( 256, 22 );
            this.barOutputPressure.TabIndex = 4;
            // 
            // labelPressure
            // 
            this.labelPressure.Dock = DockStyle.Fill;
            this.labelPressure.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.labelPressure.Location = new Point( 3, 100 );
            this.labelPressure.Name = "labelPressure";
            this.labelPressure.Size = new Size( 94, 28 );
            this.labelPressure.TabIndex = 5;
            this.labelPressure.Text = "Output Pressure:";
            // 
            // pressureCuve1
            // 
            this.pressureCuve1.BackColor = Color.FromArgb( 64, 64, 64 );
            this.pressureCuve1.CurveColor = Color.FromArgb( 224, 224, 224 );
            this.pressureCuve1.GridColor = Color.DimGray;
            this.pressureCuve1.Location = new Point( 380, 51 );
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
            this.ScreenSelection.Location = new Point( 12, 12 );
            this.ScreenSelection.Name = "ScreenSelection";
            this.ScreenSelection.Size = new Size( 312, 33 );
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
            this.tableLayoutPanel1.Controls.Add( this.barOutputPressure, 1, 4 );
            this.tableLayoutPanel1.Controls.Add( this.label1, 0, 1 );
            this.tableLayoutPanel1.Controls.Add( this.label3, 0, 2 );
            this.tableLayoutPanel1.Controls.Add( this.labelPressure, 0, 4 );
            this.tableLayoutPanel1.Controls.Add( this.labelPrimary, 1, 0 );
            this.tableLayoutPanel1.Controls.Add( this.labelOrigin, 1, 1 );
            this.tableLayoutPanel1.Controls.Add( this.labelResolution, 1, 2 );
            this.tableLayoutPanel1.Location = new Point( 12, 51 );
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 20F ) );
            this.tableLayoutPanel1.Size = new Size( 362, 128 );
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // barInputPressure
            // 
            this.barInputPressure.Dock = DockStyle.Fill;
            this.barInputPressure.Location = new Point( 103, 78 );
            this.barInputPressure.Maximum = 8096;
            this.barInputPressure.Name = "barInputPressure";
            this.barInputPressure.Size = new Size( 256, 19 );
            this.barInputPressure.TabIndex = 15;
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
            this.labelPrimary.Text = "label5";
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
            this.labelOrigin.Text = "label5";
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
            this.labelResolution.Text = "label6";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.numericUpDown1.Location = new Point( 388, 13 );
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new Size( 120, 31 );
            this.numericUpDown1.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new Font( "Microsoft Sans Serif", 15.75F );
            this.label5.Location = new Point( 330, 15 );
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
            this.ClientSize = new Size( 518, 195 );
            this.Controls.Add( this.label5 );
            this.Controls.Add( this.numericUpDown1 );
            this.Controls.Add( this.tableLayoutPanel1 );
            this.Controls.Add( this.ScreenSelection );
            this.Controls.Add( this.pressureCuve1 );
            this.ForeColor = SystemColors.ButtonFace;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = (Icon)resources.GetObject( "$this.Icon" );
            this.Name = "MainForm";
            this.Text = "AndroPen";
            this.tableLayoutPanel1.ResumeLayout( false );
            this.tableLayoutPanel1.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize)this.numericUpDown1 ).EndInit();
            ResumeLayout( false );
            PerformLayout();
        }

        #endregion
        private ProgressBar barOutputPressure;
        private Label labelPressure;
        private Controls.PressureCurve pressureCuve1;
        private ComboBox ScreenSelection;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label2;
        private Label label1;
        private Label label3;
        private Label label4;
        private ProgressBar barInputPressure;
        private Label labelPrimary;
        private Label labelOrigin;
        private Label labelResolution;
        private NumericUpDown numericUpDown1;
        private Label label5;
    }
}
