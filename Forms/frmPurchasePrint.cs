using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Signature.Classes;

namespace Signature.Forms
{
    public partial class frmPurchasePrint : frmBase

    {
        Purchase oPurchase;

        public frmPurchasePrint()
        {
            InitializeComponent();
        }
        private void frmCustomerListing_Load(object sender, EventArgs e)
        {
            oPurchase = new Purchase(base.CompanyID);
            
            
            
        }
        private void txtCustomerID_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            /*
            #region txtPOID
            if (sender == txtPurchaseID)
            {
                if (e.KeyCode.ToString() == "F2")
                {
                    if (oPurchase.View())
                    {
                        
                        txtPurchaseID.Text = oPurchase.ID;
                        if (oPurchase.Find(txtPurchaseID.Text))
                        {
                            txtName.Text = oPurchase.ID + " - " + oPurchase.VendID;
;
                            AddItem();
                            txtPurchaseID.Clear();
                            txtPurchaseID.Focus();
                            return;
                        }

                    }
                    return;
                }

                if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Tab)
                {
                    if (txtPurchaseID.Text.Trim().Length == 0)
                    {
                        
                        txtPurchaseID.Focus();
                    }

                    if (oPurchase.Find(txtPurchaseID.Text))
                    {
                        
                        txtPurchaseID.Text = oPurchase.ID;
                        txtName.Text = oPurchase.Name;
                        AddItem();
                        txtPurchaseID.Clear();
                        txtPurchaseID.Focus();
                        return;
                     }
                    
                }

            }
            #endregion
            */
            #region txtPOFrom
            if (sender == txtPOFrom)
            {
                if (e.KeyCode.ToString() == "F2")
                {
                    if (oPurchase.View())
                    {

                        txtPOFrom.Text = oPurchase.ID;
                        if (oPurchase.Find(txtPOFrom.Text))
                        {
                            txtName.Text = oPurchase.Name;
                            return;
                        }

                    }
                    return;
                }

                if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Tab)
                {
                    if (txtPOFrom.Text.Trim().Length == 0)
                    {

                        txtPOFrom.Focus();
                    }

                    if (oPurchase.Find(txtPOFrom.Text))
                    {

                        txtPOFrom.Text = oPurchase.ID;
                        txtName.Text = oPurchase.ID + " - " + oPurchase.VendID;
                        return;
                    }

                }

            }
            #endregion
            #region txtPOTo
            if (sender == txtPOTo)
            {
                if (e.KeyCode.ToString() == "F2")
                {
                    if (oPurchase.View())
                    {
                        txtPOTo.Text = oPurchase.ID;
                        txtName.Text = oPurchase.ID + " - " + oPurchase.VendID;
                        if (oPurchase.Find(txtPOTo.Text))
                        {
                            txtName.Text = oPurchase.ID + " - " + oPurchase.VendID;
                            return;
                        }

                    }
                    return;
                }

                if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Tab)
                {
                    if (txtPOTo.Text.Trim().Length == 0)
                    {

                        txtPOTo.Focus();
                    }

                    if (oPurchase.Find(txtPOTo.Text))
                    {

                        txtPOTo.Text = oPurchase.ID;
                        txtName.Text = oPurchase.ID + " - " + oPurchase.VendID;
                        return;
                    }

                }

            }
            #endregion
            #region Default Option
            //Default option
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (!e.Shift)
                        this.SelectNextControl(this.ActiveControl, true, true, true, true);
                    break;
                case Keys.Enter:
                    this.SelectNextControl(this.ActiveControl, true, true, true, true);
                    break;
                case Keys.Down:
                    this.SelectNextControl(this.ActiveControl, true, true, true, true);
                    break;
                case Keys.Up:
                    this.SelectNextControl(this.ActiveControl, false, true, true, true);
                    break;
                case Keys.F8:
                    break;
                case Keys.F3:
                    break;
                case Keys.F7:
                    this.Close();
                    break;
                case Keys.PageDown:
                    FillOut();
                    break;


                //case Keys.<some key>: 
                // ......; 
                // break; 
            }
            #endregion

        }

        private void AddItem()
        {
            Infragistics.Win.UltraWinListView.UltraListViewItem Item = new Infragistics.Win.UltraWinListView.UltraListViewItem();
            Item.Tag = oPurchase.ID;
            Item.Appearance.Image = global::Signature.Forms.Properties.Resources.office_building;

            Item.Value = oPurchase.ID+"-"+oPurchase.VendID;

            lvCustomers.Items.Add(Item);
        }
        
        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            String PrinterName = "";
            PrinterDevice PrintTo = PrinterDevice.Printer;
            Smtp oSmtp =new Smtp(); 

            switch (txtType.CheckedItem.DataValue.ToString())
            {
                case "Printer":
                    PrintTo = PrinterDevice.Printer;
                    PrinterName = Global.OpenPrintDialog();
                    if (PrinterName == "")
                    {
                        return;
                    }
                    break;
                case "PDF":
                    PrintTo = PrinterDevice.PDF;
                    oSmtp.Subject = "Purchase Orders "  + DateTime.Now.ToShortDateString() + "   " + DateTime.Now.ToShortTimeString();
                    oSmtp.To = "<" + "russ@sigfund.com" + ">"; //this.eMail + ">";
                    oSmtp.From = "\"Signature Fundraising Customer Service\" <support@sigfund.com>";

                    String strTitle = "Purchase Order\n\r";

                    oSmtp.Body = strTitle;
                    
                    
                    break;
                case "Viewer":
                    PrintTo = PrinterDevice.Viewer;
                    break;
            }
            
                if (lvCustomers.Items.Count > 0)
                {
                    foreach (Infragistics.Win.UltraWinListView.UltraListViewItem Item in lvCustomers.Items)
                    {   
                            if (oPurchase.Find(Item.Tag.ToString()))
                            {
                                if (PrintTo == PrinterDevice.PDF)
                                    oSmtp.Attachment = oPurchase.PrintPDF();  
                                else
                                    oPurchase.Print(PrintTo, PrinterName);  
                            }
                    }
                    if (PrintTo == PrinterDevice.PDF)
                        oSmtp.Send();
                        
                }
                    
            


            this.Dispose();
        }

        private void FillOut()
        {

            DataTable data = oPurchase.GetRange(txtPOFrom.Text!=""?Convert.ToInt32(txtPOFrom.Text):0, txtPOTo.Text != "" ? Convert.ToInt32(txtPOTo.Text) : 0);
            foreach (DataRow row in data.Rows)
            {
                oPurchase.Find(row["PurchaseID"].ToString());
                AddItem();
            }

        }

    }
}