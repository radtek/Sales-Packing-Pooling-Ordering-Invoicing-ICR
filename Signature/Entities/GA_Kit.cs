using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;



namespace Signature.Classes
{
    
    public class GA_Kit
    {
        #region Declarations
        private Signature.Data.MySQL oMySql = Global.oMySql;

        private Company oCompany;
        public  Customer oCustomer;
        
        public _Boxes Items = new _Boxes();

        private String _CompanyID;
        private String _CustomerID;

        private String _Description;
        public String ID;
		
               
        
        #endregion
        public GA_Kit(String CompanyID)
        {
            _CustomerID="";
		    _CompanyID = CompanyID;
            _Description = "";

            oCompany = new Company();
            
            this.oCompany.ID = CompanyID;

        }


        #region Methods related to Order class
        public bool Find(String KitID)
        {
            //Header

            
            DataRow rKit = oMySql.GetDataRow("Select * From ga_kit Where ID='"+KitID+"'", "Kit");
           
            if (rKit == null)
            {
                return false;
            }
            


            this.Clear();
                        
            //Header

            this.ID = rKit["ID"].ToString();
            this._CompanyID     = rKit["CompanyID"].ToString();
            this._Description   = rKit["Name"].ToString();
            //MessageBox.Show(this._Description);
            
            //Items.Clear();
                                    
           /* DataView tDetail = oMySql.GetDataView("Select ProductID, Description, Price  From Product  Where CompanyID=."+_CompanyID+ "'", "TMP");
            foreach (DataRow Row in tDetail.Table.Rows)
            {
                Product _Item = new Product(_CompanyID);

                _Item.ID = Row["ProductID"].ToString();
                _Item.Price = (Double) Row["Price"];
                _Item.Description = Row["Description"].ToString();

                Items.Add(Row["ProductID"].ToString(),_Item);
                
            }*/
            
            return true;
        }
        
        public bool View()
        {
            frmKitView oView = new frmKitView(this.CompanyID);
            oView.ShowDialog();
            if (oView.sSelectedID != "")
            {
                this.ID = oView.sSelectedID;
                this.Find(oView.sSelectedID);

                return true;
            }
            return false;
            
        }

        public void Clear()
        {
            
        }
        public void Save(OrderType type)
        {

            
            //MessageBox.Show(_OrderType.ToString());
            
            this.Save();
            this.SaveItems();
        }
        public void SaveItems()
        {
            
            foreach (GA_Box Row in this.Items)
            {
                //listView.Items.Add(Row.ProductID.ToString()).SubItems.AddRange(new string[] { Row.Description.ToString(), Row.Price.ToString(), Row.Quantity.ToString() });
                Row.CompanyID = _CompanyID;
                //Row.CustomerID = _CustomerID;
                
                Row.Save();

            }

        }
        
        public void Save()
        {
            
            if (if_exist())
                Update();
            else
                Insert();
        }
        public void Insert()
		{
           /* String Date = "'" + _Date.ToString("yyyy-MM-dd hh:mm:ss") + "'"; //"yyyy-MM-dd "
            String Sql = String.Format("Insert into Orders (CustomerID, Teacher, Student, Prize, Nro_Items, Retail,Collected, Tax, Printed,Date,Phone,CompanyID,Reserved ,OrderType)  Values (\"{0}\",\"{1}\",\"{2}\",'{3}','{4}','{5}','{6}','{7}','{8}',{9},'{10}','{11}','0',{12})",this._CustomerID,
                this._Teacher, this._Student, this._PrizeID, this._NoItems, this._Retail, this._Collected, this._Tax, this._Printed, Date, this._Phone, this._CompanyID, this._OrderType);
            Console.WriteLine(Sql);
			oMySql.exec_sql(Sql);	*/
			return;
			
		}
        public void Update()
        {
        /* String Date = "'" + _Date.ToString("yyyy-MM-dd hh:mm:ss") + "'"; //"yyyy-MM-dd "
         String Sql = String.Format("Update Orders Set Prize='{0}', Nro_Items='{1}', Retail='{2}',Collected='{3}', Tax='{4}', Printed='{5}',Date={6},Phone='{7}',Reserved='0', OrderType={8}"+" Where CompanyID=\""+this._CompanyID+"\""+" And CustomerID=\""+this._CustomerID+"\""+" And Teacher=\""+this._Teacher+"\""+" And Student=\""+this._Student+"\"",
				this._PrizeID,this._NoItems,this._Retail,this._Collected,this._Tax,this._Printed,Date,this._Phone, this._OrderType);
			oMySql.exec_sql(Sql);	*/
			return;
        }
        public void Delete()
        {
            //Delete Customer Orders
            String Sql = "Delete from Orders where CompanyID='" + _CompanyID + "' And CustomerID = '" + _CustomerID + "'";
            oMySql.exec_sql(Sql);
            
            return;
        }
        public bool if_exist()
        {
        /*    if (oMySql.exec_sql_no("Select count(*) from Orders Where CustomerID='" + this._CustomerID + "' And CompanyID='" + this._CompanyID + "' And Teacher=\"" + this._Teacher + "\" And Student=\"" + this._Student + "\"") == 0)
            {
                return false;
            }*/
            return true;
        }
#endregion
        #region Properties

        public String CustomerID
        {
            get { return _CustomerID; }
            set
            {
                oCustomer.Find(value);
                _CustomerID = value; 
            }
        }
        public String CompanyID
        {
            get { return _CompanyID; }
            set { _CompanyID = value; }
        }
        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        
        #endregion

        #region Classes & Enumerators
        public class _Boxes : Hashlist //, IList
        {
            

         /*   object IList.this[int index]
            {
                get
                {
                    object oTemp = base[index];
                    return (Item)oTemp;
                }
                set
                {

                }
            }
    */
            
            /* Hash List Support */
            public new GA_Box this[string Key]
            {
                get { return (GA_Box)base[Key]; }

            }
            public new GA_Box this[int index]
            {
                get {
                    object oTemp = base[index];
                    return (GA_Box)oTemp; 
                }
            }
            
            // Expose the enumerator for the associative array.
            new public  IEnumerator GetEnumerator()
            {
                return new ItemsEnumerator(this);
            }
        
        
        }
        public class ItemsEnumerator : IEnumerator
        {
            public ItemsEnumerator(_Boxes ar)
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

        #endregion


    }

}
