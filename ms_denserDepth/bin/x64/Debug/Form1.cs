using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using System.Drawing.Drawing2D;
using Emgu.CV.Util;
#if !__IOS__
using Emgu.CV.Cuda;
#endif
using Emgu.CV.XFeatures2D;

namespace ms_denserDepth
{
    public partial class Form1 : Form
    {
        int nImg = 0;
        public List<Mat> CImgs = new List<Mat>(); // input color images
        Image<Gray, Byte> Canny_img;
        Image<Gray, Byte> Canny2_img;
        List<Image<Bgr, Byte>> original_img = new List<Image<Bgr, byte>>();
        Image<Bgr, Byte> merged_img;
        Image<Gray, Byte> depth_img;
        Image<Gray, Byte> denser_img;
        List<Image<Bgr, Byte>> feature_img = new List<Image<Bgr, Byte>>();
        Image<Gray, Byte> labeled_img;
        Image<Gray, Byte> labeled2_img;
        Image<Gray, Byte> binary_img;
        Image<Gray, Byte> biCanny_img;
        Image<Gray, Byte> tripleCanny;
        List<ListViewItem> imglist = new List<ListViewItem>();
        OpenFileDialog ofd = new OpenFileDialog();
        int listcnt = 0;
        int curShowImgTypeIdx = 0;
        int targetIdx = 0;

        //@@
        int locationX = 0; // 마우스 x좌표
        int locationY = 0; // 마우스 y좌표
        int start_X = 0; // 이동 시작 x좌표
        int start_Y = 0; // 이동 시작 y좌표
        int direction_X = 0; // 이동 거리 x좌표
        int direction_Y = 0; // 이동 거리 y좌표
        int numlable = 0; // 레이블 개수
        List<Image<Gray, Byte>> layer = new List<Image<Gray, Byte>>(); // 레이어 뎁스이미지
        List<Image<Bgr, byte>> color_layer = new List<Image<Bgr, byte>>(); // 레이어 칼라이미지
        Image<Bgr, byte> App_img; // App이미지
        bool keycheck = false; // 키 눌림 확인용;


        // Data경로 생성
        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Data");


        // 각 카메라에 대한 카메라 정보
        public struct nvm_cpos
        {
            public string fileName;
            public double focalLength;
            public double w;
            public double x;
            public double y;
            public double z;
            public double cCentX;
            public double cCentY;
            public double cCentZ;
            public double radicalDistortion;
        };
        List<nvm_cpos> nvm = new List<nvm_cpos>();
        List<nvm_cpos> SortedNVM = new List<nvm_cpos>();

        // ply point cloud의 각 point
        public struct ply_pc
        {
            public double x;
            public double y;
            public double z;
            public byte r;
            public byte g;
            public byte b;
            public byte alpha;
        };
        List<ply_pc> point3f = new List<ply_pc>();

        public Form1()
        {
            InitializeComponent();

            // Data경로 생성
            di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Data");
            if (!di.Exists) { di.Create(); }
        }

        // image importing (w.by Song)
        private void button_importImg_Click(object sender, EventArgs e)
        {
            textBox_shell.Text = "importing Color images...";
            textBox_shell.Refresh();
            if (CImgs.Count == 0)
            {
                listView_imgs.View = View.Details;
                listView_imgs.Columns.Add("File name");

                ofd.Title = "select input color image files.";
                ofd.Filter = "image file(*.jpg)|*.jpg|image file(*.bmp)|*.bmp"; // 이미지 파일만 가능하도록 필터링
                ofd.FilterIndex = 1;
                ofd.Multiselect = true; // 여러 파일을 동시에 열 수 있도록

                DialogResult dRes = ofd.ShowDialog();

                if (dRes == DialogResult.OK)
                {
                    int nFiles = ofd.FileNames.Length;
                    label_numImg.Text = nFiles.ToString();
                    for (int i = 0; i < nFiles; i++)
                    {
                        /// openCVsharp
                        //Mat tempMat = Cv2.ImRead(filepath, ImreadModes.Color);

                        /// Emgu
                        Image<Bgr, Byte> imgform = new Image<Bgr, byte>(ofd.FileNames[i].ToString());
                        original_img.Add(imgform);
                        nImg++;
                        //System.Drawing.Image sImg = System.Drawing.Image.FromFile(ofd.FileNames[i]);

                        Mat tempMat = new Mat(imgform.Mat, new Rectangle(new Point(0, 0), imgform.Size));
                        CImgs.Add(tempMat);
                    }
                }
                else
                {
                    MessageBox.Show("image importing failed.");
                }

                button_runSfM.Text = "Run SfM";

                listView_imgs.BeginUpdate();
                for (int i = 0; i < CImgs.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem(Path.GetFileName(ofd.FileNames[i].ToString()));
                    imglist.Add(lvi);
                    listView_imgs.Items.Add(imglist[i]);
                }
                listView_imgs.EndUpdate();

                
                merged_img = original_img[targetIdx].Clone();

                listcnt = CImgs.Count;
            }
            else
            {
                ofd.Title = "select input color image files.";
                ofd.Filter = "image file(*.jpg)|*.jpg"; // 이미지 파일만 가능하도록 필터링
                ofd.FilterIndex = 1;
                ofd.Multiselect = true; // 여러 파일을 동시에 열 수 있도록

                DialogResult dRes = ofd.ShowDialog();

                if (dRes == DialogResult.OK)
                {
                    int nFiles = ofd.FileNames.Length;
                    label_numImg.Text = (listcnt + nFiles).ToString();
                    for (int i = 0; i < nFiles; i++)
                    {
                        /// openCVsharp
                        //Mat tempMat = Cv2.ImRead(filepath, ImreadModes.Color);

                        /// Emgu
                        Image<Bgr, Byte> imgform = new Image<Bgr, byte>(ofd.FileNames[i].ToString());
                        original_img.Add(imgform);
                        nImg++;
                        merged_img.Add(imgform);
                        //System.Drawing.Image sImg = System.Drawing.Image.FromFile(ofd.FileNames[i]);

                        Mat tempMat = new Mat(imgform.Mat, new Rectangle(new Point(0, 0), imgform.Size));
                        CImgs.Add(tempMat);
                    }
                }
                else
                {
                    MessageBox.Show("image importing failed.");
                }

                listView_imgs.BeginUpdate();
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    ListViewItem lvi = new ListViewItem(Path.GetFileName(ofd.FileNames[i].ToString()));
                    imglist.Add(lvi);
                    listView_imgs.Items.Add(imglist[i + listcnt]);
                }
                listView_imgs.EndUpdate();
                listcnt = CImgs.Count;
            }
            

            textBox_shell.Text = "importing images end";
        }

        private void pictureBox_color_Click(object sender, EventArgs e)
        {

        }

        private void listView_imgs_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeShowType();
        }

        private void changeShowType()
        {
            int indexnum;
            if (listView_imgs.FocusedItem == null)
                indexnum = 0;
            else
                indexnum = listView_imgs.FocusedItem.Index;

            switch (curShowImgTypeIdx)
            {
                case 0: // show original type image
                    imageBox_main.Image = original_img[indexnum];
                    textBox_shell.Text = "Original Images";
                    break;
                case 1: // show edge type image
                    imageBox_main.Image = Canny2_img;
                    textBox_shell.Text = "Canny Edges";
                    break;
                case 2: // show original + edge
                    imageBox_main.Image = merged_img;
                    textBox_shell.Text = "Images and Edges";
                    break;
                case 3: // show depth
                    imageBox_main.Image = depth_img;
                    textBox_shell.Text = "Depth Map";
                    break;
                case 4: // show denser depth
                    imageBox_main.Image = denser_img[targetIdx];
                    textBox_shell.Text = "Denser Depth Map";
                    break;
                case 5: // show detected object
                    imageBox_main.Image = labeled2_img;
                    textBox_shell.Text = "labeled image.";
                    break;
                case 6:
                    imageBox_main.Image = feature_img[indexnum];
                    textBox_shell.Text = "Feature Matches";
                    break;
                case 7:
                    imageBox_main.Image = labeled_img;
                    textBox_shell.Text = "Mid Level Labeled Image";
                    break;
                case 8:
                    imageBox_main.Image = App_img;
                    textBox_shell.Text = "Application Image";
                    break;
                default:
                    break;
            }
        }

        private void button_deleteImg_Click(object sender, EventArgs e)
        {
            CImgs.Clear();
            original_img.Clear();
            imglist.Clear();

            MessageBox.Show("images cleared.");
            imageBox_main.Image = null;
            label_numImg.Text = "0";

            listView_imgs.Clear();
            listcnt = 0;
        }

        private void button_runSfM_Click(object sender, EventArgs e)
        {
            visualSfM();
        }

        // run vSfM (w.by Song)
        private void visualSfM()
        {
            textBox_shell.Text = "running...";
            textBox_shell.Refresh();

            string inputPath = ofd.FileNames[0];
            char[] separatingChars = { '\\' };
            string[] words = inputPath.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            string rootPath = "";
            for (int i = 0; i < words.Length - 1; i++)
            {
                rootPath += words[i] + "\\";
            }
            rootPath += " ";
            string str_exc = "VisualSFM.exe ";
            string str_opt = "sfm+pmvs ";
            string str_ouput = "result.nvm";

            // 커맨드 조합
            string str_reconst = str_exc + str_opt + rootPath + str_ouput;


            // visual SFM 커맨드 실행
            System.Diagnostics.ProcessStartInfo proInfo = new System.Diagnostics.ProcessStartInfo();
            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            proInfo.FileName = @"cmd";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardOutput = true;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardError = true;

            pro.StartInfo = proInfo;
            pro.Start();
            pro.StandardInput.Write(str_reconst + Environment.NewLine);

            pro.StandardInput.Close();
            string resultValue = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();

            textBox_shell.Text = "SfM finished";
        }

        // nvm RT importing (w.by Song)
        private void button_getRT_Click(object sender, EventArgs e)
        {
            textBox_shell.Text = "Importing Rotation & Translation Infomation...";
            textBox_shell.Refresh();
            StreamReader reader = new StreamReader("result.nvm");

            string Line;
            int numCamera = 0;
            try
            {
                int idxLine = 0; // nvm 파일의 읽는 라인 인덱스
                int idxCLine = 0;

                do
                {
                    Line = reader.ReadLine(); 
                    char[] sepCharsName = { '\t' };
                    char[] sepCharsValues = { ' ' };

                    //파일이름과 나머지 정보를 분리 (카메라 정보 라인의 경우)
                    string[] nameParsed = Line.Split(sepCharsName, System.StringSplitOptions.RemoveEmptyEntries);


                    if (idxLine == 2)
                    {
                        numCamera = Int32.Parse(Line); // 카메라 개수
                    }

                    //카메라 정보 라인에 대한 처리
                    if (idxCLine < numCamera && nameParsed.Length > 1)
                    {
                        string[] valueParsed = nameParsed[1].Split(sepCharsValues, System.StringSplitOptions.RemoveEmptyEntries);

                        nvm_cpos tempCpos = new nvm_cpos();
                        tempCpos.fileName = nameParsed[0];
                        tempCpos.focalLength = float.Parse(valueParsed[0].ToString());
                        tempCpos.w = float.Parse(valueParsed[1].ToString());
                        tempCpos.x = float.Parse(valueParsed[2].ToString());
                        tempCpos.y = float.Parse(valueParsed[3].ToString());
                        tempCpos.z = float.Parse(valueParsed[4].ToString());
                        tempCpos.cCentX = float.Parse(valueParsed[5].ToString());
                        tempCpos.cCentY = float.Parse(valueParsed[6].ToString());
                        tempCpos.cCentZ = float.Parse(valueParsed[7].ToString());
                        tempCpos.radicalDistortion = float.Parse(valueParsed[8].ToString());
                        nvm.Add(tempCpos);

                        idxCLine++;
                    }
                    
                    idxLine++; // 한줄 넘어감
                }
                while (reader.Peek() != -1);

                textBox_shell.Text = "RT importing finished.";
            }  

            catch
            {
                textBox_shell.Text = ("nvm reading failed.");
            }

            finally
            {
                reader.Close();
            }

            // color 이미지와 순서 맞추기 위해 sort
            SortedNVM = nvm.OrderBy(o => o.fileName).ToList();
        }


        // Canny Edge Detection (w.by Jang and Song)
        void SobelDer(Image<Gray, Byte> input, ref Image<Gray, Byte> output, int[] dx_table, int[] dy_table)
        {
            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Cols; j++)
                {
                    if (i == 0 || j == 0 || i == input.Rows - 1 || j == input.Cols - 1)
                    {
                        dx_table[i * input.Cols + j] = 0;
                        dy_table[i * input.Cols + j] = 0;
                    }
                    else
                    {
                        dx_table[i * input.Cols + j]
                            = input.Data[i - 1, j - 1, 0] * (-1)
                            + input.Data[i, j - 1, 0] * (-2)
                            + input.Data[i + 1, j - 1, 0] * (-1)
                            + input.Data[i - 1, j + 1, 0] * (1)
                            + input.Data[i, j + 1, 0] * (2)
                            + input.Data[i + 1, j + 1, 0] * (1);

                        dy_table[i * input.Cols + j]
                            = input.Data[i + 1, j - 1, 0] * (-1)
                            + input.Data[i + 1, j, 0] * (-2)
                            + input.Data[i + 1, j + 1, 0] * (-1)
                            + input.Data[i - 1, j - 1, 0] * (1)
                            + input.Data[i - 1, j, 0] * (2)
                            + input.Data[i - 1, j + 1, 0] * (1);
                    }
                }
            }

            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Cols; j++)
                {
                    output.Data[i, j, 0]
                        = (Byte)((Math.Abs(dx_table[i * input.Cols + j]) 
                        + Math.Abs(dy_table[i * input.Cols + j])) / 4);
                }
            }
        }

        void CalDirec(Byte[] direction, int[] dx_table, int[] dy_table, int rows, int cols)
        {
            double PI = 3.14;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double angle = 0;
                    if (dx_table[i * cols + j] == 0)
                        angle = 0;
                    else
                        angle = Math.Atan(dy_table[i * cols + j] / dx_table[i * cols + j]);

                    if (angle > PI * (3.0f / 8.0f) || angle < -PI * (3.0f / 8.0f))
                        direction[i * cols + j] = 0;
                    else if (angle < PI * (3.0f / 8.0f) && angle > PI * (1.0f / 8.0f))
                        direction[i * cols + j] = 1;
                    else if (angle < PI * (1.0f / 8.0f) && angle > -PI * (1.0f / 8.0f))
                        direction[i * cols + j] = 2;
                    else if (angle < -PI * (1.0f / 8.0f) && angle > -PI * (3.0f / 8.0f))
                        direction[i * cols + j] = 3;
                    else
                        direction[i * cols + j] = 4;
                }
            }
        }

        void LocalMax(Image<Gray, Byte> input, ref Image<Gray, Byte> output, Byte[] direction)
        {
            for (int i = 2; i < input.Rows - 2; i++)
            {
                for (int j = 2; j < input.Cols - 2; j++)
                {
                    if (direction[i * input.Cols + j] == 0)
                    {
                        if (input.Data[i, j, 0] > input.Data[i - 1, j, 0]
                            && input.Data[i, j, 0] > input.Data[i + 1, j, 0]
                            && input.Data[i, j, 0] > input.Data[i - 2, j, 0]
                            && input.Data[i, j, 0] > input.Data[i + 2, j, 0])
                        {
                            output.Data[i, j, 0] = input.Data[i, j, 0];
                        }
                        else
                        {
                            output.Data[i, j, 0] = 0;
                        }
                    }
                    else if (direction[i * input.Cols + j] == 1)
                    {
                        if (input.Data[i, j, 0] > input.Data[i - 1, j + 1, 0]
                            && input.Data[i, j, 0] > input.Data[i + 1, j - 1, 0]
                            && input.Data[i, j, 0] > input.Data[i - 2, j + 2, 0]
                            && input.Data[i, j, 0] > input.Data[i + 2, j - 2, 0])
                        {
                            output.Data[i, j, 0] = input.Data[i, j, 0];
                        }
                        else
                        {
                            output.Data[i, j, 0] = 0;
                        }
                    }
                    else if (direction[i * input.Cols + j] == 2)
                    {
                        if (input.Data[i, j, 0] > input.Data[i, j + 1, 0]
                            && input.Data[i, j, 0] > input.Data[i, j - 1, 0]
                            && input.Data[i, j, 0] > input.Data[i, j + 2, 0]
                            && input.Data[i, j, 0] > input.Data[i, j - 2, 0])
                        {
                            output.Data[i, j, 0] = input.Data[i, j, 0];
                        }
                        else
                        {
                            output.Data[i, j, 0] = 0;
                        }
                    }
                    else if (direction[i * input.Cols + j] == 3)
                    {
                        if (input.Data[i, j, 0] > input.Data[i - 1, j - 1, 0]
                            && input.Data[i, j, 0] > input.Data[i + 1, j + 1, 0]
                            && input.Data[i, j, 0] > input.Data[i - 2, j - 2, 0]
                            && input.Data[i, j, 0] > input.Data[i + 2, j + 2, 0])
                        {
                            output.Data[i, j, 0] = input.Data[i, j, 0];
                        }
                        else
                        {
                            output.Data[i, j, 0] = 0;
                        }
                    }
                    else
                    {
                        output.Data[i, j, 0] = 0;
                    }
                }
            }
        }

        void DoubleThresh(Image<Gray, Byte> input, ref Image<Gray, Byte> output, Byte high, Byte low)
        {
            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Cols; j++)
                {
                    if (input.Data[i, j, 0] > high)
                    {
                        output.Data[i, j, 0] = 255;
                    }
                    else if (input.Data[i, j, 0] > low)
                    {
                        output.Data[i, j, 0] = 128;
                    }
                    else
                    {
                        output.Data[i, j, 0] = 0;
                    }
                }
            }
        }

        void CorrelChain(Image<Gray, Byte> input, int i, int j, Byte[] direction, int NumRecur)
        {
            for (int k = -3; k < 4; k++)
            {
                for (int l = -3; l < 4; l++)
                {
                    if (i + k >= 0 && i + k < input.Rows && j + l >= 0 && j + l < input.Cols)
                    {
                        if (input.Data[i + k, j + l, 0] == 128)
                        {
                            input.Data[i + k, j + l, 0] = 255;
                            if (NumRecur < 20)
                                CorrelChain(input, i + k, j + l, direction, NumRecur + 1);
                            else
                                return;
                        }
                    }
                }
            }
        }

        void DeterCorrel(Image<Gray, Byte> input, ref Image<Gray, Byte> output, Byte[] direction)
        {
            output = input;
            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Cols; j++)
                {
                    if (input.Data[i, j, 0] == 255)
                    {
                        CorrelChain(output, i, j, direction, 0);
                    }
                }
            }
        }

        void DelRest(Image<Gray, Byte> input, ref Image<Gray, Byte> output)
        {
            output = input;
            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Cols; j++)
                {
                    if (output.Data[i, j, 0] == 128)
                        output.Data[i, j, 0] = 0;
                }
            }
            textBox_shell.Text = "Canny Edge finished.";
        }

        // ## 샤프닝 하기
        public static Bitmap sharpen(Bitmap image)
        {
            Bitmap sharpenImage = new Bitmap(image.Width, image.Height);

            int filterWidth = 3;
            int filterHeight = 3;
            int w = image.Width;
            int h = image.Height;

            double[,] filter = new double[filterWidth, filterHeight];

            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;

            double factor = 1.0;
            double bias = 0.0;

            Color[,] result = new Color[image.Width, image.Height];

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    //=====[REMOVE LINES]========================================================
                    // Color must be read per filter entry, not per image pixel.
                    Color imageColor = image.GetPixel(x, y);
                    //===========================================================================

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + w) % w;
                            int imageY = (y - filterHeight / 2 + filterY + h) % h;

                            //=====[INSERT LINES]========================================================
                            // Get the color here - once per fiter entry and image pixel.
                            Color imageColor2 = image.GetPixel(imageX, imageY);
                            //===========================================================================

                            red += imageColor2.R * filter[filterX, filterY];
                            green += imageColor2.G * filter[filterX, filterY];
                            blue += imageColor2.B * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    sharpenImage.SetPixel(i, j, result[i, j]);
                }
            }
            return sharpenImage;
        }


        private void button_canny_Click(object sender, EventArgs e)
        {
            textBox_shell.Text = "Detecting Canny Edge...";
            textBox_shell.Refresh();

            if (listView_imgs.FocusedItem == null)
                targetIdx = 0;
            else
                targetIdx = listView_imgs.FocusedItem.Index;

            int[] dx_table = new int[original_img[targetIdx].Rows * original_img[targetIdx].Cols];
            int[] dy_table = new int[original_img[targetIdx].Rows * original_img[targetIdx].Cols];
            byte[] direction = new byte[original_img[targetIdx].Rows * original_img[targetIdx].Cols];
            
            // ## 이미지 샤프닝
            //Bitmap sharp = new Bitmap(sharpen(original_img[targetIdx].ToBitmap()));
            //Canny_img = new Image<Gray, byte>(sharp);
            // ## 샤프닝 하지 않음
            Canny_img = new Image<Gray, byte>(original_img[targetIdx].ToBitmap());

            SobelDer(Canny_img, ref Canny_img, dx_table, dy_table);
            CalDirec(direction, dx_table, dy_table, Canny_img.Rows, Canny_img.Cols);
            LocalMax(Canny_img, ref Canny_img, direction);
            DoubleThresh(Canny_img, ref Canny_img, 100, 50);
            DeterCorrel(Canny_img, ref Canny_img, direction);
            DelRest(Canny_img, ref Canny_img);

            string FileNameOnly = Path.GetFileName(ofd.FileNames[targetIdx].ToString());
            Canny_img.Save(di + @"\canny_" + FileNameOnly);
            Canny2_img = new Image<Gray, byte>(Canny_img.Data);

            // ## openCV제공 케니
            Image<Gray, Byte> cvCanny = new Image<Gray, byte>(original_img[targetIdx].ToBitmap());
            cvCanny = cvCanny.Canny(200, 100);
            cvCanny.Save(di + @"\cvCanny_" + FileNameOnly);

            Image<Gray, Byte> binary = new Image<Gray, byte>(original_img[targetIdx].Width, original_img[targetIdx].Height);
            CtoG(original_img[targetIdx], ref binary);
            
            // ## 그레이 이미지를 보존해야함..
            Image<Gray, Byte> grayimg = new Image<Gray, byte>(binary.Data);

            int[] histogram = new int[256];
            histogram = gethistogram(binary);

            int threshold = getThreshHold(histogram, binary);

            binarize(binary, threshold);

            binary_img = binary;
            binary_img.Save(di + @"\binary_" + FileNameOnly);
            
            // ## 2차 스레시홀드값을 적용한 3진 이미지를 만든다. 값은 0, 127, 255
            // ## 1. 픽셀값이 0인 바이너리 픽셀에 대응하는 그레이 이미지의 히스토그램을 얻는다.
            for (int a = 0; a < 256; a++)
                histogram[a] = 0;
            histogram = gethistogram(grayimg, binary, 0);

            // ## 2. 얻은 히스토그램을 이용하여 새로운 스레시홀드값을 얻는다.
            threshold = getThreshHold(histogram, binary, 0);

            // ## 3. 얻은 스레시홀드값을 이용하여 이진 이미지의 검은 부분을 세분화한다.
            for(int a=0; a<binary.Height; a++)
            {
                for(int b=0; b<binary.Width; b++)
                {
                    if (binary.Data[a, b, 0] == 0 && grayimg.Data[a, b, 0] >= threshold)
                        binary.Data[a, b, 0] = 127;
                }
            }

            // ## 4. 이미지를 저장한다.
            binary_img.Save(di + @"\binary2_" + FileNameOnly);
            
            Image<Gray, Byte> tmp = new Image<Gray, byte>(binary.Data);
            int[] x_table = new int[tmp.Rows * tmp.Cols];
            int[] y_table = new int[tmp.Rows * tmp.Cols];
            byte[] dir = new byte[tmp.Rows * tmp.Cols];

            biCanny_img = tmp;
            SobelDer(biCanny_img, ref biCanny_img, x_table, y_table);
            // 미디언 필터링 추가
            //median(Canny_img[i]);
            CalDirec(dir, x_table, y_table, biCanny_img.Rows, biCanny_img.Cols);
            LocalMax(biCanny_img, ref biCanny_img, dir);
            DoubleThresh(biCanny_img, ref biCanny_img, 100, 50);
            DeterCorrel(biCanny_img, ref biCanny_img, dir);
            DelRest(biCanny_img, ref biCanny_img);

            for (int a = 0; a < Canny_img.Height; a++)
            {
                for (int b = 0; b < Canny_img.Width; b++)
                {
                    if (Canny_img.Data[a, b, 0] == 0 && biCanny_img.Data[a, b, 0] != 0)
                    {
                        Canny_img.Data[a, b, 0] = biCanny_img.Data[a, b, 0];
                    }
                }
            }

            // ## 케니 3배수
            tripleCanny = new Image<Gray, byte>(Canny_img.Width, Canny_img.Height);
            for (int a = 0; a < Canny_img.Height; a++)
            {
                for (int b = 0; b < Canny_img.Width; b++)
                {
                    if (Canny_img.Data[a, b, 0] == 255)
                    {
                        for(int x = -1; x<2; x++)
                        {
                            for(int y = -1; y<2; y++)
                            {
                                int row = a + x, col = b + y;
                                if(row < 0 || col < 0 || row > Canny_img.Height-1 || col > Canny_img.Width - 1) { } // 배열을 초과한 범위에 대해서는 아무것도 하지 않는다.
                                else
                                {
                                    tripleCanny.Data[row, col, 0] = 255;
                                }
                            }
                        }
                    }
                }
            }
            tripleCanny.Save(di + @"\tripled_" + FileNameOnly);


            for (int y = 0; y < Canny_img.Rows; y++)
            {
                for (int x = 0; x < Canny_img.Cols; x++)
                {
                    if (Canny_img.Data[y, x, 0] == 255)
                    {
                        merged_img.Data[y, x, 0] = 50;
                        merged_img.Data[y, x, 1] = 130;
                        merged_img.Data[y, x, 2] = 250;
                    }
                }
            }
            Canny_img.Save(di + @"\CGcanny_" + FileNameOnly);

            for (int y = 0; y < Canny_img.Rows; y++)
            {
                for (int x = 0; x < Canny_img.Cols; x++)
                {
                    if (Canny_img.Data[y, x, 0] == 255)
                    {
                        merged_img.Data[y, x, 0] = 50;
                        merged_img.Data[y, x, 1] = 130;
                        merged_img.Data[y, x, 2] = 250;
                    }
                }
            }
        }
        // Canny Edge Detection end


        // show Type change buttons (w.by song)
        private void button_showOrigin_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 0;
            changeShowType();
        }

        private void button_showEdge_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 1;
            changeShowType();
        }

        private void button_showMerged_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 2;
            changeShowType();
        }

        private void button_showDepth_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 3;
            changeShowType();
        }

        private void button_showDenser_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 4;
            changeShowType();
        }

        private void button_Object_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 5;
            changeShowType();
        }

        private void button_showFeature_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 6;
            changeShowType();
        }

        private void button_labeled_Click(object sender, EventArgs e)
        {
            curShowImgTypeIdx = 7;
            changeShowType();
        }

        private void App_Click(object sender, EventArgs e) //@@ App
        {
            curShowImgTypeIdx = 8;
            changeShowType();
        }

        // union structure for read little endian float in binary file (w.by song)
        [StructLayout(LayoutKind.Explicit)]
        public struct UnionFloat
        {
            [FieldOffset(0)]
            public float f;
            
            [FieldOffset(0)]
            public byte b0;
            [FieldOffset(1)]
            public byte b1;
            [FieldOffset(2)]
            public byte b2;
            [FieldOffset(3)]
            public byte b3;
        }
        
        private void button_importPC_Click(object sender, EventArgs e)
        {
            Import_PointCloud();
            Generate_Depthmap();
        }

        // read binary ply file (w.by Song)
        private void Import_PointCloud()
        {
            textBox_shell.Text = "Importing Point Cloud...";
            textBox_shell.Refresh();
            int pos = 0;
            int length = 0;
            bool isBin = false;
            BinaryReader plyReader = new BinaryReader(File.Open("result.0.ply", FileMode.Open));
            string stringLine = ""; // 문자로 된 부분의 한 라인
            int curIdxUnit = 0; // binary들을 의미 단위로 나눈 인덱스 (현재 읽고있는 단위)

            ply_pc tempPC = new ply_pc();
            UnionFloat tempUnion = new UnionFloat();
            length = (int)plyReader.BaseStream.Length;
            while (pos < length)
            {
                // string area reading
                if (!isBin)
                {
                    char c = plyReader.ReadChar();
                    if (c == '\n')
                    {
                        if (stringLine == "end_header")
                        {
                            isBin = true;
                        }
                        stringLine = "";
                    }
                    else
                    {
                        stringLine += c;
                    }
                    pos += sizeof(char);
                }
                // binary area reading
                else
                {
                    switch (curIdxUnit % 7) // 라인에서 몇번째인지
                    {
                        // float 부분 (x,y,z)
                        case 0:
                        case 1:
                        case 2:
                            byte[] tempB = new byte[4];
                            tempB = plyReader.ReadBytes(4);
                            tempUnion.f = 0;
                            tempUnion.b3 = tempB[3];
                            tempUnion.b2 = tempB[2];
                            tempUnion.b1 = tempB[1];
                            tempUnion.b0 = tempB[0];

                            if (curIdxUnit % 7 == 0)
                            {
                                tempPC.x = tempUnion.f;
                            }
                            else if (curIdxUnit % 7 == 1)
                            {
                                tempPC.y = tempUnion.f;
                            }
                            else if (curIdxUnit % 7 == 2)
                            {
                                tempPC.z = tempUnion.f;
                            }

                            pos += 4 * sizeof(byte);
                            curIdxUnit++;
                            break;
                        // 무시되는 부분
                        case 3:
                        case 4:
                        case 5:
                            for (int i = 0; i < 4; i++)
                                plyReader.ReadByte();
                            pos += 4 * sizeof(byte);
                            curIdxUnit++;
                            break;
                        // byte 부분
                        case 6:
                            const int nRLoop = 7;
                            tempPC.r = plyReader.ReadByte();
                            tempPC.g = plyReader.ReadByte();
                            tempPC.b = plyReader.ReadByte();
                            tempPC.alpha = 255;
                            for (int i = 0; i < nRLoop - 3; i++)
                                plyReader.ReadByte();
                            pos += nRLoop * sizeof(byte);
                            curIdxUnit++;

                            point3f.Add(tempPC); // add point info to point struct
                            break;
                        default:
                            textBox_shell.Text = "ERROR 108";
                            textBox_shell.Refresh();
                            break;
                    }
                }
            }
            plyReader.Close();
            textBox_shell.Text = "Point Cloud Importing finished.";
        }

        // generate depthmap by RT infomation and Point cloud (w.by Song)
        private void Generate_Depthmap()
        {
            textBox_shell.Text = "Generating Depth map...";
            textBox_shell.Refresh();
            for (int n=0; n<nImg; n++)
            {
                Image<Gray, byte> tempImg = new Image<Gray, byte>(original_img[n].Width, original_img[n].Height);
                
                // quternion 획득
                double x = -SortedNVM[n].x;
                double y = -SortedNVM[n].y;
                double z = -SortedNVM[n].z;
                double w = -SortedNVM[n].w;

                double[,] m_Rot = new double[3, 3]
                {
                    { 1 - 2 * Math.Pow(y, 2) - 2 * Math.Pow(z, 2),2 * x * y - 2 * z * w,2 * x * z + 2 * y * w}
                   ,{2 * x * y + 2 * z * w,1 - 2 * Math.Pow(x, 2) - 2 * Math.Pow(z, 2),2 * y * z - 2 * x * w}
                   ,{2 * x * z - 2 * y * w, 2 * y * z + 2 * x * w,1 - 2 * Math.Pow(x, 2) - 2 * Math.Pow(y, 2)}
                };
                //MatInverse3x3(m_Rot);
                double[,] m_Transform = new double[4, 4]
                {
                    {m_Rot[0,0],m_Rot[0,1],m_Rot[0,2],0/*nvm[n].cCentX*/ }
                    ,{m_Rot[1,0],m_Rot[1,1],m_Rot[1,2],0/* nvm[n].cCentY*/ }
                    ,{m_Rot[2,0], m_Rot[2,1],m_Rot[2,2],0/*nvm[n].cCentZ*/ }
                    ,{0, 0, 0, 1 }
                };

                // intrinsic parameter
                // perspective projection matrix
                double f = SortedNVM[n].focalLength/4;
                double[,] m_Per = new double[3, 4]
                {
                    {f, 0, 0, 0 }
                    , {0, f, 0, 0 }
                    , {0, 0, 1, 0 }
                };
               
                // rotate & translate each points
                double[,] vo = new double[4, 1];
                for (int i=0; i<point3f.Count; i++)
                {
                    //원본 point
                    vo[0, 0] = point3f[i].x - nvm[n].cCentX;
                    vo[1, 0] = point3f[i].y - nvm[n].cCentY;
                    vo[2, 0] = point3f[i].z - nvm[n].cCentZ;
                    vo[3, 0] = 1;
                    
                    double[,] vr = new double[3,1];
                    double[,] tranv = new double[4, 1];
                    double[,] rotv = new double[4,1];

                    
                    // rotations & translation
                    rotv = MatMult2d(m_Transform, vo, 4, 4, 1);
                    // perspective projection
                    vr = MatMult2d( m_Per, rotv, 3, 4, 1);

                    // image plane
                    int u = (int)vr[1, 0];
                    int v = (int)vr[0, 0];

                    // shift
                    //u += depth_img[n].Rows / 2;
                    //v += depth_img[n].Cols / 2;

                    // shift 된 좌표상 인덱스 벗어나는 경우 예외처리
                    if (u < 0)
                        u = 0;
                    if (v < 0)
                        v = 0;
                    u = (u < tempImg[n].Rows - 1 ? u : tempImg[n].Rows - 1);
                    v = (v < tempImg[n].Cols - 1 ? v : tempImg[n].Cols - 1);

                    /// get gray image (for test)
                    tempImg[n].Data[u, v, 0] = (byte)((point3f[i].r + point3f[i].g + point3f[i].b)/3);

                    /// get depth
                    //tempImg[n].Data[u, v, 0] = (byte)(15*(15.0-rotv[2, 0]));

                    depth_img.Add(tempImg);
                }

                // camera loc for test
                //for (int c = 0; c < nImg; c++)
                //{
                //    double[,] vr = new double[3, 1];
                //    double[,] tranv = new double[4, 1];
                //    double[,] rotv = new double[4, 1];
                //    double[,] vc = new double[4, 1]
                //       {
                //            {nvm[c].cCentX}
                //            ,{nvm[c].cCentY }
                //            ,{nvm[c].cCentZ }
                //            ,{0 }
                //       };
                //    // rotations & translation
                //    rotv = MatMult2d(m_Rot, vc, 4, 4, 1);
                //    // perspective projection
                //    vr = MatMult2d(m_Per, rotv, 3, 4, 1);

                //    // image plane
                //    int u = (int)vr[1, 0];
                //    int v = (int)vr[0, 0];

                //    if (u > 3 && u < tempImg[c].Rows - 3 && v > 3 && v < tempImg[c].Cols - 3)
                //    {
                //        tempImg[n].Data[u, v, 0] = 255;
                //        tempImg[n].Data[u - 1, v, 0] = 255;
                //        tempImg[n].Data[u + 1, v, 0] = 255;
                //        tempImg[n].Data[u - 2, v, 0] = 255;
                //        tempImg[n].Data[u + 2, v, 0] = 255;
                //        tempImg[n].Data[u, v - 1, 0] = 255;
                //        tempImg[n].Data[u, v + 1, 0] = 255;
                //        tempImg[n].Data[u, v - 2, 0] = 255;
                //        tempImg[n].Data[u, v + 2, 0] = 255;
                //    }
                //}
            }

            textBox_shell.Text = "Depth map generated.";
            textBox_shell.Refresh();
        }

        private void MatInverse3x3(double[,] mat)
        {
            //double[,] r = new double[3, 3];

            double a = mat[0, 0];
            double b = mat[0, 1];
            double c = mat[0, 2];
            double d = mat[1, 0];
            double e = mat[1, 1];
            double f = mat[1, 2];
            double g = mat[2, 0];
            double h = mat[2, 1];
            double i = mat[2, 2];

            double A = (e * i - f * h);
            double B = -(d * i - f * g);
            double C = (d * h - e * g);
            double D = -(b * i - c * h);
            double E = (a * i - c * g);
            double F = -(a * h - b * g);
            double G = (b * f - c * e);
            double H = -(a * f - c * d);
            double I = (a * e - b * d);

            double detAi = 1.0 / ( a * A + b * B + c * C);

            mat[0, 0] = detAi * A;
            mat[0, 1] = detAi * D;
            mat[0, 2] = detAi * G;
            mat[1, 0] = detAi * B;
            mat[1, 1] = detAi * E;
            mat[1, 2] = detAi * H;
            mat[2, 0] = detAi * C;
            mat[2, 1] = detAi * F;
            mat[2, 2] = detAi * I;

            //return r;
        }

        // 2d 4x4 Matrix inverse function a to r
        //private double[,] MatInverse4x4(double[,] a)
        //{
        //    double detA = a[0, 0] * a[1, 1] * a[2, 2] * a[3, 3] + a[0, 0] * a[1, 2] * a[2, 3] * a[3,1] + a[0, 0] * a[1, 3] * a[2, 1] * a[3,2]
        //                + a[0, 1] * a[1, 0] * a[2, 3] * a[3, 2] + a[0, 1] * a[1, 2] * a[2, 0] * a[3,3] + a[0, 1] * a[1, 3] * a[2, 2] * a[3,0]
        //                + a[0, 2] * a[1, 0] * a[2, 1] * a[3, 3] + a[0, 2] * a[1, 1] * a[2, 3] * a[3,0] + a[0, 2] * a[1, 3] * a[2, 0] * a[3,1]
        //                + a[0, 3] * a[1, 0] * a[2, 2] * a[3, 1] + a[0, 3] * a[1,1 ] * a[2, 0] * a[3,2] + a[0, 3] * a[1, 2] * a[2, 1] * a[3,0]
        //                - a[0, 0] * a[1, 1] * a[2, 3] * a[3, 2] - a[0, 0] * a[1, 2] * a[2, 1] * a[3, 3] - a[0, 0] * a[1, 3] * a[2, 2] * a[3, 1]
        //                - a[0, 1] * a[1, 0] * a[2, 2] * a[3, 3] - a[0, 1] * a[1, 2] * a[2, 3] * a[3, 0] - a[0, 1] * a[1, 3] * a[2, 0] * a[3, 2]
        //                - a[0, 2] * a[1, 0] * a[2, 3] * a[3, 1] - a[0, 2] * a[1, 1] * a[2, 0] * a[3, 3] - a[0, 2] * a[1, 3] * a[2, 1] * a[3, 0]
        //                - a[0, 3] * a[1, 0] * a[2, 1] * a[3, 2] - a[0, 3] * a[1, 1] * a[2, 2] * a[3, 0] - a[0, 3] * a[1, 2] * a[2, 0] * a[3, 1];

        //    const int n = 4;
        //    double[,] r = new double[n,n];
        //    r = MatSum(MatDif(MatMultNumeric(1.0 / 6, MatSum(MatPow((tr(a)), 3), 
        //        MatMultNumeric(3, MatMult2d(tr(a), MatPow(a, 2), 4, 4, 4)), 
        //        MatMultNumeric(2, tr(MatPow(a, 3))))), MatMultNumeric(0.5, 
        //        MatMult2d(a, MatDif(MatPow(tr(a), 2), tr(MatPow(a, 2))), 4, 4, 4))),
        //        MatDif(MatMult2d(MatPow(a, 2), tr(a), 4, 4, 4), MatPow(a, 3)));
        //    r = MatMultNumeric(1 / detA, r);
        //    return r;
        //}

        static double[,] MatSum(double[,] a, double[,] b, int n, int m)
        {
            double[,] r = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    r[i, j] = a[i,j] + b[i,j];
                }
            }
            return r;
        }

        static double[,] MatSum(double[,] a, double[,] b, double[,] c,int n, int m)
        {
            double[,] r = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    r[i, j] = a[i, j] + b[i, j] + c[i,j];
                }
            }
            return r;
        }

        private double[,] MatDif(double[,] a, double[,] b, int n, int m)
        {
            double[,] r = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    r[i, j] = a[i, j] - b[i, j];
                }
            }
            return r;
        }

        private double[,] MatMultNumeric(double b, double[,] a, int n, int m)
        {
            double[,] r = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    r[i, j] = a[i, j] * b;
                }
            }
            return r;
        }

        // 2d Matrix transparent
        private double[,] tr(double[,] a, int n)
        {
            double[,] r = new double[n,n];
            for (int i=0; i<n; i++)
            {
                for(int j=0; j<n; j++)
                {
                    r[i, j] = a[j, i];
                }
            }
            return r;
        }

        // power matrix 4x4
        private double[,] MatPow(double[,] a, int k)
        {
            const int n = 4;
            double[,] r = new double[n,n];
            if (k<2)
            {
                return a;
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    r[i, j] = a[i, j];
                }
            }
            for (int i = 0; i < k - 1; i++)
            {
                r = MatMult2d( r, a, n, n, n);
            }
            return r;
        }

        // 2d Matrix muliply funcion r = a*b (nxm)*(mxl)
        private double[,] MatMult2d(double[,] a, double[,] b, int n, int m, int l)
        {
            double[,] r = new double[n, m];

            // a row loop
            for (int i=0; i<n; i++)
            {
                // b col loop
                for(int j=0; j<l;j++)
                {
                    // m loop
                    r[i,j] = 0;
                    for (int k=0;k<m;k++)
                    {
                        r[i,j] += a[i,k] * b[k,j];
                    }
                }
            }
            return r;
        }

        public struct RefType
        {
            public int i;
            public int j;
            public byte d;
            public double dist;

            public RefType(int row, int col, byte depth, double distance) : this()
            {
                this.i = row;
                this.j = col;
                this.d = depth;
                this.dist = distance;
            }
        };

        private void button_denser_Click(object sender, EventArgs e)
        {
            textBox_shell.Text = "Depth map denser processing... \n";
            textBox_shell.Refresh();

            denser_img = new Image<Gray, Byte>(depth_img.Cols, depth_img.Rows);

            // 존재하는 depth 값에 대한 reference table 생성
            List<RefType> RefTable = new List<RefType>();
            for (int i = 0; i < depth_img.Rows; i++)
            {
                for (int j = 0; j < depth_img.Cols; j++)
                {
                    if (depth_img.Data[i, j, 0] > 0)
                    {
                        RefTable.Add(new RefType(i, j, depth_img.Data[i, j, 0], 0));
                    }
                }
            }

            for (int i = 0; i < depth_img.Rows; i++)
            {
                textBox_shell.Text = "Depth map denser processing... \n" + i.ToString() + " / " + depth_img.Rows.ToString();
                textBox_shell.Refresh();
                for (int j = 0; j < depth_img.Cols; j++)
                {
                    // 거리순으로 정렬된 각 원소까지의 거리 데이터 추출
                    double[] dist = new double[RefTable.Count];
                    List<RefType> SortingRefTable = new List<RefType>();
                    List<RefType> SortedRefTable = new List<RefType>();
                    for (int n = 0; n < RefTable.Count; n++)
                    {
                        // 해당 원소와의 거리
                        dist[n] = Math.Sqrt(Math.Pow(RefTable[n].i - i, 2) + Math.Pow(RefTable[n].j - j, 2));
                        SortingRefTable.Add(new RefType(RefTable[n].i, RefTable[n].j, RefTable[n].d, dist[n]));
                    }
                    // (정렬)
                    SortedRefTable = SortingRefTable.OrderBy(o => o.dist).ToList();


                    // 거리에 따른 가중치로 가까운 픽셀 값의 합으로 값 결정
                    double value = 0;
                    double euler = 2.7182818284590452353602874713527;
                    double c = 10;
                    double a = 1 / (c * Math.Sqrt(2 * 3.14));

                    double avg = 0;
                    int numLoopAvg = SortedRefTable.Count / 15; // 가장 까까운 일부 키포인트에 대해 avg구함 (1로 나누면 전체 avg)
                    for (int n = 0; n < numLoopAvg; n++)
                    {
                        avg += SortedRefTable[n].d;
                    }
                    avg /= numLoopAvg;

                    for (int n = 0; n < SortedRefTable.Count; n++)
                    {
                        // 2 dimensional gaussian function으로 가중치 계산
                        double w = a * Math.Pow(euler, -((Math.Pow(i - SortedRefTable[n].i, 2) / (2 * Math.Pow(c, 2))) + Math.Pow(j - SortedRefTable[n].j, 2) / (2 * Math.Pow(c, 2))));
                        //double w = (1 / SortedRefTable[n].dist); // 단순 가중치
                        value += (w * SortedRefTable[n].d);
                    }
                    value *= 5; // 값 노멀라이즈

                    // 255 넘는 값 보정
                    denser_img.Data[i, j, 0] = (byte)(value < avg ? value : avg);
                }
            }

            denser_img.Save(di + @"\denser.jpg");

            textBox_shell.Text = "denser depth generated.";
            textBox_shell.Refresh();
        }
        
        public struct Grid
        {
            public List<RefType> vertex;
            public int numVtx;
        };

        // depth map denser func (w.by Song)
        private void Depth_Denser(int n)
        {
            // 그리드 생성을 위한 레퍼런스 이미지
            Image<Gray, Byte> ref_img = depth_img[n].Clone();
            denser_img.Add(depth_img[n].Clone());

            // grid number setting
            const int GUnit = 8;
            int GRow = ref_img.Rows / GUnit;
            if ((ref_img.Rows % GUnit)!=0)
                GRow++;
            int GCol = ref_img.Cols / GUnit + 1;
            if ((ref_img.Cols % GUnit) != 0)
                GCol++;

            // 그리드 내 원소를 저장할 레퍼런스 테이블
            Grid[] RefTable = new Grid[GRow*GCol];
            for(int i=0;i<GRow;i++)
            {
                for(int j=0; j<GCol; j++)
                {
                    RefTable[i * GCol + j].numVtx = 0;
                    RefTable[i * GCol + j].vertex = new List<RefType>();
                }
            }
            
            // RefTable에 저장
            for (int i=0;i<ref_img.Rows; i++)
            {
                for(int j=0; j<ref_img.Cols; j++)
                {
                    if (ref_img.Data[i, j, 0] > 0)
                    {
                        int r = i / GUnit;
                        int c = j / GUnit;

                        RefType tVtx;
                        tVtx.i = r;
                        tVtx.j = c;
                        tVtx.d = ref_img.Data[i, j, 0];
                        tVtx.dist = 0;
                        RefTable[r * GCol + c].vertex.Add(tVtx);
                        RefTable[r * GCol + c].numVtx++;
                    }
                }
            }

            // denser
            for (int i = 0; i < depth_img[n].Rows; i++)
            {
                for (int j = 0; j < depth_img[n].Cols; j++)
                {
                    if (depth_img[n].Data[i, j, 0] == 0)
                    {
                        int r = i / GUnit;
                        int c = j / GUnit;

                        double toDist = 0; // 점들간의 거리 합
                        double[] dist = new double[RefTable[r * GCol + c].numVtx]; // 각 점들간의 거리
                        // get distance
                        for (int k = 0; k < RefTable[r * GCol + c].numVtx; k++)
                        {
                            dist[k] = Math.Sqrt(Math.Pow(RefTable[r * GCol + c].vertex[k].i - i, 2)
                                + Math.Pow(RefTable[r * GCol + c].vertex[k].j - j, 2));
                            toDist += dist[k];
                        }
                        // normalize
                        double tempSum = 0;
                        for (int k = 0; k < RefTable[r * GCol + c].numVtx; k++)
                        {
                            dist[k] = 1.0 / (dist[k] * toDist);
                            tempSum += dist[k];
                        }
                        for (int k = 0; k < RefTable[r * GCol + c].numVtx; k++)
                        {
                            dist[k] /= tempSum;
                        }
                        // get weighted depth average
                        for (int k = 0; k < RefTable[r * GCol + c].numVtx; k++)
                        {
                            if((dist[k]) < 4) 
                                denser_img[n].Data[i, j, 0] += (byte)(RefTable[r * GCol + c].vertex[k].d * (dist[k]));
                        }
                    }
                }
            }
        }

        // generating depth map by SURF match points (w.by song)
        private void button_Stereo_Click(object sender, EventArgs e)
        {
            textBox_shell.Text = "Stereo...";
            textBox_shell.Refresh();

            // target index changed
            if (listView_imgs.FocusedItem == null)
                targetIdx = 0;
            else
                targetIdx = listView_imgs.FocusedItem.Index;

            // parameters for SURF feature detect
            long time = 0;
            VectorOfKeyPoint mKeyPts;
            VectorOfKeyPoint oKeyPts;
            Mat FeatureMap;
            Mat Homography;

            Image<Gray, byte> DisparityMap = new Image<Gray, byte>(original_img[targetIdx].Cols, original_img[targetIdx].Rows);

            for (int n = 0; n < nImg; n++)
            {
                textBox_shell.Text = "Stereo..." + targetIdx.ToString() + "  " + n.ToString();
                textBox_shell.Refresh();

                double normalK = 255 / (Math.Sqrt(Math.Pow((original_img[targetIdx]).Cols, 2) + Math.Pow((original_img[targetIdx].Rows), 2)));

                // 피쳐 디텍션
                MDMatch[][] matches = SURFFeature.DrawMatches.Detect(original_img[targetIdx].Mat, original_img[n].Mat, out time, out mKeyPts, out oKeyPts, out FeatureMap, out Homography);
                Image<Bgr, Byte> TFM = FeatureMap.ToImage<Bgr, Byte>();
                feature_img.Add(TFM);

                for (int i = 0; i < mKeyPts.Size; i++)
                {
                    double Distance = 0;

                    // keypoints of model image
                    PointF ptm = mKeyPts[i].Point;
                    // keypoint of matched obs image
                    PointF ptResult = new PointF();

                    // 비교 window 추출
                    byte[] window = new byte[9];
                    for (int k = -1; k < 2; k++)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            window[(k + 1) * 3 + (l + 1)] = original_img[targetIdx].Data[(int)ptm.Y + k, (int)ptm.X + l, 0];
                        }
                    }
                    int minDif = 255 * 9;
                    for (int j = 0; j < oKeyPts.Size; j++)
                    {
                        // keypoints of obs image
                        PointF pto = oKeyPts[j].Point;

                        // 가장자리 예외처리
                        if (ptm.X < 1 || ptm.X > original_img[targetIdx].Cols || ptm.Y < 1 || ptm.Y > original_img[targetIdx].Rows
                            || pto.X < 1 || pto.X > original_img[n].Cols || pto.Y < 1 || pto.Y > original_img[n].Rows)
                        {
                            break;
                        }

                        byte[] kernal = new byte[9];
                        for (int k = -1; k < 2; k++)
                        {
                            for (int l = -1; l < 2; l++)
                            {
                                kernal[(k + 1) * 3 + (l + 1)] = original_img[n].Data[(int)pto.Y + k, (int)pto.X + l, 0];
                            }
                        }
                        int resultSSD = SSD(window, kernal);
                        if (minDif > resultSSD)
                        {
                            minDif = resultSSD;

                            ptResult.X = pto.X;
                            ptResult.Y = pto.Y;
                        }
                    }
                    if (minDif < 360) // SSD 값이 너무 높으면 잘못된 매칭으로 인식
                    {
                        // model image의 keypoint를 homography로 transform
                        PointF[] pth = new PointF[]
                        {
                                new PointF(ptResult.X,ptResult.Y)
                        };
                        pth = CvInvoke.PerspectiveTransform(pth, Homography);

                        Distance = Math.Sqrt(Math.Pow(pth[0].Y - ptm.Y, 2) + Math.Pow(pth[0].X - ptm.X, 2));
                        DisparityMap.Data[(int)ptm.Y, (int)ptm.X, 0] = (byte)(Distance * normalK);
                        
                    }
                    else
                    {
                        break;
                    }
                }
            }
            depth_img = DisparityMap;

            textBox_shell.Text = "Stereo End.";
            textBox_shell.Refresh();
        }

        // detecting LMP feature map (w.by song)
        private void detectLMP()
        {
            if (nImg >= 2)
            {
                // get feature map
                for (int i = 0; i < nImg; i++)
                {
                    Image<Bgr, Byte> temp_lbp = new Image<Bgr, Byte>(original_img[i].Rows, original_img[i].Cols);
                    temp_lbp = LBP_pixel(i);
                    feature_img.Add(temp_lbp);
                }

                for (int n = 0; n < nImg - 1; n++)
                {
                    // n 이미지 중심 픽셀의 n+1의 매칭점과의 approximation direction vector

                    int[] dv = new int[2];
                    byte[] window = new byte[9];
                    for (int k = -1; k < 2; k++)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            window[(k + 1) * 3 + (l + 1)] = original_img[n].Data[feature_img[n].Rows/2 + k, feature_img[n].Cols/2 + l, 0];
                        }
                    }

                    int minDif = 255 * 9;
                    for (int i= feature_img[n].Rows*2/5; i< feature_img[n].Rows*3 / 5; i++)
                    {
                        for (int j = feature_img[n].Cols * 2 / 5; j < feature_img[n].Cols * 3 / 5; j++)
                        {
                            byte[] kernal = new byte[9];
                            for (int k = -1; k < 2; k++)
                            {
                                for (int l = -1; l < 2; l++)
                                {
                                    kernal[(k + 1) * 3 + (k + 1)] = original_img[n+1].Data[i + k, j + l, 0];
                                }
                            }
                            int resultSSD = SSD(window, kernal);
                            if (minDif > resultSSD)
                            {
                                minDif = resultSSD;
                                dv[0] = j - feature_img[n].Cols / 2;
                                dv[1] = i - feature_img[n].Rows / 2;
                            }
                        }
                    }

                    // 구한 dv 기준으로 전체 매칭
                    MessageBox.Show(dv[0].ToString() + " " + dv[1].ToString());
                }
            }
            else
            {
                textBox_shell.Text = "! This Function need more than 2 images.";
            }
        }

        // Extract LBP(Local Binary Pattern) feature (w.by song)
        private Image<Bgr, Byte> LBP_pixel(int n)
        {
            Image<Bgr, Byte> r = new Image<Bgr, Byte>(original_img[n].Cols,original_img[n].Rows);
            for(int i=1; i<r.Rows-1;i++)
            {
                for(int j=1; j<r.Cols-1; j++)
                {
                    // get LBP each channel
                    for (int c = 0; c < 3; c++)
                    {
                        string PBits = "";
                        for (int k = -1; k < 2; k++)
                        {
                            for (int l = -1; l < 2; l++)
                            {
                                if (l == 0 && k == 0)
                                    break;
                                // 더 크면 1 작으면 0
                                if (original_img[n].Data[i + k, j + l, c] > original_img[n].Data[i, j , c])
                                {
                                    PBits += "1";
                                }
                                else
                                {
                                    PBits += "0";
                                }
                            }
                        }

                        r.Data[i,j,c] = Convert.ToByte(PBits, 2);
                    }
                }
            }

            return r;
        }

        // Sum of Square Difference
        private int SSD(byte[] a, byte[] b)
        {
            int dif = 0;
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<3; j++)
                {
                    dif += Math.Abs(a[i * 3 + j] - b[i * 3 + j]);
                }
            }
            return dif;
        }

        // 픽셀값이 0이고 라벨값이 0인 픽셀을 찾는다. (w. by jang)
        private Point searchPixel(Image<Gray, Byte> img, ref Point pnt, Image<Gray, Byte> labeled)
        {
            Point tmp = new Point(img.Height - 1, img.Width - 1);

            for (int a = pnt.Y; a < img.Width; a++)
            {
                if (img.Data[pnt.X, a, 0] == 0 && labeled.Data[pnt.X, a, 0] == 0)
                {
                    tmp.X = pnt.X;
                    tmp.Y = a;
                    pnt.X = pnt.X;
                    pnt.Y = a;
                    return tmp;
                }
            }

            for (int i = pnt.X + 1; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (img.Data[i, j, 0] == 0 && labeled.Data[i, j, 0] == 0)
                    {
                        tmp.X = i;
                        tmp.Y = j;
                        pnt.X = i;
                        pnt.Y = j;
                        return tmp;
                    }
                }
            }
            return tmp;
        }

        // ## 픽셀값이 0 또는 127이며 라벨값이 0인 픽셀을 찾는다.
        private Point searchPixel2(Image<Gray, Byte> img, ref Point pnt, Image<Gray, Byte> labeled)
        {
            Point tmp = new Point(img.Height - 1, img.Width - 1);

            for (int a = pnt.Y; a < img.Width; a++)
            {
                if (img.Data[pnt.X, a, 0] == 0 && labeled.Data[pnt.X, a, 0] == 0)
                {
                    tmp.X = pnt.X;
                    tmp.Y = a;
                    pnt.X = pnt.X;
                    pnt.Y = a;
                    return tmp;
                }
                else if(img.Data[pnt.X, a, 0] == 127 && labeled.Data[pnt.X, a, 0] == 0)
                {
                    tmp.X = pnt.X;
                    tmp.Y = a;
                    pnt.X = pnt.X;
                    pnt.Y = a;
                    return tmp;
                }
            }

            for (int i = pnt.X + 1; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (img.Data[i, j, 0] == 0 && labeled.Data[i, j, 0] == 0)
                    {
                        tmp.X = i;
                        tmp.Y = j;
                        pnt.X = i;
                        pnt.Y = j;
                        return tmp;
                    }
                    else if (img.Data[i, j, 0] == 127 && labeled.Data[i, j, 0] == 0)
                    {
                        tmp.X = i;
                        tmp.Y = j;
                        pnt.X = i;
                        pnt.Y = j;
                        return tmp;
                    }
                }
            }
            return tmp;
        }

        // 전달받은 픽셀의 라벨값이 0이라면 라벨값을 할당하고 스텍에 넣는다. (w.by jang)
        private void labelcheck(Point pnt, ref Image<Gray, Byte> labeled, Stack<Point> stack, Byte labelnum)
        {
            labeled.Data[pnt.X, pnt.Y, 0] = labelnum;
            stack.Push(pnt);
        }

        // 이웃이 0이고 라벨링 되어 있지 않으면 라벨링하고 스텍에 넣는다. (w.by jang)
        private void neighborSearch(Image<Gray, Byte> img, Point pnt, Stack<Point> stack, ref Image<Gray, Byte> labeled, Byte labelnum)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int x = pnt.X + i, y = pnt.Y + j;
                    if (x < 0 || y < 0 || x > img.Height - 1 || y > img.Width - 1) { } // do nothing
                    else
                    {
                        if (i == 0 && j == 0) { }
                        else
                        {
                            if (img.Data[x, y, 0] == 0 && labeled.Data[x, y, 0] == 0)
                            {
                                Point temp = new Point(x, y);
                                labelcheck(temp, ref labeled, stack, labelnum);
                            }
                        }
                    }
                }
            }
        }

        // ## 4방향 서치를 한다. 이웃이 0이고 라벨링 되어 있지 않으면 라벨링하고 스텍에 넣는다. (w.by jang)
        private void neighborSearch3(Image<Gray, Byte> img, Point pnt, Stack<Point> stack, ref Image<Gray, Byte> labeled, Byte labelnum)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int x = pnt.X + i, y = pnt.Y + j;
                    if (x < 0 || y < 0 || x > img.Height - 1 || y > img.Width - 1) { } // do nothing
                    else
                    {
                        if (i == 0 && j == 0) { }
                        else if(Math.Abs(i + j) == 1)
                        {
                            if (img.Data[x, y, 0] == 0 && labeled.Data[x, y, 0] == 0)
                            {
                                Point temp = new Point(x, y);
                                labelcheck(temp, ref labeled, stack, labelnum);
                            }
                        }
                    }
                }
            }
        }

        // ## 해당 픽셀의 값과 같은 값을 가진 픽셀을 찾아 라벨링하고 스텍에 넣는다.
        private void neighborSearch2(Image<Gray, Byte> img, Point pnt, Stack<Point> stack, ref Image<Gray, Byte> labeled, Byte labelnum)
        {
            byte val = img.Data[pnt.X, pnt.Y, 0];

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int x = pnt.X + i, y = pnt.Y + j;
                    if (x < 0 || y < 0 || x > img.Height - 1 || y > img.Width - 1) { } // do nothing
                    else
                    {
                        if (i == 0 && j == 0) { }
                        else
                        {
                            if (img.Data[x, y, 0] == val && labeled.Data[x, y, 0] == 0)
                            {
                                Point temp = new Point(x, y);
                                labelcheck(temp, ref labeled, stack, labelnum);
                            }
                        }
                    }
                }
            }
        }

        // 컬러 이미지를 그레이 이미지로 변환. (w.by jang)
        private void CtoG(Image<Bgr, Byte> source, ref Image<Gray, Byte> dest)
        {
            Image<Gray, Byte> tempDest = new Image<Gray, Byte>(source.Cols, source.Rows);
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    int gr = (source.Data[i, j, 0] + source.Data[i, j, 1] + source.Data[i, j, 2]) / 3;
                    tempDest.Data[i, j, 0] = (byte)gr;
                }
            }
            dest = tempDest;
        }

        // 그레이 이미지의 히스토그램을 얻는다. (w.by jang)
        private int[] gethistogram(Image<Gray, Byte> binary)
        {
            int[] histogram = new int[256];
            for (int i = 0; i < 256; i++)
            {
                histogram[i] = 0;
            }

            for (int i = 0; i < binary.Height; i++)
            {
                for (int j = 0; j < binary.Width; j++)
                {
                    histogram[binary.Data[i, j, 0]]++;
                }
            }
            return histogram;
        }

        // 타겟 픽셀값을 가지고 있는 이진 이미지의 픽셀과 대응하는 그레이 이미지의 픽셀에 대하여 히스토그램을 얻는다.
        private int[] gethistogram(Image<Gray, Byte> gray, Image<Gray, Byte> binary, byte targetVal)
        {
            int[] histogram = new int[256];
            for (int i = 0; i < 256; i++)
            {
                histogram[i] = 0;
            }

            for (int i = 0; i < gray.Height; i++)
            {
                for (int j = 0; j < gray.Width; j++)
                {
                    if(binary.Data[i, j, 0] == targetVal)
                        histogram[gray.Data[i, j, 0]]++;
                }
            }
            return histogram;
        }

        // 오츠 알고리즘을 통해 이진화에 쓰일 쓰레시홀드값을 얻는다. (w.by jang)
        private int getThreshHold(int[] histogram, Image<Gray, Byte> binary)
        {
            double[] betweenVariance = new double[256];
            double maxValue = 0;
            int threshold = 0;

            double weightB, meanB;
            double weightF, meanF;
            double temp;
            for (int t = 0; t < 256; t++)
            {
                weightB = 0; meanB = 0;
                weightF = 0; meanF = 0;

                for (int i = 0; i <= t; i++)
                {
                    weightB = weightB + histogram[i];
                    meanB = meanB + (i * histogram[i]);
                }

                temp = weightB;
                if (temp == 0)
                    temp = 0.00001;
                else if (temp == 1)
                    temp = 0.99999;

                weightB = weightB / (binary.Width * binary.Height);
                meanB = meanB / temp;

                for (int i = t + 1; i < 256; i++)
                {
                    weightF = weightF + histogram[i];
                    meanF = meanF + (i * histogram[i]);
                }

                temp = weightF;
                if (temp == 0)
                    temp = 0.00001;
                else if (temp == 1)
                    temp = 0.99999;

                weightF = weightF / (binary.Width * binary.Height);
                meanF = meanF / temp;

                betweenVariance[t] = weightB * weightF * (meanB - meanF) * (meanB - meanF);
            }

            maxValue = betweenVariance[0];
            for (int i = 0; i < 256; i++)
            {
                if (maxValue < betweenVariance[i])
                {
                    maxValue = betweenVariance[i];
                    threshold = i;
                }
            }

            return threshold;

        }

        // 오츠 알고리즘을 통해 제한된 영역에서의 홀드값을 얻는다.
        private int getThreshHold(int[] histogram, Image<Gray, Byte> binary, byte targetVal)
        {
            double[] betweenVariance = new double[256];
            double maxValue = 0;
            int threshold = 0;

            double weightB, meanB;
            double weightF, meanF;
            double temp;

            int valArea = 0;
            for(int i=0; i<binary.Height; i++)
            {
                for(int j=0; j<binary.Width; j++)
                {
                    if (binary.Data[i, j, 0] == targetVal)
                        valArea++;
                }
            }

            for (int t = 0; t < 256; t++)
            {
                weightB = 0; meanB = 0;
                weightF = 0; meanF = 0;

                for (int i = 0; i <= t; i++)
                {
                    weightB = weightB + histogram[i];
                    meanB = meanB + (i * histogram[i]);
                }

                temp = weightB;
                if (temp == 0)
                    temp = 0.00001;
                else if (temp == 1)
                    temp = 0.99999;

                weightB = weightB / (valArea);
                meanB = meanB / temp;

                for (int i = t + 1; i < 256; i++)
                {
                    weightF = weightF + histogram[i];
                    meanF = meanF + (i * histogram[i]);
                }

                temp = weightF;
                if (temp == 0)
                    temp = 0.00001;
                else if (temp == 1)
                    temp = 0.99999;

                weightF = weightF / (valArea);
                meanF = meanF / temp;

                betweenVariance[t] = weightB * weightF * (meanB - meanF) * (meanB - meanF);
            }

            maxValue = betweenVariance[0];
            for (int i = 0; i < 256; i++)
            {
                if (maxValue < betweenVariance[i])
                {
                    maxValue = betweenVariance[i];
                    threshold = i;
                }
            }

            return threshold;
        }

        // 구한 쓰레드홀드값을 이용하여 이진화 적용 (w.by jang)
        private void binarize(Image<Gray, Byte> binary, int threshold)
        {
            for (int i = 0; i < binary.Height; i++)
            {
                for (int j = 0; j < binary.Width; j++)
                {
                    if (binary.Data[i, j, 0] >= threshold)
                    {
                        binary.Data[i, j, 0] = 255;
                    }
                    else
                    {
                        binary.Data[i, j, 0] = 0;
                    }
                }
            }
        }

        // Labeling (w.by jang)
        private void button_labeling_Click(object sender, EventArgs e)
        {
            // 이미지가 추가되어 있어야만 한다.
            if (original_img.Count == 0)
            {
                textBox_shell.Text = "Import images";
                textBox_shell.Refresh();
                return;
            }
            // 엣지를 먼저 찾아야만 한다.
            else if (Canny_img.Rows == 0)
            {
                textBox_shell.Text = "Detect edge";
                textBox_shell.Refresh();
                return;
            }
            else
            {
                textBox_shell.Text = "Labeling ...";
                textBox_shell.Refresh();
            }

            for (int i = 0; i < 1; i++)
            {
                // 이진화 이미지를 사용하여 라벨링한다.
                Image<Gray, Byte> labeled = new Image<Gray, byte>(binary_img.Width, binary_img.Height);
                Point pnt = new Point(0, 0);
                Point now = new Point(0, 0);
                Stack<Point> stack = new Stack<Point>();
                Byte labelnum = 30;
                bool flag = false;

                // 라벨 이미지를 초기화한다.
                for (int a = 0; a < labeled.Height; a++)
                {
                    for (int b = 0; b < labeled.Width; b++)
                    {
                        labeled.Data[a, b, 0] = 0;
                    }
                }

                while (true)
                {
                    // 픽셀값이 0이며, 라벨값이 0인 픽셀을 찾는다.
                    now = searchPixel(binary_img, ref pnt, labeled);
                    if (now.X == binary_img.Height - 1 && now.Y == binary_img.Width - 1)
                        break;

                    // 해당 픽셀에 대하여 라벨값을 주고 스텍에 넣는다.
                    labelcheck(now, ref labeled, stack, labelnum);

                    // 해당 픽셀을 기준으로 8방향 이웃탐색을 실시한다. (연결된 객체를 찾는다.)
                    neighborSearch(binary_img, stack.Pop(), stack, ref labeled, labelnum);

                    // 스텍이 빌때까지 이웃탐색을 하며 라벨링을 한다.
                    while (stack.Count != 0)
                    {
                        neighborSearch(binary_img, stack.Pop(), stack, ref labeled, labelnum);
                    }

                    if (flag == false)
                        labelnum += 30;
                    else
                        labelnum -= 30;

                    if (labelnum == 210)
                        flag = true;
                    else if (labelnum == 30)
                        flag = false;
                }

                // 구해진 라벨 이미지를 저장한다.
                labeled_img = labeled;
                string FileNameOnly = Path.GetFileName(ofd.FileNames[targetIdx].ToString());
                labeled_img.Save(di + @"\labeled_" + FileNameOnly);


                // ## 3진 이미지를 사용하여 라벨링
                Image<Gray, Byte> labeled2 = new Image<Gray, byte>(binary_img.Width, binary_img.Height);
                Point pnt2 = new Point(0, 0);
                Point now2 = new Point(0, 0);
                Stack<Point> stack2 = new Stack<Point>();
                Byte labelnum2 = 1;
                bool flag2 = false;

                // 라벨 이미지를 초기화한다.
                for (int a = 0; a < labeled2.Height; a++)
                {
                    for (int b = 0; b < labeled2.Width; b++)
                    {
                        labeled2.Data[a, b, 0] = 0;
                    }
                }

                while (true)
                {
                    // 픽셀값이 0 또는 127 또는 255이며, 라벨값이 0인 픽셀을 찾는다.
                    now2 = searchPixel2(binary_img, ref pnt2, labeled2);
                    if (now2.X == binary_img.Height - 1 && now2.Y == binary_img.Width - 1)
                        break;

                    // 해당 픽셀에 대하여 라벨값을 주고 스텍에 넣는다.
                    labelcheck(now2, ref labeled2, stack2, labelnum2);

                    // 해당 픽셀을 기준으로 8방향 이웃탐색을 실시하며 같은 값을 가진 픽셀을 찾는다. (연결된 객체를 찾는다.)
                    neighborSearch2(binary_img, stack2.Pop(), stack2, ref labeled2, labelnum2);

                    // 스텍이 빌때까지 이웃탐색을 하며 라벨링을 한다.
                    while (stack2.Count != 0)
                    {
                        neighborSearch2(binary_img, stack2.Pop(), stack2, ref labeled2, labelnum2);
                    }

                    if (flag2 == false)
                        labelnum2 += 1;
                    else
                        labelnum2 -= 1;

                    if (labelnum2 == 255)
                        flag2 = true;
                    else if (labelnum2 == 5)
                        flag2 = false;
                }

                // 구해진 라벨 이미지를 저장한다.
                labeled2_img = labeled2;
                labeled2_img.Save(di + @"\labeled2_" + FileNameOnly);


                // ## Canny를 이용하여 라벨링한다.
                Image<Gray, Byte> labeled3 = new Image<Gray, byte>(tripleCanny.Width, tripleCanny.Height);
                Point pnt3 = new Point(0, 0);
                Point now3 = new Point(0, 0);
                Stack<Point> stack3 = new Stack<Point>();
                Byte labelnum3 = 1;
                bool flag3 = false;

                // 라벨 이미지를 초기화한다.
                for (int a = 0; a < labeled3.Height; a++)
                {
                    for (int b = 0; b < labeled3.Width; b++)
                    {
                        labeled3.Data[a, b, 0] = 0;
                    }
                }

                while (true)
                {
                    // 픽셀값이 0이며, 라벨값이 0인 픽셀을 찾는다.
                    now3 = searchPixel(tripleCanny, ref pnt3, labeled3);
                    if (now3.X == tripleCanny.Height - 1 && now3.Y == tripleCanny.Width - 1)
                        break;

                    // 해당 픽셀에 대하여 라벨값을 주고 스텍에 넣는다.
                    labelcheck(now3, ref labeled3, stack3, labelnum3);

                    // 해당 픽셀을 기준으로 4방향 이웃탐색을 실시하며 같은 값을 가진 픽셀을 찾는다. (연결된 객체를 찾는다.)
                    neighborSearch3(tripleCanny, stack3.Pop(), stack3, ref labeled3, labelnum3);

                    // 스텍이 빌때까지 이웃탐색을 하며 라벨링을 한다.
                    while (stack3.Count != 0)
                    {
                        neighborSearch(tripleCanny, stack3.Pop(), stack3, ref labeled3, labelnum3);
                    }

                    if (flag3 == false)
                        labelnum3 += 1;
                    else
                        labelnum3 -= 1;

                    if (labelnum3 == 255)
                        flag3 = true;
                    else if (labelnum3 == 1)
                        flag3 = false;
                }

                // 구해진 라벨 이미지를 저장한다.
                labeled_img = labeled3;
                labeled_img.Save(di + @"\labeled3_" + FileNameOnly);
            }



            textBox_shell.Text = "Labeling done.";
            textBox_shell.Refresh();
        }

        //@@ 레이어 저장, App이미지 생성
        private void button_getResult_Click(object sender, EventArgs e)
        {
            App_img = original_img[0].Clone();

            textBox_shell.Text = "Saving layer...";

            int[] layer_count = new int[256];

            for (int y = 0; y < original_img[0].Rows; y++)
            {
                for (int x = 0; x < original_img[0].Cols; x++)
                {
                    bool check = false;
                    for (int count = 0; count < numlable; count++)
                    {
                        if (labeled_img.Data[y, x, 0] == layer_count[count])
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check == false)
                    {
                        layer_count[numlable] = labeled_img.Data[y, x, 0];
                        numlable++;
                    }

                }
            }

            for (int i = 0; i < numlable; i++)
            {
                Image<Gray, byte> tmp = new Image<Gray, byte>(labeled_img.Cols, labeled_img.Rows);
                Image<Bgr, byte> tmp_color = new Image<Bgr, byte>(labeled_img.Cols, labeled_img.Rows);
                for (int y = 0; y < original_img[0].Rows; y++)
                {
                    for (int x = 0; x < original_img[0].Cols; x++)
                    {
                        if (labeled_img.Data[y, x, 0] == layer_count[i])
                            tmp.Data[y, x, 0] = labeled_img.Data[y, x, 0];
                    }
                }
                layer.Add(tmp);
                color_layer.Add(tmp_color);
            }

            for (int y = 0; y < original_img[0].Rows; y++)
            {
                for (int x = 0; x < original_img[0].Cols; x++)
                {
                    for (int i = 0; i < numlable; i++)
                    {
                        if (layer[i].Data[y, x, 0] != 0)
                        {
                            color_layer[i].Data[y, x, 0] = original_img[targetIdx].Data[y, x, 0];
                            color_layer[i].Data[y, x, 1] = original_img[targetIdx].Data[y, x, 1];
                            color_layer[i].Data[y, x, 2] = original_img[targetIdx].Data[y, x, 2];
                        }
                    }
                }
            }

            for (int i = 0; i < numlable; i++)
            {
                color_layer[i].Save("Layer\\color_layer" + i + ".jpg");
            }

            App_img = original_img[0].Clone();

            textBox_shell.Text = "Saving layer finished";
        }

        //@@ 마우스 좌표 계산
        private void imageBox_main_MouseMove(object sender, MouseEventArgs e)
        {
            if (original_img.Count != 0)
            {
                double ratioX = (double)original_img[0].Cols / 800;
                double ratioY = (double)original_img[0].Rows / 680;

                if (original_img[0].Cols <= original_img[0].Rows)
                {
                    locationX = (int)((e.Location.X - ((800 - (original_img[0].Cols / ratioY)) / 2)) * ratioY);
                    locationY = (int)((e.Location.Y) * ratioY);
                }
                else if (original_img[0].Cols > original_img[0].Rows)
                {
                    locationX = (int)((e.Location.X) * ratioX);
                    locationY = (int)((e.Location.Y - ((680 - (original_img[0].Rows / ratioX)) / 2)) * ratioX);
                }
                if (locationX >= 0 && locationX <= original_img[0].Cols && locationY >= 0 && locationY <= original_img[0].Rows)
                    textBox_shell.Text = Convert.ToString(locationX) + "." + Convert.ToString(locationY);
            }
        }

        //@@ 마우스 클릭시 해당 레이어 색변경
        private void imageBox_main_MouseDown(object sender, MouseEventArgs e)
        {
            int gray_Data = labeled_img.Data[locationY, locationX, 0];
            for (int y = 0; y < App_img.Rows; y++)
            {
                for (int x = 0; x < App_img.Cols; x++)
                {
                    if (gray_Data == labeled_img.Data[y, x, 0])
                    {
                        if (x != 0 && x != App_img.Cols - 1 && y != 0 && y != App_img.Rows - 1)
                        {
                            if (labeled_img.Data[y, x, 0] != labeled_img.Data[y, x - 1, 0] || labeled_img.Data[y, x, 0] != labeled_img.Data[y, x + 1, 0])
                            {
                                App_img.Data[y, x, 0] = 0;
                                App_img.Data[y, x, 1] = 255;
                                App_img.Data[y, x, 2] = 0;
                            }
                            else
                            {
                                App_img.Data[y, x, 0] = 220;
                                App_img.Data[y, x, 1] = 140;
                                App_img.Data[y, x, 2] = 20;
                            }
                        }
                    }

                }
            }
            imageBox_main.Refresh();
        }

        //@@ 마우스 클릭 해제시 이미지 초기화
        private void imageBox_main_MouseUp(object sender, MouseEventArgs e)
        {
            App_img = original_img[0].Clone();
            imageBox_main.Refresh();
        }

        /*  @@레이어 움직이기 실패...
        private void imageBox_main_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.M)
            {
                if (keycheck == false)
                {
                    if (locationX >= 0 && locationX < original_img[0].Cols && locationY >= 0 && locationY < original_img[0].Rows)
                    {
                        start_X = locationX;
                        start_Y = locationY;
                        keycheck = true;

                    }
                }
                else if (keycheck == true)
                {
                    direction_X = locationX - start_X;
                    direction_Y = locationY - start_Y;

                    Image<Bgr, byte> tmp_color = new Image<Bgr, byte>(original_img[0].Cols, original_img[0].Rows);

                    if (locationX >= 0 && locationX < original_img[0].Cols && locationY >= 0 && locationY < original_img[0].Rows)
                    {
                        int choose_layer = 0;
                        for (int i = 0; i < numlable; i++)
                        {
                            if (labeled_img.Data[start_Y, start_X, 0] == labeled_img[i].Data[start_Y, start_X, 0])
                            {
                                choose_layer = i;
                                break;
                            }
                        }

                        for (int y = 0; y < original_img[0].Rows; y++)
                        {
                            for (int x = 0; x < original_img[0].Cols; x++)
                            {
                                if (color_layer[choose_layer].Data[y, x, 0] != 0)
                                {
                                    if (x + direction_X >= 0 && x + direction_X < original_img[0].Cols && y + direction_Y >= 0 && y + direction_Y < original_img[0].Rows)
                                    {
                                        tmp_color.Data[y + direction_Y, x + direction_X, 0] = color_layer[choose_layer].Data[y, x, 0];
                                        tmp_color.Data[y + direction_Y, x + direction_X, 1] = color_layer[choose_layer].Data[y, x, 1];
                                        tmp_color.Data[y + direction_Y, x + direction_X, 2] = color_layer[choose_layer].Data[y, x, 2];
                                        labeled_img.Data[y + direction_Y, x + direction_X, 0] = labeled_img.Data[y, x, 0];
                                    }
                                    else
                                    {
                                        tmp_color.Data[y, x, 0] = 0;
                                        tmp_color.Data[y, x, 0] = 0;
                                        tmp_color.Data[y, x, 0] = 0;
                                        labeled_img.Data[y, x, 0] = 0;
                                    }
                                }
                            }
                        }
                        App_img = tmp_color;
                        keycheck = false;
                        imageBox_main.Refresh();
                    }
                   
                }
            }
        }
        */
       
    }
}

// open source SURF Library usage code
namespace SURFFeature
{
    public static class DrawMatches
    {
        public static void FindMatch(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.8;
            double hessianThresh = 300;

            Stopwatch watch;
            homography = null;

            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();

#if !__IOS__
            if (CudaInvoke.HasCuda)
            {
                CudaSURF surfCuda = new CudaSURF((float)hessianThresh);
                using (GpuMat gpuModelImage = new GpuMat(modelImage))
                //extract features from the object image
                using (GpuMat gpuModelKeyPoints = surfCuda.DetectKeyPointsRaw(gpuModelImage, null))
                using (GpuMat gpuModelDescriptors = surfCuda.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                using (CudaBFMatcher matcher = new CudaBFMatcher(DistanceType.L2))
                {
                    surfCuda.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    using (GpuMat gpuObservedImage = new GpuMat(observedImage))
                    using (GpuMat gpuObservedKeyPoints = surfCuda.DetectKeyPointsRaw(gpuObservedImage, null))
                    using (GpuMat gpuObservedDescriptors = surfCuda.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                    //using (GpuMat tmp = new GpuMat())
                    //using (Stream stream = new Stream())
                    {
                        matcher.KnnMatch(gpuObservedDescriptors, gpuModelDescriptors, matches, k);

                        surfCuda.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                        mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(255));
                        Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                        int nonZeroCount = CvInvoke.CountNonZero(mask);
                        if (nonZeroCount >= 4)
                        {
                            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                               matches, mask, 1.5, 20);
                            if (nonZeroCount >= 4)
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                                   observedKeyPoints, matches, mask, 2);
                        }
                    }
                    watch.Stop();
                }
            }
            else
#endif
            {
                using (UMat uModelImage = modelImage.ToUMat(AccessType.Read))
                using (UMat uObservedImage = observedImage.ToUMat(AccessType.Read))
                {
                    SURF surfCPU = new SURF(hessianThresh);
                    //extract features from the object image
                    UMat modelDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    UMat observedDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
                    BFMatcher matcher = new BFMatcher(DistanceType.L2);
                    matcher.Add(modelDescriptors);

                    matcher.KnnMatch(observedDescriptors, matches, k, null);
                    mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                    int nonZeroCount = CvInvoke.CountNonZero(mask);
                    if (nonZeroCount >= 4)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                           matches, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                               observedKeyPoints, matches, mask, 2);
                    }

                    watch.Stop();
                }
            }
            matchTime = watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Draw the model image and observed image, the matched features and homography projection.
        /// </summary>
        /// <param name="modelImage">The model image</param>
        /// <param name="observedImage">The observed image</param>
        /// <param name="matchTime">The output total time for computing the homography matrix.</param>
        /// <param name="KeyPoints">The keypoints.</param>
        /// <returns>The model image and observed image, the matched features and homography projection.</returns>
        public static MDMatch[][] Detect(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint mKeyPoints, out VectorOfKeyPoint oKeyPoints, out Mat featureMap, out Mat homography)
        {
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                Mat mask;
                FindMatch(modelImage, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, matches,
                   out mask, out homography);
                MDMatch[][] matchArr = matches.ToArrayOfArray();
                mKeyPoints = modelKeyPoints;
                oKeyPoints = observedKeyPoints;


                //Draw the matched keypoints
                Mat result = new Mat();
                Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
                   matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);

                #region draw the projected region on the image

                if (homography != null)
                {
                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
                    PointF[] pts = new PointF[]
                    {
                  new PointF(rect.Left, rect.Bottom),
                  new PointF(rect.Right, rect.Bottom),
                  new PointF(rect.Right, rect.Top),
                  new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);

                    Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                    }

                }

                #endregion
                featureMap = result;
                return matchArr;

            }
        }
    }
}