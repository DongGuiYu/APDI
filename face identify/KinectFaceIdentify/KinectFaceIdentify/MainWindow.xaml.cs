// KinectFaceIdentify 2019/05/16 FELab
// Windows 10 Visual Studio Community 2017
// 讀取 personGroupId 群組內所有的個人：
// await faceClient.PersonGroupPerson.ListAsync(personGroupId);
// 辨識人臉：
// await faceClient.Face.IdentifyAsync(faceIds, personGroupId);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Azure.CognitiveServices.Vision.Face;        // 使用 Face API
using Microsoft.Azure.CognitiveServices.Vision.Face.Models; // 使用 Face API
using Microsoft.Kinect;     // 使用 Kinect SDK 2.0
using System.IO;            // 使用 FileStream、Stream 類別做檔案、串流存取
using System.Windows.Threading;             // 使用 DispatcherTimer (計時器)
using MySql.Data;
using MySql.Data.MySqlClient;



namespace KinectFaceIdentify
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        // 請使用自己的 Face API Subscription Key
        private const string faceSubscriptionKey =
            "897857545c7c4eaf91fec1dd67d62c60";

        // 秀在畫面的資料部份隱藏之 Face API Subscription Key
        private const string faceSubscriptionKeyHide =
            "897857xxxxxxxxxxxxxxxxxxxxd62c60";

        // 群組命名不能使用英文大寫字母，儲存欲建立或已建立欲操作的群組名稱
        private string personGroupId = "fguai";

        // 請使用自己的 Face API Subscription Key 之 Endpoint
        private const string faceEndpoint =
            "https://westus.api.cognitive.microsoft.com/";

        // 使用 Face API 的金鑰，建立微軟雲端臉部辨識服務連線
        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(faceSubscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        // faceList 是 DetectedFace 的串列，儲存偵測到的臉部原始屬性資料
        private IList<DetectedFace> faceList;

        // faceDescriptions 是字串陣列，儲存偵測到的臉部描述字串
        private string[] faceDescriptions;

        private string fileName;    // 一個影像檔的完整名稱
        BitmapImage bitmapSource;   // BitmapImage 物件參考
        bool isShoot = true;        // 有人靠近 2 公尺內是/否要處理true/false
        DispatcherTimer timer;      // 計時器物件參考

        // sensor 代表接到電腦的那一台 Kinect (只能接一台)
        private KinectSensor sensor;
        // ColorFrameReader 物件參考
        private ColorFrameReader colorFrameReader;
        // FrameDescription 物件參考
        private FrameDescription frameDescription;
        // WriteableBitmap 物件參考，做為儲存影像的記憶體區塊
        private WriteableBitmap wbData;
        // 儲存影像每一 pixel 之 BGRA 值的陣列參考
        private byte[] byteData;
        
        // BodyFrameReader 物件參考
        private BodyFrameReader bodyFrameReader;
        // 骨架陣列，陣列每一個元素代表一個人的骨架
        private Body[] bodies;
        // Shoot 類別的物件參考，偵測有人靠近至設定的範圍(2 公尺)
        Shoot shoot = new Shoot();

        public MainWindow()
        {
            InitializeComponent();

            // 檢查 Face Api Subscription Key 的端點格式是否正確，
            // 只檢查是否為網址，端點填錯無法查出，例如將 westus 改為
            // universe，不會判定為錯誤
            if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
            {
                faceClient.Endpoint = faceEndpoint;
            }
            else
            {
                MessageBox.Show(faceEndpoint, "不正確的 URI",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            // Kinect 每秒傳入 30 個 1920x1080 的彩色影像，
            // 每個影像傳入時觸發 FrameArrived 事件，指定此事件由
            // ColorFrameReader_FrameArrived 函式負責處理
            sensor = KinectSensor.GetDefault();
            colorFrameReader = sensor.ColorFrameSource.OpenReader();
            colorFrameReader.FrameArrived += ColorFrameReader_FrameArrived;

            // 取得傳入彩色影像的資料描述
            frameDescription =
                sensor.ColorFrameSource.CreateFrameDescription(
                    ColorImageFormat.Bgra);

            // wbData參考一個 WriteableBitmap 物件(儲存影像的記憶體區塊)
            wbData = new WriteableBitmap(
                frameDescription.Width, frameDescription.Height, 96, 96,
                PixelFormats.Bgr32, null);
            // byteData 參考一個儲存影像每一個 pixel 之 BGRA 值的 byte 陣列
            byteData = new byte[frameDescription.Width *
                frameDescription.Height * frameDescription.BytesPerPixel];

            // Kinect 傳入的骨架資料，觸發 FrameArrived 事件，指定此事件由
            // BodyFrameReader_FrameArrived 函式負責處理
            bodyFrameReader = sensor.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;
            // shoot 觸發的 Detected 事件由 Shoot_Detected() 處理
            shoot.Detected += Shoot_Detected;

            // 啟動 Kinect Sensor
            sensor.Open();

            // timer 指向 DispatcherTimer 物件
            timer = new DispatcherTimer
            {
                // timer 的時間間隔設為 5 秒
                Interval = new TimeSpan(0, 0, 5)
            };
            // 指定監聽 Tick 事件，處理函式為自動產生的 Timer_Tick()
            timer.Tick += Timer_Tick;
            // 起動 timer
            timer.Start();
            // 計時器停止
            timer.IsEnabled = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            // 指定 ColorImage 影像來源為 wbData (WriteableBitmap 物件)
            ColorImage.Source = wbData;
            // Kinect 重新傳送資料
            colorFrameReader.IsPaused = false;
            // 顯示訊息
            Result.Text += "\n\nKinect 傳送影像";
            // 取消已挑選的圖檔         
            fileName = null;
            // 計時器停止
            timer.IsEnabled = false;
            // isShoot 的值設為 true
            isShoot = true;
        }

        // 自行定義的函式，由 Shoot_Detected 函式呼叫，傳入一個人臉的
        // 原始屬性資料，傳回一個描述臉部的字串
        private string FaceDescription(DetectedFace face)
        {
            // sb 是 StringBuilder 類別的物件，用以加入描述臉部屬性的字串
            StringBuilder sb = new StringBuilder();

            // 加入性別資料 Male、Female 或 其他
            var gender = face.FaceAttributes.Gender.ToString();
            if (gender == "Male")
                sb.Append("男，");
            else if (gender == "Female")
                sb.Append("女，");
            else
                sb.Append(face.FaceAttributes.Gender);

            // 加入年齡資料
            sb.Append(face.FaceAttributes.Age);
            sb.Append("歲，");

            // 加入情緒值資料，顯示八種情緒成份中超過 1% 的資料
            Emotion emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.01f)
                sb.Append(String.Format("生氣 {0:F2}%  ",
                    emotionScores.Anger * 100));
            if (emotionScores.Contempt >= 0.01f)
                sb.Append(String.Format("藐視 {0:F2}%  ",
                    emotionScores.Contempt * 100));
            if (emotionScores.Disgust >= 0.01f)
                sb.Append(String.Format("厭惡 {0:F2}%  ",
                    emotionScores.Disgust * 100));
            if (emotionScores.Fear >= 0.01f)
                sb.Append(String.Format("恐懼 {0:F2}%  ",
                    emotionScores.Fear * 100));
            if (emotionScores.Happiness >= 0.01f)
                sb.Append(String.Format("快樂 {0:F2}%  ",
                    emotionScores.Happiness * 100));
            if (emotionScores.Neutral >= 0.01f)
                sb.Append(String.Format("中性 {0:F2}%  ",
                    emotionScores.Neutral * 100));
            if (emotionScores.Sadness >= 0.01f)
                sb.Append(String.Format("傷心 {0:F2}%  ",
                    emotionScores.Sadness * 100));
            if (emotionScores.Surprise >= 0.01f)
                sb.Append(String.Format("驚訝 {0:F2}%  ",
                    emotionScores.Surprise * 100));

            // 傳回已建立的字串資料
            return sb.ToString();
        }

        // 自行定義的函式，由 Shoot_Detected 函式呼叫，傳入一個影像檔案，
        // 傳回每個人臉的原始屬性資料。
        // UploadAndDetectFaces 函式呼叫 Face API 的 DetectWithStreamAsync 
        // 函式，對上傳影像做人臉偵測
        private async Task<IList<DetectedFace>> UploadAndDetectFaces(
            string fileName)
        {
            // 指定傳回人臉的 3 項 attributes，包括性別、年齡、情緒
            IList<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Emotion
                };

            // 呼叫 Face API 的 DetectWithStreamAsync 函式
            try
            {
                using (Stream imageFileStream = File.OpenRead(fileName))
                {
                    // 第一個參數是 Shoot_Detected 函式呼叫 
                    // UploadAndDetectFaces 函式時傳入的影像檔名字，
                    // 第二個參數為 false，表示不傳回 faceId，
                    // 第三個參數為 false，表示不傳回臉部的 27 個 landmarks，
                    // 第四個參數指名挑選的 3 項 attributes
                    // DetectWithStreamAsync 函式將偵測完傳回的資料，存入
                    // faceList 串列，每一個人臉的資料是 faceList 的一個元素
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, 
                            returnFaceAttributes: faceAttributes);
                    return faceList;
                }
            }
            catch (APIErrorException f) // 捕抓與顯示呼叫 Face API 的錯誤
            {
                MessageBox.Show(f.Message);
                return new List<DetectedFace>();
            }
            catch (Exception e)         // 捕抓與顯示呼叫其他錯誤
            {
                MessageBox.Show(e.Message, "Error");
                return new List<DetectedFace>();
            }
        }


        private async void Shoot_Detected(object sender, EventArgs e)
        {
            //連接資料庫
            string connStr = @"Server=localhost; Database=faceid; Uid=root; Pwd=10515003; ";
            MySqlConnection conn = new MySqlConnection(connStr);
            // throw new NotImplementedException();
            // 如果 isShoot 的值是 false，則甚麼都不做
            if (!isShoot)
                return;
            else
                isShoot = false;
            Result.Text += "\n偵測到有人靠近 2 公尺內，拍照存檔";

            // 將 Kinect 的影像存成圖檔，檔名依系統時間命名，
            // 例如：C20190101231019.jpg
            fileName = "C:\\Users\\10515003\\Desktop\\pic\\pic" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            using (FileStream saveImage = new FileStream(fileName,
                FileMode.CreateNew))
            {
                // 欲儲存影像，先暫停 Kinect 傳送影像
                colorFrameReader.IsPaused = true;
                // 從 ColorImage.Source 處取出一張影像，
                // 轉為 BitmapSource 格式，儲存到 imageSource
                BitmapSource imageSource = (BitmapSource)ColorImage.Source;
                // 挑選 Joint Photographic Experts Group (JPEG) 影像編碼器
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                // 將取出的影像加到編碼器的影像集
                encoder.Frames.Add(BitmapFrame.Create(imageSource));
                // 儲存影像與後續影像清除工作
                encoder.Save(saveImage);
                saveImage.Flush();
                saveImage.Close();
                saveImage.Dispose();
            }

            // 存檔後就自動挑該圖檔為欲偵測的檔案
            /*
            fileName = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetEntryAssembly().Location)
                + @"\" + fileName;
            */

            // ColorImage 秀出欲偵測人臉圖檔之影像
            Uri fileUri = new Uri(fileName);
            bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();
            ColorImage.Source = bitmapSource;
            //Result.Text += "\n載入檔案 " + fileName;

            // 呼叫 UploadAndDetectFaces 函式以偵測人臉，將傳回的所有人臉
            // 原始屬性資料存入 faceList 串列，每一個人臉的原始屬性資料是
            // faceList 的一個元素
            //Result.Text += "\n偵測人臉‧‧‧";
            faceList = await UploadAndDetectFaces(fileName);
            /*Result.Text += String.Format("\n偵測完成， {0} 個人臉", 
                faceList.Count);*/

            SolidColorBrush[] brushes = { Brushes.Red, Brushes.Orange,
                    Brushes.Yellow, Brushes.Green, Brushes.Blue,
                    Brushes.Indigo, Brushes.Purple };
            if (faceList.Count > 1)
                Result.Text += "\n依方框紅橙黃綠藍靛紫顏色順序";

            // 臉部畫出方框
            if (faceList.Count > 0)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;
                faceDescriptions = new String[faceList.Count];

                for (int i = 0; i < faceList.Count; i++)
                {
                    // face 儲存一個人臉的資料
                    DetectedFace face = faceList[i];
                    // 依紅橙黃綠藍靛紫顏色順序畫出人臉方框
                    drawingContext.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(brushes[i % 7], 2),
                        new Rect(
                            face.FaceRectangle.Left * resizeFactor,
                            face.FaceRectangle.Top * resizeFactor,
                            face.FaceRectangle.Width * resizeFactor,
                            face.FaceRectangle.Height * resizeFactor
                        )
                    );
                    // 呼叫 FaceDescription 函式時，將一個人臉的原始屬性資料
                    // 傳給它，將 FaceDescription 函式傳回的臉部描述字串，儲
                    // 存於 faceDescriptions 字串陣列，每一個人臉的描述字串
                    // 是 faceDescriptions 的一個元素
                    faceDescriptions[i] = FaceDescription(face);
                }
                drawingContext.Close();
                // 秀出影像與人臉方框
                RenderTargetBitmap faceWithRectBitmap = new 
                    RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);
                faceWithRectBitmap.Render(visual);
                ColorImage.Source = faceWithRectBitmap;                
            }

            // 將偵測出的每個人臉之 Guid 值存入串列 faceIds 
            IList<Guid> faceIds =
                faceList.Select(face => face.FaceId.Value).ToList();

            // 人臉與群組的某人相似度最高，而且信心值大於
            // 自訂門檻值，則視為群組中的某人；
            // 如果人臉與群組中人的相似度最高者，信心值都未超過
            // 自訂門檻值，則視為不是群組中人
            double confidenceThreshold = 0.55;    // 自訂門檻值

            // 人臉辨識 1~10 人為限
            if (faceList.Count > 0 && faceList.Count < 11)
            {
                IList<IdentifyResult> results = await
                    faceClient.Face.IdentifyAsync(faceIds, personGroupId);
                int i = 0;
                foreach (var identifyResult in results)
                {
                    if (identifyResult.Candidates.Count == 0)
                    {
                        Result.Text += "\n歡迎光臨佛光大學資訊應用學系，";
                    }
                    else
                    {
                        var candidateId =
                            identifyResult.Candidates[0].PersonId;
                        var confidenceValue =
                            identifyResult.Candidates[0].Confidence;
                        var person = await
                            faceClient.PersonGroupPerson.GetAsync(
                                personGroupId, candidateId);
                        if (confidenceValue > confidenceThreshold)
                        {
                            Result.Text += "\n" + person.Name + "。";
                            // 連線到資料庫
                            try
                            {
                                conn.Open();
                            }
                            catch (MySql.Data.MySqlClient.MySqlException ex)
                            {
                                switch (ex.Number)
                                {
                                    case 0:
                                        Console.WriteLine("無法連線到資料庫.");
                                        break;
                                    case 1045:
                                        Console.WriteLine("使用者帳號或密碼錯誤,請再試一次.");
                                        break;
                                }
                            }
                            //string SQL = "select distinct id, by_id, (select c_name from teach_std where teach_std.id=event_rec.id), (select c_name from teach_std where teach_std.id=event_rec.by_id),event_desc,eve_locl,eve_date from event_rec where eve_date>=curdate() and id='" + person.Name + "'";
                            //string SQL = "select id, byid, (select c_sname from std_id where std_id.id=event_rec.id), (select c_tname from teach_id where teach_id.id=event_rec.byid),eve_desc,eve_locl,eve_time from event_rec where eve_time>=curdate() and id='" + person.Name + "'";
                            string SQL = "select id, byid, eve_desc,eve_locl,eve_time from event_rec where eve_time>=curdate() and id='" + person.Name + "'";
                            try
                            {
                                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                                MySqlDataReader myData = cmd.ExecuteReader();

                                if (!myData.HasRows)
                                {
                                    // 如果沒有資料,顯示沒有資料的訊息
                                    Console.WriteLine("No data.");
                                }
                                else 
                                {
                                    // 讀取資料並且顯示出來
                                    myData.Read();
                                    /*while (myData.Read())
                                    {
                                        /*
                                        Console.Write("Text={0}", myData.GetString(0));
                                        Console.Write("....");
                                        Console.Write("Text={0}", myData.GetString(1));
                                        Console.Write("....");
                                        Console.Write("Text={0}", myData.GetString(2));
                                        Console.Write("....");
                                        Console.Write("Text={0}", myData.GetString(3));
                                        Console.Write("....");
                                        Console.Write("Text={0}", myData.GetString(4));
                                        Console.Write("....");
                                        Console.WriteLine("Text={0}", myData.GetString(5));
                                        
                                        labIDName.Text = myData.GetString(2);
                                        txtByID.Text = myData.GetString(1);
                                        labByIDName.Text = myData.GetString(3);
                                        txtDesc.Text = myData.GetString(4);
                                        cobLocal.Text = myData.GetString(5);
                                        dTP_date.Text = myData.GetString(6);
                                        */
                                        //Result.Text += "\n您有通知請到通知系統讀取";
                                        //Result.Text += myData.GetString(2) + "同學， " + myData.GetString(3) + " 先生/小姐，因 " + myData.GetString(4) + " " + " 找您請於 " + myData.GetString(6) + " 至 " + myData.GetString(5) + "\n"; // + " 到 " + myData.GetString(7); ;
                                        Result.Text += "\n" + myData.GetString(0) + " 同學，您有通知訊息，請至通知系統查看。 " ;
                                    //}
                                    myData.Close();
                                    conn.Close();
                                }
                            }
                            catch (MySql.Data.MySqlClient.MySqlException ex)
                            {
                                Console.WriteLine("Error " + ex.Number + " : " + ex.Message);
                            }


                        }
                        else
                            Result.Text += "\n歡迎光臨佛光大學資訊應用學系，";
                    }
                    //Result.Text += faceDescriptions[i];
                    i++;
                }            
            }
            else
                Result.Text += "\n人臉數目不在 1~10 範圍，所以不做辨識";
            About.ScrollToEnd();    // 讓捲軸移至最新顯示文字處
            Result.ScrollToEnd();   // 讓捲軸移至最新顯示文字處

            // 計時器開始計時
            timer.IsEnabled = true;
        }

        private void BodyFrameReader_FrameArrived(object sender, 
            BodyFrameArrivedEventArgs e)
        {
            // throw new NotImplementedException();
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                // 如果骨架資料不存在就直接離開事件處理常式
                if (bodyFrame == null)
                    return;
                // 產生骨架陣列，陣列長度為 6
                bodies = new Body[bodyFrame.BodyCount];
                // 將骨架資料複製到骨架陣列
                bodyFrame.GetAndRefreshBodyData(bodies);
                for (int i = 0; i < bodies.Length; i++)
                {
                    // 檢查那一個骨架處於被追蹤狀態
                    if (bodies[i].IsTracked)
                    {
                        // 把關節與時間資料傳給 Shoot 類別的 Detection 函式，
                        // 以偵測有人靠近設定的範圍，以便觸發 Detected 事件，
                        // 才能讓系統呼叫 Shoot_Detected 函式來處理
                        shoot.Detection(
                            bodies[i].Joints[JointType.Head],
                            bodies[i].Joints[JointType.SpineShoulder],
                            bodies[i].Joints[JointType.SpineMid],
                            bodyFrame.RelativeTime);
                    }
                }
            }
        }

        private void ColorFrameReader_FrameArrived(object sender, 
            ColorFrameArrivedEventArgs e)
        {
            // throw new NotImplementedException();
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                // 如果影格資料不存在，就直接離開事件處理函式
                if (colorFrame == null)
                    return;
                Int32Rect bitmapRect = new Int32Rect(0, 0,
                    frameDescription.Width, frameDescription.Height);
                // 將影格每一個 pixel 之 BGRA 值儲存至 byteData 陣列
                colorFrame.CopyConvertedFrameDataToArray(byteData,
                    ColorImageFormat.Bgra);
                // 將 byteData 陣列的資料存入 wbData
                wbData.WritePixels(bitmapRect, byteData, (int)
                    (frameDescription.Width * frameDescription.BytesPerPixel),
                    0);
            }
        }

        private async void KinectFaceIdentify_Loaded(object sender, 
            RoutedEventArgs e)
        {
            // 指定 ColorImage 影像來源為 wbData (WriteableBitmap 物件)
            ColorImage.Source = wbData;

            // 顯示訊息在 Result
            Result.Text += "Kinect 傳送影像";
            
            // 如果群組已存在則秀出訊息，不存在則提示後，結束程式
            try
            {
                IList<Person> persons = await 
                    faceClient.PersonGroupPerson.ListAsync(personGroupId);
                // 顯示訊息在 About
                About.Text = "有人進入 2 公尺範圍內，自動觸發拍照、辨識";
                  //  + "、問候的動作";
                /*About.Text += "\n使用 " + faceSubscriptionKeyHide + " 之 "
                    + personGroupId + " 群組\n可辨識 "
                    + persons.Count() + " 人：";
                 
                for (int i = 0; i < persons.Count; i++)
                {
                    About.Text += persons[i].Name + "   ";
                }
                */
            }
            catch
            {
                About.Text = faceSubscriptionKeyHide + " " + personGroupId 
                    + " 群組不存在，即將結束程式";
                MessageBox.Show("結束程式！");
                Application.Current.Shutdown();
            }
            About.ScrollToEnd();    // 讓捲軸移至最新顯示文字處
        }

        private void KinectFaceIdentify_Unloaded(object sender, 
            RoutedEventArgs e)
        {
            // 取消監聽 ColorFrameReader 之 FrameArrived 事件
            colorFrameReader.FrameArrived -= ColorFrameReader_FrameArrived;
            // 取消監聽 BodyFrameReader 之 FrameArrived 事件
            bodyFrameReader.FrameArrived -= BodyFrameReader_FrameArrived;
            // 取消監聽 Shoot 之 Detected 事件
            shoot.Detected -= Shoot_Detected;
            // 停止 Kinect Sensor
            sensor.Close();
        }
    }
}
