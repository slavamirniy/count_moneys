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
    public partial class MoneysForm : Form
    {
        MyMoneyEntities data;
        public int moneyUserID = 1;
        int goalCategoryId;

        public MoneysForm()
        {
            InitializeComponent();
            data = new MyMoneyEntities();
            ShowMoneys();
            ShowCategory();

            if (data.Categories.Where(c => c.CategoryName == "Цель").Count() == 0)
            {
                var c = new Categories();
                c.CategoryName = "Цель";
                data.Categories.Add(c);
                data.SaveChanges();
                goalCategoryId = c.CategoryId;
            }
            else
                goalCategoryId = data.Categories.Where(c => c.CategoryName == "Цель").First().CategoryId;

            // Установить выбор дат в период этого месяца
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;
            }
            else
            {
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }
            ShowMoneys();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                comboBox1.Enabled = true;
            else
                comboBox1.Enabled = false;
            ShowMoneys();

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                comboBox2.Enabled = true;
            else
                comboBox2.Enabled = false;
            ShowMoneys();

        }


        // Добавить операцию
        private void button1_Click(object sender, EventArgs e)
        {
            string category = comboBox3.Text;
            int sum = Convert.ToInt32(numericUpDown1.Value);
            string desc = richTextBox1.Text;
            DateTime date = dateTimePicker3.Value;

            Categories categories;
            if (category == "") category = "Без категории";
            if(data.Categories.Where(c => c.CategoryName == category).Count() == 0)
            {
                categories = new Categories();
                categories.CategoryName = category;
                data.Categories.Add(categories);
                data.SaveChanges();
            } else
            {
                categories = data.Categories.Where(c => c.CategoryName == category).First();
            }

            if (desc == "") desc = "Без названия";

            Operations operation = new Operations();
            operation.CategoryId = categories.CategoryId;
            operation.Money = sum;
            operation.OperationDate = date;
            operation.UserId = moneyUserID;
            operation.OperationName = desc;

            data.Operations.Add(operation);
            data.SaveChanges();

            MessageBox.Show("Готово!");
            ShowMoneys();
            ShowCategory();

            comboBox3.Text = "";
            numericUpDown1.Value = 0;
            richTextBox1.Text = "";
            dateTimePicker3.Value = DateTime.Now;
        }

        // Отобразить операции
        private void ShowMoneys()
        {
            listView1.Items.Clear();
            var operations = data.Operations.Where(o => o == o);
            if (checkBox1.Checked)
            {
                DateTime start = dateTimePicker1.Value;
                DateTime end = dateTimePicker2.Value;
                operations = operations.Where(o => o.OperationDate >= start && o.OperationDate <= end);
            }

            if (checkBox2.Checked && comboBox1.Text != "")
            {
                string category = comboBox1.Text;
                operations = operations.Where(o => o.Categories.CategoryName == category);
            }

            if (checkBox3.Checked && comboBox2.SelectedValue != null)
            {
                int userId = (int) comboBox2.SelectedValue;
                operations = operations.Where(o => o.UserId == userId);
            }

            operations = operations.OrderByDescending(o => o.OperationDate);

            int plus = 0, minus = 0;

            foreach (var operation in operations)
            {
                string category = data.Categories.Find(operation.CategoryId).CategoryName;
                int code = operation.OperationId;
                string user = data.Users.Find(operation.UserId).UserName;
                int sum = operation.Money;

                if (sum > 0) plus += sum;
                if (sum < 0) minus += sum;

                ListViewItem item = new ListViewItem(new string[] {code.ToString(), category, user, sum.ToString()});
                item.ForeColor = Color.White;
                if (sum > 0)
                    item.BackColor = Color.Green;
                if (sum < 0)
                    item.BackColor = Color.Red;
                listView1.Items.Add(item);
            }
            label8.Text = $"Итог: {plus + minus} руб. | Приход: {plus} руб. | Уход: {Math.Abs(minus)} руб.";
        }

        private void ShowCategory()
        {
            comboBox1.Items.Clear();
            comboBox3.Items.Clear();

            var cat = data.Categories;

            foreach (var c in cat)
            {
                comboBox1.Items.Add(c.CategoryName);
                if (c.CategoryName == "Цель") continue;
                comboBox3.Items.Add(c.CategoryName);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ShowMoneys();
        }

        private void MoneysForm_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "myMoneyGoalsDataSet.Goals". При необходимости она может быть перемещена или удалена.
            this.goalsTableAdapter.Fill(this.myMoneyGoalsDataSet.Goals);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "myMoneyDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this.myMoneyDataSet.Users);

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowMoneys();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            ShowMoneys();
        }

        // Отобразить информацию об операции
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            int id = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text);
            var oper = data.Operations.Find(id);
            string name = data.Users.Find(oper.UserId).UserName;
            string category = data.Categories.Find(oper.CategoryId).CategoryName;
            string desc = oper.OperationName;
            int money = oper.Money;
            DateTime date = oper.OperationDate.Value;

            DialogResult res = MessageBox.Show($"Пользователь {name}\nсовершил операцию {desc}\n({category})\nна сумму {money} руб.\n{date.ToString("D")}\nУдалить операцию?",
                "Информация", MessageBoxButtons.YesNo);

            if(res == DialogResult.Yes && MessageBox.Show("Вы уверены, что хотите удалить операцию?", "Удалить операцию", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                data.GoalsOperations.RemoveRange(data.GoalsOperations.Where(g => g.OperationId == id));
                data.Operations.Remove(data.Operations.Find(id));
                data.SaveChanges();
                ShowMoneys();
            }
        }

        public void SetUserName(string name)
        {
            label3.Text = "Пользователь: " + name;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            goalsBindingSource.Filter = "UserId=" + moneyUserID.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            goalsBindingSource.Filter = "";

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedValue == null) return;

            var goal = data.Goals.Find(listBox1.SelectedValue);
            label9.Text = $"Имя цели: {goal.GoalName}\nПользователь: {data.Users.Find(goal.UserId).UserName}" +
                $"\nСумма: {goal.GoalSum}";

            var operations = data.GoalsOperations.Where(o => o.GoalId == goal.GoalId);
            listView2.Items.Clear();
            int sum = 0;
            foreach (var op in operations)
            {
                var operation = data.Operations.Find(op.OperationId);
                string user = data.Users.Find(operation.UserId).UserName;
                listView2.Items.Add(new ListViewItem(new string[] {user, Math.Abs(operation.Money).ToString(), operation.OperationDate.Value.ToString("d")}));
                sum += Math.Abs(operation.Money);
            }
            label9.Text += $"\nНакоплено: {sum}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedValue == null) return;

            GoalsOperations goalOperation = new GoalsOperations();
            goalOperation.Operations = new Operations();
            goalOperation.Operations.CategoryId = goalCategoryId;
            goalOperation.Operations.UserId = moneyUserID;
            goalOperation.Operations.Money = Convert.ToInt32( numericUpDown2.Value ) * -1;
            goalOperation.Operations.OperationDate = DateTime.Now;
            goalOperation.Operations.OperationName = "На цель " + data.Goals.Find(listBox1.SelectedValue).GoalName;
            goalOperation.GoalId = Convert.ToInt32(listBox1.SelectedValue);
            data.Operations.Add(goalOperation.Operations);
            data.GoalsOperations.Add(goalOperation);
            data.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[GoalsOperations] ON");
            data.SaveChanges();
            ShowMoneys();
            listBox1_SelectedIndexChanged(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Заполните название цели!");
                return;
            }

            Goals goal = new Goals();
            goal.GoalName = textBox1.Text;
            goal.UserId = moneyUserID;
            goal.GoalSum = Convert.ToInt32(numericUpDown3.Value);
            data.Goals.Add(goal);
            data.SaveChanges();

            textBox1.Text = "";
            numericUpDown3.Value = 1;

            MessageBox.Show("Цель добавлена");

            this.goalsTableAdapter.Fill(this.myMoneyGoalsDataSet.Goals);
        }
    }
}
