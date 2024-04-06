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
            private const int radius = 20;
            private bool selected = false;
            public CCircle() { x = 0; y = 0; }
            public CCircle(int x, int y) { this.x = x; this.y = y; }
            public CCircle(CCircle circle) { x = circle.x; y = circle.y; }
            ~CCircle() {}
            public void DrawMe(PaintEventArgs e)
            {
                Pen pen = new Pen(Color.Black, 3);
                e.Graphics.DrawEllipse(pen, x - radius, y - radius, radius*2, radius*2);
                if (selected)
                {
                    e.Graphics.FillEllipse(Brushes.Blue, x - radius, y - radius, radius * 2, radius * 2);
                }
                else
                {
                    e.Graphics.FillEllipse(Brushes.Black, x - radius, y - radius, radius * 2, radius * 2);
                }
            }
            public bool SelectionCheck(MouseEventArgs e)
            {
                if ((e.X > x - radius && e.X < x + radius) && (e.Y > y - radius && e.Y < y + radius))
                {
                    selected = true;
                    return true;
                }
                else
                {
                    selected = false;
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
            public bool SelectionCheckAll(MouseEventArgs e)
            {
                bool anyIsSelected = false;
                for (int i = 0;i < size; i++)
                {
                    if (array[i].SelectionCheck(e) == true) { anyIsSelected = true; }
                }
                return anyIsSelected;
            }
            public void DeleteSelectedAll()
            {
                for (int i = 0; i< size; i++)
                {
                    if (array[i].IsSelected())
                    {
                        DeleteObj(array[i]);
                    }
                }
            }
            public void DeleteObj(CCircle CircleToDelete)
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
        }

        container c = new container();

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!c.SelectionCheckAll(e)) 
            {
                CCircle createdCircle = new CCircle(e.X, e.Y);
                c.Push_back(createdCircle);
            }
            this.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            c.DrawAll(e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                c.DeleteSelectedAll();
                // Нужно как-то заполнить удалённые (выделенные) круги белым цветом
            }
        }
    }
}
