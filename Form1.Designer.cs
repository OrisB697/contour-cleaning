namespace Lab1_OZR1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelScene;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.ListBox lstShapes;
        private System.Windows.Forms.Button btnCreateShape;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnOrderUp;
        private System.Windows.Forms.Button btnOrderDown;
        private System.Windows.Forms.PropertyGrid propertyGrid;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.panelScene = new System.Windows.Forms.Panel();
            this.panelControls = new System.Windows.Forms.Panel();
            this.lstShapes = new System.Windows.Forms.ListBox();
            this.btnCreateShape = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnOrderUp = new System.Windows.Forms.Button();
            this.btnOrderDown = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();

            this.SuspendLayout();

            // 
            // panelScene
            // 
            this.panelScene.BackColor = System.Drawing.Color.White;
            this.panelScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScene.Location = new System.Drawing.Point(0, 0);
            this.panelScene.Name = "panelScene";
            this.panelScene.Size = new System.Drawing.Size(900, 800);
            this.panelScene.TabIndex = 0;

            // 
            // panelControls
            // 
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControls.Location = new System.Drawing.Point(900, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(300, 800);
            this.panelControls.TabIndex = 1;

            // 
            // lstShapes
            // 
            this.lstShapes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstShapes.IntegralHeight = false;
            this.lstShapes.Location = new System.Drawing.Point(0, 0);
            this.lstShapes.Name = "lstShapes";
            this.lstShapes.Size = new System.Drawing.Size(300, 200);
            this.lstShapes.TabIndex = 0;

            // 
            // btnCreateShape
            // 
            this.btnCreateShape.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCreateShape.Location = new System.Drawing.Point(0, 200);
            this.btnCreateShape.Name = "btnCreateShape";
            this.btnCreateShape.Size = new System.Drawing.Size(300, 40);
            this.btnCreateShape.TabIndex = 1;
            this.btnCreateShape.Text = "Нарисовать фигуру";
            this.btnCreateShape.UseVisualStyleBackColor = true;

            // 
            // btnDelete
            // 
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDelete.Location = new System.Drawing.Point(0, 240);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(300, 30);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Удалить выбранное";
            this.btnDelete.UseVisualStyleBackColor = true;

            // 
            // btnOrderUp
            // 
            this.btnOrderUp.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOrderUp.Location = new System.Drawing.Point(0, 270);
            this.btnOrderUp.Name = "btnOrderUp";
            this.btnOrderUp.Size = new System.Drawing.Size(300, 30);
            this.btnOrderUp.TabIndex = 3;
            this.btnOrderUp.Text = "Порядок: выше";
            this.btnOrderUp.UseVisualStyleBackColor = true;

            // 
            // btnOrderDown
            // 
            this.btnOrderDown.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOrderDown.Location = new System.Drawing.Point(0, 300);
            this.btnOrderDown.Name = "btnOrderDown";
            this.btnOrderDown.Size = new System.Drawing.Size(300, 30);
            this.btnOrderDown.TabIndex = 4;
            this.btnOrderDown.Text = "Порядок: ниже";
            this.btnOrderDown.UseVisualStyleBackColor = true;

            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 330);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(300, 470);
            this.propertyGrid.TabIndex = 5;

            // 
            // panelControls
            // 
            // Порядок Add обратен визуальному при Dock=Top.
            // propertyGrid (Fill) добавляется первым — он займёт остаток.
            // 
            this.panelControls.Controls.Add(this.propertyGrid);
            this.panelControls.Controls.Add(this.btnOrderDown);
            this.panelControls.Controls.Add(this.btnOrderUp);
            this.panelControls.Controls.Add(this.btnDelete);
            this.panelControls.Controls.Add(this.btnCreateShape);
            this.panelControls.Controls.Add(this.lstShapes);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.panelScene);
            this.Controls.Add(this.panelControls);
            this.Name = "Form1";
            this.Text = "Form1";

            this.ResumeLayout(false);
        }

        #endregion
    }
}