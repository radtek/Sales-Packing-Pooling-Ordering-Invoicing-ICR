using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using Signature.Data;
using Signature.Classes;
using Signature.Forms;


namespace Signature.Classes
{
    public enum ShoppingType
    {
        None = 0,
        SigFund = 1,
        ChristianCollection = 2,
        WLOT = 3,
        GA = 4,
        PersonalTouchCollection = 5,
        ChocolateWithACause = 6
    }
    
    public enum Activity
    {
        None    = 0,
        Add     = 1,
        Delete  = 2,
        Change  = 3
    }
    
    public enum OrderType
    {
        Regular =  0,
        Scanned =  1,
        OSAS    =  2,
        Giftco  =  3,
        Internet=  4,
        GA_Internet = 5,
        CampaignSoftware = 6,
        Test    =  10
    }

    public enum OrderProcess
    {
        Enter       = 0,
        Scanning    = 1,
        Discrepancy = 2,
        Packing     = 3,
        Scan        = 4,
        ScanFixing  = 5,
        Internet    = 6,
        GiftAvenue  = 7,
        CustomOrder = 8,
        Ticket      = 9
        
    }
    
    public class Order:Company  
    {

        #region Declarations
        
        DateTime DNull = new DateTime(1900, 01, 01);

        private Company oCompany;
        public  Product oProduct;
        public  Customer oCustomer;
        private Brochure oBrochure;
        public Prize oPrize;
        public  Student oStudent;
        public  Teacher oTeacher;
        private Box oBox;
        
       
        private  String  _CustomerID;
		private String  _Teacher;
        private String  _Student;
        private String  _PrizeID;
        private int     _NoItems;
        private Double  _Retail;
        private Double  _Collected;
        private Double  _Tax;
        public Double   TotalUpperRange;
        
        public  DateTime PackedDate;
        private DateTime _Date;
        public  new String   Phone;
        public  Double   Diff;
        private Int32    _ID;
        //private String   NoBoxes;
        private String   BoxesString;

        private OrderType  _OrderType;
        public OrderProcess Process;

        private Boolean _Printed=false;
        private Boolean _Packed;
        public Boolean   Scanned;
        private Discrepancy.DiscrepancyActivity _Discrepancy;    // "Y" or "N"
        private int     _BoxesPacked;    // 
        private int     _BoxesScanned;    // 
        private int     StudentSeq;
        private int     TeacherSeq;

        private String  _LastTeacher;
        private int     _curTeacher;
        private int     _cur_Student;

        public  int     IOrderID;
        public Int32    _ShortageID;
        public Boolean isPrintingByLine = false;
        public Int32 ShortageID
        {
            get
            {
                if (_ShortageID != 0)
                    return _ShortageID;
                else
                {
                    DataRow row;
                    if (this.CompanyID.Substring(0,2)=="GA")
                        row = oMySql.GetDataRow(String.Format("Select ShortageID FROM Shortage  Where OrderID='{0}' And OrderID<>0 And Type='M'", this.ID), "Tmp");
                    else
                        row = oMySql.GetDataRow(String.Format("Select ShortageID FROM Shortage  Where OrderID='{0}' And OrderID<>0 And Type='I'", this.ID), "Tmp");

                    if (row == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return row["ShortageID"] == DBNull.Value ? 0 : (Int32)row["ShortageID"];
                    }
                }
            }
            set
            {
                _ShortageID = value;
            }
        }
        public int OrdersEntered
        {
            get
            {
                return oMySql.exec_sql_no(String.Format("SELECT count(*) FROM Orders Where CompanyID='" + CompanyID + "' And CustomerID='" + this.CustomerID + "'"));

            }
        }
        public Order    oCurrentOrder;
        public DataTable CurrentDetail;
        public ScannedImage oImage = new ScannedImage();
        public _Boxes Boxes;
        public String LastError = "";
        private Boolean ShowTotals = true;
        private Int32 NoMaxPrint = 1;

        public _Items Items = new _Items();
        public _Lines Lines;
        

        public Int32 StudentID;
        public Int32 TeacherID;

        public Int32 OrderID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public ShoppingType ShoppingCart;

        #endregion

        public Order(Order oOrder)
        {
            this.CompanyID      = oOrder.CompanyID;
            this.CustomerID     = oOrder.CustomerID;
            this.Teacher        = oOrder.Teacher;
            this.Student        = oOrder.Student;
            this.PrizeID        = oOrder.PrizeID;
            this.NoItems        = oOrder.NoItems;
            this.Retail         = oOrder.Retail;
            this.Collected      = oOrder.Collected;
            this.Tax            = oOrder.Tax;
            this.Printed        = oOrder.Printed;
            this.Scanned        = oOrder.Scanned;
            this.Date           = oOrder.Date;
            this.Phone          = oOrder.Phone;
            this.Diff           = oOrder.Diff;
            this.ID             = oOrder.ID;
            this.BoxesPacked    = oOrder.BoxesPacked;
            this.BoxesScanned   = oOrder.BoxesScanned;
            this.Discrepancy    = oOrder.Discrepancy;
            this.PackedDate     = oOrder.PackedDate;
            this.TeacherSeq     = oOrder.TeacherSeq;
            this.StudentSeq     = oOrder.StudentSeq;
            this.Packed         = oOrder.Packed;

            this.Items          = new _Items(oOrder.Items);

            this.oCompany       = oOrder.oCompany;

            this.oProduct       = oOrder.oProduct;
            this.oCustomer      = oOrder.oCustomer;
            this.oBrochure      = oOrder.oBrochure;
            this.oPrize         = oOrder.oPrize;
            this.oBox           = oOrder.oBox;
            this.oStudent       = oOrder.oStudent;
            this.oTeacher       = oOrder.oTeacher;
            this.Boxes          = oOrder.Boxes;
            this.CurrentDetail  = oOrder.CurrentDetail;
            this.TotalUpperRange = oOrder.TotalUpperRange;
        }
        
        public Order(String CompanyID):base(CompanyID)
        {
            this.CompanyID = CompanyID;
            _CustomerID="";
		    _Teacher="";
            _Student="";
            _PrizeID="";
            _NoItems=0;
            _Retail=0;
            _Collected=0;
            _Tax=0;
            _Printed=false;
            Scanned = false;
            _Date=DateTime.Now;
            Phone="";
             Diff=0;
            _ID = 0;
            //NoBoxes = null;
            _curTeacher = 0;
            _cur_Student = 0;
            _BoxesPacked = 0;
            _BoxesScanned = 0;
            TotalUpperRange = 0.00;

            Process = OrderProcess.Enter;

            oCompany = new Company();
            
            oProduct = new Product(CompanyID);
            oCustomer = new Customer(CompanyID);
            oBrochure = new Brochure(CompanyID);
            oPrize    = new Prize(CompanyID);
            oBox     =  new Box(CompanyID);
            oStudent = new Student(CompanyID);
            oTeacher = new Teacher(CompanyID);
            oImage = new ScannedImage();
            Boxes = new _Boxes(CompanyID);
            Lines = new _Lines(this.CompanyID);
            oCurrentOrder = null;

            ShortageID = 0;
            
        }
        
        private LinePrint oPrint;
        
        #region Methods related to Order class
        public void DeleteOrderActivity(OrderProcess Process)
        {
            oMySql.exec_sql(String.Format("Delete From  OrderActivity Where OrderID='{0}' And Process='{1}' ", ID, Process.ToString()));
        }
        public void InsertOrderActivity(OrderProcess Process)
        {
            InsertOrderActivity(Process, Global.CurrentUser);
        }
        public void InsertOrderActivity(OrderProcess Process, String user)
        {
            this.DeleteOrderActivity(Process);
            oMySql.exec_sql(String.Format("Insert OrderActivity (OrderID, Items, User, Process, Date, CompanyID, Boxes, Retail) Values('{0}','{1}','{2}','{3}',NOW(),'{4}','{5}','{6}')", ID, this.NoItems, user, Process.ToString(), this.CompanyID, this.BoxesPacked, this.Retail));

        }
        public void InsertOrderActivity(OrderProcess Process, String user, Activity nActivity)
        {
            this.DeleteOrderActivity(Process);
            oMySql.exec_sql(String.Format("Insert OrderActivity (OrderID, Items, User, Process, Date, CompanyID, Boxes, Retail, Activity) Values('{0}','{1}','{2}','{3}',NOW(),'{4}','{5}','{6}','{7}')", ID, this.NoItems, user, Process.ToString(), this.CompanyID, this.BoxesPacked, this.Retail, (Int32)nActivity));

        }
        public virtual bool Find(Int32 OrderID,String PackID)
        {
            if (FindHeader(OrderID))
            {
                FindDetail(OrderID,PackID, false, false);
                return true;
            }
            else
            {
                ID = "0";
                return false;
            }
        }
        public virtual bool Find(Int32 OrderID)
        {
            if (FindHeader(OrderID))
            {
                FindDetail(OrderID);
                return true;
            }
            else
            {
                ID = "0";
                return false;
            }
        }
        public virtual bool Find(String Teacher, String Student)
        {
            //Header
            if (!FindHeader(Teacher, Student))
                return false;

            FindDetail(Convert.ToInt32(ID));
            return true;
        }
        public Int32 FindByIOrderID(Int32 IOrderID)
        {
            DataRow rOrder = this.oMySql.GetDataRow("Select * From Orders Where IOrderID='" + IOrderID + "' And CompanyID='"+this.CompanyID+"'", "Orders");
            if (rOrder == null)
            {
                return 0;
            }
            return (Int32)rOrder["ID"];
        }
        public Int32 FindByTeacherStudent(String Teacher, String Student)
        {
            if (Teacher.Length > 30)
                Teacher = Teacher.Substring(0, 30);
            if (Student.Length > 40)
                Student = Student.Substring(0, 40);

            DataRow rOrder = this.oMySql.GetDataRow("Select * From Orders Where CustomerID='" + CustomerID + "' And Teacher=\"" + Teacher + "\" And Student=\"" + Student + "\"  And CompanyID='" + CompanyID + "'", "Orders");
            if (rOrder == null)
            {
                return 0;
            }
            return (Int32)rOrder["ID"];
        }
        public bool Exist(String Teacher, String Student)
        {
            
            if (Teacher.Length > 30)
                Teacher = Teacher.Substring(0, 30);
            if (Student.Length > 40)
                Student = Student.Substring(0, 40);
            
            DataRow rOrder = this.oMySql.GetDataRow("Select * From Orders Where CustomerID='" + this.CustomerID + "' And Teacher=\"" + Teacher + "\" And Student=\"" + Student + "\"  And CompanyID='" + CompanyID + "'", "Orders");
            if (rOrder == null)
            {
                return false;
            }

            return true;
        }
        public bool FindHeader(String Teacher, String Student)
        {
            if (Teacher.Length > 30)
                Teacher = Teacher.Substring(0, 30);
            if (Student.Length > 40)
                Student = Student.Substring(0, 40);


            DataRow rOrder = this.oMySql.GetDataRow("Select * From Orders Where CustomerID='" + CustomerID + "' And Teacher=\"" + Teacher + "\" And Student=\"" + Student + "\"  And CompanyID='" + CompanyID + "'", "Orders");

            if (rOrder == null)
            {
                ID = "0";
                return false;
            }

            this._ID = (Int32)rOrder["ID"];
            FindHeader(_ID);
            return true;
        }
        public bool FindHeader(Int32 OrderID)
        {
            DataRow rOrder = this.oMySql.GetDataRow("Select * From Orders Where ID='" + OrderID.ToString() + "'", "Tmp");

            if (rOrder == null)
            {
                ID = "0";
                return false;
            }

            this.Clear();
            this.LoadHeader(rOrder);

            //Header
            oCustomer.CompanyID = rOrder["CompanyID"].ToString();
            if (!this.oCustomer.Find(rOrder["CustomerID"].ToString()))
                MessageBox.Show("Customr Not Found: " + rOrder["CustomerID"].ToString() + "(" + oCustomer.CompanyID + ")");
            oCustomer.Brochures.Load(CompanyID, CustomerID);
            Lines.Load(this);
            oImage.Find(Convert.ToInt32(this.ID));
            
            return true;
        }
        internal void LoadHeader(DataRow rOrder)
        {
            this.CompanyID = rOrder["CompanyID"].ToString();
            this.CustomerID = rOrder["CustomerID"].ToString();
            this.Student = rOrder["Student"].ToString();
            this.Teacher = rOrder["Teacher"].ToString();
            this.ID = rOrder["ID"].ToString();
            this._ID = (Int32)rOrder["ID"];
            this.CompanyID = rOrder["CompanyID"].ToString();
            this._Teacher = rOrder["Teacher"].ToString();
            this._Student = rOrder["Student"].ToString();
            this._Retail = (double)rOrder["Retail"];
            this.NoItems = (int)rOrder["Nro_Items"];
            this._Collected = (double)rOrder["Collected"];
            this._Tax = (double)rOrder["Tax"];
            this.Diff = (double)rOrder["Collected"] - ((double)rOrder["Retail"] + (double)rOrder["Tax"]);
            this._PrizeID = rOrder["Prize"].ToString();
            this._CustomerID = rOrder["CustomerID"].ToString();
            this._Printed = (Boolean)rOrder["Printed"];
            this._Packed = (Boolean)rOrder["Packed"];
            this.Scanned = (Boolean)rOrder["Scanned"];
            this._Date = (rOrder["Date"] == DBNull.Value) ? DateTime.Now : (DateTime)rOrder["Date"];
            this._Discrepancy = ((Boolean)rOrder["Discrepancy"] ? Signature.Classes.Discrepancy.DiscrepancyActivity.Checked : Signature.Classes.Discrepancy.DiscrepancyActivity.NoAction); //Global.ToEnum<Signature.Classes.Discrepancy.DiscrepancyActivity>((Byte)rOrder["Discrepancy"]); 
            this._BoxesPacked = (int)rOrder["BoxesPacked"];
            this._BoxesScanned = (int)rOrder["BoxesScanned"];
            this.PackedDate = rOrder["PackedDate"] == DBNull.Value ? DNull : (DateTime)rOrder["PackedDate"];
            this.TeacherSeq = (Int32)rOrder["TeacherSeq"];
            this.StudentSeq = (Int32)rOrder["StudentSeq"];
            this.Phone = rOrder["Phone"].ToString();
            this.BoxesString = String.Empty;
            this.Type = Global.ToEnum<OrderType>((int)rOrder["OrderType"]);
            this.IOrderID = (Int32)rOrder["IOrderID"];
            this.StudentID = (Int32)rOrder["StudentID"];
            this.TeacherID = (Int32)rOrder["TeacherID"];
            this.ShoppingCart = Global.ToEnum<ShoppingType>((int)rOrder["IShoppID"]);
            this.Imprinted = (Boolean)rOrder["Imprinted"];

        }
        public bool Exist(Int32 OrderID,ShoppingType IShoppID)
        {
            DataRow rOrder = this.oMySql.GetDataRow("Select * From Orders Where IOrderID='" + OrderID.ToString() + "' And  IShoppID='"+  (Int32) IShoppID + "'", "Tmp");

            if (rOrder == null)
            {
                ID = "0";
                return false;
            }

            this.Find(Convert.ToInt32(rOrder["ID"].ToString()));
            return true;
        }
        public bool FindDetail(Int32 OrderID, String PackID, Boolean IsImprint, Boolean WithPrizes)
        {
            Items.Clear();

            if (PackID == "")
            {
                CurrentDetail = oMySql.GetDataTable(String.Format("Select d.ProductID,d.Quantity, d.PackID,p.InvCode,p.SectionID   From OrderDetail as d Left Join Product as p On d.ProductID=p.ProductID And d.CompanyID=p.CompanyID  Where d.OrderID='{0}' Order By p.InvCode", OrderID), "Detail");
            }
            else
            {   
                //CurrentDetail = oMySql.GetDataView(String.Format("Select d.ProductID,d.Quantity, d.PackID   From OrderDetail as d Left Join Product as p On d.ProductID=p.ProductID And d.CompanyID=p.CompanyID  Where d.OrderID='{0}' And PackID='{1}' Order By p.InvCode", OrderID, PackID), "Detail");
                CurrentDetail = oMySql.GetDataTable(String.Format("Select d.ProductID,  sum(d.Quantity) as Quantity, d.PackID, p.InvCode, p.SectionID   From OrderDetail as d Left Join Product as p On d.ProductID=p.ProductID And d.CompanyID=p.CompanyID  Where d.OrderID='{0}' And PackID='{1}' Group By  p.InvCode,p.Description", OrderID, PackID), "Detail");
            }

            
            if (WithPrizes)
            {
                oPrize.Find(oCustomer.PrizeID);
                if (oPrize.PackID == PackID)
                {

                    DataView dvPrizes = oPrize.GetItems(oCustomer.PrizeID, NoItems);

                    CurrentDetail.DefaultView.Sort = "InvCode";

                    foreach (DataRow Row in dvPrizes.Table.Rows)
                    {
                        DataRow row = CurrentDetail.NewRow();
                        row["ProductID"] = Row["ProductID"];
                        row["InvCode"] = Row["InvCode"];
                        row["SectionID"] = Row["SectionID"];
                        row["Quantity"] = 1;

                        CurrentDetail.DefaultView.Table.Rows.Add(row);
                    }
                }
                
            }
            
            foreach (DataRowView Row in CurrentDetail.DefaultView)
            {
                //String InvCode = Row["InvCode"].ToString() + " -- " + Row["SectionID"].ToString();

                if (!oProduct.Find(Row["ProductID"].ToString()))
                    continue;

                if (IsImprint)
                {
                    if (!oProduct.IsCustomized)
                        continue;
                }

                Item oItem = new Item();
                oItem.Description = oProduct.Description;
                oItem.Price = oProduct.ExtendedPrice(oCustomer);
                oItem.CompanyID = CompanyID;
                oItem.CustomerID = CustomerID;
                oItem.ProductID = Row["ProductID"].ToString();
                oItem.Quantity = Convert.ToInt32(Row["Quantity"].ToString());
                oItem.Taxable = oProduct.Taxable;
                oItem.PackID = oProduct.PackID(oCustomer); //Row["PackID"].ToString()==""?oProduct.PackID(oCustomer):Row["PackID"].ToString();
                oItem.Customized = oProduct.IsCustomized;
                oItem.UpperRange = oProduct.UpperRange;

                oItem.Tickets.Load(this, oItem.ProductID);

                
                Items.Add(Row["ProductID"].ToString(), oItem);
            }

            GetTotals();

            oCurrentOrder = new Order(this);

            return true;

        }
        private bool FindDetail(Int32 OrderID)
        {
            return FindDetail(OrderID, "", false,false);
        }
        public bool FindTeacher(String Teacher)
        {
            if (Teacher == "")
                return false;

            DataRow r = oMySql.GetDataRow("Select * From Orders Where CompanyID='" + this.CompanyID + "' And CustomerID='" + _CustomerID + "' And Teacher=\"" + _Teacher + "\"", "Teacher");
            if (r == null)
            {
                if (MessageBox.Show("Is this a new teacher?", "Teacher", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {

                    return false;
                }
    
                return true;
            }
         return true;

        }
        public bool View(String _CustomerID, String _Teacher)
        {
            this.Teacher = _Teacher;
            this.CustomerID = _CustomerID;
            return View();
        }
        public new bool View()
        {
            frmViewOrderByStudent oView = new frmViewOrderByStudent(this.CompanyID, this.CustomerID, this.Teacher);
            oView.ShowDialog();
            if (oView.SelectedID != "")
            {
                ID = oView.SelectedID;
                if (this.Find(Convert.ToInt32(ID)))
                    return true;
            }
            ID = "";
            return false;
        }
        public void Clear()
        {
            Collected = 0;
            Diff = 0;
            NoItems = 0;
            Phone = "";
            Tax = 0;
            oCurrentOrder = null;
            Items.Clear();
            ID = "0";
            IOrderID = 0;
            ShortageID = 0;
            Printed = false;
            Packed = false;
            Scanned = false;
            Discrepancy = Signature.Classes.Discrepancy.DiscrepancyActivity.NoAction;
            ShoppingCart = ShoppingType.None;
            oTeacher.Clear();
            TeacherID = 0;
            StudentID = 0;
        }
        private void CommitInventory()
        {
            if (!oCustomer.oCustomerExtra.isPineValley)
            {
                Inventory oInventory = new Inventory(CompanyID);
                oInventory.Update(this, Inventory.InventoryType.Commit);
            }
        }
        private void BackToInventory()
        {
            if (!oCustomer.oCustomerExtra.isPineValley)
            {
                Inventory oInventory = new Inventory(CompanyID);
                oInventory.Update(oCurrentOrder, Inventory.InventoryType.RollBack);
            }
            return;
        }
        public void DeleteItems()
        {
            BackToInventory();
            
            String Sql = String.Format("Delete from OrderDetail Where OrderID='" + ID + "'");
            oMySql.exec_sql(Sql);

     //       Sql = String.Format("Delete from OrderTicket Where OrderID='" + ID + "'");
     //       oMySql.exec_sql(Sql);
           
            Sql = String.Format("Delete from OrderBoxes Where OrderID='" + ID + "'");
            oMySql.exec_sql(Sql);
            return;
        }
        public override void Save()
        {
            if ((Student.Trim() != "" && Teacher.Trim() != "" && Items.Count > 0) || this.Type == OrderType.Scanned || this.Type == OrderType.CampaignSoftware)
            {

                if (Items.Count == 0)
                {
                    //    MessageBox.Show("Wait a minute");
                }

                if (oCurrentOrder != null)
                {
                    if (this.Teacher != oCurrentOrder.Teacher)
                    {
                        TeacherSeq = GetTeacherSeq();
                        if (TeacherSeq == 0)
                        {
                            TeacherSeq = GetMaxTeacherSeq() + 1;
                        }

                    }
                    UpdatePrinted(false);
                }
                oCustomer.HasChanged = true;
                this.Printed = false;
                if (this.Type == OrderType.CampaignSoftware)
                {
                    Student = this.StudentID.ToString() + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    /*
                    if (this.Exist(Teacher, Student))
                    {
                        if (Student.Length >= 33)
                            Student = Student.Substring(0, 33);
                        Student += " - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"); //Date.ToString("MM/dd/yyyy");
                    }
                    */

                    if (if_exist())
                        Update();
                    else
                        Insert();
                    return;
                }

                //if (this.TeacherID == 0)
                {
                    this.oTeacher.CustomerID = this.CustomerID;
                    this.TeacherID = this.oTeacher.FindOrCreate(this.Teacher);
                    this.TeacherSeq = this.oTeacher.Seq;
                    /*
                    oOrder.oStudent.CustomerID = oOrder.CustomerID;
                    oOrder.oStudent.OrderID = Convert.ToInt32(oOrder.ID);
                    oOrder.StudentID = oOrder.oStudent.FindOrCreate(oOrder.Student, oOrder.TeacherID);
                     */
                 //   this.UpdateTeacherStudent(TeacherID, StudentID);
                }

                this.Date = DateTime.Now;
                this.GetTotals();
                if (if_exist())
                    Update();
                else
                    Insert();
                this.SaveItems();
                this.Lines.Load(this);


                this.CommitInventory();
                this.Boxes.Save(this.ID);
                if (oCustomer.GetTotalInvoicedAmount() > 0)
                {   
                    if (this.Process == OrderProcess.Enter)
                    {
                        this.Compare(oCurrentOrder, this, true);
                        oCurrentOrder = null;
                    }
                }
                oCustomer.HasChanged = true;
            }
            else
            {
                if (this.Process != OrderProcess.Internet)
                    MessageBox.Show("This order cannot be saved...\n" + "\nTeacher:" + Teacher + "\nStudent:" + Student + "\nItems:" + Items.Count.ToString());
            }
            Global.ClosePhotoGallery("");
        }
        public void Save(OrderType type)
        {
            _OrderType = type;
            this.Save();
        }
        private void SaveItems()
        {
            DeleteItems();
            foreach (Item Row in this.Items)
            {
                Row.CompanyID = CompanyID;
                Row.CustomerID = _CustomerID;
                Row.OrderID = _ID.ToString();
                if (Row.Quantity > 0)
                {
                    Row.Insert();
                    Row.Tickets.Save(this._ID);
                }
            }
            Lines.Save(this);
        }
        public override void Insert()
		{
            String Date = "'" + this.Date.ToString("yyyy-MM-dd hh:mm:ss") + "'"; //"yyyy-MM-dd "
            String Sql = String.Format("Insert into Orders (CustomerID, Teacher, Student, Prize, Nro_Items, Retail,Collected, Tax, Printed,Date,Phone,CompanyID,Reserved ,OrderType, Discrepancy, StudentID, TeacherID, IShoppID)  Values (\"{0}\",\"{1}\",\"{2}\",'{3}','{4}','{5}','{6}','{7}','{8}',{9},'{10}','{11}','0',{12},'{13}','{14}','{15}','{16}')", this._CustomerID,
                MySQL.PrepareSql(this._Teacher), MySQL.PrepareSql(this._Student), this._PrizeID, this._NoItems, this._Retail, this._Collected, this._Tax, this._Printed?'1':'0', Date, this.Phone, this.CompanyID, (int)this._OrderType, (int)this._Discrepancy, this.StudentID,this.TeacherID, (int)this.ShoppingCart);
            //Console.WriteLine(Sql);
			this._ID = oMySql.exec_sql_id(Sql);
            if (this.Type != OrderType.Internet)
                this.InsertOrderActivity(OrderProcess.Enter);
			return;
			
		}
        public override void Update()
        {
         String Date = "'" + this.Date.ToString("yyyy-MM-dd hh:mm:ss") + "'"; //"yyyy-MM-dd "
         String Sql = String.Format("Update Orders Set Prize='{0}', Nro_Items='{1}', Retail='{2}',Collected='{3}', Tax='{4}', Printed='{5}',Date={6},Phone='{7}',Reserved='0', OrderType={8}, Teacher=\"{9}\", Student=\"{10}\", CustomerID='{11}', TeacherID='{12}', StudentID='{13}', IShoppID='{14}'" + " Where ID='" + this._ID + "'",
				this._PrizeID,this._NoItems,this._Retail,this._Collected,this._Tax,this._Printed?'1':'0',Date,this.Phone, (int)this._OrderType,MySQL.PrepareSql(this.Teacher), MySQL.PrepareSql(this.Student),this.CustomerID,this.TeacherID, this.StudentID, (int) this.ShoppingCart);
			oMySql.exec_sql(Sql);	
			return;
        }
        public override void Delete()
        {
            if (_ID != 0)
            {
                oCustomer.HasChanged = true;
                String Sql = "Delete from Orders where ID=" + _ID;
                oMySql.exec_sql(Sql);
                this.InsertOrderActivity(OrderProcess.Discrepancy, Global.CurrentUser, Activity.Delete);
                this.DeleteItems();
                if (oImage.OrderID == _ID)
                {
                    oImage.OrderID = 0;
                    oImage.Status = ScannedOrderStatus.JustScanned;
                    oImage.Message = "Order Deleted...";
                    oImage.UpdateStatus();
                }
                Order.PackByOrder oPack = new PackByOrder();
                oPack.DeleteAll(_ID);

                Ticket oTicket = new Ticket(this.CompanyID);
                oTicket.DeleteByOrder(_ID);
            }
            return;
        }
        public void GetTotals()
        {
            this.TotalUpperRange = 0;
            this.Retail = 0;
            this.NoItems = 0;
            this.Tax = 0;
            this.Diff = 0;
            TaxByState oTaxState = new TaxByState(CompanyID);

            oCustomer.Find(this.CustomerID);
            
            oBox.Clear();

            /*
            foreach (Item Row in Items)
            {
                Double _Tax = 0;
                Double _ProfitPercent = 0.00;
                ///Double Price = Row.Price;
                
                oProduct.Find(Row.ProductID);
                Double Price = oProduct.ExtendedPrice(oCustomer);

                oBox.AddCube(oCustomer, oProduct, Row.Quantity);
                
                this.Retail += Price * Row.Quantity;
                this.NoItems += Row.Quantity;

                oTaxState.Find(Row.ProductID, oCustomer.State);

                _ProfitPercent = (oProduct.AbsoluteProfitPercent > 0? oProduct.AbsoluteProfitPercent : oCustomer.GetProfitByBrochure(oBrochure.GetBrochureByProduct(Row.ProductID))) + oProduct.AdjustProfitPercent;
                _Tax = Row.Quantity * Math.Round(Price * (oTaxState.Taxable=="Y" && oTaxState.Tax > 0.00?oTaxState.Tax:oCustomer.SalesTax) / 100,2);
                _Tax = Math.Round(_Tax*(oCustomer.ApplyTax=="Y" && oCustomer.CollectTax!="R"?(1-(_ProfitPercent/100)):1),2);
                _Tax = _Tax * (oTaxState.Taxable == "Y" ? 1 : oTaxState.Taxable == "N" ? 0 : (oProduct.Taxable == "Y" ? 1 : 0));
                
                Items[Row.ProductID].Tax = _Tax;
                
                if (oCustomer.ApplyTax == "R" || oCustomer.CollectTax == "R")
                {
                    this.Tax += _Tax;
                }
            }
            */
            for (int x = 0; x < Items.Count; x++)
            {
                Double _Tax = 0;
                Double _ProfitPercent = 0.00;
                ///Double Price = Row.Price;

                oProduct.Find(Items[x].ProductID);
                Double Price = oProduct.ExtendedPrice(oCustomer);
                
                //For Box Calculation
                oBox.AddCube(oCustomer, oProduct, Items[x].Quantity);

                oTaxState.Find(Items[x].ProductID, oCustomer.State);

                if (oCustomer.CollectTax == "W")
                {
          //IF(c.ApplyTax='Y' And c.CollectTax!='R',1,0)* IF(c.ApplyTax='N',0,1) * ROUND(IF(bd.Price=0,p.Price-p.Fee,bd.Price) - ",@vPrice,",10),
          //Price = CONCAT("IF(bd.Price=0,p.Price-p.Fee,bd.Price)/IF((",@Condition,") And IF(ts.Taxable='Y',1,IF(ts.Taxable='N',0,IF(p.Taxable='Y',1,0))) = 1,(1+",@vSalesTax,"/100),1)");
                    _Tax  = Math.Round(Price * (oTaxState.Taxable == "Y" && oTaxState.Tax > 0.00 ? oTaxState.Tax : oCustomer.SalesTax) / 100, 2);
                    Price = Math.Round(Price / (1 + _Tax/100),2);
                    _Tax =  oProduct.ExtendedPrice(oCustomer) - Price;
                }
                
                this.Retail += Price * Items[x].Quantity;
                this.TotalUpperRange += Items[x].UpperRange * Items[x].Quantity;

                if (oProduct.IsCustomized  && Items[x].Tickets.Count > 0)
                    Items[x].Quantity = Items[x].Tickets.Quantity;
                
                this.NoItems += Items[x].Quantity;

                

                _ProfitPercent = (oProduct.AbsoluteProfitPercent > 0 ? oProduct.AbsoluteProfitPercent : oCustomer.GetProfitByBrochure(oBrochure.GetBrochureByProduct(Items[x].ProductID))) + oProduct.AdjustProfitPercent;

                if (oCustomer.CollectTax != "W")
                {
                    _Tax = Items[x].Quantity * Math.Round(Price * (oTaxState.Taxable == "Y" && oTaxState.Tax > 0.00 ? oTaxState.Tax : oCustomer.SalesTax) / 100, 2);
                    _Tax = Math.Round(_Tax * (oCustomer.ApplyTax == "Y" && oCustomer.CollectTax != "R" ? (1 - (_ProfitPercent / 100)) : 1), 2);
                    _Tax = _Tax * (oTaxState.Taxable == "Y" ? 1 : oTaxState.Taxable == "N" ? 0 : (oProduct.Taxable == "Y" ? 1 : 0));
                }
                Items[x].Tax = _Tax;

                if (oCustomer.ApplyTax == "R" || oCustomer.CollectTax == "R" || oCustomer.CollectTax == "W")
                {
                    this.Tax += _Tax;
                }
            }


            Retail = Math.Round(Retail, 2);
            if (oCustomer.CollectTax != "N")
            {
                Diff = Math.Round(Retail + Tax - Collected,2);
            }
            else
            {
                this.Tax = 0.00;
                Diff = Math.Round(Retail - Collected,2);
            }
           
            _PrizeID      = oPrize.GetLevel(oCustomer.PrizeID, NoItems);
            
            //Calculating Boxes
            //Adding Prizes
            DataView _Prizes = oPrize.GetItems(oCustomer.PrizeID, this.NoItems);
            foreach(DataRowView _Prize  in _Prizes)
            {
                if (oProduct.Find(_Prize["ProductID"].ToString()))
                {
                    oBox.AddCube(oCustomer, oProduct, 1);
                }
            }
            BoxesString   = oBox.GetBoxes();
            Boxes.Load(oBox.Boxes);

          // MessageBox.Show(BoxesString);
            
            return;
        }
        public bool if_exist()
        {
               if (oMySql.exec_sql_no("Select count(*) from Orders Where ID='" + this._ID + "'") == 0)
                {
                    return false;
                }
            return true;
        }
        public Boolean Compare(Order oOld, Order oNew, Boolean bSave)
        {
            Overage oOverage = new Overage(this.CompanyID, this);

            Order._Items oOldItems = new Order._Items();
            if (oOld != null)
                oOldItems = new _Items(oOld.Items);
            
            Order._Items oNewItems = new _Items(oNew.Items);
            
            
            if (oNew == null)
            {
                MessageBox.Show("Bad arguments...");
                return false;
            }

            if (oOld != null)
            {
                //First Stage
                foreach (Order.Item oItem in oOldItems)
                {
                    if (oNewItems.ContainsKey(oItem.ProductID))
                    {
                        if (oNewItems[oItem.ProductID].Quantity != oOldItems[oItem.ProductID].Quantity)
                        {
                            if (bSave)
                            {
                                oOverage.Insert(oItem.ProductID, oNewItems[oItem.ProductID].Quantity - oOldItems[oItem.ProductID].Quantity);
                            }
                            else
                            {

                                return true;
                            }
                        }
                        oNewItems.Remove(oItem.ProductID);

                    }
                    else
                    {
                        if (bSave)
                        {
                            oOverage.Insert(oItem.ProductID, oOldItems[oItem.ProductID].Quantity * -1);
                        }
                        else
                        {
                            return true;
                        }
                    }
                    //oOldItems.Remove(oItem.ProductID);
                    if (oOldItems.Count == 0)
                        break;
                }
            }
            //Second Stage
            foreach (Order.Item oItem in oNewItems)
            {
                 if (bSave)
                    {
                        oOverage.Insert(oItem.ProductID, oNewItems[oItem.ProductID].Quantity);
                    }
                    else
                    {
                        return true;
                    }
            }
            
            return false;


        }
        public void UpdateTeacherStudent(Int32 TeacherID, Int32 StudentID)
        {

            oMySql.exec_sql(String.Format("Update Orders Set StudentID='{0}',TeacherID='{1}' Where ID='{2}'", StudentID, TeacherID, _ID.ToString()));

        }
        
        #region Discrepancy
        public void UpdateDiscrepancy()
        {   
           /* if (Math.Abs(_Diff) >= 1.5)
                _Discrepancy = "Y";
            else
                _Discrepancy = "N";
            */
            _Discrepancy = Signature.Classes.Discrepancy.DiscrepancyActivity.Checked;
            oMySql.exec_sql(String.Format("Update Orders Set Discrepancy='{0}' Where ID={1}", (int)_Discrepancy, _ID.ToString()));

        }
         #endregion
#endregion
        #region Print
        public void OpenPrinter(int PrinterIndex)
        {
            switch (PrinterIndex)
            {
                case 0:
                    oPrint = new LinePrint("\\\\srv1\\Packing_1");
                    break;
                case 1:
                    oPrint = new LinePrint("\\\\srv1\\Packing_2");
                    break;
                case 2:
                    oPrint = new LinePrint("\\\\srv1\\Packing_3");
                    break;
                case 3:
                    oPrint = new LinePrint("\\\\srv1\\Packing_4");
                    break;
                case 4:
                    oPrint = new LinePrint("\\\\srv1\\Packing_5");
                    break;
                case 5:
                    oPrint = new LinePrint("\\\\srv1\\WH_P5205");
                    break;
                case 6:
                    oPrint = new LinePrint("\\\\srv1\\Plain");
                    break;
                case 7:
                    oPrint = new LinePrint("Packing_5");
                    break;
                default:
                    oPrint = new LinePrint("\\\\srv1\\Packing_2");
                    break;

            }

            //LinePrint oPrint = new LinePrint("\\\\Alvaro\\P5250");
            //LinePrint oPrint = new LinePrint("\\\\Alvaro\\hp");

           // oPrint.DocName = "School :  " + (oCustomer.Name.Trim() == "" ? "Batch Printing ..." : oCustomer.Name);
            oPrint.Open();

        }
        public void ClosePrinter()
        {
            //oPrint.Print(LinePrint.Control.FF);
            oPrint.Print(LinePrint.Control.FF);
            oPrint.Close();
        }
        public void Print(List<string> TheseItems)
        {
            if (TheseItems.Count == 0)
                this.Print();
            else
            {
                Order._Items BackItems = new _Items(this.Items);
                this.Items.Clear();

                foreach (Order.Item oItem in BackItems)
                {
                    if (TheseItems.Contains(oItem.ProductID))
                    {
                        this.Items.Add(oItem.ProductID,oItem);
                    }
                }
                this.GetTotals();
                isPrintingByLine = true;
                this.Print();
            }
        }
        public void Print()
        {


            

            #region IsCombo && Printing By Line

            if (this.Lines.Count > 0 && !isPrintingByLine)
            {
                Double TotalRetail = this.Retail;
                Double TotalTax = this.Tax;
                Double TotalDiff = this.Diff;
                String TotalPrize = this.PrizeID;
                Boolean IsPrizePrinted = false;
                Int32 nLines = 0;

                this.ShowTotals = false;


                Prize oPrize = new Prize(this.CompanyID);
                oPrize.Find(oCustomer.PrizeID);

                Pack oPack = new Pack(this.CompanyID);

                this.isPrintingByLine = true;
                foreach (PackByOrder oBC in this.Lines)
                {
                    if (this.FindDetail(_ID, oBC.PackID, false,true))
                    {
                        if (oPack.Find(oBC.PackID))
                        {
                            if (this.NoItems > 0)
                            {
                                nLines++;
                                if (this.Lines.Count == 1)
                                {
                                    this.Retail = TotalRetail;
                                    this.Tax = TotalTax;
                                    this.Diff = TotalDiff;
                                    this.ShowTotals = true;

                                    if (oBC.PackID == oPrize.PackID)
                                    {
                                        IsPrizePrinted = true;
                                        this.PrizeID = TotalPrize;
                                    }
                                    else
                                        this.PrizeID = "";

                                }

                                else if (this.Lines.Count >= 2)
                                {
                                    this.Retail = TotalRetail;
                                    this.Tax = TotalTax;
                                    this.Diff = TotalDiff;
                                    if (!this.ShowTotals)
                                    {
                                        if (this.Lines.Count == nLines)
                                            this.ShowTotals = true;
                                    }
                                    if (oBC.PackID == oPrize.PackID & !IsPrizePrinted)
                                    {
                                        IsPrizePrinted = true;
                                        this.PrizeID = TotalPrize;
                                    }
                                    else
                                        this.PrizeID = "";

                                }
                                this.NoMaxPrint = oPack.NoCopies;
                                this.Print();

                            }
                        }
                    }
                }

                if (!IsPrizePrinted && TotalPrize != "")
                {
                    this.ShowTotals = false;
                    this.Items.Clear();
                    this.GetTotals();
                    this.PrizeID = TotalPrize;
                    this.NoMaxPrint = 1;
                    this.Print();
                    this.ShowTotals = true;
                }


                // this.BrochureID = "";
                this.isPrintingByLine = false;
                this.Find(_ID);
                return;
            }
            else if (!isPrintingByLine)
            {
                this.NoMaxPrint = 1;
            }
            #endregion


            if (_ID == 0)
            {
                MessageBox.Show("This order number : " + ID + " can't be printed");
                return;
            }

            if (StudentSeq == 0)
            {
                if (_cur_Student == 0)
                {
                    _cur_Student = GetMaxStudentSeq();
                }
                /*
                if (_curTeacher == 0)
                {
                    _curTeacher = GetTeacherSeq();
                    if (_curTeacher == 0)
                        _curTeacher = GetMaxTeacherSeq();
                    else
                        _LastTeacher = Teacher;
                }
                */
                if (_LastTeacher != Teacher)
                {
                    _curTeacher++;
                    _LastTeacher = Teacher;
                }

                _cur_Student++;

                
                //TeacherSeq = _curTeacher;
                StudentSeq = _cur_Student;
            }
            if (this.TeacherID == 0)
            {
                this.oTeacher.CustomerID = this.CustomerID;
                this.TeacherID = this.oTeacher.FindOrCreate(this.Teacher);
                /*
                oOrder.oStudent.CustomerID = oOrder.CustomerID;
                oOrder.oStudent.OrderID = Convert.ToInt32(oOrder.ID);
                oOrder.StudentID = oOrder.oStudent.FindOrCreate(oOrder.Student, oOrder.TeacherID);
                 */
                this.UpdateTeacherStudent(TeacherID, StudentID);
            }

            if (this.oTeacher.Find(this.TeacherID))
                TeacherSeq = this.oTeacher.Seq; //_curTeacher;
            //else
            //    TeacherSeq = _curTeacher;
            

            for (Int16 i = 0; i < this.NoMaxPrint; i++) //oCustomer.Brochures.MaxNoPrint; i++)
            {
                PrintPackingSlip();
                UpdatePrinted(true);
            }
        }
        private Int32 GetTeacherSeq()
        {
            DataRow row = oMySql.GetDataRow(String.Format("SELECT TeacherSeq FROM Orders Where CompanyID='{0}' And CustomerID='{1}' And Teacher=\"{2}\" And TeacherSeq > 0", CompanyID, CustomerID, Teacher), "tmp");
            if (row == null)
            {

                return 0;
            }
            else
            {
                return Convert.ToInt32(row["TeacherSeq"].ToString());
            }
        }
        private Int32 GetMaxStudentSeq()
        {
            DataRow row = oMySql.GetDataRow(String.Format("SELECT Max(StudentSeq) as MaxStudent FROM Orders Where CompanyID='{0}' And CustomerID='{1}'", CompanyID, CustomerID), "tmp");
            if (row == null)
            {

                return 0;
            }
            else
            {
                return Convert.ToInt32(row["MaxStudent"].ToString().Trim());
            }
        }
        private Int32 GetMaxTeacherSeq()
        {
            DataRow row = oMySql.GetDataRow(String.Format("SELECT Max(TeacherSeq) as MaxTeacher FROM Orders Where CompanyID='{0}' And CustomerID='{1}'", CompanyID, CustomerID), "tmp");
            if (row == null)
            {

                return 0;
            }
            else
            {
                return Convert.ToInt32(row["MaxTeacher"].ToString());
            }
        }

        private void GroupItems()
        {
            Order._Items newItems = new _Items();
            foreach (Item Row in this.Items)
            {
                    oProduct.Find(Row.ProductID);
                    
            }
        }
        public void PrintPackingSlip()
        {
            if (this.IsGPI) //this.CompanyID == "GPI")
            {
                this.Diff = 0.00;
            }
            
            int TotalLen = 6;
            int HeaderLen = 8;
            int Capacity = 36;
            int NumLines = 18;
            int NumPages = (int)((this.Items.Count + TotalLen / 2 - 1) / NumLines);


            NumPages += ((this.Items.Count + TotalLen) > NumPages * NumLines ? 1 : 0);

            //MessageBox.Show(Student + " "+NumPages.ToString());

            int CurPage = 0;


            CurPage++;
            PrintHeader(oPrint, CurPage, NumPages);
            int Index = 0;
            foreach (Item Row in this.Items)
            {
                Index++;
                oProduct.Find(Row.ProductID);
                if (this.oCustomer.IsPostPay)
                {
                    if (this.CompanyID == "F11" && oCustomer.Brochures.Contains("CC"))
                        oPrint.LPrint(oProduct.InvCode.PadLeft(4) + Row.Quantity.ToString().PadLeft(3) + " " + Row.Description.PadRight(27, '-').Substring(0, 27) + " " + Row.ProductID.PadLeft(4));
                    else
                        oPrint.LPrint(oProduct.InvCode.PadLeft(4) + Row.Quantity.ToString().PadLeft(3) + " " + Row.Description.PadRight(13, '-').Substring(0, 13) + " " + (Row.Price * Row.Quantity).ToString("####.00").PadLeft(6) + "-" + (Row.Quantity * Row.UpperRange).ToString("####.00").PadLeft(6) + " " + Row.ProductID.PadLeft(4));
                }
                else if (this.IsGiftAvenue)
                    oPrint.LPrint(oProduct.InvCode.PadRight(6) + Row.Quantity.ToString().PadLeft(5) + " " + Row.Description.PadRight(20, '-').Substring(0, 20) + " " + oProduct.Code.ToString().PadLeft(2) + " " + Row.ProductID.PadLeft(4));
                else if (this.Diff * -1 >= 0 || this.IsGPI)
                    oPrint.LPrint(oProduct.InvCode.PadLeft(4) + Row.Quantity.ToString().PadLeft(3) + " " + Row.Description.PadRight(27, '-').Substring(0, 27) + " " +  Row.ProductID.PadLeft(4));
                else
                    oPrint.LPrint(oProduct.InvCode.PadLeft(4) + Row.Quantity.ToString().PadLeft(3) + " " + Row.Description.PadRight(20, '-').Substring(0, 20) + " " + (Row.Price * Row.Quantity).ToString("####.00").PadLeft(6) + " " + Row.ProductID.PadLeft(4));

                oPrint.Print(LinePrint.Control.CRLF);

                int Result = 0;
                Math.DivRem(Index, NumLines, out Result);
                if (Result == 0)
                {
                    if (NumPages > 1)
                    {
                        PrintBarCodes(oPrint);
                        CurPage++;
                        PrintHeader(oPrint, CurPage, NumPages);
                    }
                }

            }
            if (oPrint._CurrentLine - HeaderLen > (Capacity - TotalLen))
            {
                PrintBarCodes(oPrint);
                CurPage++;
                PrintHeader(oPrint, CurPage, NumPages);
            }

            PrintTotals(oPrint);
            PrintBarCodes(oPrint);

            oPrint.Print(LinePrint.Control.EP_Off);


        }
        private void PrintBarCodes(LinePrint oPrint)
        {
            Shortage oShortage = new Shortage(this.CompanyID);
            if ((this.Type == OrderType.Internet || this.CompanyID.Substring(0, 2) == "GA") && this.ShortageID > 0)
                oShortage.Find(this.ShortageID.ToString());

            oPrint.LPrint("", 45);
            for (int x = 0; x <= 1; x++)
            {
                oPrint.Print(LinePrint.Control.EP_On);
                oPrint.SetLine(3, Student);

                if (this.Type != OrderType.Internet)
                    oPrint.SetLine(21, Student);
                else
                {
                    if (Student.Contains("-"))
                    {
                        oPrint.SetLine(21, LinePrint.Control.EP_Off + Student.Substring(Student.IndexOf("-") + 1) + LinePrint.Control.EP_On);
                    }
                    else
                    {
                        oPrint.SetLine(21, LinePrint.Control.EP_Off + Student + LinePrint.Control.EP_On);
                    }

                }
                oPrint.LPrintLine(); // Set Y Coordinate


                oPrint.SetLine(7, TeacherSeq.ToString());
                if (this.Type == OrderType.Internet && this.ShortageID > 0)
                    oPrint.SetLine(25, "State : " + oShortage.State);
                else
                    oPrint.SetLine(25, LinePrint.Control.EP_On + TeacherSeq.ToString());

                if (Diff > 1.5 && !this.oCustomer.IsPostPay)
                {
                    oPrint.SetLine(10, "*DISC*");
                    oPrint.SetLine(30, "*DISC*");
                }
                oPrint.LPrintLine(); // Set Y Coordinate

                oPrint.Print(LinePrint.Control.EP_Off);
                oPrint.SetLine(5, Teacher);
                oPrint.SetLine(26, Global.FormatPhone(this.Phone));
                oPrint.SetLine(41, Teacher);
                oPrint.SetLine(62, Global.FormatPhone(this.Phone));
                oPrint.LPrintLine(); // Set Y Coordinate

                oPrint.SetLine(5, oCustomer.Name);
                oPrint.SetLine(41, oCustomer.Name);
                oPrint.LPrintLine(); // Set Y Coordinate


                //String pStr = "\x01" + "|};cD;*" + ID.PadLeft(10,'0') + "*;PN;H03;N1;0010;0000;D";
                String pStr = "";
                pStr = "\x01" + "|};cD;*" + ID + "*;PN;H03;N1;0010;0000;D";


                if (this.Type == OrderType.Internet || this.CompanyID.Substring(0, 2) == "GA")
                {
                    pStr += "\x01" + "|};cD;*" + this.ShortageID.ToString() + "*;PN;H03;N1;0025;0000;D";
                }
                else
                    pStr += "\x01" + "|};cD;*" + ID + "*;PN;H03;N1;0025;0000;D";

                oPrint.LPrint(pStr);
                oPrint.Print(LinePrint.Control.CRLF);


                oPrint.SetLine(6, "______ of ______");
                oPrint.SetLine(30, oCustomer.ID);
                oPrint.SetLine(43, "______ of ______");
                if (this.Type == OrderType.Internet || this.CompanyID.Substring(0, 2) == "GA")
                    oPrint.SetLine(68, this.ShortageID.ToString());
                else
                    oPrint.SetLine(65, ID);

                oPrint.LPrintLine(); // Set Y Coordinate


                oPrint.Print(LinePrint.Control.CRLF);
                oPrint.Print(LinePrint.Control.CRLF);

            }

        }
        private void PrintHeader(LinePrint oPrint, int CurPage, int TotalPages)
        {
            oPrint.Print(LinePrint.Control.EP_Off);
            oPrint.Print(LinePrint.Control.FF);
            oPrint.SetLine(1, StudentSeq.ToString());
            oPrint.SetLine(15, oCustomer.ID);
            /*
            if (this.IOrderID != 0)
            {
                oPrint.SetLine(30, this.IOrderID.ToString().PadLeft(10));
                oPrint.SetLine(39, "I:"+this.IOrderID.ToString().PadLeft(10));
            }*/
            oPrint.SetLine(50, ID.PadLeft(10, '0'));
            oPrint.SetLine(65, String.Format("Page:  {0} of {1}", CurPage, TotalPages));
            oPrint.LPrintLine(0); // Set Y Coordinate

            oPrint.Print(LinePrint.Control.EP_On);
            oPrint.LPrint(oCustomer.Name, 1);
            oPrint.LPrint(Teacher, 2);


            oPrint.SetLine(35, "BOX");
            oPrint.SetLine(39, this.BoxesString); //LinePrint.Control.EP_Off +
            oPrint.LPrintLine(4); // Set Y Coordinate


            oPrint.SetLine(1, Student);
           // oPrint.SetLine(34, LinePrint.Control.EP_Off + this.BoxesString);
            oPrint.LPrintLine(5); // Set Y Coordinate

            oPrint.Print(LinePrint.Control.EP_On);

            oPrint.SetLine(1, "PRIZE:");
            oPrint.SetLine(10, PrizeID);
            oPrint.LPrintLine(6); // Set Y Coordinate
            if (oCustomer.IsPostPay)
            {
                if (this.CompanyID=="F11" && oCustomer.Brochures.Contains("CC"))
                    oPrint.LPrint("Item#   Qty Product                 Item", 7);
                else
                    oPrint.LPrint("Item#   Qty Product Donation Range$ Item", 7);
            }
            else if (this.IsGiftAvenue)
                oPrint.LPrint("Item#   Qty Product          PCode  ", 7);
            else if (this.Diff*-1 >= 0 || this.IsGPI)
                oPrint.LPrint("Item#   Qty Product                 Item", 7);
            else
                oPrint.LPrint("Invty Qty Product     Minimum $Due  Item", 7);
                
            //oPrint.Print(LinePrint.Control.SP_n_72(20));

        }
        private void PrintTotals(LinePrint oPrint)
        {
            

            if (!this.ShowTotals || this.CompanyID == "GPI")
            {
                oPrint.Print(LinePrint.Control.CRLF);

                oPrint.Print(LinePrint.Control.EP_On);
                oPrint.SetLine(1, "Total");
                oPrint.LPrintLine();

                oPrint.SetLine(1, String.Format("Items:{0}", _NoItems));

                if (this.IsGiftAvenue && this.Teacher == "RE-ORDER")
                    oPrint.SetLine(11, "Total Due:" + (this.Retail).ToString("####.00").PadLeft(7));


                oPrint.LPrintLine();

                return;
            }
            oPrint.Print(LinePrint.Control.CRLF);

            oPrint.Print(LinePrint.Control.EP_On);

            if (!oCustomer.IsPostPay)
            {
                oPrint.SetLine(1, "Total");
                if (this.Diff*-1 < 0 && !this.IsGPI)
                    oPrint.SetLine(11, "Total Minimum Donation:" + _Retail.ToString("####.00").PadLeft(7));
                oPrint.LPrintLine();

                if (oCustomer.CollectTax != "N")
                {
                    //oPrint.SetLine(1, String.Format("Items:{0} Tax Due: {1:N}", _NoItems, _Tax));
                    oPrint.SetLine(1, String.Format("Items:{0} ", _NoItems));
                    if (this.Diff * -1 < 0 && !this.IsGPI)
                        oPrint.SetLine(11, "Total Minimum Due:" + (this.Retail + _Tax).ToString("####.00").PadLeft(7));
                }
                else
                {
                    oPrint.SetLine(1, String.Format("Items:{0} ", _NoItems));
                    if (this.Diff * -1 < 0 && !this.IsGPI)
                        oPrint.SetLine(11, "Total Minimum Due:" + (this.Retail).ToString("####.00").PadLeft(7));
                }
                
                oPrint.LPrintLine();
            }
            else
            {
                oPrint.SetLine(1, "Total");

                if (this.CompanyID == "F11" && oCustomer.Brochures.Contains("CC"))
                    oPrint.SetLine(11, " ");
                else
                    oPrint.SetLine(11, "Total Donation Range Due:");
                oPrint.LPrintLine();
                oPrint.SetLine(1, String.Format("Items:{0}", _NoItems));
                if (this.CompanyID=="F11" && oCustomer.Brochures.Contains("CC"))
                    oPrint.SetLine(11, " ");
                else
                    oPrint.SetLine(11, "$" + _Retail.ToString("####.00").PadLeft(7) + " to $" + TotalUpperRange.ToString("####.00").PadLeft(7));
                
                oPrint.LPrintLine();
                
            }

            if (oCustomer.CollectTax != "N")
            {
                oPrint.SetLine(19, "Tax Due  :" + _Tax.ToString("####.00").PadLeft(7));
                oPrint.LPrintLine();
            }
            else
            {
                oPrint.Print(LinePrint.Control.CRLF);
            }


            if (!oCustomer.IsPostPay)
            {
                //oPrint.Print(LinePrint.Control.CRLF);
                if (Diff > 0)
                {
                    oPrint.SetLine(12, "Amount donated  :" + _Collected.ToString("####.00").PadLeft(7));
                    oPrint.LPrintLine();
                }

                oPrint.SetLine(1, " ");
                if (Diff <= 0)
                {
                    // oPrint.SetLine(24, "OVER:" + ((Double)Math.Abs(Diff)).ToString("####.00").PadLeft(7));
                }
                else if (Diff > 0)
                    oPrint.SetLine(23, "SHORT:" + ((Double)Math.Abs(Diff)).ToString("####.00").PadLeft(7));
                else
                    oPrint.SetLine(23, "      " + ((Double)Math.Abs(Diff)).ToString("####.00").PadLeft(7));
                oPrint.LPrintLine();
            }
            else
            {
                //oPrint.Print(LinePrint.Control.CRLF);
                oPrint.SetLine(1, LinePrint.Control.EP_Off + "PLEASE MAKE CHECKS PAYABLE TO: " + oCustomer.PayableTo); //LinePrint.Control.EP_On +
                oPrint.LPrintLine();

                oPrint.SetLine(1, LinePrint.Control.EP_Off + "PLEASE TURN MONEY IN TO THE SCHOOL ON: " + LinePrint.Control.EP_On + oCustomer.DatePayable.ToString("MM/dd/yyyy"));
                oPrint.LPrintLine();
            }


            //oPrint.Print(LinePrint.Control.EP_Off);

        }
        public void UpdatePrinted(Boolean Printed)
        {

            oMySql.exec_sql(String.Format("Update Orders Set Printed='{0}',TeacherSeq='{1}',StudentSeq='{2}' Where ID='{3}'", Printed ? "1" : "0", TeacherSeq, StudentSeq, _ID.ToString()));

        }
        public Int32 GetNumberOfBrochures()
        {
            DataTable dt = oMySql.GetDataTable(String.Format("SELECT  bc.BrochureID FROM OrderDetail od left join Product p On od.CompanyID=p.CompanyID And od.ProductID=p.ProductID Left Join BrochureByCustomer bc On od.CompanyID=bc.CompanyID And od.CustomerID=bc.CustomerID Left Join BrochureDetail bd On od.ProductID=bd.ProductID And bc.BrochureID=bd.BrochureID And od.CompanyID=bd.CompanyID Where od.ProductID=bd.ProductID And OrderID='{0}'  Group By bc.BrochureID Order By  p.InvCode", this.ID), "tmp");
            if (dt == null)
            {

                return 0;
            }
            else
            {
                return dt.Rows.Count;
            }
        }
        public Int32 GetLastID()
        {
            DataRow row = oMySql.GetDataRow(String.Format("SELECT ID FROM Orders Where CompanyID='{0}' And CustomerID='{1}' Order By ID DESC LIMIT 1", CompanyID, CustomerID, Teacher), "tmp");
            if (row == null)
            {

                return 0;
            }
            else
            {
                return (Int32)row["ID"];
            }
        }
        #endregion
        #region Properties

        public new String ID
        {
            get { return _ID.ToString(); }
            set
            {
                if (value == "")
                    _ID = 0;
                else
                    _ID = Convert.ToInt32(value);
            }
        }
        public String CustomerID
        {
            get { return _CustomerID; }
            set
            {
                //oCustomer.Find(value);
                _CustomerID = value; 
            }
        }
        
        public String Teacher
        {
            get { return _Teacher; }
            set { _Teacher = value;  }
        }
        public String Student
        {
            get { return _Student; }
            set { _Student = value; }
        }
        public int NoItems
        {
            get { return _NoItems; }
            set { _NoItems = value; }
        }
        public Double Retail
        {
            get { return _Retail; }
            set { _Retail = value; }
        }
        public Double Collected
        {
            get { return _Collected; }
            set { _Collected = value; }
        }
        
        public Double Tax
        {
            get { return _Tax; }
            set { _Tax = value; }
        }
        public Boolean Printed
        {
            get { return _Printed; }
            set { _Printed = value; }
        }

        private Boolean _Imprinted;
        public Boolean Imprinted
        {
            get
            {
                return _Imprinted;
            }
            set { _Imprinted = value; }
        }

        public Boolean Packed
        {
            get
            {
                return _Packed;
            }
            set { _Packed = value; }
        }
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }
        public Discrepancy.DiscrepancyActivity Discrepancy
        {
            get { return _Discrepancy; }
            set { _Discrepancy = value; }
        }
        public String PrizeID
        {
            get { return _PrizeID; }
            set { _PrizeID = value; }
        }
        public OrderType Type
        {
            get { return _OrderType; }
            set { _OrderType = value; }
        }
        public int BoxesPacked
        {
            get
            {
                if (this.Lines.Count > 1)
                    return Lines.BoxesPacked;

                else
                    return _BoxesPacked; }
            set { _BoxesPacked = value; }
        }
        public int BoxesScanned
        {
            get
            {
                if (this.Lines.Count > 1)
                    return Lines.BoxesScanned;
                else
                    return _BoxesScanned; }
            set { _BoxesScanned = value; }
        }
        public String Deleter(String OrderID)
        {
                DataRow rPayment = oMySql.GetDataRow(String.Format("Select User FROM OrderActivity  Where CompanyID='{0}' And OrderID='{1}' And Activity='{2}'", this.CompanyID, OrderID,(Int32) Activity.Delete), "Tmp");
                
                if (rPayment == null)
                {
                    return "";
                }
                else
                {
                    return rPayment["User"] == DBNull.Value ? "" : rPayment["User"].ToString();
                }
            
        }
        
        

        int _TicketsSeq = 0;
        public Int32 TicketsSeq
        {
            get
            {
                return _TicketsSeq++;
            }
            set
            {
                _TicketsSeq = value;
            }
        }

        #endregion

        #region Classes & Enumerators
        
        [Serializable]
        public class Item               //Single instance of detail
        {
            public String OrderID;
            public String CompanyID;
            public String CustomerID;
            private String _Description;
            private String _ProductID;
            private Int32 _Quantity;
            public Double Tax;
            public Double _Price;
            public int No_Invoiced;
            public Double Length;
            public Double Width;
            public Double Height;
            public String Taxable;
            public String PackID;
            public Boolean _Customized;
            public _Tickets Tickets;
            public Double UpperRange;

            /*
            public _Tickets Tickets
            {
                get { return tickets; }
                set { tickets = value; }
            }
            */
            Signature.Data.MySQL oMySql = Global.oMySql;

            public void UpdateBrochure(String BrochureID)
            {
                oMySql.exec_sql(String.Format("Update OrderDetail Set BrochureID='{0}' Where OrderID='{1}' And ProductID='{2}'", BrochureID,this.OrderID,this.ProductID));
            }

            public Item()
            {
                CompanyID = "";
                CustomerID = "";
                Description = "";
                ProductID = "";
                _Quantity = 0;
                Tax = 0;
                Price = 0;
                No_Invoiced = 0;
                Length = 0;
                Width = 0;
                Height = 0;
                Taxable = "N";
                PackID = "";
                _Customized = false;
                UpperRange = 0.00;
                
                Tickets = new _Tickets();

            }
            public Item(Item oItem)
            {
                CompanyID = oItem.CompanyID; ;
                CustomerID = oItem.CustomerID;
                Description = oItem.Description;
                ProductID = oItem.ProductID;
                Quantity = oItem.Quantity;
                Tax = oItem.Tax;
                Price = oItem.Price;
                No_Invoiced = oItem.No_Invoiced;
                Length = oItem.Length;
                Width = oItem.Width;
                Height = oItem.Height;
                Taxable = oItem.Taxable;
                PackID = oItem.PackID;
                UpperRange = oItem.UpperRange;
            }
            public void Save()
            {
                if (if_exist())
                    Update();
                else
                    Insert();
                if (this.Tickets.Count > 0)
                    this.Tickets.Save(Convert.ToInt32(this.OrderID));



            }
            public void Update()
            {
                String Sql = String.Format("Update OrderDetail  Set  ProductID='{0}', Quantity='{1}', Tax='{2}', QuantityInvoiced='{3}', Reserved='0', OrderID={4}, PackID='{5}'" + " Where CompanyID=\"" + this.CompanyID + "\"" + " And CustomerID=\"" + this.CustomerID + "\"" + " And ProductID=\"" + this.ProductID + "\"",
                     this.ProductID, this.Quantity, this.Tax, this.No_Invoiced, this.OrderID, this.PackID);
                oMySql.exec_sql_afected(Sql);
            }
            public void Insert()
            {
                String Sql = String.Format("Insert into OrderDetail (ProductID, Quantity, Tax, QuantityInvoiced, CustomerID, CompanyID, Reserved, OrderID, PackID )  Values (\"{0}\",'{1}','{2}','{3}','{4}','{5}','0',{6},'{7}')",
                      this.ProductID, this.Quantity, this.Tax, this.No_Invoiced, this.CustomerID, this.CompanyID, this.OrderID, this.PackID);
                oMySql.exec_sql_afected(Sql);
            }
            public void Delete()
            {
                String Sql = "Delete from OrderDetail where CompanyID='" + CompanyID + "' And CustomerID = '" + CustomerID + "' And ProductID = '" + ProductID + "'";
                oMySql.exec_sql(Sql);
            }
            public bool if_exist()
            {
                if (oMySql.exec_sql_no("Select count(*) from OrderDetail Where OrderID=" + this.OrderID + " And ProductID='" + this.ProductID + "'") == 0)
                {
                    return false;
                }
                return true;
            }
            public String ProductID
            {
                get { return _ProductID; }


                set
                {
                    _ProductID = value;
                }
            }
            public String Description
            {
                get { return _Description; }


                set
                {
                    _Description = value;
                }
            }
            public Double Price
            {
                get { return _Price; }


                set
                {
                    //if (value == null)
                    //    value = 0.0;

                    _Price = value;
                }
            }
            public int Quantity
            {
                get { return _Quantity; }


                set
                {
                    _Quantity = value;
                }
            }
            public Boolean Customized
            {
                get
                {/*
                    if (oMySql.exec_sql_no(String.Format("SELECT count(*) FROM OrderTicket Where OrderID='{0}' And ProductID='{1}'", this.OrderID, this.ProductID)) > 0)
                        return true;
                    else
                        return false;
                    */
                  //  return _Customized;
                    return Tickets.Count > 0 ? true : false;
                }
                set { _Customized = value; }

            }
        }
        public class _Items : Hashlist
        {
            
            public _Items()
            { }

            public DataTable Table
            {
                get
                {
                    DataTable _table = CreateTable();
                    foreach (Item oItem in this)
                    {
                        DataRow row = _table.NewRow();
                        row["ProductID"] = oItem.ProductID;
                        row["Description"] = oItem.Description;
                        row["Price"] = oItem.Price;
                        row["Quantity"] = oItem.Quantity;
                        row["Total"] = oItem.Price * oItem.Quantity;
                        _table.Rows.Add(row);
                    }
                    return _table;
                }
                set
                {
                    if (value != null)
                    {
                        Product oProduct = new Product(this.CompanyID);
                        this.Clear();
                        foreach (DataRow row in value.Rows)
                        {
                            Item oItem = new Item();
                            oItem.ProductID = row["ProductID"].ToString();
                            oProduct.Find(row["ProductID"].ToString());
                            oItem.Quantity = (Int32)row["Quantity"];
                            oItem.Description = oProduct.Description;
                            oItem.Price = (Double)row["Price"];
                            this.Add(row["ProductID"].ToString(), oItem);
                        }
                    }
                }
            }

            private DataTable CreateTable()
            {
                DataTable  dtGrid = new DataTable();
                DataColumn dcProductID  = new DataColumn("ProductID");
                DataColumn dcDescription= new DataColumn("Description");
                DataColumn dcPrice      = new DataColumn("Price");
                DataColumn dcQuantity   = new DataColumn("Quantity");
                DataColumn dcTotal      = new DataColumn("Total");

                dtGrid.Columns.Add(dcProductID);
                dtGrid.Columns.Add(dcDescription);
                dtGrid.Columns.Add(dcPrice);
                dtGrid.Columns.Add(dcQuantity);
                dtGrid.Columns.Add(dcTotal);
                return dtGrid;
            }

            public _Items(_Items oItems)
            {
                this.Clear();
                foreach (Item oItem in oItems)
                {
                    this.Add(oItem.ProductID, new Item(oItem));
                }
                this.CompanyID = oItems.CompanyID;
                this.ID = oItems.ID;

            }

            /* Hash List Support */
            public new Item this[string Key]
            {
                get { return (Item)base[Key]; }

            }

            public new Item this[int index]
            {
                get
                {
                    object oTemp = base[index];
                    return (Item)oTemp;
                }
            }

            // Expose the enumerator for the associative array.
            new public IEnumerator GetEnumerator()
            {
                return new ItemsEnumerator(this);
            }
        }
        public class ItemsEnumerator : IEnumerator
        {
            public ItemsEnumerator(_Items ar)
            {
                _ar = ar;
                _currIndex = -1;
            }
            public object Current
            {
                get
                {
                    return _ar.m_oValues[_ar.m_oKeys[_currIndex]];

                }
            }
            public bool MoveNext()
            {
                _currIndex++;
                if (_currIndex == _ar.Length)
                    return false;
                else
                    return true;
            }
            public void Reset()
            {
                _currIndex = -1;
            }

            // The index of the item this enumerator applies to.
            protected int _currIndex;
            // A reference to this enumerator's associative array.
            protected _Items _ar;


        }
        public class _Boxes : Hashlist
        {
            public _Boxes(String CompanyID):base(CompanyID) {}
            public void Load1(String Boxes)
            {
                Box _oBox = new Box(CompanyID);
                String[] Fields = Boxes.Split(':');
                foreach (String Box in Fields)
                {
                    if (_oBox.Find(Box))
                    {
                        OrderBox oBox = new OrderBox(CompanyID);
                        if (this.Contains(Box))
                            this[Box].Quantity++;
                        else
                        {
                            oBox.ProductID  = _oBox.ProductID;
                            oBox.BoxID      = Box;
                            oBox.Quantity   = 1;
                            Add(Box, oBox);
                        }
                    }
                }
            }
            public void Load(Box._Boxes oBoxes)
            {
                this.Clear();
                foreach (Box.BoxAssigned _oBox in oBoxes)
                {
                    OrderBox oOBox = new OrderBox(CompanyID);
                    oOBox.ProductID = _oBox.ProductID;
                    oOBox.BoxID = _oBox.BoxID;
                    oOBox.Quantity = _oBox.Quantity;
                    Add(_oBox.BoxID, oOBox);
                }
            }
            public void Save(String OrderID)
            {
                foreach (OrderBox oBox in this)
                {
                    //MessageBox.Show(oBox.BoxID);
                    oBox.Save(OrderID);
                }
            }

            /* Hash List Support */
            public new OrderBox this[string Key]
            {
                get { return (OrderBox)base[Key]; }

            }
            public new OrderBox this[int index]
            {
                get
                {
                    object oTemp = base[index];
                    return (OrderBox)oTemp;
                }
            }

            // Expose the enumerator for the associative array.
            new public IEnumerator GetEnumerator()
            {
                return new _BoxesEnumerator(this);
            }

        }
        public class _BoxesEnumerator : IEnumerator
        {
            public _BoxesEnumerator(_Boxes ar)
            {
                _ar = ar;
                _currIndex = -1;
            }
            public object Current
            {
                get
                {
                    return _ar.m_oValues[_ar.m_oKeys[_currIndex]];

                }
            }
            public bool MoveNext()
            {
                _currIndex++;
                if (_currIndex == _ar.Length)
                    return false;
                else
                    return true;
            }
            public void Reset()
            {
                _currIndex = -1;
            }

            // The index of the item this enumerator applies to.
            protected int _currIndex;
            // A reference to this enumerator's associative array.
            protected _Boxes _ar;


        }
        public class OrderBox:Company
        {
            public String BoxID;
            public String ProductID;

            public Int32 Quantity=0;

            public OrderBox(String CompanyID):base(CompanyID)
            {

            }

            public void Save(String OrderID)
            {
                oMySql.exec_sql(String.Format("Insert Into OrderBoxes (OrderID,BoxID,ProductID,Quantity) Values('{0}','{1}','{2}','{3}')", OrderID, BoxID, ProductID, Quantity));
               // MessageBox.Show(ProductID + "  " + Quantity.ToString());
            }
        }
        internal class Overage:Company
        {
            private Int32 OrderID;
            private String CustomerID = "";

            public Overage(String CompanyID, Order oOrder):base(CompanyID)
            {
                this.OrderID = Convert.ToInt32(oOrder.ID);
                this.CustomerID = oOrder.CustomerID;
            }
            public void Insert(String ProductID, Int32 Quantity)
            {
                String Date = "'" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "'"; //"yyyy-MM-dd "
                String Sql = String.Format("Insert into OrderOverage (CompanyID, OrderID, ProductID, Quantity, Date, User, CustomerID)  Values (\"{0}\",\"{1}\",\"{2}\",'{3}', {4},'{5}','{6}')",
                    this.CompanyID, this.OrderID, ProductID, Quantity, Date,Global.CurrentUser, CustomerID);
                
                oMySql.exec_sql(Sql);	
            }
        }
        public class PackByOrder : Company
        {
            public Int32 OrderID = 0;
            public String PackID  = "";
            public new String CompanyID = "";
            private new SqlBuilder oBuild;
            public Int32 BoxesPacked = 0;
            public Int32 BoxesScanned = 0;
            public Int32 Items = 0;
            public String User = "";
            public Boolean Packed = false;
            public Boolean Scanned = false;
            public String Packer = ""; // Global.CurrentUser;
            public DateTime PackedDate = DateTime.Today;
            public Boolean Imprinted = false;
            public String Imprinter;


            private void Clear()
            {
                OrderID = 0;
                PackID = "";
                CompanyID = "";
                Packed = false;
                Scanned = false;
                Items = 0;
                BoxesScanned = 0;
                BoxesPacked = 0;
            }
            public bool Find(Int32 OrderID, String PackID)
            {
                DataRow rRow = oMySql.GetDataRow("Select * From OrderByLine Where OrderID='" + OrderID + "' And PackID='" + PackID.ToString() + "'", "Tmp");

                if (rRow == null)
                {
                    this.OrderID = OrderID;
                    this.PackID = PackID;
                    this.BoxesPacked = 0;
                    this.BoxesScanned = 0;
                    this.Items = 0;
                    this.Packed = false;
                    this.Scanned = false;
                    this.Packer = "";
                    this.Imprinted = false;
                    this.Imprinter = "";
                    return false;
                }

                this.CompanyID = rRow["CompanyID"].ToString();
                this.OrderID = (Int32)rRow["OrderID"];
                this.PackID = rRow["PackID"].ToString();
                this.BoxesPacked = (Int32)rRow["BoxesPacked"];
                this.BoxesScanned = (Int32)rRow["BoxesScanned"];
                this.Items = (Int32)rRow["Items"];
                this.Packed = (Boolean)rRow["Packed"];
                this.Scanned = (Boolean)rRow["Scanned"];
                this.Imprinted = (Boolean)rRow["Imprinted"];
                this.Imprinter = rRow["Imprinter"].ToString();
                this.Packer = rRow["Packer"].ToString();
                this.PackedDate = rRow["PackedDate"] == DBNull.Value ? Global.DNull : (DateTime)rRow["PackedDate"];
                return true;

            }
            public void DeleteAll(Int32 OrderID)
            {
                String Sql = "Delete from OrderByLine Where OrderID='" + OrderID + "' And Packed='0'";
                oMySql.exec_sql(Sql);
            }

            public new void Save()
            {
               
                if (Exists())
                    Update();
                else
                    Insert();
            }
            public new void Update()
            {
                FillFields();
                // MessageBox.Show(oBuild.Update("Where CallID='" + this.CallID + "'"));
                Global.oMySql.exec_sql(oBuild.Update("Where OrderID='" + this.OrderID + "' And PackID='"+PackID.ToString()+"'"));
            }
            public new void Insert()
            {
                //   MessageBox.Show(oBuild.Insert());
                FillFields();
                Global.oMySql.exec_sql(oBuild.Insert());

            }

            private new void FillFields()
            {
                oBuild = new SqlBuilder();

                oBuild.AddRange("OrderByLine", new String[] { 
                    "CompanyID", 
                    "OrderID",
                    "PackID",
                    "BoxesPacked",
                    "BoxesScanned",
                    "Items",
                    "User",
                    "Packer",
                    "Packed",
                    "Scanned",
                    "PackedDate"
                },
                new Object[] {
                    this.CompanyID,
                    this.OrderID,
                    this.PackID,
                    this.BoxesPacked,
                    this.BoxesScanned,
                    this.Items,
                    this.User,
                    this.Packer,
                    this.Packed,
                    this.Scanned,
                    this.PackedDate
                });

            }
            public new bool Exists()
            {
                if ((oMySql.exec_sql_no("Select count(*) from OrderByLine Where OrderID='" + this.OrderID + "' And PackID='" + this.PackID + "'")) == 0)
                    return false;
                else
                    return true;
            }
            public new void Delete()
            {
                String Sql = "Delete From OrderByBrochure Where OrderID='" + this.OrderID + "'";
                oMySql.exec_sql(Sql);
            }
        }
        public class _Lines : Hashlist
        {
            public Int32 BoxesPacked
            {
                get
                {
                    Int32 Boxes = 0;
                    foreach (PackByOrder oBO in this)
                    {
                        Boxes += oBO.BoxesPacked;
                    }
                    return Boxes;
                }
            }
            public Int32 BoxesScanned
            {
                get
                {
                    Int32 Boxes = 0;
                    foreach (PackByOrder oBO in this)
                    {
                        Boxes += oBO.BoxesScanned;
                    }
                    return Boxes;
                }
            }
            public Boolean Scanned
            {
                get
                {
                    Boolean _Scanned = false;
                    foreach (PackByOrder oBO in this)
                    {
                        if (!oBO.Scanned)
                            return false;
                        else
                            _Scanned = true;
                    }
                    return _Scanned;
                }
            }
            public Boolean Packed
            {
                get
                {
                    Boolean _Packed = false;
                    foreach (PackByOrder oBO in this)
                    {
                        if (!oBO.Packed)
                            return false;
                        else
                            _Packed = true;
                    }
                    return _Packed;
                }
            }
            public Int32 Items
            {
                get
                {
                    Int32 Items = 0;
                    foreach (PackByOrder oBO in this)
                    {
                        Items += oBO.Items;
                    }
                    return Items;
                }
            }

            public _Lines(String CompanyID) : base(CompanyID) { }
            public void Load(Order oOrder)
            {
                this.Clear();
                DataTable dt = oMySql.GetDataTable(String.Format("SELECT PackID, sum(Quantity) as Quantity FROM OrderDetail od Where OrderID='{0}'  Group By PackID", oOrder.ID));
                if (dt == null)
                {
                    return;
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        PackByOrder LO = new PackByOrder();
                        if (LO.Find(Convert.ToInt32(oOrder.ID),row["PackID"].ToString()))
                            this.Add(row["PackID"].ToString(), LO);
                    }
                }
            }
            public void Save(Order oOrder)
            {
                this.Clear();
                // DataTable dt = oMySql.GetDataTable(String.Format("SELECT p.Line FROM SigData.OrderDetail od Left Join Product p On od.ProductID=p.ProductID And od.CompanyID=p.CompanyID Where OrderID='{0}'  Group By p.Line", oOrder.ID));
                DataTable dt = oMySql.GetDataTable(String.Format("SELECT PackID, sum(Quantity) as Quantity FROM OrderDetail od Where OrderID='{0}'  Group By PackID", oOrder.ID));
                if (dt == null)
                {
                    return;
                }
                else
                {
                    PackByOrder LO = new PackByOrder();

                    if (oOrder.oPrize.Find(oOrder.oCustomer.PrizeID))
                    {
                        if (oOrder.PrizeID.Trim() != "")
                        {
                            Boolean isPrizeIncluded = false;
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["PackID"].ToString() == oOrder.oPrize.PackID)
                                    isPrizeIncluded = true;
                            }
                            if (!isPrizeIncluded)
                            {
                                DataRow _row = dt.NewRow();
                                _row["PackID"] = oOrder.oPrize.PackID;
                                _row["Quantity"] = 1;
                                dt.Rows.Add(_row);
                            }
                        }
                    }
                    
                    
                    LO.DeleteAll(Convert.ToInt32(oOrder.ID));
                    foreach (DataRow row in dt.Rows)
                    {
                    
                        if (!LO.Find(Convert.ToInt32(oOrder.ID), row["PackID"].ToString()))
                        {
                            LO.OrderID = Convert.ToInt32(oOrder.ID);
                            LO.PackID = row["PackID"].ToString();
                            LO.Items = Convert.ToInt32(row["Quantity"].ToString());
                            LO.CompanyID = oOrder.CompanyID;
                            LO.Insert();
                        }
                        else
                        {
                            LO.Items = Convert.ToInt32(row["Quantity"].ToString());
                            LO.CompanyID = oOrder.CompanyID;
                            LO.Update();
                        }
                    }

                }
            }
           
            /* Hash List Support */
            public new PackByOrder this[string Key]
            {
                get { return (PackByOrder)base[Key]; }

            }
            public new PackByOrder this[int index]
            {
                get
                {
                    object oTemp = base[index];
                    return (PackByOrder)oTemp;
                }
            }

            // Expose the enumerator for the associative array.
            new public IEnumerator GetEnumerator()
            {
                return new _LinesEnumerator(this);
            }

        }
        public class _LinesEnumerator : IEnumerator
        {
            public _LinesEnumerator(_Lines ar)
            {
                _ar = ar;
                _currIndex = -1;
            }
            public object Current
            {
                get
                {
                    return _ar.m_oValues[_ar.m_oKeys[_currIndex]];

                }
            }
            public bool MoveNext()
            {
                _currIndex++;
                if (_currIndex == _ar.Length)
                    return false;
                else
                    return true;
            }
            public void Reset()
            {
                _currIndex = -1;
            }

            // The index of the item this enumerator applies to.
            protected int _currIndex;
            // A reference to this enumerator's associative array.
            protected _Lines _ar;


        }
        public class _Tickets : Hashlist
        {
            public _Tickets()
            { }

            public _Tickets(_Tickets oItems)
            {
                this.Clear();
                foreach (Ticket oItem in oItems)
                {
                   // this.Add(oItem.ProductID, new Ticket(oItem));
                }
                this.CompanyID = oItems.CompanyID;
                this.ID = oItems.ID;

            }
            public void Save(Order oOrder)
            {
                foreach (Ticket oTicket in this)
                {
                    oTicket.OrderID = oOrder._ID;
                    oTicket.Save();
                }
                this.Clear();
            }
            public void Save(Int32 OrderID)
            {
                
                foreach (Ticket oTicket in this)
                {
                    oTicket.OrderID = OrderID;
                    oTicket.Save();
                }
                //this.Clear();
            }

            public Int32 Quantity
            {
                get
                {
                    Int32 NoItems = 0;
                    foreach (Ticket oTicket in this)
                    {
                        NoItems += oTicket.Quantity;
                    }
                    return NoItems;
                }
            }


            public void Load(Order oOrder)
            {
                this.Clear();
                DataTable dt = oMySql.GetDataTable(String.Format("SELECT ID as TicketID, Quantity FROM OrderTicket Where OrderID='{0}'", oOrder.ID));
                if (dt == null)
                {
                    return;
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Ticket LO = new Ticket(oOrder.CompanyID);
                        LO.Find((Int32) row["TicketID"]);
                        this.Add(row["TicketID"].ToString(), LO);
                    }
                }

            }

            public void Load(Order oOrder, String ProductID)
            {
                this.Clear();
                DataTable dt = oMySql.GetDataTable(String.Format("SELECT ID as TicketID, Quantity FROM OrderTicket Where OrderID='{0}' And ProductID='{1}' ", oOrder.ID,ProductID));
                if (dt == null)
                {
                    return;
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Ticket LO = new Ticket(oOrder.CompanyID);
                        LO.Find((Int32)row["TicketID"]);
                        LO.Lines.Load(LO);
                        this.Add(row["TicketID"].ToString(), LO);
                    }
                }

            }
            /* Hash List Support */
            public new Ticket this[string Key]
            {
                get { return (Ticket)base[Key]; }

            }
            public new Ticket this[int index]
            {
                get
                {
                    object oTemp = base[index];
                    return (Ticket)oTemp;
                }
            }

            // Expose the enumerator for the associative array.
            new public IEnumerator GetEnumerator()
            {
                return new TicketsEnumerator(this);
            }
        }
        public class TicketsEnumerator : IEnumerator
        {
            public TicketsEnumerator(_Tickets ar)
            {
                _ar = ar;
                _currIndex = -1;
            }
            public object Current
            {
                get
                {
                    return _ar.m_oValues[_ar.m_oKeys[_currIndex]];

                }
            }
            public bool MoveNext()
            {
                _currIndex++;
                if (_currIndex == _ar.Length)
                    return false;
                else
                    return true;
            }
            public void Reset()
            {
                _currIndex = -1;
            }

            // The index of the item this enumerator applies to.
            protected int _currIndex;
            // A reference to this enumerator's associative array.
            protected _Tickets _ar;


        }

        #endregion

    }
}
