using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMoneyApp
{
    public partial class Form1 : Form
    {
        MyMoneyEntities data;
        public Form1()
        {
            InitializeComponent();
            data = new MyMoneyEntities();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "myMoneyDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this.myMoneyDataSet.Users);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedValue != null)
            {
                MoneysForm form = new MoneysForm();
                form.moneyUserID = (int) comboBox1.SelectedValue;
                form.SetUserName(data.Users.Find(comboBox1.SelectedValue).UserName);
                form.ShowDialog();
                return;
            }

            if(comboBox1.Text == "")
            {
                MessageBox.Show("Введите имя пользователя!");
                return;
            }

            if(MessageBox.Show("Данный пользователь не найден.\nСоздать нового пользователя с таким именем?", "Создать", MessageBoxButtons.YesNo) == DialogResult.Yes){
                Users u = new Users();
                u.UserName = comboBox1.Text;
                data.Users.Add(u);
                data.SaveChanges();

                MoneysForm form = new MoneysForm();
                form.moneyUserID = u.UserId;
                form.SetUserName(u.UserName);
                form.ShowDialog();
                this.usersTableAdapter.Fill(this.myMoneyDataSet.Users);
            }
        }
    }
}
