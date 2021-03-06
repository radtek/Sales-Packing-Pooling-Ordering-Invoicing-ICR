using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Drawing.Design;

namespace Signature.Web.Controls
{

    [DefaultProperty("AutoCompleteEnabled")]
    [ToolboxData("<{0}:Header runat=server></{0}:Header>")]
    public class Header : WebControl
	{
        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; }
        }
        private TopMenu.MenuType _menu = TopMenu.MenuType.MainMenu;
        public TopMenu.MenuType MenuType
        {
            get { return _menu; }
            set { _menu = value; }
        }


        protected TextBox searchBox;
 
		protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            this.PageTitle = "Signature Fundrasing, Inc.";
            //this.Controls.Add(BuildPage( GenerateWebControl() ));
            BuildHeader(this);
			
		}

        protected virtual void BuildHeader(WebControl form)
        {
                form.Controls.Add(new LiteralControl(@"
				<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>
				<html>
					<head >"));
    
                form.Controls.Add(new Head());
                form.Controls.Add(new LiteralControl(@"    </head>
					<body>
						<table border='0' align='center'  bgcolor = 'white' width='780px'>
						<tr>
                            <td colspan=3>
                                 <table width='100%' height='100px' border='0'  align='center' cellpadding='0' cellspacing='0'>
 	                                <tr>	
		                                <td> "));

                form.Controls.Add(new Banner());
                form.Controls.Add(new LiteralControl(@"
		                                </td>
	                                </tr>
                                </table>
                            </td>
						</tr>
                        "));

                 BuilMenu(form);
        }

        protected virtual void BuildFooter(WebControl form)
        {

            form.Controls.Add(new Footer());
        }

        protected virtual void BuilMenu(WebControl form)
        {
            form.Controls.Add(new LiteralControl(@"
                <tr> 
						<td  height='15px' width='100%' colspan= '3'> 
                                    "));
         //  form.Controls.Add((TopMenuControl)LoadControl("~/TopMenuControl.ascx"));

            form.Controls.Add(new TopMenu(this.MenuType));
 
           form.Controls.Add(new LiteralControl(@"
                        </td> 		
			    </tr> "));
            
        }

        protected virtual WebControl BuildPage(WebControl form)
		{	
            BuildHeader(form);
          //  AddControlsFromDerivedPage(form);
           // AddSearch(form);
           // BuildFooter(form);
            return form;
		}

		protected void AddSearch( WebControl form )
		{
			searchBox = new TextBox();
			Button searchButton = new Button();
			searchButton.Text = "Search";
			searchButton.Click += new EventHandler( this.OnSearchButtonClicked );
			form.Controls.Add( searchBox );
			form.Controls.Add( searchButton );
			form.Controls.Add( new LiteralControl("<br>") );
		}

		protected void OnSearchButtonClicked( object sender, EventArgs e )
		{
			string searchText = searchBox.Text;
		}

		protected virtual void AddControlsFromDerivedPage( WebControl form)
		{
			int count = this.Controls.Count;
			for( int i = 0; i<count; ++i )
			{
				System.Web.UI.Control ctrl  = this.Controls[0];
				form.Controls.Add( ctrl );
				this.Controls.Remove( ctrl );
			}
		}
	}
}
