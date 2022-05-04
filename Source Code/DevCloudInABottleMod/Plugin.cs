using BepInEx;
using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.XR;
using Utilla;

namespace DevCloudInABottleMod
{
    [Description("HauntedModMenu")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [ModdedGamemode]
    public class Plugin : BaseUnityPlugin
    {
        /*Assetloading*/
        public static readonly string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static GameObject bottle; // the cloud in a bottle gameobject
        static GameObject particleFolder; // where the particles are placed

        /*Dynamic mod enabling/disabling*/
        static bool modActive = false; // is the mod active
        static bool inModLobby = false; // is the player in a modded lobby

        /*Keybind*/
        static bool canDoubleJump = true; // can the player use the double jump ability
        bool isPrimaryDown; // is the primary button down

        void OnEnable()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
            bottle.SetActive(true);
            particleFolder.SetActive(true);
            if (inModLobby)
            {
                modActive = true;
            }
        }

        void OnDisable()
        {
            Utilla.Events.GameInitialized -= OnGameInitialized;
            bottle.SetActive(false);
            particleFolder.SetActive(false);
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            #pragma warning disable IDE0017 // Simplify object initialization
            particleFolder = new GameObject();
            particleFolder.name = "CloudInABottle_Particles";

            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevCloudInABottleMod.Assets.cloud");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            GameObject cloudinabottle = bundle.LoadAsset<GameObject>("bottle");
            bottle = Instantiate(cloudinabottle);
            GameObject rightHand = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R");
            bottle.transform.SetParent(rightHand.transform, false);
            bottle.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottle.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            bottle.transform.localScale = new Vector3(1f, 1f, 1f);
            bottle.name = "BottleParticles";
        }

        void Update()
        {
            if (GorillaLocomotion.Player.Instance.wasLeftHandTouching || GorillaLocomotion.Player.Instance.wasRightHandTouching) // much better than Instance.IsHandTouching()
            {
                if (!canDoubleJump && modActive)
                {
                    canDoubleJump = true;
                }
            }

            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out isPrimaryDown);

            if (isPrimaryDown && canDoubleJump && modActive)
            {
                canDoubleJump = false;
                float JumpHeightMonke = 10;
                Rigidbody PlayerRigidbody = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>();
                PlayerRigidbody.AddForce(new Vector3(0f, JumpHeightMonke, 0f), ForceMode.VelocityChange);
                GameObject particles = GameObject.Instantiate(bottle);
                GameObject.Destroy(particles.transform.GetChild(0).gameObject);
                particles.transform.GetChild(1).gameObject.SetActive(true);
                particles.transform.position = bottle.transform.parent.transform.position;
                particles.transform.rotation = bottle.transform.parent.transform.rotation;
                particles.transform.rotation = bottle.transform.parent.transform.rotation;
                particles.transform.localScale = bottle.transform.parent.transform.localScale;
                particles.transform.SetParent(particleFolder.transform, true);
                particles.AddComponent<RemoveOverTime>(); // be glad i put this in
            }
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            inModLobby = true;
            modActive = this.enabled; // never knew that existed (this.enabled)
            particleFolder.SetActive(true);
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inModLobby = false;
            modActive = false; // no cheating
            particleFolder.SetActive(false);
        }
    }
}
