using System.Net;
using MelonLoader;
using OWOGame;

namespace MyOWOSkin
{
    public class OWOSkin
    {
        public bool suitDisabled = true;
        public bool systemInitialized = false;
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);
        private static ManualResetEvent NeckTingle_mrse = new ManualResetEvent(false);
        private static ManualResetEvent TelekinesisR_mrse = new ManualResetEvent(false);
        private static ManualResetEvent TelekinesisL_mrse = new ManualResetEvent(false);
        public Dictionary<String, Sensation> FeedbackMap = new Dictionary<String, Sensation>();


        public OWOSkin()
        {
            InitializeOWO();
        }

        private async void InitializeOWO()
        {
            LOG("Initializing OWO skin");

            var gameAuth = GameAuth.Create(AllBakedSensations()).WithId("0"); ;

            OWO.Configure(gameAuth);
            string[] myIPs = getIPsFromFile("OWO_Manual_IP.txt");
            if (myIPs.Length == 0) await OWO.AutoConnect();
            else
            {
                await OWO.Connect(myIPs);
            }

            if (OWO.ConnectionState == ConnectionState.Connected)
            {
                suitDisabled = false;
                LOG("OWO suit connected.");
                OWO.Send(OWOGame.BakedSensation.Dart);
            }
            if (suitDisabled) LOG("OWO is not enabled?!?!");
        }

        public BakedSensation[] AllBakedSensations()
        {
            var result = new List<BakedSensation>();

            foreach (var sensation in FeedbackMap.Values)
            {
                if (sensation is BakedSensation baked)
                {
                    LOG("Registered baked sensation: " + baked.name);
                    result.Add(baked);
                }
                else
                {
                    LOG("Sensation not baked? " + sensation);
                    continue;
                }
            }
            return result.ToArray();
        }

        public string[] getIPsFromFile(string filename)
        {
            List<string> ips = new List<string>();
            string filePath = Directory.GetCurrentDirectory() + "\\Mods\\" + filename;
            if (File.Exists(filePath))
            {
                LOG("Manual IP file found: " + filePath);
                var lines = File.ReadLines(filePath);
                foreach (var line in lines)
                {
                    IPAddress address;
                    if (IPAddress.TryParse(line, out address)) ips.Add(line);
                    else LOG("IP not valid? ---" + line + "---");
                }
            }
            return ips.ToArray();
        }

        ~OWOSkin()
        {
            LOG("Destructor called");
            DisconnectOwo();
        }

        public void DisconnectOwo()
        {
            LOG("Disconnecting Owo skin.");
            OWO.Disconnect();
        }

        public void HeartBeatFunc()
        {
            while (true)
            {
                HeartBeat_mrse.WaitOne();
                //bHapticsLib.bHapticsManager.PlayRegistered("HeartBeat");
                Thread.Sleep(1000);
            }
        }

        public void NeckTingleFunc()
        {
            while (true)
            {
                NeckTingle_mrse.WaitOne();
                //bHapticsLib.bHapticsManager.PlayRegistered("NeckTingleShort");
                Thread.Sleep(2050);
            }
        }

        public void TelekinesisRFunc()
        {
            while (true)
            {
                TelekinesisR_mrse.WaitOne();
                //bHapticsLib.bHapticsManager.PlayRegistered("Telekinesis_R");
                Thread.Sleep(2050);
            }
        }
        public void TelekinesisLFunc()
        {
            while (true)
            {
                TelekinesisL_mrse.WaitOne();
                //bHapticsLib.bHapticsManager.PlayRegistered("Telekinesis_L");
                Thread.Sleep(2050);
            }
        }

        //public OWOSkin()
        //{
        //    LOG("Initializing suit");
        //    suitDisabled = false;
        //    RegisterAllTactFiles();
        //    LOG("Starting HeartBeat and NeckTingle thread...");
        //    Thread HeartBeatThread = new Thread(HeartBeatFunc);
        //    HeartBeatThread.Start();
        //    Thread NeckTingleThread = new Thread(NeckTingleFunc);
        //    NeckTingleThread.Start();
        //    Thread TeleRThread = new Thread(TelekinesisRFunc);
        //    TeleRThread.Start();
        //    Thread TeleLThread = new Thread(TelekinesisLFunc);
        //    TeleLThread.Start();
        //}

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }

        void RegisterAllTactFiles()
        {
            string configPath = Directory.GetCurrentDirectory() + "\\Mods\\OWO";
            DirectoryInfo d = new DirectoryInfo(configPath);
            FileInfo[] Files = d.GetFiles("*.owo", SearchOption.AllDirectories);
            for (int i = 0; i < Files.Length; i++)
            {
                string filename = Files[i].Name;
                string fullName = Files[i].FullName;
                string prefix = Path.GetFileNameWithoutExtension(filename);
                if (filename == "." || filename == "..")
                    continue;
                string tactFileStr = File.ReadAllText(fullName);
                try
                {
                    Sensation test = Sensation.Parse(tactFileStr);
                    FeedbackMap.Add(prefix, test);
                }
                catch (Exception e) { LOG(e.ToString()); }

            }

            systemInitialized = true;
        }

        public void Feel(String key, float intensity = 1.0f, float duration = 1.0f)
        {
            OWO.Send(OWOGame.BakedSensation.Dart);

            if (FeedbackMap.ContainsKey(key))
            {
                OWO.Send(FeedbackMap[key]);
            }
            else LOG("Feedback not registered: " + key);
        }

        public void GunRecoil(bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            //var scaleOption = new bHapticsLib.ScaleOption(intensity, duration);
            //var rotationFront = new bHapticsLib.RotationOption(0f, 0f);
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string keyArm = "Recoil" + postfix;
            string keyVest = "RecoilVest" + postfix;
            //bHapticsLib.bHapticsManager.PlayRegistered(keyArm, keyArm, scaleOption, rotationFront);
            //bHapticsLib.bHapticsManager.PlayRegistered(keyVest, keyVest, scaleOption, rotationFront);
        }

        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
        }

        public void StartNeckTingle()
        {
            NeckTingle_mrse.Set();
        }

        public void StopNeckTingle()
        {
            NeckTingle_mrse.Reset();
        }

        public void StartTelekinesis(bool isRight)
        {
            if (isRight) { TelekinesisR_mrse.Set(); }
            else { TelekinesisL_mrse.Set(); }
        }

        public void StopTelekinesis(bool isRight)
        {
            if (isRight) { TelekinesisR_mrse.Reset(); StopHapticFeedback("Telekinesis_R"); }
            else { TelekinesisL_mrse.Reset(); StopHapticFeedback("Telekinesis_L"); }
        }

        public bool IsPlaying(String effect)
        {
            return false; //Puesto a proposito para tener un return.
            //return bHapticsLib.bHapticsManager.IsPlaying(effect);
        }

        public void StopHapticFeedback(String effect)
        {
            //bHapticsLib.bHapticsManager.StopPlaying(effect);
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            foreach (String key in FeedbackMap.Keys)
            {
                //bHapticsLib.bHapticsManager.StopPlaying(key);
            }
        }

        public void StopThreads()
        {
            StopHeartBeat();
            StopNeckTingle();
            StopTelekinesis(true);
            StopTelekinesis(false);
        }


    }
}
