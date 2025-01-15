using System.Net;
using Il2CppSG.Phoenix.Assets.Code.Backstage;
using MelonLoader;
using OWOGame;

namespace MyOWOSkin
{
    public class OWOSkin
    {
        public bool suitDisabled = true;
        public bool systemInitialized = false;
        private static bool heartBeatIsActive = false;
        private static bool neckTingleIsActive = false;
        private static bool telekinesisIsActive = false;
        private static bool telekinesisRIsActive = false;
        private static bool telekinesisLIsActive = false;
        public Dictionary<String, Sensation> FeedbackMap = new Dictionary<String, Sensation>();


        public OWOSkin()
        {
            RegisterAllSensationsFiles();
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

        public async Task HeartBeatFuncAsync()
        {
            while (heartBeatIsActive)
            {                
                Feel("HeartBeat");
                await Task.Delay(1000);
            }
        }

        public async Task NeckTingleFunc()
        {
            while (neckTingleIsActive)
            {                
                Feel("NeckTingleShort");
                await Task.Delay(2050);
            }
        }

        public async Task TelekinesisFuncAsync()
        {
            String toFeel = "";

            while (telekinesisRIsActive || telekinesisLIsActive)
            {
                if (telekinesisRIsActive)
                    toFeel = "Telekinesis_R";

                if (telekinesisLIsActive)
                    toFeel = "Telekinesis_L";

                if (telekinesisRIsActive && telekinesisLIsActive)
                    toFeel = "Telekinesis_RL";


                Feel(toFeel);
                await Task.Delay(1000);
            }

            telekinesisIsActive = false;
        }

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }

        void RegisterAllSensationsFiles()
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
            if (FeedbackMap.ContainsKey(key))
            {
                OWO.Send(FeedbackMap[key]);
                LOG("SENSATION: " + key);
            }
            else LOG("Feedback not registered: " + key);
        }

        public void GunRecoil(bool isRightHand, float intensity = 1.0f)
        {
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string recoilWithArm = "Recoil" + postfix;

            Feel(recoilWithArm);
        }

        public void StartHeartBeat()
        {
            if (heartBeatIsActive) return;

            heartBeatIsActive = true;
            HeartBeatFuncAsync();
        }

        public void StopHeartBeat()
        {
            heartBeatIsActive = false;
        }

        public void StartNeckTingle()
        {
            if (neckTingleIsActive) return;

            neckTingleIsActive = true;
            NeckTingleFunc();
        }

        public void StopNeckTingle()
        {
            neckTingleIsActive = false;
        }

        public void StartTelekinesis(bool isRight)
        {
            if(isRight)
                telekinesisRIsActive = true;

            if (!isRight)
                telekinesisLIsActive = true;

            
            if (!telekinesisIsActive)
                TelekinesisFuncAsync();

            telekinesisIsActive = true;
        }

        public void StopTelekinesis(bool isRight)
        {
            if (isRight)
            {
                telekinesisRIsActive = false;
            }
            else
            {
                telekinesisLIsActive = false;
            }
        }

        public bool IsPlaying(String effect)
        {
            return false; //Puesto a proposito para tener un return.
            //return bHapticsLib.bHapticsManager.IsPlaying(effect);
        }

        public void StopAllHapticFeedback()
        {
            StopHeartBeat();
            StopNeckTingle();            
            StopTelekinesis(true);
            StopTelekinesis(false);

            OWO.Stop();
        }


    }
}
