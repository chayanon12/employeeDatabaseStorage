using employeeDatabaseStorage.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//ADD insert name and Lastname
namespace employeeDatabaseStorage
{
    public partial class Form1 : Form
    {
        Data.Connection conn = new Data.Connection();
        public Form1()
        {
            InitializeComponent();
            lblSucess.Visible = false;
            lblError.Visible = false;
            dgvDb.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                Data.Connection.dataSource();
                conn.connOpen();
                lblSucess.Visible = true;
                lblError.Visible = false;
                conn.connClose();
            }
            catch (Exception)
            { 
                lblError.Visible = true;
                lblSucess.Visible = false;  
                conn.connClose();
            }
            finally
            {
               
                conn.connClose ();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string lastName = txtLastname.Text;
                string address = txtAddress.Text;

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lastName))
                {
                    MessageBox.Show("Both name and last name are required.");
                    return;
                }

                using (MySqlConnection connection = Data.Connection.dataSource())
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO employees (Name, LastName,Address) VALUES (@name, @lastName, @address)";
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue ("address", address);   

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data add successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Data insertion failed.");
                        }
                    }
                }
                btnReload_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = Data.Connection.dataSource())
            {
                connection.Open();
                string query = "select * from employees";
                MySqlCommand cmd = new MySqlCommand(query,connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dgvDb.DataSource = dt;
                dgvDb.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRowIndex = dgvDb.SelectedCells[0].RowIndex;
                int idToDelete = Convert.ToInt32(dgvDb.Rows[selectedRowIndex].Cells["id"].Value);

                using (MySqlConnection connection = Data.Connection.dataSource())
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM employees WHERE id = @idToDelete";
                    using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@idToDelete", idToDelete);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data deleted successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Data deletion failed.");
                        }
                    }
                }

                btnReload_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
