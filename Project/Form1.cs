using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int activeUserId;
        // This is a function that returns the major id of the active student
        private int getMajorId()
        {
            string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connString);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = "Select id from Major where name = '" + InfoMajor.Text + "'";

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    return (int) reader["id"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
              
            }
            finally { conn.Close(); }
            return -1;
        }
       
        // passwords encryption
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                txtPassword.PasswordChar = '\0';
            else txtPassword.PasswordChar = '*';
            txtPassword.Focus();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                txtRegisterConfirmPass.PasswordChar = '\0';
                txtRegisterPass.PasswordChar = '\0';
            }
            else
            {
                txtRegisterConfirmPass.PasswordChar = '*';
                txtRegisterPass.PasswordChar = '*';
            }
            txtRegisterPass.Focus();
        }

        private bool IsValidName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("Fill your first name please");
                return false;
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Fill your last name please");
                return false;
            }
            if (!Regex.IsMatch(firstName, @"^[a-zA-Z]{5,20}$"))
            {
                MessageBox.Show("Your first name should only contain characters");
                return false;
            }
            if (!Regex.IsMatch(lastName, @"^[a-zA-Z]{5,20}$"))
            {
                MessageBox.Show("Your last should only contain characters");
                return false;
            }
            return true;
        }
        private bool IsValidPasswords(string pass1, string pass2)
        {
            if (string.IsNullOrWhiteSpace(pass1))
            {
                MessageBox.Show("Fill your password please");
                return false;
            }
            if (string.IsNullOrWhiteSpace(pass2))
            {
                MessageBox.Show("Fill your confirmation password please");
                return false;
            }
            if (pass1.Length < 5)
            {
                MessageBox.Show("Your Password should be at least 6 characters");
                return false;
            }
            if (pass1 != pass2)
            {
                MessageBox.Show("Passwords are not the same");
                return false;
            }
            if(!Regex.IsMatch(pass1, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{7,}$"))
            {
                MessageBox.Show("A password should have at least one captial letter and one number. And should be at least 7 characters");
                return false;
            }
            return true;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            bool validName = false, validPasswords = false, validMajor = false ;
            if (Major.SelectedIndex == -1)
                MessageBox.Show("Select a Major");
            else validMajor = true;
            if (IsValidName(txtRegFirstName.Text, txtRegLastName.Text));
                validName = true;
            if (IsValidPasswords(txtRegisterPass.Text, txtRegisterConfirmPass.Text))
                validPasswords = true;
            if(validName == true && validPasswords == true && validMajor == true)
            {
                string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
                SqlConnection conn = new SqlConnection(connString);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = $"INSERT INTO Student (firstName, lastName, major , password) VALUES ('{txtRegFirstName.Text}', '{txtRegLastName.Text}', '{Major.SelectedItem}' , '{txtRegisterPass.Text}') ";

                // Getting the last id in the Student table to get the id of the new registered Student

                SqlCommand cmd2 = new SqlCommand();
                cmd2.Connection = conn;

                cmd2.CommandText = $"SELECT TOP (1) id FROM Student ORDER BY id DESC";

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    SqlDataReader reader = cmd2.ExecuteReader();

                    if(reader.Read())
                        activeUserId = (int)reader["id"];

                    reader.Close();


                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                { conn.Close(); }


                

                tabControl1.Visible = false;
                tab.Visible = true;

                displayStudentInfo(activeUserId, (string)Major.SelectedItem);

            }
        }

        // password visible.
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                txtOldPass.PasswordChar = '\0';
                txtNewPass.PasswordChar = '\0';
                txtConfirmNewPass.PasswordChar = '\0';
            }
            else
            {
                txtOldPass.PasswordChar = '*';
                txtNewPass.PasswordChar = '*';
                txtConfirmNewPass.PasswordChar = '*';
            }
            txtNewPass.Focus();
        }

        // Clearing all fields of the registration form
        public void clearFields()
        {
            txtRegFirstName.Clear();
            txtRegLastName.Clear();
            txtRegisterPass.Clear();   
            txtRegisterConfirmPass.Clear(); 
            Major.SelectedIndex = -1;
            txtUsername.Clear();
            txtPassword.Clear();
            checkBox1.Checked = false;
            checkBox2.Checked = false;  
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Fill the user name field please");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Fill the password field please");
                return;
            }
            if (Regex.IsMatch(txtUsername.Text, "^[1-9]$"))
            {
                MessageBox.Show("The id should be a number");
                return;
            }
            string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connString);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * from Student where id = " + txtUsername.Text;

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    MessageBox.Show("Wrong id");
                    return;
                }
                else
                {
                    if (txtPassword.Text != (string)reader["password"])
                    {
                        MessageBox.Show("Wrong password");
                        return;
                    }
                    activeUserId = (int)reader["id"];
                    tabControl1.Visible = false;
                    tab.Visible = true;
                    displayStudentInfo((int)reader["id"], reader["major"].ToString());
                    tab.SelectedTab = InfoPage;
                    fillInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
            finally
            {
                conn.Close();
            }

        }

        private void displayStudentInfo(int id, string major)
        {
            InfoId.Text = id.ToString();
          
            InfoMajor.Text = major;
            InfoCreditsLeft.Text = 100.ToString();

            string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select creditCost from Major where name = '" + major + "'";

            try
            {
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    InfoCreditCost.Text = ((int)dr["creditCost"]).ToString();
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
            finally { conn.Close(); }
        }
        // This is a function that fill information about the active student
        private void fillInfo()
        {
            string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connString);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = "Select * from enroll where studentId = " + activeUserId;

            // This is a list to add the registered courses id if any

            List<int> ids = new List<int>();
            int majorId = getMajorId();

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ids.Add((int)reader["courseId"]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
            finally
            {
                conn.Close();
            }

            if (ids.Count > 0)
            {
                int nbOfRegisteredCredits = 0;
                SqlCommand cmd2 = new SqlCommand();
                cmd2.Connection = conn;

                try
                {
                    conn.Open();

                    foreach (var id in ids)
                    {

                        cmd2.CommandText = "Select * from Course where id = " + id;
                        SqlDataReader reader = cmd2.ExecuteReader();
                        if (reader.Read())
                        {
                            ListViewItem listViewItem = new ListViewItem((string)reader["code"]);
                            listViewItem.SubItems.Add((string)reader["title"]);
                            listViewItem.SubItems.Add(((int)reader["credits"]).ToString());
                            nbOfRegisteredCredits += (int)reader["credits"];
                            listViewItem.SubItems.Add((string)reader["schedule"]);
                            listViewItem.SubItems.Add((string)reader["instructor"]);
                            classView.Items.Add(listViewItem);

                            ListViewItem listViewItem2 = new ListViewItem((string)reader["code"]);
                            listViewItem2.SubItems.Add((string)reader["title"]);
                            listViewItem2.SubItems.Add(reader["credits"].ToString());
                            listViewItem2.SubItems.Add((int.Parse(InfoCreditCost.Text) * (int)reader["credits"]).ToString());

                            paymentView.Items.Add(listViewItem2);
                        }
                        InfoCreditsLeft.Text = (100 - nbOfRegisteredCredits).ToString();

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    conn.Close();
                }

            }

            SqlCommand cmd3 = new SqlCommand();
            cmd3.Connection = conn;

            cmd3.CommandText = "Select * from Course where majorId = '" + majorId + "'";

            try
            {
                conn.Open();
                SqlDataReader reader = cmd3.ExecuteReader();

                while (reader.Read())
                {
                    ListViewItem listViewItem = new ListViewItem((string)reader["code"]);
                    listViewItem.SubItems.Add(reader["title"].ToString());
                    listViewItem.SubItems.Add(reader["credits"].ToString());
                    listViewItem.SubItems.Add(reader["schedule"].ToString());
                    viewCourses.Items.Add(listViewItem);
                    chckListCourses.Items.Add((string)reader["code"]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
            finally { conn.Close(); }



        }
        private void btnRegCourses_Click(object sender, EventArgs e)
        {

            if(chckListCourses.SelectedIndex == -1)
            {
                MessageBox.Show("You have not choosen any course to register");
                return;
            }

            bool containsValue = false;

            foreach (ListViewItem item in classView.Items)
            {
                if (item.Text == (string)chckListCourses.SelectedItem)
                {
                    containsValue = true;
                    break;
                }
            }

            if (containsValue)
            {
                MessageBox.Show("You have already been enrolled the course " + (string)chckListCourses.SelectedItem);
                return;
            }
            else
            {
                string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
                SqlConnection conn = new SqlConnection(connString);

                SqlCommand cmd = new SqlCommand();
                SqlCommand cmd2 = new SqlCommand();

                cmd.Connection = conn;
                cmd2.Connection = conn;

                cmd.CommandText = "Select * from Course where code = '" + chckListCourses.SelectedItem + "'";

                int creditsLeft = int.Parse(InfoCreditsLeft.Text);

                int registeredCourseId = 0;

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        registeredCourseId = (int)reader["id"];

                        ListViewItem listViewItem = new ListViewItem((string)reader["code"]);
                        listViewItem.SubItems.Add((string)reader["title"]);
                        listViewItem.SubItems.Add(((int)reader["credits"]).ToString());
                        creditsLeft -= (int)reader["credits"];
                        listViewItem.SubItems.Add((string)reader["schedule"]);
                        listViewItem.SubItems.Add((string)reader["instructor"]);
                        classView.Items.Add(listViewItem);
                        InfoCreditsLeft.Text = creditsLeft.ToString();

                        ListViewItem listViewItem2 = new ListViewItem((string)reader["code"]);
                        listViewItem2.SubItems.Add((string)reader["title"]);
                        listViewItem2.SubItems.Add(reader["credits"].ToString());
                        listViewItem2.SubItems.Add((int.Parse(InfoCreditCost.Text) * (int)reader["credits"]).ToString());

                        paymentView.Items.Add(listViewItem2);

                        reader.Close();

                        tab.SelectedTab = myClassesPage;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally { conn.Close(); }

                try
                {
                    conn.Open();
                    cmd2.CommandText = $"INSERT INTO enroll (studentId , courseId) VALUES ('{activeUserId}' , '{registeredCourseId}')";
                    cmd2.ExecuteNonQuery();
                }
                catch(Exception ex) { MessageBox.Show(ex.Message); }
                finally { conn.Close(); }
            }

        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            if (chckListCourses.SelectedIndex == -1)
            {
                MessageBox.Show("You have not choosen any course to register");
                return;
            }

            bool containsValue = false;

            foreach (ListViewItem item in classView.Items)
            {
                if (item.Text == (string)chckListCourses.SelectedItem)
                {
                    containsValue = true;
                    classView.Items.Remove(item);
                    break;
                }
            }
            // If the class list view contains this course than payments must contain this course
            if (containsValue)
            {
                foreach (ListViewItem item in paymentView.Items)
                {
                    if (item.Text == (string)chckListCourses.SelectedItem)
                    {
                        paymentView.Items.Remove(item);
                        break;
                    }
                }
            }


            if (containsValue)
            {
                // This variable to store the course id that the user need to drop
                int CourseId = 0;

                string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
                SqlConnection conn = new SqlConnection(connString);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "Select * from Course where code = '" + (string)chckListCourses.SelectedItem + "'";
                try
                {
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                        CourseId = (int)dr["id"];

                    InfoCreditsLeft.Text = (int.Parse(InfoCreditsLeft.Text) + (int)dr["credits"]).ToString();
                    dr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally { conn.Close(); }

                SqlCommand cmd2 = new SqlCommand();
                cmd2.Connection = conn;

                cmd2.CommandText = "Delete from enroll where courseId = '" + CourseId + "' AND studentId = '" + activeUserId + "'";

                try
                {
                    conn.Open();
                    cmd2.ExecuteNonQuery();

                    tab.SelectedTab = myClassesPage;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally { conn.Close(); }
            }
            else
            {
                MessageBox.Show("You are not enrolled in this course");
                return;
            }
        }

        private void btnConfirmNewPass_Click(object sender, EventArgs e)
        {
            string connString = "Data Source=.;Initial Catalog=uniManagement;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select * from Student where id = " + txtUsername.Text;

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                
                if(!reader.Read())
                {
                    MessageBox.Show("lkjfsdalkjfsad");
                }
                else
                {
                if (txtOldPass.Text != (string)reader["password"])
                {
                    MessageBox.Show("Old password is wrong");
                    return;
                }   else
                {
                    if (string.IsNullOrWhiteSpace(txtNewPass.Text))
                    {
                        MessageBox.Show("Fill the password field please");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(txtConfirmNewPass.Text))
                    {
                        MessageBox.Show("Fill the confirmation password field please");
                        return;
                    }
                    if(txtNewPass.Text != txtConfirmNewPass.Text)
                    {
                        MessageBox.Show("Passwords are not the same");
                        return;
                    }
                    if(txtNewPass.Text == txtOldPass.Text)
                    {
                        MessageBox.Show("The new passwords is the same as the old password");
                        return;
                    }
                    reader.Close();
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = conn;
                        cmd2.CommandText = "update Student set password = '" + txtConfirmNewPass.Text + "' where id = " + activeUserId;
                        cmd2.ExecuteNonQuery();
                        MessageBox.Show("Password Changed succsefully");

                        // Clearing the passwords feilds
                        txtOldPass.Clear();
                        txtNewPass.Clear();
                        txtConfirmNewPass.Clear();
                }
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally {
                conn.Close();
            }   
        }

        // Reset the form when the form is visible
        private void studentForm_VisibleChanged(object sender, EventArgs e)
        {
            if (tab.Visible)
            {
                txtOldPass.Clear();
                txtNewPass.Clear();
                txtConfirmNewPass.Clear();
                checkBox3.Checked = false;
                viewCourses.Items.Clear();
                paymentView.Items.Clear();
                classView.Items.Clear();
                chckListCourses.Items.Clear();
                tab.SelectedTab = InfoPage;
            }
        }

        // Logout Button
        private void button_Click(object sender, EventArgs e)
        {
            clearFields();
            tabControl1.Visible = true;
            tabControl1.SelectedTab = logInPage;
            tab.Visible = false;
        }

    }
}
