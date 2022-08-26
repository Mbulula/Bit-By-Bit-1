using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Dashboard : Form
    {
        private object con;

        public Dashboard()
        {
            InitializeComponent();
        }

        public int ticketId { get; private set; }
        public object DataGridView2 { get; private set; }
        public decimal TicketPrice { get; set; } = 0;
        public decimal CurrentDiscount { get; set; } = 0;
        public int TicketID { get; set; } = 0;

        private void Dashboard_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Items' table. You can move, or remove it, as needed.
            this.itemsTableAdapter.Fill(this.groupPmb8DataSet.Items);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.ItemServices' table. You can move, or remove it, as needed.
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Items' table. You can move, or remove it, as needed.
            this.itemsTableAdapter.Fill(this.groupPmb8DataSet.Items);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Payment' table. You can move, or remove it, as needed.
            this.paymentTableAdapter.Fill(this.groupPmb8DataSet.Payment);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.SelectedService' table. You can move, or remove it, as needed.
            this.selectedServiceTableAdapter.Fill(this.groupPmb8DataSet.SelectedService);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.ItemServices' table. You can move, or remove it, as needed.
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Service' table. You can move, or remove it, as needed.
            this.serviceTableAdapter.Fill(this.groupPmb8DataSet.Service);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Admin' table. You can move, or remove it, as needed.
            this.adminTableAdapter.Fill(this.groupPmb8DataSet.Admin);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Items' table. You can move, or remove it, as needed.
            ////////this.itemsTableAdapter.Fill(this.groupPmb8DataSet.Items);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Ticket' table. You can move, or remove it, as needed.
            this.ticketTableAdapter.Fill(this.groupPmb8DataSet.Ticket);
            // TODO: This line of code loads data into the 'groupPmb8DataSet.Customer' table. You can move, or remove it, as needed.
            this.customerTableAdapter.Fill(this.groupPmb8DataSet.Customer);
            AddServiceBtn.Visible = false;
            addServiceCombobox.Visible = false;
        }

        // add customer
        private void AddCustomerButton_Click(object sender, EventArgs e)
        {
            string fname = CustomerFirstName.Text;
            string lname = CustomerLastName.Text;
            string email = CustomerEmail.Text;
            string gender = customerGender.SelectedItem.ToString();
            string haddress = CustomerHomeAddress.Text;
            string phone = customerPhoneNumber.Text;

            // validation

            if (string.IsNullOrWhiteSpace(fname))
            {
                MessageBox.Show("Missing customer name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //

            // this.customerTableAdapter.AddCustomer(fname, lname, gender, phone, haddress, email);

            this.customerTableAdapter.AddCustomer(Email: email, Gender: gender, Address: haddress, Phone: phone, LastName: lname, FirstName: fname);

            // refresh customer data grid, to see newly inserted customer
            this.customerTableAdapter.Fill(this.groupPmb8DataSet.Customer);

        }

        private void ItemsTab_Click(object sender, EventArgs e)
        {

        }

        private void CustomerFirstName_TextChanged(object sender, EventArgs e)
        {

        }

        private void addTicketButton_Click(object sender, EventArgs e)
        {
            // item data
            string item = ItemName.Text;
            int qua = 0;

            // check if quantity is a number
            if (!int.TryParse(quantity.Text, out qua))
            {
                MessageBox.Show("Please enter digit for quantity", "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // return if quanity not number
                return;
            }

            // ticket data
            int customerId = int.Parse(selectCustomer.SelectedValue.ToString());
            int adminId = int.Parse(selectAdmin.SelectedValue.ToString());
            string dropOff = DateTime.Now.Date.ToString();
            string pickUp = pickUpDate.Value.ToString();
            decimal amountDue = decimal.Parse(selectedServicePrice.SelectedValue.ToString()) * qua;
            string collectionMethod = selectCollection?.SelectedItem.ToString();

            if (collectionMethod.ToLower() == "delivery")
            {
                // add delivery fee if customer selected delivery
                amountDue += 50;
            }

            ticketId = (int)this.ticketTableAdapter.AddTicket(CustomerId: customerId, AdminId: adminId, DropoffDate: dropOff, p1: pickUp, Collection: collectionMethod, AmountDue: amountDue);

            int itemid = (int)this.itemsTableAdapter.AddItem(ItemName: item, Quantity: qua.ToString(), TicketId: ticketId);

            // add selected service
            this.selectedServiceTableAdapter.AddService(ItemId: itemid, ServiceId: (int)selectedService.SelectedValue);

            // refresh ticket data grid
            this.ticketTableAdapter.Fill(this.groupPmb8DataSet.Ticket);

            // refresh items data grid
            this.itemsTableAdapter.Fill(this.groupPmb8DataSet.Items);
        }

        private void payButton_Click(object sender, EventArgs e)
        {
            if (TicketPrice > 0)
            {
                DialogResult pay = MessageBox.Show($"Pay {TicketPrice.ToString("C")} for Ticket No. {TicketID}", "Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pay == DialogResult.Yes)
                {
                    this.paymentTableAdapter.MakePayment(TicketId: TicketID, PaymentDate: DateTime.Now.Date, PaymentStatus: "success", Amount: TicketPrice);
                    //// update amount due for ticket
                    this.ticketTableAdapter.UpdateTicketAmount(AmountDue: 0, TicketId: TicketID);

                    //// refersh payment datagrid
                    this.paymentTableAdapter.Fill(this.groupPmb8DataSet.Payment);

                    //// refresh ticket data grid
                    this.ticketTableAdapter.Fill(this.groupPmb8DataSet.Ticket);

                    TicketID = 0;
                    CurrentDiscount = 0;
                    payButton.Enabled = false;
                    TicketPrice = 0;
                }
            }

        }

        private void amountDue_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            int id = 0;
            var custId = customerDatagrid.SelectedRows[0].Cells[0].Value.ToString();
            if (int.TryParse(custId, out id))
            {
                var msg = MessageBox.Show("Are you sure you want to delete customer", "delete user", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msg == DialogResult.Yes)
                {
                    this.customerTableAdapter.DeleteCustomer(CustomerId: id);
                    this.customerTableAdapter.Fill(this.groupPmb8DataSet.Customer);
                    MessageBox.Show("Customer deleted successfully", "delete user", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Failed to get customer id", "delete user", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Searchbtn_Click(object sender, EventArgs e)
        {

        }

        private void paymentsTab_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void searchCustomer_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchCustomer.Text))
            {
                this.customerTableAdapter.FillBy(this.groupPmb8DataSet.Customer, searchCustomer.Text);
            }
            else
            {
                // if search field is empty return all clothes
                this.customerTableAdapter.Fill(this.groupPmb8DataSet.Customer);
            }

        }

        private void itemsDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int id = 0;
            var itemID = itemsDataGrid.SelectedRows[0].Cells[0].Value.ToString();
            if (int.TryParse(itemID, out id))
            {
                try
                {
                    itemServicesTableAdapter.FillBy(this.groupPmb8DataSet.ItemServices, itemId: id);
                    AddServiceBtn.Visible = true;
                    addServiceCombobox.Visible = true;
                }
                catch (Exception ex)
                {
                    foreach (DataRow dr in this.groupPmb8DataSet.ItemServices)
                    {
                        if (dr.HasErrors)
                        {
                            MessageBox.Show(" has error: " + dr.RowError);
                        }
                    }
                }
            }
            else
            {
                AddServiceBtn.Visible = false;
                addServiceCombobox.Visible = false;
            }
        }

        private void AddServiceBtn_Click(object sender, EventArgs e)
        {
            int id = 0;
            int tId = 0;
            var itemID = itemsDataGrid.SelectedRows[0].Cells[0].Value.ToString();
            var item = itemsDataGrid.SelectedRows[0].Cells[1].Value.ToString();
            var ticketId = itemsDataGrid.SelectedRows[0].Cells[3].Value.ToString();
            decimal amount = 0;
            if (int.TryParse(itemID, out id) && int.TryParse(ticketId, out tId) && decimal.TryParse(AddServicePrice.SelectedValue.ToString(), out amount))
            {
                try
                {
                    this.selectedServiceTableAdapter.AddService(ItemId: id, ServiceId: (int)addServiceCombobox.SelectedValue);
                }
                catch (Exception)
                {
                    MessageBox.Show($"service is already added for {item}", "Already Exist", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                itemServicesTableAdapter.FillBy(this.groupPmb8DataSet.ItemServices, itemId: id);
                ticketTableAdapter.UpdateAmountDue(amount, OTicketId: tId, TicketId: tId);
                // new amount
                this.ticketTableAdapter.Fill(this.groupPmb8DataSet.Ticket);
                MessageBox.Show($"Service Added for {item}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to add new service", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addServiceCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddServicePrice.SelectedIndex = addServiceCombobox.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dashboard Discount_Form = new Discount_Form();
            Discount_Form.Show();
            this.Hide();
        }

        public static implicit operator Dashboard(Discount_Form v)
        {
            throw new NotImplementedException();
        }

        private void payTicketDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            payButton.Enabled = true;
            TicketID = (int)payTicketDataGrid.SelectedRows[0].Cells[0].Value;
            var amount = (decimal)payTicketDataGrid.SelectedRows[0].Cells[5].Value;

            amountDueTextBox.Text = amount.ToString("C");

            addDiscountBtn.Visible = amount >= 200;
            if (amount >= 200)
            {
                addDiscountBtn.Visible = true;
                if (amount >= 200 && amount < 250)
                {
                    addDiscountBtn.Text = "Apply 5% discount";
                    CurrentDiscount = 5;
                }
                else if (amount >= 250 && amount < 300)
                {
                    addDiscountBtn.Text = "Apply 10% discount";
                    CurrentDiscount = 10;
                }
                else if (amount >= 300 && amount < 350)
                {
                    addDiscountBtn.Text = "Apply 15% discount";
                    CurrentDiscount = 15;
                }
                else if (amount >= 350 && amount < 400)
                {
                    addDiscountBtn.Text = "Apply 20% discount";
                    CurrentDiscount = 20;
                }
                else
                {
                    addDiscountBtn.Text = "Apply 25% discount";
                    CurrentDiscount = 25;
                }
            }
            else
            {
                addDiscountBtn.Visible = false;
                CurrentDiscount = 0;
            }
            TicketPrice = amount;
        }

        private void addDiscountBtn_Click(object sender, EventArgs e)
        {
            if (CurrentDiscount > 0 && TicketPrice >= 200)
            {
                var x = CurrentDiscount / 100;
                var discountedPrice = TicketPrice * x;
                DialogResult applyDisc = MessageBox.Show($"Apply a {discountedPrice.ToString("C")} discount", "Discount", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (applyDisc == DialogResult.Yes)
                {
                    TicketPrice = TicketPrice - discountedPrice;
                    amountDueTextBox.Text = TicketPrice.ToString("C");
                    CurrentDiscount = 0;
                    addDiscountBtn.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("No discount to apply", "Discount", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

