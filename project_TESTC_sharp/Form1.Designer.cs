namespace project_TESTC_sharp
{
    partial class OBSM
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
            TxtBoxPrix = new TextBox();
            TxtBoxNomDuProduit = new TextBox();
            btn_afficher = new Button();
            DataGridView1 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)DataGridView1).BeginInit();
            SuspendLayout();
            // 
            // TxtBoxPrix
            // 
            TxtBoxPrix.Location = new Point(481, 93);
            TxtBoxPrix.Name = "TxtBoxPrix";
            TxtBoxPrix.Size = new Size(225, 27);
            TxtBoxPrix.TabIndex = 1;
            TxtBoxPrix.TextChanged += textBox1_TextChanged_1;
            TxtBoxPrix.Enter += TxtBoxPrix_Enter;
            TxtBoxPrix.Leave += TxtBoxPrix_Leave;
            // 
            // TxtBoxNomDuProduit
            // 
            TxtBoxNomDuProduit.Location = new Point(137, 93);
            TxtBoxNomDuProduit.Name = "TxtBoxNomDuProduit";
            TxtBoxNomDuProduit.Size = new Size(225, 27);
            TxtBoxNomDuProduit.TabIndex = 2;
            TxtBoxNomDuProduit.TextChanged += textBox1_TextChanged_2;
            TxtBoxNomDuProduit.Enter += TxtBoxNomDuProduit_Enter;
            TxtBoxNomDuProduit.Leave += TxtBoxNomDuProduit_Leave;
            // 
            // btn_afficher
            // 
            btn_afficher.BackColor = Color.FromArgb(202, 94, 197);
            btn_afficher.Location = new Point(900, 84);
            btn_afficher.Name = "btn_afficher";
            btn_afficher.Size = new Size(112, 44);
            btn_afficher.TabIndex = 3;
            btn_afficher.Text = "Affichez";
            btn_afficher.UseVisualStyleBackColor = false;
            btn_afficher.Click += BtnAfficher_Click;
            // 
            // DataGridView1
            // 
            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView1.Location = new Point(137, 155);
            DataGridView1.Name = "DataGridView1";
            DataGridView1.RowHeadersWidth = 51;
            DataGridView1.Size = new Size(875, 255);
            DataGridView1.TabIndex = 4;
            DataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // OBSM
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(1141, 449);
            Controls.Add(DataGridView1);
            Controls.Add(btn_afficher);
            Controls.Add(TxtBoxNomDuProduit);
            Controls.Add(TxtBoxPrix);
            Name = "OBSM";
            Text = "Form1";
            Load += Form1_Load;
            KeyPress += Form1_KeyPress;
            ((System.ComponentModel.ISupportInitialize)DataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox TxtBoxPrix;
        private TextBox TxtBoxNomDuProduit;
        private Button btn_afficher;
        private DataGridView DataGridView1;
    }
}
