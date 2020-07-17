using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageResizer
{
    public partial class Form1 : Form
    {
        FolderBrowserDialog fbd;
        string source;
        string destination;
        bool sourceSelected;
        bool destinationSelected;
        string[] fileNames;
        List<string> imagePaths;
        List<Image> newImages;
        public Form1()
        {
            
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                source = fbd.SelectedPath;
                sourceSelected = true;
                fileNames = Directory.GetFiles(source, "*.*");
                label1.Text = fileNames.Length + " Image Files Found";                
            }
            else
            {
                source = "";
                sourceSelected = false;
                label1.Text = "Select a Source Folder";
                MessageBox.Show("Please select valid source folder");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                destination = fbd.SelectedPath;
                destinationSelected = true;
                label2.Text = destination;
            }
            else
            {
                destination = "";
                destinationSelected = false;
                label2.Text = "Select a Source Folder";
                MessageBox.Show("Please select valid destination folder");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(!sourceSelected || !destinationSelected)
            {
                MessageBox.Show("Choose Destination");
            }
            imagePaths = new List<string>();
            label3.Text = "";              
            foreach (var fileName in fileNames)
            {
                if (fileName.ToLower().EndsWith(".jpg") || fileName.ToLower().EndsWith(".jpeg") || fileName.ToLower().EndsWith(".png"))
                {
                    Image newImage = (ResizeImage(.75, fileName));
                    string filePath = Path.Combine(destination, Path.GetFileName(fileName));
                    newImage.Save(filePath);
                }                   
            }
            label3.Text = imagePaths.Count() + " Images Loaded";
        }

        public Image ResizeImage(double pct, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            if ((sourceHeight + sourceWidth) >= 5000)
                pct = .90;
            else if ((sourceHeight + sourceWidth) >= 4000)
                pct = .875;
            else if ((sourceHeight + sourceWidth) >= 3000)
                pct = .83;
            else if ((sourceHeight + sourceWidth) >= 2000)
                pct = .75;
            else if ((sourceHeight + sourceWidth) >= 1500)
                pct = .6666;
            else if ((sourceHeight + sourceWidth) >= 1000)
                pct = .5;
            else if ((sourceHeight + sourceWidth) >= 500)
                pct = 0;


            int newHeight = (int)Math.Round((double)(sourceHeight * (1 - pct)));
            int newWidth = (int)Math.Round((double)(sourceWidth * (1 - pct)));



            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }

    }
}
