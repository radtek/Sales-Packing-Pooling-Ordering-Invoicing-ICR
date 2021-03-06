using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;


namespace Signature.Card
{
    public enum CardPrintType
    {
        Text = 0,
        Image = 1,
        TextAndImage = 2
    }
    
    public class CardPrinter : System.Drawing.Printing.PrintDocument
    {
        #region Property Variables 
        
        private ArrayList Lines = new ArrayList();
        public CardPrintType PrintType = CardPrintType.Text;

        [Browsable(true)]
        private short _Copies = 0;
        public short Copies
        {
            get { return _Copies; }
            set { _Copies = value; }
        }

        [Browsable(true)]
        private short _Rotation = 0;
        public short Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }

        [Browsable(true)]
        public Boolean TestMode = false;

        [Browsable(true)]
        private RectangleF _PrintArea;
        public RectangleF PrintArea
        {
            get { return _PrintArea; }
            set { _PrintArea = value; }
        }


        [Browsable(true)]
        private PointF _Point;
        public PointF Point
        {
            get { return _Point; }
            set { _Point = value; }
        }

        [Browsable(true)]
        private Boolean _UpperCase;
        public Boolean UpperCase
        {
            get { return _UpperCase; }
            set { _UpperCase = value; }
        }

        [Browsable(true)]
        private System.Drawing.Font _Font;
        public System.Drawing.Font Font
        {
            get { return _Font; }
            set { _Font = value; }
        }

        public String FileName = "";

        public enum cardOrientation
        {
            Custom = 0,
            Horizontal  = 180,
            Vertical    = 270
        }

        private System.Drawing.Printing.PaperSize _PaperSize;
        public System.Drawing.Printing.PaperSize PaperSize
        {
            get { return _PaperSize; }
            set { _PaperSize = value; }
        }

        #endregion



        #region  OnBeginPrint 
        protected override void OnBeginPrint(System.Drawing.Printing.PrintEventArgs e)
        {
            this.Settings();
            // Run base code
            base.OnBeginPrint(e);
        }
        #endregion

        #region  OnPrintPage 
        protected override void OnPrintPage(System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Run base code
            base.OnPrintPage(e);
           
            PointF _Center;
            Double printHeight;
            Double printWidth;
            Double top;
            Double left;

            printHeight = base.DefaultPageSettings.PrintableArea.Height;
            printWidth  = base.DefaultPageSettings.PrintableArea.Width;
            left        = base.DefaultPageSettings.PrintableArea.X;
            top         = base.DefaultPageSettings.PrintableArea.Y;


            //Create a rectangle printing are for our document
            _PrintArea = new RectangleF((float)left, (float)top, (float)printWidth, (float)printHeight);

            if (this.TestMode)
                e.Graphics.DrawRectangle(new Pen(Brushes.Red), new Rectangle((int)_PrintArea.X, (int)_PrintArea.Y, (int)_PrintArea.Width-1, (int)_PrintArea.Height-1));

       //     if (Rotation == (int)cardOrientation.Horizontal)
      //          _Center = new PointF(_PrintArea.Width / 2, (float)(Point.Y - top));
      //      else
                _Center = new PointF((float)(Point.X - left), (float)(Point.Y - top) );


            if (this.TestMode)
                e.Graphics.DrawEllipse(new Pen(Brushes.Red, 0.5f), _Center.X - 7, _Center.Y - 7, 14, 14);

            this.Point = _Center;

            switch (PrintType)
            {
                case CardPrintType.Text:
                    this.PrintText(e.Graphics);
                    break;
                case CardPrintType.Image:
                    this.PrintImage(e.Graphics);
                    break;
                default:
                    this.PrintText(e.Graphics);
                    break;

            }
            
            
            //Just One Page
            e.HasMorePages = false;
        }
        #endregion

        #region Helpers
        public PointF GetPoint(PointF startPoint, Double angle, Single radius)
        {
            Double y, x;
            y = Convert.ToDouble(startPoint.Y - (Math.Cos(angle * Math.PI / 180) * radius));
            x = Convert.ToDouble(startPoint.X + (Math.Sin(angle * Math.PI / 180) * radius));
            return new PointF((float)x, (float)y);
        }
        public float GetRadius(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }
        public float GetAngle(float x, float y)
        {
            return (float)Math.Atan(x / y);
        }
        private void DrawLine(PointF center, float fThickness, float fLength, Color color, float fRadians, Graphics e)
        {
            e.DrawLine(new Pen(color, fThickness),
                center.X,
                center.Y,
                center.X + (float)(fLength * System.Math.Sin(fRadians * Math.PI / 180)),
                center.Y - (float)(fLength * System.Math.Cos(fRadians * Math.PI / 180)));
        }
        #endregion

        
        public void Add(String Line)
        {
            Lines.Add(Line);
        }

        public void Clear()
        {
            Lines.Clear();
        }

        private void Settings()
        {
           
            this.DefaultPageSettings.PaperSize  = this.PaperSize;
            
            this.DefaultPageSettings.Margins    = new Margins(0, 0, 0, 0);
            this.DefaultPageSettings.Landscape  = false;

            this.PrinterSettings.Copies         = _Copies;
            this.PrinterSettings.MaximumPage    = 1;
            this.PrinterSettings.ToPage         = 1;
            this.PrinterSettings.Duplex         = Duplex.Simplex;

            this.DocumentName = "testing print";
            Point = new PointF(Point.X, (float)(Point.Y)); //Ajust for Non-Printable Area
            /*
            if (this.PaperSize.Width > this.PaperSize.Height)
            {
                this.DefaultPageSettings.Landscape = true;
                Point = new PointF(Point.Y, (float)(Point.X)); //Ajust for Non-Printable Area
                this.Rotation += 90;
            }
           */ 
        }

        private void PrintText(Graphics g)
        {
            float _Rotation = this.Rotation - 90;
            float _Height = 0;
            float _Width = 0;
            foreach (String _text in Lines)
            {
                SizeF textSize = g.MeasureString(this.UpperCase ? _text.ToUpper() : _text, this.Font);
                if (textSize.Width > _Width)
                    _Width = textSize.Width;
                _Height += (float)Font.Height;
            }
            
    //        Console.WriteLine("Height: "+_Height.ToString() + "  "+"Width: "+_Width.ToString());

            float _Radius   = this.GetRadius(_Height / 2, _Width / 2);
            float _Angle    = this.GetAngle(_Height / 2, _Width / 2)/(float)Math.PI * 180;
            PointF _Point   = this.GetPoint(new PointF(Point.X, Point.Y), _Rotation + _Angle,_Radius);

            if (this.TestMode)
            {
 //               g.DrawEllipse(new Pen(Brushes.Black, 0.5f), Point.X - 5, Point.Y - 5, 10, 10);
              //  this.DrawLine(new PointF(oCardStyle.Canvas.X, oCardStyle.Canvas.Y), 1f, _Radius, Color.Black, this.oCardStyle.Rotation, g);
              //  this.DrawLine(new PointF(oCardStyle.Canvas.X, oCardStyle.Canvas.Y), 1f, _Radius, Color.Black, this.oCardStyle.Rotation + _Angle, g);

//                g.DrawEllipse(new Pen(Brushes.Black, 0.5f), Point.X - _Radius, Point.Y - _Radius, _Radius * 2, _Radius * 2);
               // g.DrawEllipse(new Pen(Brushes.Blue, 0.5f), _Point.X - 5, _Point.Y - 5, 10, 10);
            }
            
            g.TranslateTransform(_Point.X, _Point.Y);
            g.RotateTransform(_Rotation + 90);
            
            if (this.TestMode)
                  g.DrawRectangle(new Pen(Brushes.Black, 0.4f), 0f, 0f, _Width, _Height);

            
            float top = 0 ;
            foreach (String _text in Lines)
            {  
                SizeF textSize = g.MeasureString(this.UpperCase?_text.ToUpper():_text, Font);
                float h_pos = _Width/2 - (textSize.Width / 2);
                g.DrawString(this.UpperCase ? _text.ToUpper() : _text, Font, Brushes.Black, h_pos, top);
                top += (float)Font.Height;
            }
            g.ResetTransform();
        }

        private void PrintImage(Graphics g)
        {
          //  FileName = @"Picture1.jpg";
            
            float _Rotation = this.Rotation - 90;
            float _Height = 100;
            float _Width = 360;
            


            float _Radius   = this.GetRadius(_Height / 2, _Width / 2);
            float _Angle    = this.GetAngle(_Height / 2, _Width / 2) / (float)Math.PI * 180;
            PointF _Point   = this.GetPoint(new PointF(Point.X, Point.Y), _Rotation + _Angle, _Radius);

            if (this.TestMode)
            {
                //               g.DrawEllipse(new Pen(Brushes.Black, 0.5f), Point.X - 5, Point.Y - 5, 10, 10);
                //  this.DrawLine(new PointF(oCardStyle.Canvas.X, oCardStyle.Canvas.Y), 1f, _Radius, Color.Black, this.oCardStyle.Rotation, g);
                //  this.DrawLine(new PointF(oCardStyle.Canvas.X, oCardStyle.Canvas.Y), 1f, _Radius, Color.Black, this.oCardStyle.Rotation + _Angle, g);

                //                g.DrawEllipse(new Pen(Brushes.Black, 0.5f), Point.X - _Radius, Point.Y - _Radius, _Radius * 2, _Radius * 2);
                // g.DrawEllipse(new Pen(Brushes.Blue, 0.5f), _Point.X - 5, _Point.Y - 5, 10, 10);
            }

            g.TranslateTransform(_Point.X, _Point.Y);
            g.RotateTransform(_Rotation + 90);

            if (this.TestMode)
                g.DrawRectangle(new Pen(Brushes.Black, 0.4f), 0f, 0f, _Width, _Height);

            /*
            float top = 0;
            foreach (String _text in Lines)
            {
                SizeF textSize = g.MeasureString(this.UpperCase ? _text.ToUpper() : _text, Font);
                float h_pos = _Width / 2 - (textSize.Width / 2);
                g.DrawString(this.UpperCase ? _text.ToUpper() : _text, Font, Brushes.Black, h_pos, top);
                top += (float)Font.Height;
            }
            */

            

            
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(FileName);
            /*
            PictureBox picBarcode = new PictureBox();
            //picBarcode.Size = new Size(oPrint.PaperSize.Width, oPrint.PaperSize.Height);
            picBarcode.Size = new Size((int)_Width,(int)_Height);
            picBarcode.SizeMode = PictureBoxSizeMode.AutoSize;
            picBarcode.Image = bitmap;
            picBarcode.Refresh();
            */

            Image final_image = resizeImage(bitmap, new Size((int)_Width, (int)_Height));

            g.DrawImage(final_image, _Width/2-final_image.Width/2, _Height/2-final_image.Height/2);

            //g.DrawImage(bitmap,new Rectangle(0,0,300,100));
            g.ResetTransform();
        }

        public void GetDisplaySettings(Graphics g)
        {


            //Graphics graphics;
            //graphics = this.CreateGraphics();

            float currentDpiX = g.DpiX;
            float currentDpiY = g.DpiY;
            g.Dispose(); // dont forget to release the unnecessary resources

        }
        private Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }
        public void ResizeImage(string OriginalFile, string NewFile, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
        {
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFile);

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (OnlyResizeIfWider)
            {
                if (FullsizeImage.Width <= NewWidth)
                {
                    NewWidth = FullsizeImage.Width;
                }
            }

            int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                NewHeight = MaxHeight;
            }

            System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            FullsizeImage.Dispose();

            // Save resized picture
            NewImage.Save(NewFile);
        }
        public Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight) 
        { 
                Bitmap result = new Bitmap(nWidth, nHeight); 
                using (Graphics g = Graphics.FromImage((Image)result))    
                    g.DrawImage(b, 0, 0, nWidth, nHeight); 
                return result; 
        }
    }

}
