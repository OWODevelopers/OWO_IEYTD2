using MelonLoader;
using HarmonyLib;
using MyOWOSkin;

using UnityEngine;
using Il2CppSG.Phoenix.Assets.Code.InputManagement;
using Il2CppSG.Phoenix.Assets.Code.Interactables;
using Il2CppSG.Phoenix.Assets.Code.WorldAttributes;
using Il2CppSG.Phoenix.Assets.Code.WorldAttributes.PlayerManagement;
using Il2CppSG.Phoenix.Assets.Code.PrivateJet;
using Il2CppSG.Phoenix.Assets.Code.Elevator;
using static MelonLoader.MelonLaunchOptions;

[assembly: MelonInfo(typeof(OWO_IEYTD2.OWO_IEYTD2), "OWO_IEYTD2", "0.0.1", "OWO Game")]
[assembly: MelonGame("Schell Games", "I Expect You To Die 2")]

namespace OWO_IEYTD2
{
    public class OWO_IEYTD2 : MelonMod
    {
        public static OWOSkin owoSkin = null!;
        public static bool shooterLoaded = false;

        public override void OnInitializeMelon()
        {
            owoSkin = new OWOSkin();
            owoSkin.Feel("HeartBeat");
        }

        #region Player hit

        [HarmonyPatch(typeof(Player), "OnExplosionImpact")]
        public class owo_PlayerExplosionImpact
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                owoSkin.LOG("ExplosionImpact");
                owoSkin.LOG(damageData.DeathDescription);
                owoSkin.Feel("ExplosionFace");
            }
        }

        [HarmonyPatch(typeof(Player), "OnLaserImpact")]
        public class owo_PlayerLaserImpact
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                owoSkin.LOG(damageData.DamageRate.ToString());
                if (!owoSkin.IsPlaying("HitByLaser"))
                    { owoSkin.Feel("HitByLaser"); }
            }
        }

        [HarmonyPatch(typeof(Player), "OnPenetratorImpact")]
        public class owo_PlayerPenetratorImpact
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                owoSkin.LOG("PenetratorImpact");
                owoSkin.LOG(damageData.DeathDescription);
                owoSkin.Feel("ArrowPierce");
            }
        }

        [HarmonyPatch(typeof(Player), "OnShotImpact")]
        public class owo_PlayerShotImpact
        {
            [HarmonyPostfix]
            public static void Postfix(Vector3 impactDirection, DamageData damageData)
            {
                owoSkin.LOG("x: " + impactDirection.x.ToString() + " y: " + impactDirection.y.ToString() + " z: " + impactDirection.z.ToString());
                owoSkin.LOG(damageData.DeathDescription);
                owoSkin.Feel("BulletHit");
            }
        }

        [HarmonyPatch(typeof(Player), "AddDamage")]
        public class owo_PlayerAddDamage
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                String damageName = damageData.name;
                if (damageName.Contains("GasDamage")|damageName.Contains("Suffocation") | damageName.Contains("PoisonGas"))
                {
                    owoSkin.StartHeartBeat();
                    if (!owoSkin.IsPlaying("GasDeath")) { owoSkin.Feel("GasDeath"); }
                    return;
                }
                if (damageName.Contains("Flame") | damageName.Contains("Fire"))
                {
                    owoSkin.StartHeartBeat();
                    if (!owoSkin.IsPlaying("FlameThrower")) { owoSkin.Feel("FlameThrower"); }
                    return;
                }
                if (damageName.Contains("PendulumDamage"))
                {
                    owoSkin.Feel("AxeHit");
                    return;
                }
                if (damageName.Contains("Electrocution"))
                {
                    owoSkin.Feel("Electrocution");
                    return;
                }
                if (damageName.Contains("MS_ExplosiveTntDamage"))
                {
                    owoSkin.Feel("ExplosionFace");
                    return;
                }
                if (damageName.Contains("DartDamage")|damageName.Contains("Explosion")|damageName.Contains("Grenade")|
                    damageName.Contains("LaserDamage")|damageName.Contains("CablesCut") | damageName.Contains("ContinuousLaser")|
                    damageName.Contains("LaserSight")| damageName.Contains("JuniperShootDamage") | (damageName == "Death_PrivateJet_EngineDestroyed"))
                    { return; }
                owoSkin.LOG("Unknown Damage: " + damageData.name);
            }
        }

        [HarmonyPatch(typeof(Player), "RemoveDamage")]
        public class owo_PlayerRemoveDamage
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                owoSkin.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(Player), "Kill")]
        public class owo_PlayerKill
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //Mandar sensacion de muerte.
                owoSkin.StopAllHapticFeedback();
            }
        }

        #endregion

        #region Player interactables


        [HarmonyPatch(typeof(Shooter), "LoadProjectile")]
        public class owo_LoadProjectile
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.LOG("Arma cargada!");
                shooterLoaded = true;
            }
        }
/*
        [HarmonyPatch(typeof(Shooter), "UnloadProjectile")]
        public class owo_UnloadProjectile
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("Unload shooter");
                shooterLoaded = false;
            }
        }
*/
        [HarmonyPatch(typeof(Interactable), "OnUse")]
        public class owo_UseGunbow
        {
            [HarmonyPostfix]
            public static void Postfix(Interactable __instance)
            {
                if (!shooterLoaded) { return; }
                if (__instance.name.Contains("Gunbow"))
                {
                    shooterLoaded = false;
                    bool isRight = false;
                    if (__instance.heldHand.HandIndex == 1) { isRight = true; }
                    owoSkin.LOG("Interactable name: " + __instance.name);
                    owoSkin.LOG(" ");
                    owoSkin.GunRecoil(isRight, 0.6f);
                }
            }
        }

        /*
        [HarmonyPatch(typeof(Shooter), "Shoot")]
        public class owo_Shoot
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.LOG("Fire. IsRight: " + lastHandRightHand.ToString());
                tactsuitVr.GunRecoil(lastHandRightHand, 0.6f);
            }
        }
*/
        [HarmonyPatch(typeof(Interactable), "OnInteract")]
        public class owo_InteractableGrab
        {
            [HarmonyPostfix]
            public static void Postfix(VRHandInput hand)
            {
                bool isRight = false;
                if (hand.HandIndex == 1) { isRight = true; }
                if (hand.TelekinesisEnabled) { owoSkin.StartTelekinesis(isRight); }
                else { owoSkin.StopTelekinesis(isRight); }
            }
        }

        [HarmonyPatch(typeof(Interactable), "OnRelease")]
        public class owo_InteractableRelease
        {
            [HarmonyPostfix]
            public static void Postfix(VRHandInput hand)
            {
                bool isRight = false;
                if (hand.HandIndex == 1) { isRight = true; }
                if (hand.TelekinesisEnabled) { owoSkin.StopTelekinesis(isRight); }
            }
        }

        [HarmonyPatch(typeof(Interactable), "OnExit")]
        public class owo_InteractableExit
        {
            [HarmonyPostfix]
            public static void Postfix(VRHandInput hand)
            {
                bool isRight = false;
                if (hand.HandIndex == 1) { isRight = true; }
                owoSkin.StopTelekinesis(isRight);
            }
        }

        [HarmonyPatch(typeof(Edible), "Eat")]
        public class owo_EatEdible
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(Mouth), "OnTriggerEnter")]
        public class owo_EnterMouth
        {
            [HarmonyPostfix]
            public static void Postfix(Mouth __instance, Collider other)
            {
                String objectName = "None";
                try { objectName = other.attachedRigidbody.name; }
                catch { return; }
                if (objectName.Contains("Cigar"))
                {
                    owoSkin.Feel("Smoking");
                }
                else if (objectName.Contains("Glass")|objectName.Contains("Bottle") | objectName.Contains("Cup") | objectName.Contains("Straw"))
                {
                    owoSkin.Feel("Drinking");
                }
                else
                {
                    owoSkin.LOG("RigidBodyName: " + objectName);
                }
            }
        }

        [HarmonyPatch(typeof(Explosive), "Explode")]
        public class owo_ExplosiveExplode
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("ExplosionUp");
            }
        }

        #endregion

        #region Player nervous

        [HarmonyPatch(typeof(Targetter), "BeginTargetSearch")]
        public class owo_BeginTargetSearch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StartNeckTingle();
            }
        }

        [HarmonyPatch(typeof(Targetter), "EndTargetSearch")]
        public class owo_EndTargetSearch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StopNeckTingle();
            }
        }

        #endregion

        #region Chapter 1: Backstage

        /*
                [HarmonyPatch(typeof(AnimationUtils), "InvokeAnimationEvent")]
                public class owo_updateBackstageDebug
                {
                    [HarmonyPostfix]
                    public static void Postfix(int index)
                    {
                        tactsuitVr.LOG("AnimationEvent: " + index.ToString());
                        tactsuitVr.LOG(" ");
                        owoSkin.PlaybackHaptics("ExplosionUp");
                    }
                }

                [HarmonyPatch(typeof(AnimationEventListener), "TriggerResponse")]
                public class owo_TriggerResponse
                {
                    [HarmonyPostfix]
                    public static void Postfix(string key)
                    {
                        tactsuitVr.LOG("AnimationTriggerResponse: " + key);
                        tactsuitVr.LOG(" ");
                        owoSkin.PlaybackHaptics("ExplosionUp");
                    }
                }
        */
        #endregion

        #region Chapter 2: Private Jet

        [HarmonyPatch(typeof(JetLightningStrike), "PerformLightningStrike")]
        public class owo_LightningStrike
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Thunder");
            }
        }

        [HarmonyPatch(typeof(JetMissileImpact), "PlayImpact")]
        public class owo_MissileImpact
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("ExplosionFace");
            }
        }

        #endregion

        #region Chapter 3: Shop


        #endregion

        #region Chapter 4: WineCellar


        #endregion

        #region Chapter 5: Movie Set

        #endregion

        #region Chapter 6: Elevator

        [HarmonyPatch(typeof(ElevatorController), "PlayHaptics")]
        public class owo_StartElevatorHaptics
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("ElevatorTingle");
            }
        }

        /*
        [HarmonyPatch(typeof(ElevatorController), "StopHaptics")]
        public class owo_StopElevatorHaptics
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopNeckTingle();
            }
        }
        */

        [HarmonyPatch(typeof(ElevatorController), "FallToPosition")]
        public class owo_ElevatorFallToPosition
        {
            [HarmonyPostfix]
            public static void Postfix(Transform transform)
            {
                owoSkin.LOG("FallToPosition");
                owoSkin.Feel("ElevatorFall");
            }
        }

        [HarmonyPatch(typeof(ElevatorController), "Fall")]
        public class owo_ElevatorFall
        {
            [HarmonyPostfix]
            public static void Postfix(float amount)
            {
                owoSkin.LOG("Fall: " + amount.ToString());
                owoSkin.Feel("ElevatorFall");
            }
        }


        #endregion
    }
}
