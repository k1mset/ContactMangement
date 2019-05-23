using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace InventoryApplication
{
    public partial class BizContacts : Form
    {
        // Connection string used to hold the location of the database
        string connString = @"Data Source=DESKTOP-5PR7HGP\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;Connect Timeout=30;
            Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // Variables used for handeling the database
        SqlDataAdapter dataAdapter;
        DataTable table;
        SqlConnection conn;
        string selectionStatement = "Select * from BizContacts";

        public BizContacts()
        {
            InitializeComponent();
        }

        // Sets the form on load
        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0;
            dataGridView.DataSource = bindingSource;

            GetData(selectionStatement);
        }

        // GetData method pulls information from the database using the selectCommand string
        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString);
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                bindingSource.DataSource = table;
                dataGridView.Columns[0].ReadOnly = true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Inserts text field information, as well as the picture, into the database when the user clicks on the Add button.
        private void btnAdd_Click(object sender, EventArgs e)
        {
            SqlCommand command;

            string insert = @"insert into BizContacts(Date_Added, Company, Website, Title, First_Name, Last_Name, Address, City, State, Postal_Code,
                Mobile, Notes, Image)
                values(@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name, @Address, @City, @State, @Postal_Code,
                @Mobile, @Notes, @Image)";

            using(conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker.Value.Date);
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"First_Name", txtFirstName.Text);
                    command.Parameters.AddWithValue(@"Last_Name", txtLastName.Text);
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);
                    command.Parameters.AddWithValue(@"City", txtCity.Text);
                    command.Parameters.AddWithValue(@"State", txtState.Text);
                    command.Parameters.AddWithValue(@"Postal_Code", txtPostalCode.Text);
                    command.Parameters.AddWithValue(@"Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Notes", txtNotes.Text);

                    if (openFileDialog.FileName != "")
                    {
                        command.Parameters.AddWithValue(@"Image", File.ReadAllBytes(openFileDialog.FileName));
                    } else
                    {
                        command.Parameters.Add(@"Image", SqlDbType.VarBinary).Value = DBNull.Value;
                    }

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            GetData(selectionStatement);
            dataGridView.Update();
        }

        // Updates information in the database when the user edits the information in the data grid.
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            try
            {
                bindingSource.EndEdit();
                dataAdapter.Update(table);
                MessageBox.Show("Update Successful.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Deletes a row of the database information when the user selects that row, then clicks the delete button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView.CurrentCell.OwningRow;
            string value = row.Cells["ID"].Value.ToString();
            string fname = row.Cells["First_Name"].Value.ToString();
            string lname = row.Cells["Last_Name"].Value.ToString();
            DialogResult result = MessageBox.Show("Do you really want to delete " + fname + " " + lname + ", record " + value + "?","Message", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string deleteStatement = @"Delete from BizContacts where id = '" + value+"'";

            if(result == DialogResult.Yes)
            {
                using(conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(deleteStatement, conn);
                        comm.ExecuteNonQuery();
                        GetData(selectionStatement);
                        dataGridView.Update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        // Searches the database for the users desired information
        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (cboSearch.SelectedItem.ToString())
            {
                case "First Name":
                    GetData("Select * from BizContacts where lower(first_name) like '%" + txtSearch.Text.ToLower()+"%'");
                    break;

                case "Last Name":
                    GetData("Select * from BizContacts where lower(last_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;

                case "Company":
                    GetData("Select * from BizContacts where lower(company) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
            }
        }

        // Opens a dialog box to select an image from the harddrive
        private void btnGetImage_Click(object sender, EventArgs e)
        {

             if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Load(openFileDialog.FileName);
            }
        }

        // Expands the image to be seen better if it is difficult to view in the form
        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Text = "Image Viewer";
            frm.BackgroundImage = pictureBox.Image;
            frm.Size = pictureBox.Image.Size;
            frm.Show();
        }
    }
}
