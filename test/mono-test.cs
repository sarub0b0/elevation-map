using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Database
{
    //定数を一覧にするクラス
    public class ConstValue
    {
        public const int WidthImg = 200;
        public const int HeightImg = 200;
        public const int WidthDiv = 10;    // 横方向の区切り数
        public const int HeightDiv = 10;   // 縦方向の区切り数
        public const int ColorMax = 255;    // HSVのHueは0～360で一周するが，このプログラムでは赤から青の0～255までを使う
        public const int RandomMax = 100;   // 入力データ生成に用いる乱数の最大値
        public const int RandomMin = 0;     // 入力データ生成に用いる乱数の最小値
    }

    class Program
    {
        static void Main(string[] args)
        {
            InputData ipd = new InputData();

            MySqlConnection dbcon = new MySqlConnection();

            //接続パラメータ
            string server = "192.168.15.30";
            string database = "radiomap";
            string user = "kosei";
            string pass = "radiradio";

            // 接続文字列
            string constr = string.Format(
                "Server={0};Database={1};Uid={2};Pwd={3}",
                server,
                database,
                user,
                pass);
            try
            {
                // SQL文字列
                string sstr = "SELECT id,elevation " +
                                  "FROM e1015170210";

                dbcon = new MySqlConnection(constr);

                MySqlDataAdapter da = new MySqlDataAdapter(sstr, dbcon);
                DataTable dt = new DataTable();

                // SELECT実行
                da.Fill(dt);

                List<double> elevList = new List<double>();
                
                foreach (DataRow dr in dt.Rows)
                {
                    Console.WriteLine("ID:{0} ELEVATION:{1}", dr[0], dr[1]);
                    double elev = double.Parse(dr[1].ToString());

                    elevList.Add(elev);
                }

                ipd.CreateData(elevList);               // 入力データ生成
                DrawMapClass dmc = new DrawMapClass(ConstValue.WidthImg, ConstValue.HeightImg);
                
                dmc.DrawValue(ipd.Data);        // マップ描画
                dmc.Image.Save("err.jpg", ImageFormat.Jpeg);
				dmc.Image.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (dbcon.State == ConnectionState.Open)
                {
                    dbcon.Close();
                    dbcon.Dispose();
                }
            }
			
				
        }
    }

    // 入力データ管理クラス
    public class InputData
    {
        // この2次元配列にデータ（数字）が入る
        private double[,] data = new double[ConstValue.WidthDiv,ConstValue.HeightDiv];

        // 入力データを生成（0～100のどれかがランダムで入るようになってます)
        public void CreateData(List<double> el)
        {
            int div = (int)Math.Sqrt(el.Count);
            for(int y = 0; y < div; y++)
            {
                for (int x = 0; x < div; x++)
                {
                    data[x, y] = el[x + y * 10];
                }
            }
        }

        public double[,] Data
        {
            get { return data; }
            set { data = value; }
        }
    }

    // マップ描画クラス
    public class DrawMapClass
    {
        private Bitmap bmp;
        private Graphics gph;
        private Pen meshPen /*, meshPenEst, meshPenCor/**/;
        private int BrushListWhite;
//        private int BrushListBlue;
//        private int BrushListRed;
        private List<SolidBrush> BrushList = new List<SolidBrush>();

        private int height, width;
        private int heightDiv, widthDiv;


        public DrawMapClass(int Width, int Height)
        {
            height = Height;
            width = Width;
            heightDiv = height / ConstValue.HeightDiv;
            widthDiv = width / ConstValue.WidthDiv;

            bmp = new Bitmap(Width, Height);
            gph = Graphics.FromImage(bmp);

            gph.Clear(Color.White);

            //ブラシの色を変えるときはここ
            for (int i = 0; i <= ConstValue.ColorMax; i++)
            {
                int[] t = HSVtoRGB(255 - i, 255, 255); //Hueは変化させるけど他はそのまま．255-iとしているのは青を最小値とするため
                BrushList.Add(new SolidBrush(Color.FromArgb(255, t[0], t[1], t[2]))); //0～255の色のブラシを作る
            }
            BrushList.Add(new SolidBrush(Color.White));
            BrushListWhite = BrushList.Count - 1;
//            BrushListBlue = 0;
//            BrushListRed = BrushList.Count - 2;
        }

        public void DrawValue(double[,] data)
        {
            double min, max;

            //min，maxの初期値はありえない値にしておく
            min = ConstValue.RandomMax + 1;
            max = ConstValue.RandomMin - 1;

            for (int y = 0; y < ConstValue.HeightDiv; y++)
            {
                for (int x = 0; x < ConstValue.WidthDiv; x++)
                {
                    if (data[x, y] > max)
                    {
                        max = data[x, y];
                    }
                    if (data[x, y] < min)
                    {
                        min = data[x, y];
                    }
                }
            }

            double a;
            a = 255 / (max - min);

            for (int y = 0; y < ConstValue.HeightDiv; y++)
            {
                for (int x = 0; x < ConstValue.WidthDiv; x++)
                {
                    if (data[x, y] == min)
                    {
                        DrawPoint(BrushListWhite, x, y);
                    }
                    else
                    {
                        DrawPoint((int)(a * (data[x, y] - min)), x, y);
                    }
                }
            }
        }

        public void DrawPoint(int value, int x, int y)
        {
            gph.FillRectangle(BrushList[value], x * widthDiv, y * heightDiv, widthDiv, heightDiv);
        }

        public void DrawPoint(int value, Point p)
        {
            gph.FillRectangle(BrushList[value], p.X * widthDiv, p.Y * heightDiv, widthDiv, heightDiv);
        }

        public Point MousePoint(int x, int y)
        {
            Point t = new Point(x / widthDiv, y / heightDiv);
            return t;
        }
        public Point MousePoint(Point p)
        {
            Point t = new Point(p.X / widthDiv, p.Y / heightDiv);
            return t;
        }

        private int[] HSVtoRGB(int h, int s, int v)
        {
            double f;
            int i, p, q, t;
            int[] rgb = new int[3];

            i = (int)Math.Floor(h / 60.0) % 6;
            f = (float)(h / 60.0) - (float)Math.Floor(h / 60.0);
            p = (int)Math.Round(v * (1.0 - (s / 255.0)));
            q = (int)Math.Round(v * (1.0 - (s / 255.0) * f));
            t = (int)Math.Round(v * (1.0 - (s / 255.0) * (1.0 - f)));

            switch (i)
            {
                case 0: rgb[0] = v; rgb[1] = t; rgb[2] = p; break;
                case 1: rgb[0] = q; rgb[1] = v; rgb[2] = p; break;
                case 2: rgb[0] = p; rgb[1] = v; rgb[2] = t; break;
                case 3: rgb[0] = p; rgb[1] = q; rgb[2] = v; break;
                case 4: rgb[0] = t; rgb[1] = p; rgb[2] = v; break;
                case 5: rgb[0] = v; rgb[1] = p; rgb[2] = q; break;
            }

            return rgb;
        }

        // マップを最新状態にする
        public Bitmap Image
        {
            get
            {
                meshPen = new Pen(Color.LightGray, 1);
                for (int i = 1; i < height; i++)
                {
                    gph.DrawLine(meshPen, 0, heightDiv * i, width, heightDiv * i);
                }
                for (int i = 1; i < width; i++)
                {
                    gph.DrawLine(meshPen, widthDiv * i, 0, widthDiv * i, height);
                }
                meshPen.Dispose();

                return bmp;
            }
            set { bmp = value; }
        }

        public int HeightDiv
        {
            get { return heightDiv; }
        }

        public int WidthDiv
        {
            get { return widthDiv; }
        }

        //デストラクタ
        ~DrawMapClass()
        {
            //リソースを開放する
            BrushList.Clear();
            gph.Dispose();
        }
    }
}

