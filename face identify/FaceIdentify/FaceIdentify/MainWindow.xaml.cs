// FaceIdentify 2019/05/08 FELab
// Windows 10 Visual Studio Community 2017
// 群組、人、人臉、訓練，以達成辨識群組中人的能力
// 讀取一個 Face API 的 Subscription Key 已有多少群組：
// await faceClient.PersonGroup.ListAsync();
// 讀取 personGroupId 群組內所有的個人：
// await faceClient.PersonGroupPerson.ListAsync(personGroupId);
// 建立群組：
// await faceClient.PersonGroup.CreateAsync(personGroupId, personGroupId);
// 刪除群組：
// await faceClient.PersonGroup.DeleteAsync(personGroupId);
// 建立個人(群組加入個人)：
// await faceClient.PersonGroupPerson.CreateAsync(
//         personGroupId, personNames[i]);
// 刪除個人：
// await faceClient.PersonGroupPerson.DeleteAsync(
//         personGroupId, persons[i].PersonId);
// 加入人臉：
// await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(
//         personGroupId, person.PersonId, s);
// 訓練群組：
// await faceClient.PersonGroup.TrainAsync(personGroupId);
// 查詢訓練群組進度：
// await faceClient.PersonGroup.GetTrainingStatusAsync(personGroupId);
// 偵測人臉：
// await faceClient.Face.DetectWithStreamAsync(
//         s, returnFaceAttributes: requiedFaceAttributes);
// 辨識人臉：
// await faceClient.Face.IdentifyAsync(faceIds, personGroupId);
// 讀取群組內指定個人的資料：
// await faceClient.PersonGroupPerson.GetAsync(personGroupId, candidateId);

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
using System.IO;                             // 使用 Stream、FileStream 類別

namespace FaceIdentify
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        // 請使用自己的 Face API Subscription Key
        private const string faceSubscriptionKey = 
            "897857545c7c4eaf91fec1dd67d62c60";

        // 請使用自己的 Face API Subscription Key 之 Endpoint
        private const string faceEndpoint =
            "https://westus.api.cognitive.microsoft.com/";

        // 使用 Face API 的金鑰，建立微軟雲端臉部辨識服務連線
        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(faceSubscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        private string fileName;    // 一個影像檔的完整名稱
        BitmapImage bitmapSource;   // BitmapImage 物件參考

        // 秀在畫面的資料部份隱藏之 Face API Subscription Key
        private const string faceSubscriptionKeyHide = 
            "897857xxxxxxxxxxxxxxxxxxxxd62c60";

        // 群組命名不能使用英文大寫字母，指定欲建立或已建立的群組名稱
        private string personGroupId = "fguai";

        // personGroupId 指定的群組是/否存在：true/false
        private bool isGroupExist = false;

        // personGroupId 指定的群組已加入人數
        private int personCount = 0;

        // 儲存預設的 桌面\ChenPhoto\Name.txt 所有 person 的名字 
        private string[] personNames;

        // 是/否要顯示從 Name.txt 讀入的人名：true/false
        private bool isShowName = true;

        // 桌面之 ChenPhoto 檔案夾的完整名稱
        private string path = Environment.GetFolderPath(
            Environment.SpecialFolder.DesktopDirectory) + @"\fguai\";

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
        }

        private async void FaceIdentify_Loaded(object sender,
            RoutedEventArgs e)
        {
            // 操作說明
            Result.Text = "TRAIN (Group)\t訓練 " + personGroupId + " 群組"
                + "\n【按一次 TRAIN (Group)，訓練一人】"
                + "\nCREATE (Group)\t建立 " + personGroupId + " 群組"
                + "\nDELETE (Group)\t刪除 " + personGroupId + " 群組"
                + "\nBROWSE (Face)\t挑選圖檔"
                + "\nDETECT (Face)\t辨識人臉"
                + "\nERASE (Person)\t刪除指定名字的個人"
                + "\nName： \t填入欲刪除的個人名字\n";

            // 秀出此 Subscription Key 已產生之群組數目與群組名字，
            // 如果 personGroupId 指定的群組已存在，秀出此群組所有個人名字，
            // 將群組成員的人數儲存於全域變數 personCount；
            // 如果 personGroupId 指定的群組不存在，則提示使用者先建立群組
            try
            {
                // 串列 groups 儲存所有群組資料，Count 屬性可取得元素個數，
                // groups[i].Name 可取得第 i 個群組的名稱
                IList<PersonGroup> groups =
                    await faceClient.PersonGroup.ListAsync();
                Result.Text += "\nSubscription Key " 
                    + faceSubscriptionKeyHide 
                    + "\n有 " + groups.Count + " 個群組";
                for (int i = 0; i < groups.Count; i++)
                    Result.Text += "\n" + groups[i].Name;

                // 串列 persons 儲存 personGroupId 群組內所有個人資料，
                // Count 屬性可取得元素個數，
                // persons[j].Name 可取得第 j 個個人的名稱
                IList<Person> persons = await 
                    faceClient.PersonGroupPerson.ListAsync(personGroupId);
                Result.Text += "\n" + personGroupId + " 群組有 "
                    + persons.Count + " 人";
                for (int j = 0; j < persons.Count; j++)
                    Result.Text += "\n" + persons[j].Name;
                isGroupExist = true;
                personCount = persons.Count;
            }
            catch
            {
                Result.Text += "\n" + personGroupId
                    + " 群組不存在，\n請按 CREATE (Group) 建立群組";
            }
            Result.ScrollToEnd();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            // 瀏覽以挑選 jpg 或 png、gif、bmp 等其他格式影像檔，
            // 如果挑選影像檔沒有成功，則結束函式，
            // 如果挑選影像檔成功，則將檔名存入 fileName
            var openDlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JPEG Image(*.jpg)|*.jpg|All files (*.*)|*.*"
            };
            bool? result = openDlg.ShowDialog(this);
            if (!(bool)result)
            {
                return; // 結束函式
            }

            fileName = openDlg.FileName;    // 儲存挑選的檔案之完整名稱

            // 如果挑選影像檔成功，則在 ColorImage 秀出影像，
            // 並且在 Result 顯示訊息
            Uri fileUri = new Uri(fileName);
            bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();
            ColorImage.Source = bitmapSource;
            Result.Text += "\n\n選定圖檔 " + fileName;
            Result.ScrollToEnd();
        }

        private async void Detect_Click(object sender, RoutedEventArgs e)
        {
            // 如果尚未選定影像檔，提示後，結束函式
            if (fileName == null)
            {
                Result.Text += "\n尚未選定影像檔，無法偵測，請先挑選檔案";
                Result.ScrollToEnd();
                return;
            }

            // 辨識是否為群組中的某人，分為二個步驟
            // 首先偵測圖像中的人臉，
            // 然後將人臉逐一送至群組比對
            if (isGroupExist)
            {
                // 指定傳回來的屬性資料包括年齡和性別
                IList<FaceAttributeType> requiedFaceAttributes = 
                    new FaceAttributeType[]
                {
                    FaceAttributeType.Age,
                    FaceAttributeType.Gender
                };
                FaceRectangle[] faceRects;      // 儲存臉部方框座標的陣列
                FaceAttributes[] faceAttrs;     // 儲存屬性值的陣列

                // 指定筆刷顏色依序為紅、橙、黃、綠、藍、靛、紫
                SolidColorBrush[] brushes = { Brushes.Red, Brushes.Orange,
                    Brushes.Yellow, Brushes.Green, Brushes.Blue,
                    Brushes.Indigo, Brushes.Purple };

                using (Stream s = File.OpenRead(fileName))
                {
                    // 串列 faces 儲存圖像中偵測到的每一個人臉資料，
                    // faces 每個元素的 FaceRectangle 可取得人臉方框座標，
                    // faces 每個元素的 FaceAttributes 可取得人臉年齡和性別
                    // 屬性值
                    IList<DetectedFace> faces = await
                        faceClient.Face.DetectWithStreamAsync(
                            s, returnFaceAttributes: requiedFaceAttributes);

                    // 將每個人臉方框座標存入陣列 faceRects，
                    // 將每個人臉年齡和性別屬性值存入陣列 faceAttrs
                    faceRects = faces.Select(
                        face => face.FaceRectangle).ToArray();
                    faceAttrs = faces.Select(
                        face => face.FaceAttributes).ToArray();

                    // 顯示偵測結果
                    Result.Text += "\n偵測完成！\n" + faceRects.Length +
                        " 個人臉被偵測出";

                    // 依人臉尺寸大小次序，在偵測出的每個人臉畫出方框，
                    // 顏色依序為紅、橙、黃、綠、藍、靛、紫、紅、橙、黃．．．
                    if (faceRects.Length > 0)
                    {
                        DrawingVisual visual = new DrawingVisual();
                        DrawingContext drawingContext = visual.RenderOpen();
                        drawingContext.DrawImage(bitmapSource,
                            new Rect(0, 0, bitmapSource.Width,
                            bitmapSource.Height));
                        double dpi = bitmapSource.DpiX;
                        double resizeFactor = 96 / dpi;
                        FaceRectangle faceRect;

                        for (int i = 0; i < faceRects.Length; i++)
                        {
                            faceRect = faceRects[i];
                            drawingContext.DrawRectangle(
                                Brushes.Transparent,
                                new Pen(brushes[i % 7], 2),
                                new Rect(faceRect.Left * resizeFactor,
                                faceRect.Top * resizeFactor,
                                faceRect.Width * resizeFactor,
                                faceRect.Height * resizeFactor)
                            );
                        }
                        drawingContext.Close();

                        RenderTargetBitmap faceWithRectBitmap =
                            new RenderTargetBitmap(
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
                        faces.Select(face => face.FaceId.Value).ToList();

                    // 人臉與群組的某人相似度最高，而且信心值大於
                    // 自訂門檻值，則視為群組中的某人；
                    // 如果人臉與群組中人的相似度最高者，信心值都未超過
                    // 自訂門檻值，則視為不是群組中人
                    double confidenceThreshold = 0.6;    // 自訂門檻值
                    int k = 0;

                    // faceClient 做人臉辨識限制 1~10 人
                    if (faces.Count >= 1 && faces.Count <= 10)
                    {
                        // 串列 results 儲存每個人臉與群組的比對結果
                        // results 每個元素的串列屬性 Candidates 依序儲存
                        // 群組中與該人臉相似度較高者的資料(超過預設門檻)，
                        // Candidates 的 Count 屬性可取得相似度超過預設門檻
                        // 的個數，Candidates 每個元素的 Confidence 屬性，
                        // 可取得其信心值，PersonId 屬性，可取得其 Guid
                        IList<IdentifyResult> results = await
                            faceClient.Face.IdentifyAsync(faceIds,
                                personGroupId);

                        // 圖像人數二個或以上，才印出臉部方框顏色說明
                        if (faces.Count > 1)
                            Result.Text += 
                                "\n依紅、橙、黃、綠、藍、靛、紫顏色順序";

                        // 印出自訂門檻值
                        Result.Text += "\n自訂相似度信心門檻值 " 
                            + confidenceThreshold;

                        foreach (var identifyResult in results)
                        {
                            // 群組中沒有人與該人臉相似度超過預設門檻(0.5)
                            if (identifyResult.Candidates.Count == 0)
                            {
                                Result.Text += "\n不認識， 0.5 以下";
                            }
                            else
                            {
                                // confidenceValue 儲存群組中與該人臉相似度
                                // 最高者的信心值
                                double confidenceValue = 
                                    identifyResult.Candidates[0].Confidence;

                                // 信心值是否超過自訂門檻值
                                if (confidenceValue > confidenceThreshold)
                                {
                                    // candidateId 儲存群組中與該人臉相似度
                                    // 最高者的 Guid
                                    Guid candidateId = 
                                        identifyResult.Candidates[0].PersonId;

                                    // person 儲存群組中與該人臉相似度
                                    // 最高者的資料，其 Name 屬性可取得名稱
                                    Person person = await
                                        faceClient.PersonGroupPerson.GetAsync(
                                            personGroupId, candidateId);

                                    // 印出名稱
                                    Result.Text += "\n" + person.Name + "， " 
                                        + confidenceValue;
                                }
                                else
                                    Result.Text += "\n不認識， " 
                                        +confidenceValue;
                            }
                            // 印出年齡、性別
                            Result.Text += "， " + faceAttrs[k].Age + " 歲"
                                + "， " + faceAttrs[k].Gender;
                            k++;
                        }
                    }
                    else if (faces.Count > 10)
                        Result.Text += "\n超過 10 人，不顯示個別資料";
                }
            }
            else
                Result.Text += "\n" + personGroupId
                    + " 群組不存在，無法做人臉辨識";
            Result.ScrollToEnd();
        }

        private async void Erase_Click(object sender, RoutedEventArgs e)
        {
            // 如果群組或個人不存在則提醒無法刪除，群組與個人存在則刪除個人
            bool isPersonErased = false;
            if (isGroupExist)
            {
                IList<Person> persons = await
                    faceClient.PersonGroupPerson.ListAsync(
                        personGroupId);
                for (int i = 0; i < persons.Count; i++)
                {
                    if (ErasedPerson.Text == persons[i].Name)
                    {
                        await faceClient.PersonGroupPerson.DeleteAsync(
                            personGroupId, persons[i].PersonId);
                        isPersonErased = true;
                        personCount--;
                        Result.Text += "\n刪除 " + ErasedPerson.Text 
                            +"，現在群組有 " + personCount + " 人";

                    }
                }
                if (isPersonErased == false)
                    Result.Text += 
                        "\n" + ErasedPerson.Text + " 不存在，無法刪除！";
            }
            else
                Result.Text += "\n" + personGroupId
                    + " 群組不存在，無法做個人刪除";
            ErasedPerson.Text = "";       // 消除 ErasedPerson 上填入的人名
            Result.ScrollToEnd();
        }

        // 自行定義的函式，開啟桌面 ChenPhoto 檔案夾內的 Name.txt 檔案，
        // 如果 ChenPhoto 檔案夾不存在，或是預設檔案不存在，則提示後結束程式
        // 如果 ChenPhoto\Name.txt 檔案存在，讀取檔案內容，
        // 存入字串陣列 personNames
        private void GetFile()
        {
            // fileName 儲存桌面的 ChenPhoto\Name.txt 檔案完整名稱
            string fileName = path + "Name.txt";

            // 測試桌面之 ChenPhoto 檔案夾是否存在，如果不存在，提示後結束程式
            if (!Directory.Exists(path))
            {
                MessageBox.Show("桌面找不到預設檔案夾 ChenPhoto，結束程式！");
                Application.Current.Shutdown();
            }

            // 測試桌面 ChenPhoto 檔案夾內 Name.txt 檔案是否存在，
            // 如果不存在，提示後結束程式
            // 如果存在，則將檔案內的人名，逐一讀入字串陣列 personNames
            if (!File.Exists(fileName))
            {
                MessageBox.Show("找不到預設檔案 Name.txt，結束程式！");
                Application.Current.Shutdown();
            }
            else
            {
                // 開啟 Name.txt 檔案，並限定只能讀取檔案內容
                FileStream fs = new FileStream(fileName, FileMode.Open,
                                                FileAccess.Read);

                // 產生讀取串流，並連接到已開啟的檔案，
                // 指定編碼格式是系統預設編碼，所以 ANSI、Unicode、UTF-8 都行
                StreamReader sr = new StreamReader(fs, Encoding.Default);

                // 宣告 List<string> 變數 names，可以視為可改變長度的字串陣列
                // 將檔案的內容一次一列全部讀出，利用 Add 方法將字串加入
                // names，names 的長度會自動增加
                List<string> names = new List<string>();
                string line;
                while ((line = sr.ReadLine()) != null)
                    names.Add(line);
                fs.Close();     // 關閉檔案

                // 將 List<string> names 的內容轉為字串陣列，存入全域變數的
                // 字串陣列 personNames 中
                personNames = names.ToArray();
            }
        }

        private async void Train_Click(object sender, RoutedEventArgs e)
        {
            // 如果群組不存在則提示產生群組；
            // 如果群組已存在則讀入人名、秀出人名、加入個人、加入影像檔、
            // 啟動訓練
            if (!isGroupExist)
                Result.Text += "\n" + personGroupId
                    + " 群組不存在，無法訓練，請先建立群組";
            else
            {
                // 呼叫自訂的函式 GetFile() 讀入欲加入群組的人名，
                // 如果人名讀取失敗則 GetFile() 會結束程式
                GetFile();

                // GetFile() 執行後，字串陣列 personNames 儲存所有欲加入群組
                // 的個人人名，將人名顯示出來，僅秀出一次
                if (isShowName)
                {
                    Result.Text += "\n讀入 " + personNames.Length + " 個人名";
                    for (int index = 0; index < personNames.Length; index++)
                        Result.Text += "\n" + personNames[index];
                    isShowName = false;
                }

                // 將群組內所有個人資料讀入 persons
                IList<Person> persons =
                    await faceClient.PersonGroupPerson.ListAsync(
                        personGroupId);

                // 所有欲加入群組的個人，逐ㄧ比對，如果還有人未在群組內，
                // 則以一次加入一人的方式，逐一加入群組
                int i = 0;                  // 第 i 個欲加入群組
                bool isNeeded = false;      // 第 i 個名字是否需加入群組
                while (!isNeeded && i < personNames.Length)
                {
                    int j = 0;              // 第 j 個群組會員
                    bool isMember = false;  // 第 i 個名字是/否為會員
                    while (!isMember && j < persons.Count)
                    {
                        if (personNames[i] != persons[j].Name)
                            j++;
                        else
                            isMember = true;
                    }
                    // 第 i 個名字是會員，那就比對下一個名字
                    // 第 i 個名字不是會員，那就設定為需加入群組
                    if (isMember)
                        i++;
                    else
                        isNeeded = true;
                }

                if (isNeeded)
                {
                    // 第 i 個名字的個人加入群組
                    Person person =
                        await faceClient.PersonGroupPerson.CreateAsync(
                            personGroupId, personNames[i]);
                    Result.Text += "\n加入 " + personNames[i];

                    // 第 i 個名字的個人加入 .jpg 人臉影像檔
                    // 注意：第 0 個名字的人臉影像檔案夾為 person0001
                    string subDirectory1 = "person"
                        + (i + 1).ToString("0000");

                    // 測試檔案夾是否存在，如果不存在則提示後結束程式
                    // (path 是桌面之 ChenPhoto 檔案夾的完整名稱)
                    if (!Directory.Exists(path + subDirectory1))
                    {
                        MessageBox.Show("桌面找不到 " + path
                            + subDirectory1 + " 檔案夾，結束程式！");
                        Application.Current.Shutdown();
                    }

                    // 將 .jpg 影像檔加入群組之個人
                    int k = 0;
                    foreach (string imagePath in
                        Directory.GetFiles(path + subDirectory1, "*.jpg"))
                    {
                        using (Stream s = File.OpenRead(imagePath))
                        {
                            await
                            faceClient.PersonGroupPerson.AddFaceFromStreamAsync(
                                personGroupId, person.PersonId, s);
                        }
                        k++;
                    }
                    Result.Text += "\n加入 " + k + " 個 " + personNames[i]
                        + " 圖檔";

                    // 訓練群組
                    await faceClient.PersonGroup.TrainAsync(
                        personGroupId);
                    TrainingStatus trainingStatus = null;
                    while (true)
                    {
                        trainingStatus = await
                            faceClient.PersonGroup.GetTrainingStatusAsync(
                                personGroupId);
                        if (trainingStatus.Status != 
                            TrainingStatusType.Running)
                        {
                            break;
                        }
                        await Task.Delay(1000);
                    }
                    personCount++;
                    Result.Text += "\n訓練完成，現在群組有 "
                        + personCount + " 人";
                }
                else
                    Result.Text += "\n所有人皆已加入群組";
            }
            Result.ScrollToEnd();
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            // 底下四種寫法都可以判斷群組是否存在
            // 1.IList<Person> persons = await
            //       faceClient.PersonGroupPerson.ListAsync(personGroupId);
            // 2.await faceClient.PersonGroupPerson.ListAsync(personGroupId);
            // 3.IList<PersonGroup> group = await 
            //       faceClient.PersonGroup.ListAsync(personGroupId);
            // 4.await faceClient.PersonGroup.ListAsync(personGroupId);
            try
            {
                // 讀取 personGroupId 群組內的個人資料，如果群組存在，
                // 讀取成功，則提醒無法建立；
                // 如果群組不存在，讀取失敗，則跳到 catch 區塊，
                // 建立 personGroupId 群組
                IList<Person> persons = await
                   faceClient.PersonGroupPerson.ListAsync(personGroupId);
                Result.Text += "\n" + personGroupId 
                    + " 群組已存在，無法建立";
            }
            catch
            {
                await faceClient.PersonGroup.CreateAsync(
                    personGroupId, personGroupId);
                Result.Text += "\n建立 " + personGroupId + "群組";
                isGroupExist = true;
            }
            Result.ScrollToEnd();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            // 底下四種寫法都可以判斷群組是否存在
            // 1.IList<Person> persons = await
            //       faceClient.PersonGroupPerson.ListAsync(personGroupId);
            // 2.await faceClient.PersonGroupPerson.ListAsync(personGroupId);
            // 3.IList<PersonGroup> group = await 
            //       faceClient.PersonGroup.ListAsync(personGroupId);
            // 4.await faceClient.PersonGroup.ListAsync(personGroupId);
            try
            {
                // 讀取 personGroupId 群組資料，如果群組存在，讀取成功，
                // 讓使用者確認是否真的要刪除群組，刪除或取消刪除；
                // 如果群組不存在，讀取失敗，則跳到 catch 區塊，
                // 提醒無法刪除群組
                IList<PersonGroup> group = await 
                    faceClient.PersonGroup.ListAsync(personGroupId);
                MessageBoxResult result = MessageBox.Show(
                    "確定要刪除 " + personGroupId + " 群組？"
                    , "刪除群組或取消", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    Result.Text += "\n取消刪除 " + personGroupId + " 群組";
                }
                else
                {
                    await faceClient.PersonGroup.DeleteAsync(personGroupId);
                    Result.Text += "\n刪除 " + personGroupId + " 群組";
                    isGroupExist = false;
                    // 再次秀出此 Subscription Key 之群組數目與群組名字，
                    // 確認 personGroupId 群組是否還存在
                    IList<PersonGroup> groups = await
                        faceClient.PersonGroup.ListAsync();
                    Result.Text += "\n\nSubscription Key "
                        + faceSubscriptionKeyHide + " 有 "
                        + groups.Count + " 個群組";
                    for (int i = 0; i < groups.Count; i++)
                        Result.Text += "\n" + groups[i].Name;
                    Result.Text += "\n" + personGroupId 
                        + " 群組不存在，\n請按 CREATE (Group) 建立群組";
                }
            }
            catch
            {
                Result.Text += "\n" + personGroupId
                    + " 群組不存在，無法刪除，" 
                    + "\n請按 CREATE(Group) 建立群組";;
            }
            Result.ScrollToEnd();
        }
    }
}
