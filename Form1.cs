using employeeDatabaseStorage.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                string skills = txtSkills.Text;
                string phone = txtPhone.Text; 
                string selectedDate = dtpDate.Value.ToString("yyyy-MM-dd");

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lastName))
                {
                    MessageBox.Show("Both name and last name are required.");
                    return;
                }

                using (MySqlConnection connection = Data.Connection.dataSource())
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO employees (Name, LastName,Address,Skills,Phone,DateOfBirth) VALUES (@name, @lastName, @address,@skills,@phone,@dateOfBirth)";
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue ("address", address);
                        command.Parameters.AddWithValue("skills", skills);
                        command.Parameters.AddWithValue("Phone", phone);
                        command.Parameters.AddWithValue("@dateOfBirth", selectedDate);

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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRowIndex = dgvDb.SelectedCells[0].RowIndex;
                int idToUpdate = Convert.ToInt32(dgvDb.Rows[selectedRowIndex].Cells["id"].Value);

                using (MySqlConnection connection = Data.Connection.dataSource())
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM employees WHERE id = @idToUpdate";
                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@idToUpdate", idToUpdate);
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader["Name"].ToString();
                                string lastName = reader["LastName"].ToString();
                                string address = reader["Address"].ToString();
                                string skills = reader["Skills"].ToString();
                                string phone = reader["Phone"].ToString() ;
                                string selectedDate = reader["DateOfBirth"].ToString();

                                txtName.Text = name;
                                txtLastname.Text = lastName;
                                txtAddress.Text = address;
                                txtSkills.Text = skills;
                                txtPhone.Text = phone;
                                dtpDate.Text = selectedDate;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRowIndex = dgvDb.SelectedCells[0].RowIndex;
                int idToUpdate = Convert.ToInt32(dgvDb.Rows[selectedRowIndex].Cells["id"].Value);

                string name = txtName.Text;
                string lastName = txtLastname.Text;
                string address = txtAddress.Text;
                string skills = txtSkills.Text; 
                string phone = txtPhone.Text;
                string selectedDate = dtpDate.Value.ToString("yyyy-MM-dd");

                using (MySqlConnection connection = Data.Connection.dataSource())
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM employees WHERE id = @idToUpdate";
                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@idToUpdate", idToUpdate);

                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string currentName = reader["Name"].ToString();
                                string currentLastName = reader["LastName"].ToString();
                                string currentAddress = reader["Address"].ToString();
                                string currentSkills = reader["Skills"].ToString() ;
                                string currentPhone = reader["Phone"].ToString();
                                string currentDate = reader["DateOfBirth"].ToString();

                                if (string.IsNullOrWhiteSpace(name))
                                {
                                    name = currentName;
                                }

                                if (string.IsNullOrWhiteSpace(lastName))
                                {
                                    lastName = currentLastName;
                                }

                                reader.Close();

                                if (name != currentName || lastName != currentLastName || address != currentAddress || skills != currentSkills || phone != currentPhone || selectedDate != currentDate)
                                {
                                    string updateQuery = "UPDATE employees SET Name = @name, LastName = @lastName, Address = @address, Skills = @skills, Phone = @phone,DateOfBirth = @DateOfBirth  WHERE id = @idToUpdate";
                                    using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                                    {
                                        command.Parameters.AddWithValue("@name", name);
                                        command.Parameters.AddWithValue("@lastName", lastName);
                                        command.Parameters.AddWithValue("@address", address);
                                        command.Parameters.AddWithValue("@skills", skills);
                                        command.Parameters.AddWithValue("@phone", phone);
                                        command.Parameters.AddWithValue("DateOfBirth", selectedDate);
                                        command.Parameters.AddWithValue("@idToUpdate", idToUpdate);

                                        int rowsAffected = command.ExecuteNonQuery();
                                        if (rowsAffected > 0)
                                        {
                                            MessageBox.Show("Data updated successfully.");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Data update failed.");
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No changes were made.");
                                }
                            }
                        }
                    }
                }
                txtName.Text = "";
                txtLastname.Text = "";
                txtAddress.Text = "";
                txtSkills.Text = "";
                txtPhone.Text = "";
    
                btnReload_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(MySqlConnection connection = Data.Connection.dataSource())
            {
                connection.Open();
                string query = "select * from employees";
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    query += " WHERE Name LIKE @searchQuery COLLATE utf8mb4_unicode_ci OR LastName LIKE @searchQuery COLLATE utf8mb4_unicode_ci OR Address LIKE @searchQuery COLLATE utf8mb4_unicode_ci OR Skills LIKE @searchQuery COLLATE utf8mb4_unicode_ci";


                }

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@searchQuery", "%" + txtSearch.Text + "%");

                MySqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dgvDb.DataSource = dt;
                dgvDb.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            dtpDate.CustomFormat = "dd/MM/yyyy";
        }

        private void dtpDate_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Back)
            {
                dtpDate.CustomFormat = " ";
            }
        }
    }
}
