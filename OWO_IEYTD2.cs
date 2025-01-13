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
        public class bhaptics_PlayerExplosionImpact
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                //tactsuitVr.LOG("ExplosionImpact");
                //tactsuitVr.LOG(damageData.DeathDescription);
                owoSkin.Feel("ExplosionFace");
            }
        }

        [HarmonyPatch(typeof(Player), "OnLaserImpact")]
        public class bhaptics_PlayerLaserImpact
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                //tactsuitVr.LOG(damageData.DamageRate.ToString());
                if (!owoSkin.IsPlaying("HitByLaser"))
                    { owoSkin.Feel("HitByLaser"); }
            }
        }

        [HarmonyPatch(typeof(Player), "OnPenetratorImpact")]
        public class bhaptics_PlayerPenetratorImpact
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                //tactsuitVr.LOG("PenetratorImpact");
                //tactsuitVr.LOG(damageData.DeathDescription);
                owoSkin.Feel("ArrowPierce");
            }
        }

        [HarmonyPatch(typeof(Player), "OnShotImpact")]
        public class bhaptics_PlayerShotImpact
        {
            [HarmonyPostfix]
            public static void Postfix(Vector3 impactDirection, DamageData damageData)
            {
                //tactsuitVr.LOG("x: " + impactDirection.x.ToString() + " y: " + impactDirection.y.ToString() + " z: " + impactDirection.z.ToString());
                //tactsuitVr.LOG(damageData.DeathDescription);
                owoSkin.Feel("BulletHit");
            }
        }

        [HarmonyPatch(typeof(Player), "AddDamage")]
        public class bhaptics_PlayerAddDamage
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                String damageName = damageData.name;
                if (damageName.Contains("GasDamage")|damageName.Contains("Suffocation") | damageName.Contains("PoisonGas"))
                {
                    //owoSkin.StartHeartBeat();
                    if (!owoSkin.IsPlaying("GasDeath")) { owoSkin.Feel("GasDeath"); }
                    return;
                }
                if (damageName.Contains("Flame") | damageName.Contains("Fire"))
                {
                    //tactsuitVr.StartHeartBeat();
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
        public class bhaptics_PlayerRemoveDamage
        {
            [HarmonyPostfix]
            public static void Postfix(DamageData damageData)
            {
                owoSkin.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(Player), "Kill")]
        public class bhaptics_PlayerKill
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StopThreads();
            }
        }

        #endregion

        #region Player interactables


        [HarmonyPatch(typeof(Shooter), "LoadProjectile")]
        public class bhaptics_LoadProjectile
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //tactsuitVr.LOG("Load shooter");
                shooterLoaded = true;
            }
        }
/*
        [HarmonyPatch(typeof(Shooter), "UnloadProjectile")]
        public class bhaptics_UnloadProjectile
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
        public class bhaptics_UseGunbow
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
                    //tactsuitVr.LOG("Interactable name: " + __instance.name);
                    //tactsuitVr.LOG(" ");
                    owoSkin.GunRecoil(isRight, 0.6f);
                }
            }
        }

        /*
        [HarmonyPatch(typeof(Shooter), "Shoot")]
        public class bhaptics_Shoot
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
        public class bhaptics_InteractableGrab
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
        public class bhaptics_InteractableRelease
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
        public class bhaptics_InteractableExit
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
        public class bhaptics_EatEdible
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Eating");
            }
        }

        [HarmonyPatch(typeof(Mouth), "OnTriggerEnter")]
        public class bhaptics_EnterMouth
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
                    //tactsuitVr.LOG("RigidBodyName: " + objectName);
                }
            }
        }

        [HarmonyPatch(typeof(Explosive), "Explode")]
        public class bhaptics_ExplosiveExplode
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
        public class bhaptics_BeginTargetSearch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StartNeckTingle();
            }
        }

        [HarmonyPatch(typeof(Targetter), "EndTargetSearch")]
        public class bhaptics_EndTargetSearch
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
                public class bhaptics_updateBackstageDebug
                {
                    [HarmonyPostfix]
                    public static void Postfix(int index)
                    {
                        tactsuitVr.LOG("AnimationEvent: " + index.ToString());
                        tactsuitVr.LOG(" ");
                        //tactsuitVr.PlaybackHaptics("ExplosionUp");
                    }
                }

                [HarmonyPatch(typeof(AnimationEventListener), "TriggerResponse")]
                public class bhaptics_TriggerResponse
                {
                    [HarmonyPostfix]
                    public static void Postfix(string key)
                    {
                        tactsuitVr.LOG("AnimationTriggerResponse: " + key);
                        tactsuitVr.LOG(" ");
                        //tactsuitVr.PlaybackHaptics("ExplosionUp");
                    }
                }
        */
        #endregion

        #region Chapter 2: Private Jet

        [HarmonyPatch(typeof(JetLightningStrike), "PerformLightningStrike")]
        public class bhaptics_LightningStrike
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Thunder");
            }
        }

        [HarmonyPatch(typeof(JetMissileImpact), "PlayImpact")]
        public class bhaptics_MissileImpact
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
        public class bhaptics_StartElevatorHaptics
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("ElevatorTingle");
            }
        }

        /*
        [HarmonyPatch(typeof(ElevatorController), "StopHaptics")]
        public class bhaptics_StopElevatorHaptics
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopNeckTingle();
            }
        }
        */

        [HarmonyPatch(typeof(ElevatorController), "FallToPosition")]
        public class bhaptics_ElevatorFallToPosition
        {
            [HarmonyPostfix]
            public static void Postfix(Transform transform)
            {
                //tactsuitVr.LOG("FallToPosition");
                owoSkin.Feel("ElevatorFall");
            }
        }

        [HarmonyPatch(typeof(ElevatorController), "Fall")]
        public class bhaptics_ElevatorFall
        {
            [HarmonyPostfix]
            public static void Postfix(float amount)
            {
                //tactsuitVr.LOG("Fall: " + amount.ToString());
                owoSkin.Feel("ElevatorFall");
            }
        }


        #endregion
    }
}
