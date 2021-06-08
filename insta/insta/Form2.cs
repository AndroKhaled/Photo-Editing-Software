using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using openCV;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using AForge;
using AForge.Imaging;
using AForge.Math;
using AForge.Imaging.ComplexFilters;


namespace insta
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        //for resizing bitmap..
        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }
            return result;
        }

        IplImage image1,img2,img3;
        IplImage img;
        Bitmap bmp;
        Bitmap bitmap;

        private void Form2_Load(object sender, EventArgs e)
        {
            //open image...
            openFileDialog1.FileName = " ";
            openFileDialog1.Filter = "JPEG|*JPG|Bitmap|*.bmp|All|*.*-11";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image1 = cvlib.CvLoadImage(openFileDialog1.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
                    CvSize size = new CvSize(pictureBox1.Width, pictureBox1.Height);
                    IplImage resized_image = cvlib.CvCreateImage(size, image1.depth, image1.nChannels);
                    cvlib.CvResize(ref image1, ref resized_image, cvlib.CV_INTER_LINEAR);
                    pictureBox1.BackgroundImage = (System.Drawing.Image)resized_image;
                    pictureBox2.BackgroundImage = (System.Drawing.Image)resized_image;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            /*                                
              
                                    red image effect                  
             
             */
            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAdd = image1.imageData.ToInt32();
            int dstAdd = img.imageData.ToInt32();
            unsafe
            {
                int srcIndex, dstIndex;
                for (int r = 0; r < img.height; r++)
                    for (int c = 0; c < img.width; c++)
                    {
                        srcIndex = dstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(dstAdd + dstIndex + 0) = 0;
                        *(byte*)(dstAdd + dstIndex + 1) = 0;
                        *(byte*)(dstAdd + dstIndex + 2) = *(byte*)(srcAdd + srcIndex + 2);
                    }
            }

            CvSize rsize = new CvSize(button1.Width, button1.Height);
            IplImage rresized_image = cvlib.CvCreateImage(rsize, img.depth, img.nChannels);
            cvlib.CvResize(ref img, ref rresized_image, cvlib.CV_INTER_LINEAR);
            button1.Image = (System.Drawing.Image)rresized_image;

            /*                                
              
                                   green image effect                  
             
            */
            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int gsrcAdd = image1.imageData.ToInt32();
            int gdstAdd = img.imageData.ToInt32();
            unsafe
            {
                int gsrcIndex, gdstIndex;
                for (int r = 0; r < img.height; r++)
                    for (int c = 0; c < img.width; c++)
                    {
                        gsrcIndex = gdstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(gdstAdd + gdstIndex + 0) = 0;
                        *(byte*)(gdstAdd + gdstIndex + 1) = *(byte*)(gsrcAdd + gsrcIndex + 1);
                        *(byte*)(gdstAdd + gdstIndex + 2) = 0;
                    }
            }
            CvSize gsize = new CvSize(button1.Width, button1.Height);
            IplImage gresized_image = cvlib.CvCreateImage(gsize, img.depth, img.nChannels);
            cvlib.CvResize(ref img, ref gresized_image, cvlib.CV_INTER_LINEAR);
            button2.Image = (System.Drawing.Image)gresized_image;

            /*                                
              
                                  BLUE image effect                  
             
            */

            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int bsrcAdd = image1.imageData.ToInt32();
            int bdstAdd = img.imageData.ToInt32();
            unsafe
            {
                int bsrcIndex, bdstIndex;
                for (int r = 0; r < img.height; r++)
                    for (int c = 0; c < img.width; c++)
                    {
                        bsrcIndex = bdstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(bdstAdd + bdstIndex + 0) = *(byte*)(bsrcAdd + bsrcIndex + 0);
                        *(byte*)(bdstAdd + bdstIndex + 1) = 0;
                        *(byte*)(bdstAdd + bdstIndex + 2) = 0;
                    }
            }
            CvSize bsize = new CvSize(button1.Width, button1.Height);
            IplImage bresized_image = cvlib.CvCreateImage(bsize, img.depth, img.nChannels);
            cvlib.CvResize(ref img, ref bresized_image, cvlib.CV_INTER_LINEAR);
            button3.BackgroundImage = (System.Drawing.Image)bresized_image;

            /*                                
              
                                GREY image effect                  
             
          */
            bmp = (Bitmap)image1;
            Bitmap newImage = ResizeBitmap(bmp, button4.Width, button4.Height);
            int width = button4.Width;
            int height = button4.Height;
            Color p;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = newImage.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    newImage.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }
            button4.BackgroundImage = (System.Drawing.Image)newImage;

            /*                                
              
                             BRIGHT image effect                  
             
             */
            int brightness = 70;
            Bitmap temp = (Bitmap)image1;
            Bitmap newImagebb = ResizeBitmap(temp, button7.Width, button7.Height);
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color cbb;
            for (int i = 0; i < newImagebb.Width; i++)
            {
                for (int j = 0; j < newImagebb.Height; j++)
                {
                    cbb = newImagebb.GetPixel(i, j);
                    int cR = cbb.R + brightness;
                    int cG = cbb.G + brightness;
                    int cB = cbb.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    newImagebb.SetPixel(i, j,
        Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            button7.BackgroundImage = (Bitmap)newImagebb.Clone();


            /*                                
              
                            CONTRAST image effect                  
             
            */
            double contrast = 70;
            Bitmap tempcc = (Bitmap)image1;
            Bitmap newImagecc = ResizeBitmap(tempcc, button8.Width, button8.Height);
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color ccc;
            for (int i = 0; i < newImagecc.Width; i++)
            {
                for (int j = 0; j < newImagecc.Height; j++)
                {
                    ccc = newImagecc.GetPixel(i, j);
                    double pR = ccc.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = ccc.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = ccc.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    newImagecc.SetPixel(i, j,
        Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            button8.BackgroundImage = (Bitmap)newImagecc.Clone();

            /*                                
              
                         INVERT image effect                  
             
          */
            Bitmap tempi = (Bitmap)image1;
            Bitmap newImagei = ResizeBitmap(tempi, button9.Width, button9.Height);
            Color ci;
            for (int i = 0; i < newImagei.Width; i++)
            {
                for (int j = 0; j < newImagei.Height; j++)
                {
                    ci = newImagei.GetPixel(i, j);
                    newImagei.SetPixel(i, j,
      Color.FromArgb(255 - ci.R, 255 - ci.G, 255 - ci.B));
                }
            }
            button9.BackgroundImage = (Bitmap)newImagei.Clone();

            /*                                
              
                        FLIP image effect                  
             
         */
            Bitmap tempf = (Bitmap)image1;
            Bitmap newImagef = ResizeBitmap(tempf, button10.Width, button10.Height);
            newImagef.RotateFlip(RotateFlipType.RotateNoneFlipX);
            button10.BackgroundImage = (Bitmap)newImagef.Clone();

            /*                                
              
                       ROTATE image effect                  
             
        */
            Bitmap tempR = (Bitmap)image1;
            Bitmap newImageR = ResizeBitmap(tempR, button10.Width, button10.Height);
            newImageR.RotateFlip(RotateFlipType.Rotate180FlipNone);
            button13.BackgroundImage = (Bitmap)newImageR.Clone();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int srcAdd = image1.imageData.ToInt32();
            int dstAdd = img.imageData.ToInt32();
            unsafe
            {
                int srcIndex, dstIndex;
                for (int r = 0; r < img.height; r++)
                    for (int c = 0; c < img.width; c++)
                    {
                        srcIndex = dstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(dstAdd + dstIndex + 0) = 0;
                        *(byte*)(dstAdd + dstIndex + 1) = 0;
                        *(byte*)(dstAdd + dstIndex + 2) = *(byte*)(srcAdd + srcIndex + 2);
                    }
            }

            CvSize rsize = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage rresized_image = cvlib.CvCreateImage(rsize, img.depth, img.nChannels);
            cvlib.CvResize(ref img, ref rresized_image, cvlib.CV_INTER_LINEAR);
            pictureBox2.Image = (System.Drawing.Image)rresized_image;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int gsrcAdd = image1.imageData.ToInt32();
            int gdstAdd = img.imageData.ToInt32();
            unsafe
            {
                int gsrcIndex, gdstIndex;
                for (int r = 0; r < img.height; r++)
                    for (int c = 0; c < img.width; c++)
                    {
                        gsrcIndex = gdstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(gdstAdd + gdstIndex + 0) = 0;
                        *(byte*)(gdstAdd + gdstIndex + 1) = *(byte*)(gsrcAdd + gsrcIndex + 1);
                        *(byte*)(gdstAdd + gdstIndex + 2) = 0;
                    }
            }
            CvSize gsize = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage gresized_image = cvlib.CvCreateImage(gsize, img.depth, img.nChannels);
            cvlib.CvResize(ref img, ref gresized_image, cvlib.CV_INTER_LINEAR);
            pictureBox2.Image = (System.Drawing.Image)gresized_image;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            img = cvlib.CvCreateImage(new CvSize(image1.width, image1.height), image1.depth, image1.nChannels);
            int bsrcAdd = image1.imageData.ToInt32();
            int bdstAdd = img.imageData.ToInt32();
            unsafe
            {
                int bsrcIndex, bdstIndex;
                for (int r = 0; r < img.height; r++)
                    for (int c = 0; c < img.width; c++)
                    {
                        bsrcIndex = bdstIndex = (img.width * r * img.nChannels) + (c * img.nChannels);
                        *(byte*)(bdstAdd + bdstIndex + 0) = *(byte*)(bsrcAdd + bsrcIndex + 0);
                        *(byte*)(bdstAdd + bdstIndex + 1) = 0;
                        *(byte*)(bdstAdd + bdstIndex + 2) = 0;
                    }
            }
            CvSize bsize = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage bresized_image = cvlib.CvCreateImage(bsize, img.depth, img.nChannels);
            cvlib.CvResize(ref img, ref bresized_image, cvlib.CV_INTER_LINEAR);
            pictureBox2.Image = (System.Drawing.Image)bresized_image;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            Bitmap newImage = ResizeBitmap(bmp, pictureBox2.Width, pictureBox2.Height);
            int width = pictureBox2.Width;
            int height = pictureBox2.Height;
            Color p;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = newImage.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    newImage.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }
            pictureBox2.Image = (System.Drawing.Image)newImage;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = " ";
            openFileDialog1.Filter = "JPEG|*JPG|Bitmap|*.bmp|All|*.*-11";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    img2 = cvlib.CvLoadImage(openFileDialog1.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
                    CvSize size = new CvSize(pictureBox1.Width, pictureBox1.Height);
                    IplImage resized_image = cvlib.CvCreateImage(size, image1.depth, image1.nChannels);
                    cvlib.CvResize(ref img2, ref resized_image, cvlib.CV_INTER_LINEAR);
                    //pictureBox1.BackgroundImage = (Image)resized_image;
                    pictureBox2.BackgroundImage = (System.Drawing.Image)resized_image;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
            /////////////////////////////////////



            /////////////////////////////////////

            int min_width = 0, min_height = 0;
            if (img.width < img2.width)
                min_width = img.width;
            else
                min_width = img2.width;

            if (img.height < img2.height)
                min_height = img.height;
            else
                min_height = img2.height;

            img3 = cvlib.CvCreateImage(new CvSize(min_width, min_height), img.depth, img.nChannels);

            /////////////////////////////////////////////////LOOP 
            int srcX = img.imageData.ToInt32();
            int srcY = img2.imageData.ToInt32();
            int dstAddress = img3.imageData.ToInt32();
            unsafe
            {
                for (int r = 0; r < img.height; r++)
                {
                    for (int c = 0; c < img.width; c++)
                    {

                        int srcIndexX, srcIndexY, disIndex;
                        srcIndexX = (img.width * r * img.nChannels) + (img.nChannels * c);
                        srcIndexY = (img2.width * r * img2.nChannels) + (img2.nChannels * c);
                        disIndex = (img3.width * r * img3.nChannels) + (img3.nChannels * c);

                        byte* redX = (byte*)(srcX + srcIndexX + 2);
                        byte* greenX = (byte*)(srcX + srcIndexX + 1);
                        byte* blueX = (byte*)(srcX + srcIndexX + 0);

                        byte* redY = (byte*)(srcY + srcIndexY + 2);
                        byte* greenY = (byte*)(srcY + srcIndexY + 1);
                        byte* blueY = (byte*)(srcY + srcIndexY + 0);

                        byte red = (byte)(255 - (*redX + *redY));
                        byte green = (byte)(255 - (*greenX + *greenY));
                        byte blue = (byte)(255 - (*blueX + *blueY));
                        *(byte*)(dstAddress + disIndex + 2) = red;
                        *(byte*)(dstAddress + disIndex + 1) = green;
                        *(byte*)(dstAddress + disIndex + 0) = blue;
                    }
                }
            }
            CvSize size2 = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage resized_image2 = cvlib.CvCreateImage(size2, img3.depth, img3.nChannels);
            cvlib.CvResize(ref img3, ref resized_image2, cvlib.CV_INTER_LINEAR);
            pictureBox2.Image = (System.Drawing.Image)resized_image2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = " ";
            openFileDialog1.Filter = "JPEG|*JPG|Bitmap|*.bmp|All|*.*-11";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    img2 = cvlib.CvLoadImage(openFileDialog1.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
                    CvSize size = new CvSize(pictureBox1.Width, pictureBox1.Height);
                    IplImage resized_image = cvlib.CvCreateImage(size, image1.depth, image1.nChannels);
                    cvlib.CvResize(ref img2, ref resized_image, cvlib.CV_INTER_LINEAR);
                    //pictureBox1.BackgroundImage = (Image)resized_image;
                    pictureBox2.BackgroundImage = (System.Drawing.Image)resized_image;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            /////////////////////////////////////



            /////////////////////////////////////

            int min_width = 0, min_height = 0;
            if (img.width < img2.width)
                min_width = img.width;
            else
                min_width = img2.width;

            if (img.height < img2.height)
                min_height = img.height;
            else
                min_height = img2.height;

            img3 = cvlib.CvCreateImage(new CvSize(min_width, min_height), img.depth, img.nChannels);

            /////////////////////////////////////////////////LOOP 
            int srcX = img.imageData.ToInt32();
            int srcY = img2.imageData.ToInt32();
            int dstAddress = img3.imageData.ToInt32();
            unsafe
            {
                for (int r = 0; r < img.height; r++)
                {
                    for (int c = 0; c < img.width; c++)
                    {

                        int srcIndexX, srcIndexY, disIndex;
                        srcIndexX = (img.width * r * img.nChannels) + (img.nChannels * c);
                        srcIndexY = (img2.width * r * img2.nChannels) + (img2.nChannels * c);
                        disIndex = (img3.width * r * img3.nChannels) + (img3.nChannels * c);

                        byte* redX = (byte*)(srcX + srcIndexX + 2);
                        byte* greenX = (byte*)(srcX + srcIndexX + 1);
                        byte* blueX = (byte*)(srcX + srcIndexX + 0);

                        byte* redY = (byte*)(srcY + srcIndexY + 2);
                        byte* greenY = (byte*)(srcY + srcIndexY + 1);
                        byte* blueY = (byte*)(srcY + srcIndexY + 0);

                        byte red = (byte)((*redX - *redY));
                        byte green = (byte)((*greenX - *greenY));
                        byte blue = (byte)((*blueX - *blueY));
                        *(byte*)(dstAddress + disIndex + 2) = red;
                        *(byte*)(dstAddress + disIndex + 1) = green;
                        *(byte*)(dstAddress + disIndex + 0) = blue;
                    }
                }
            }
            CvSize size2 = new CvSize(pictureBox2.Width, pictureBox2.Height);
            IplImage resized_image2 = cvlib.CvCreateImage(size2, img3.depth, img3.nChannels);
            cvlib.CvResize(ref img3, ref resized_image2, cvlib.CV_INTER_LINEAR);
            pictureBox2.Image = (System.Drawing.Image)resized_image2;
        }

        //Brigh func
        public void SetBrightness(int brightness)
        {
            Bitmap temp = (Bitmap)image1;
            Bitmap newImage = ResizeBitmap(temp, pictureBox2.Width, pictureBox2.Height);
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < newImage.Width; i++)
            {
                for (int j = 0; j < newImage.Height; j++)
                {
                    c = newImage.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    newImage.SetPixel(i, j,
        Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
           pictureBox2.Image = (Bitmap)newImage.Clone();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetBrightness(70);
        }

        //cont func
        public void SetContrast(double contrast)
        {
            Bitmap temp = (Bitmap)image1;
            Bitmap newImage = ResizeBitmap(temp, pictureBox2.Width, pictureBox2.Height);
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color c;
            for (int i = 0; i < newImage.Width; i++)
            {
                for (int j = 0; j < newImage.Height; j++)
                {
                    c = newImage.GetPixel(i, j);
                    double pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    newImage.SetPixel(i, j,
        Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            pictureBox2.Image = (Bitmap)newImage.Clone();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetContrast(70);
        }

        //inv funcc
        public void SetInvert()
        {
            Bitmap temp = (Bitmap)image1;
            Bitmap newImage = ResizeBitmap(temp, pictureBox2.Width, pictureBox2.Height);
            Color c;
            for (int i = 0; i < newImage.Width; i++)
            {
                for (int j = 0; j < newImage.Height; j++)
                {
                    c = newImage.GetPixel(i, j);
                    newImage.SetPixel(i, j,
      Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            pictureBox2.Image = (Bitmap)newImage.Clone();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SetInvert();
        }

        //rotate func
        public void RotateFlip(RotateFlipType rotateFlipType)
        {
            Bitmap temp = (Bitmap)image1;
            Bitmap newImage = ResizeBitmap(temp, pictureBox2.Width, pictureBox2.Height);
            newImage.RotateFlip(rotateFlipType);
            pictureBox2.Image = (Bitmap)newImage.Clone();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            RotateFlip(RotateFlipType.RotateNoneFlipX);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            RotateFlip(RotateFlipType.Rotate180FlipNone);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Specify a file name and file path";
            sfd.Filter = "Png Images(*.png)|*.png|Jpeg Images(*.jpg)|*.jpg";
            sfd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileExtension = Path.GetExtension(sfd.FileName).ToUpper();
                ImageFormat imgFormat = ImageFormat.Png;

                if (fileExtension == "BMP")
                {
                    imgFormat = ImageFormat.Bmp;
                }
                else if (fileExtension == "JPG")
                {
                    imgFormat = ImageFormat.Jpeg;
                }
                StreamWriter streamWriter = new StreamWriter(sfd.FileName, false);
                pictureBox2.Image.Save(streamWriter.BaseStream, imgFormat);
                streamWriter.Flush();
                streamWriter.Close();
                bmp = null;
            }
        }
    }
}
