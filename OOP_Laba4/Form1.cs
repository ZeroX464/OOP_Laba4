using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OOP_Laba4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        class CCircle
        {
            private int x, y;
            private const int radius = 40;
            public bool selected = false;
            private bool deleted = false; // Вместо закрашивания круга нужно закрасить весь background image и отрисовать всё заново
            public CCircle() { x = 0; y = 0; }
            public CCircle(int x, int y) { this.x = x; this.y = y; }
            public CCircle(CCircle circle) { x = circle.x; y = circle.y; }
            ~CCircle() {}
            public void DrawMe(PaintEventArgs e) // Сюда нужно передавать Graphics от PictureBox
            {
                Pen pen = new Pen(Color.Brown, 3);
                if (selected && !deleted)
                {
                    e.Graphics.DrawEllipse(pen, x - radius, y - radius, radius * 2, radius * 2);
                    e.Graphics.FillEllipse(Brushes.Blue, x - radius, y - radius, radius * 2, radius * 2);
                }
                else if (deleted){
                    pen.Color = SystemColors.Control;
                    e.Graphics.DrawEllipse(pen, x - radius, y - radius, radius * 2, radius * 2);
                    e.Graphics.FillEllipse(SystemBrushes.Control, x - radius, y - radius, radius * 2, radius * 2);
                }
                else
                {
                    e.Graphics.DrawEllipse(pen, x - radius, y - radius, radius * 2, radius * 2);
                    e.Graphics.FillEllipse(Brushes.Black, x - radius, y - radius, radius * 2, radius * 2);
                }
            }
            public bool SelectionCheck(MouseEventArgs e, bool CrtlPressed, bool MultiplySelections)
            {
                    if ((e.X > x - radius && e.X < x + radius) && (e.Y > y - radius && e.Y < y + radius))
                    {
                    if (MultiplySelections) { selected = true; } // Здесь не нужно MultiplySelections и CrtlPressed, точка не должна знать о существовании других точек
                    return true;
                    }
                    else
                    {
                    if (!CrtlPressed) { selected = false; }
                    return false;
                    }
            }
            public bool IsSelected() 
            { 
                if (selected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public void DeleteMeIfSelected()
            {
                if (selected)
                {
                    deleted = true;
                }
            }
            public int getRLength(MouseEventArgs e)
            {
                return (int)Math.Sqrt(Math.Pow((e.X - x), 2) + Math.Pow((e.Y - y), 2));
            }
        }
        class container
        {
            CCircle[] array;
            private int size, capacity;
            public container() { size = 0; capacity = 1; array = new CCircle[capacity]; }
            public container(int capacity) { size = 0; this.capacity = capacity; array = new CCircle[capacity]; }
            public container(container c) { size = c.size; capacity = c.capacity; array = new container(c).array; }
            ~container() {}
            public void Push_back(CCircle element) {
                if (size >= capacity)
                {
                    capacity *= 2;
                    CCircle[] newArr = new CCircle[capacity];
                    for (int i = 0; i < size; i++)
                    {
                        newArr[i] = array[i];
                    }
                    array = newArr;
                }
                array[size++] = element;
            }
            public void DrawAll(PaintEventArgs e)
            {
                for (int i = 0; i < size; i++)
                {
                    array[i].DrawMe(e);
                }
            }
            public bool SelectionCheckAll(MouseEventArgs e, bool CtrlPressed, bool MultiplySelections)
            {
                bool anyIsSelected = false;
                if (MultiplySelections)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (array[i].SelectionCheck(e, CtrlPressed, MultiplySelections) == true) { anyIsSelected = true; }
                    }
                }
                else
                {
                    int minRLength = 10000;
                    CCircle nearestObj = null;
                    for (int i = 0; i < size; i++)
                    {
                        if (array[i].SelectionCheck(e, CtrlPressed, MultiplySelections) == true)
                        {
                            anyIsSelected = true;
                            if (array[i].getRLength(e) < minRLength)
                            {
                                nearestObj = array[i];
                            }

                        }
                    }
                    if (nearestObj != null) 
                    {
                        if (!CtrlPressed) { MakeAllObjsUnselected(); }
                        nearestObj.selected = true; 
                    }
                    
                }
                return anyIsSelected;
            }
            public void DeleteSelectedFromContainer()
            {
                int const_size = size;
                for (int i = 0; i < const_size; i++)
                {
                    if (array[i] != null)
                    {
                        if (array[i].IsSelected())
                        {
                            DeleteObjFromContainer(array[i]);
                            i--;
                        }
                    }
                }
            }
            public void DeleteObjFromContainer(CCircle CircleToDelete)
            {
                CCircle[] newArr = new CCircle[capacity];
                bool elemFind = false;
                size--;
                for (int i = 0; i < size; i++)
                {
                    if (array[i] == CircleToDelete)
                    {
                        elemFind = true;
                    }
                    if (elemFind) { newArr[i] = array[i + 1]; }
                    else { newArr[i] = array[i]; }
                }
                if (array[size] == CircleToDelete) { elemFind = true; } //Для последнего элемента
                array = newArr;
                if (!elemFind) { throw new Exception("ObjNotFoundInContainer"); }
            }
            public void MakeSelectedObjsDeleted()
            {
                for (int i = 0; i < size; i++)
                {
                    array[i].DeleteMeIfSelected();
                }
            }
            public void MakeLastObjSelected()
            {
                if (size > 0)
                {
                    array[size - 1].selected = true;
                }
            }
            public void MakeAllObjsUnselected()
            {
                for (int i = 0; i < size; i++)
                {
                    array[i].selected = false;
                }
            }
        }

        container c = new container();
        bool CtrlPressed = false;
        bool MultiplySelections = false;

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!c.SelectionCheckAll(e, CtrlPressed, MultiplySelections)) 
            {
                CCircle createdCircle = new CCircle(e.X, e.Y);
                c.Push_back(createdCircle);
                createdCircle.selected = true;
            }
            this.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            c.DrawAll(e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) 
            {
                label2.Text = "CtrlPressed = false";
                CtrlPressed = false;
            }
            if (e.KeyCode == Keys.Delete)
            {
                c.MakeSelectedObjsDeleted();
                c.DeleteSelectedFromContainer();
                c.MakeLastObjSelected();
                this.Refresh();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           if (e.KeyCode == Keys.ControlKey && checkedListBox1.CheckedIndices.Contains(0))
            {
                label2.Text = "CtrlPressed = true";
                CtrlPressed = true;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedIndices.Contains(1))
            {
                label1.Text = "MultiplySelections = true";
                MultiplySelections = true;
            }
            else
            {
                label1.Text = "MultiplySelections = false";
                MultiplySelections = false;
            }
        }
    }
}
