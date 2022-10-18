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
        /*Mod under the MIT license, if you reproduce please credit*/

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

        void Start()
        {
            if (bottle == null)
                Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            if (bottle != null && particleFolder != null)
            {
                bottle.SetActive(enabled);
                bottle.SetActive(enabled);
            }
        }

        void OnDisable()
        {
            if (bottle != null && particleFolder != null)
            {
                bottle.SetActive(enabled);
                bottle.SetActive(enabled);
            }
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            particleFolder = new GameObject();
            particleFolder.name = "CloudInABottle_Particles";

            AssetBundle bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevCloudInABottleMod.Assets.cloud"));
            bottle = Instantiate(bundle.LoadAsset<GameObject>("bottle"));

            GameObject rightHand = GorillaTagger.Instance.rightHandTransform.parent.GetChild(0).gameObject;

            bottle.transform.SetParent(rightHand.transform, false);

            bottle.transform.localPosition = new Vector3(0f, 0f, 0f);
            bottle.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            bottle.transform.localScale = new Vector3(1f, 1f, 1f);

            bottle.name = "BottleParticles";
        }

        void Update()
        {
            if (!inModLobby || !enabled || bottle == null || particleFolder == null)
                return;

            if (GorillaLocomotion.Player.Instance.IsHandTouching(false) || GorillaLocomotion.Player.Instance.IsHandTouching(true))
                if (!canDoubleJump)
                    canDoubleJump = true;

            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out isPrimaryDown);

            if (isPrimaryDown && canDoubleJump)
            {
                canDoubleJump = false;

                float JumpHeightMonke = 10;

                Rigidbody PlayerRigidbody = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>();
                PlayerRigidbody.AddForce(new Vector3(0f, JumpHeightMonke, 0f), ForceMode.VelocityChange);

                GameObject particles = Instantiate(bottle);
                Destroy(particles.transform.GetChild(0).gameObject);

                particles.transform.GetChild(1).gameObject.SetActive(true);
                particles.transform.position = bottle.transform.parent.transform.position;
                particles.transform.rotation = bottle.transform.parent.transform.rotation;
                particles.transform.rotation = bottle.transform.parent.transform.rotation;
                particles.transform.localScale = bottle.transform.parent.transform.localScale;

                particles.transform.SetParent(particleFolder.transform, true);
                particles.AddComponent<RemoveOverTime>(); // be glad i put this in
            }
        }

        [ModdedGamemodeJoin] public void OnJoin() => inModLobby = true;
        [ModdedGamemodeLeave] public void OnLeave() => inModLobby = false;
    }
}
