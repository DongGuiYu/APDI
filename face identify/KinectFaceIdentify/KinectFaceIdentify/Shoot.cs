// Shoot.cs for KinectFaceIdentify 2019/01/02 FELab
// Windows 10 Visual Studio Community 2017
// Shoot 類別偵測 "有人靠近 2 公尺內"
// Shoot 類別偵測到正確姿勢後觸發 Detected 事件
// Shoot 類別判定姿勢是否正確的函式為 Detection()
// Detection() 利用 Head、SpineShoulder、SpineMid
// 三個關節座標做判斷
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;     // 使用 Kinect SDK 2.0

namespace KinectFaceIdentify
{
    class Shoot
    {
        // 間隔時間門檻 (0.5 秒)
        TimeSpan TimeThreshold = new TimeSpan(5000000);
        const double DistThreshold1 = 2; // Head 距離門檻(2米)
        const double DistThreshold2 = 2; // SpineShoulder 距離門檻(2米)
        const double DistThreshold3 = 2; // SpineMid 距離門檻(2米)
        // 擺出姿勢的起始時間
        TimeSpan initialTimestamp = new TimeSpan(-1);
        // 是否偵測 "有人靠近 2 公尺內"   true:偵測   false:不偵測
        bool detecting = true;
        // Shoot 類別偵測到正確姿勢後觸發 Detected 事件
        public event EventHandler Detected;
        // 偵測 Shoot 姿勢的函式為 Detection
        // 利用 Head、SpineShoulder、SpineMid 三個關節座標
        public void Detection(Joint head, Joint spineShoulder,
            Joint spineMid, TimeSpan currentTimestamp)
        {
            // 如果尚未偵測出姿態，將起始時間設為骨架傳來的時間
            if (initialTimestamp == new TimeSpan(-1))
                initialTimestamp = currentTimestamp;
            // 偵測是否距離小於 2 公尺
            if ((head.Position.Z < DistThreshold1)
                || (spineShoulder.Position.Z < DistThreshold2)
                || (spineMid.Position.Z < DistThreshold3))
            {
                if (((currentTimestamp - initialTimestamp) > TimeThreshold)
                    && (detecting == true))
                {
                    Detected(this, new EventArgs());    // 觸發 Detected 事件
                    detecting = false;  // 持續 "有人靠近2公尺內" 不會重複觸發
                    initialTimestamp = new TimeSpan(-1);
                }
            }
            else if ((head.Position.Z > DistThreshold1 + 0.1)
                && (spineShoulder.Position.Z > DistThreshold2 + 0.1)
                && (spineMid.Position.Z > DistThreshold3 + 0.1))
            {
                if (((currentTimestamp - initialTimestamp) > TimeThreshold)
                    && (detecting == false))
                {
                    detecting = true;   // 不再 "有人靠近2公尺內" 之後重新開始
                    initialTimestamp = new TimeSpan(-1);
                }
            }
        }
    }
}
