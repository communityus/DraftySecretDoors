using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Utility;

namespace DraftySecretDoors
{
    public class DraftySecretDoors : MonoBehaviour
    {
        public static Mod mod;

        private PlayerEnterExit playerEnterExit;

        private string currentLocationName = null;
        private float volume = 1.0f;
        private float minDist = 2.5f;
        private float maxDist = 12.0f;
        private float pitch = 1.0f;

        private void Start()
        {
            ModSettings settings = mod.GetSettings();

            volume = settings.GetValue<float>("Settings", "Volume");
            minDist = settings.GetValue<float>("Settings", "MinVolumeDistance");
            maxDist = settings.GetValue<float>("Settings", "MaxVolumeDistance");
            pitch = settings.GetValue<float>("Settings", "Pitch");

            playerEnterExit = GameManager.Instance.PlayerEnterExit;

            SaveLoadManager.OnLoad += (saveData) => { currentLocationName = null; };
            StartGameBehaviour.OnNewGame += () => { currentLocationName = null; };
        }

        void LateUpdate()
        {
            if (!GameManager.Instance.IsPlayerInsideDungeon && currentLocationName != null)
                currentLocationName = null;
            else if (GameManager.Instance.IsPlayerInsideDungeon && playerEnterExit.Dungeon.Summary.LocationName != currentLocationName)
                CreateAudio();
        }

        private void CreateAudio()
        {
            currentLocationName = playerEnterExit.Dungeon.Summary.LocationName;

            DaggerfallActionDoor[] actionDoors = FindObjectsOfType<DaggerfallActionDoor>();

            if (actionDoors != null)
            {
                foreach (DaggerfallActionDoor actionDoor in actionDoors)
                {
                    string meshFilterName = actionDoor.GetComponent<MeshFilter>().name;

                    if (!meshFilterName.Contains("55000") && !meshFilterName.Contains("55001") && !meshFilterName.Contains("55002") && !meshFilterName.Contains("55003") &&
                        !meshFilterName.Contains("55004") && !meshFilterName.Contains("55005"))
                    {
                        // Secret door

                        DaggerfallAudioSource daggerfallAudioSource = new GameObject("SecretDoorAudio").AddComponent<DaggerfallAudioSource>();
                        daggerfallAudioSource.transform.position = actionDoor.transform.position;
                        daggerfallAudioSource.transform.parent = actionDoor.transform;

                        daggerfallAudioSource.SetSound(SoundClips.AmbientWindBlow1b, AudioPresets.LoopIfPlayerNear);
                        daggerfallAudioSource.AudioSource.rolloffMode = AudioRolloffMode.Linear;
                        daggerfallAudioSource.AudioSource.minDistance = minDist;
                        daggerfallAudioSource.AudioSource.maxDistance = maxDist;
                        daggerfallAudioSource.AudioSource.volume = volume;
                        daggerfallAudioSource.AudioSource.pitch = pitch;
                    }
                }
            }
        }

        [Invoke(StateManager.StateTypes.Game, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            new GameObject("DraftySecretDoors").AddComponent<DraftySecretDoors>();

            ModManager.Instance.GetMod(initParams.ModTitle).IsReady = true;
        }
    }
}