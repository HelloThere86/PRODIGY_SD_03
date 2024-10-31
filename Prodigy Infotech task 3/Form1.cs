using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Prodigy_Infotech_task_3
{
    public partial class ContactManagerForm : Form
    {
        private List<Contact> contacts = new List<Contact>();
        private string dataFile = "contacts.txt";

        public ContactManagerForm()
        {
            InitializeComponent();
            LoadContacts();
            DisplayContacts();
        }

        private void UpdateContactButton_Click(object sender, EventArgs e)
        {
            if (contactsListBox.SelectedIndex != -1)
            {
                Contact selectedContact = contacts[contactsListBox.SelectedIndex];

                // Update contact details
                selectedContact.FirstName = firstNameTextBox.Text;
                selectedContact.LastName = lastNameTextBox.Text;
                selectedContact.PhoneNumber = phoneNumberTextBox.Text;
                selectedContact.Email = emailTextBox.Text;

                DisplayContacts();
                MessageBox.Show("Contact updated successfully.");
            }
            else
            {
                MessageBox.Show("Please select a contact to update.");
            }
        }

        private void DeleteContactButton_Click(object sender, EventArgs e)
        {
            if (contactsListBox.SelectedIndex != -1)
            {
                contacts.RemoveAt(contactsListBox.SelectedIndex);
                DisplayContacts();
                MessageBox.Show("Contact deleted successfully.");
            }
            else
            {
                MessageBox.Show("Please select a contact to delete.");
            }
        }

        private void contactsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (contactsListBox.SelectedIndex != -1)
            {
                Contact selectedContact = contacts[contactsListBox.SelectedIndex];
                firstNameTextBox.Text = selectedContact.FirstName;
                lastNameTextBox.Text = selectedContact.LastName;
                phoneNumberTextBox.Text = selectedContact.PhoneNumber;
                emailTextBox.Text = selectedContact.Email;
            }
        }

        private void DisplayContacts()
        {
            contactsListBox.Items.Clear();
            var sortedContacts = contacts.OrderBy(c => c.FirstName).ThenBy(c => c.LastName);
            foreach (Contact contact in sortedContacts)
            {
                contactsListBox.Items.Add($"{contact.FirstName} {contact.LastName}");
            }
        }

        private void LoadContacts()
        {
            try
            {
                if (File.Exists(dataFile))
                {
                    using (StreamReader reader = new StreamReader(dataFile))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length == 4)
                            {
                                Contact contact = new Contact(parts[0], parts[1], parts[2], parts[3]);
                                contacts.Add(contact);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading contacts: {ex.Message}");
            }
        }

        private void SaveContacts()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(dataFile))
                {
                    foreach (Contact contact in contacts)
                    {
                        writer.WriteLine($"{contact.FirstName},{contact.LastName},{contact.PhoneNumber},{contact.Email}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving contacts: {ex.Message}");
            }
        }

        private void ContactManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveContacts();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string firstName = firstNameTextBox.Text;
            string lastName = lastNameTextBox.Text;
            string phoneNumber = phoneNumberTextBox.Text;
            string email = emailTextBox.Text;

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            if (!IsValidPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Invalid phone number format.");
                return;
            }

            if (contacts.Any(c => c.Email == email || c.PhoneNumber == phoneNumber))
            {
                MessageBox.Show("A contact with this email or phone number already exists.");
                return;
            }

            Contact newContact = new Contact(firstName, lastName, phoneNumber, email);
            contacts.Add(newContact);

            ClearInputFields();
            DisplayContacts();
            MessageBox.Show("Contact added successfully.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\+?[1-9]\d{1,14}$");
        }

        private void ClearInputFields()
        {
            firstNameTextBox.Clear();
            lastNameTextBox.Clear();
            phoneNumberTextBox.Clear();
            emailTextBox.Clear();
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.ToLower();
            contactsListBox.Items.Clear();
            var filteredContacts = contacts
                .Where(c => c.FirstName.ToLower().Contains(searchTerm) || c.LastName.ToLower().Contains(searchTerm))
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName);

            foreach (Contact contact in filteredContacts)
            {
                contactsListBox.Items.Add($"{contact.FirstName} {contact.LastName}");
            }
        }
    }

    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Contact(string firstName, string lastName, string phoneNumber, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
