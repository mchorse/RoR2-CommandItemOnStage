using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace McHorse
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(CommandHelper))]
    public class CommandItemOnStage : BaseUnityPlugin
    {
        private static CommandItemOnStage instance;

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "McHorse";
        public const string PluginName = "CommandItemOnStage";
        public const string PluginVersion = "1.0.0";

        /* It seems like command item thingy is taking rarity of given item's
         * ID instead of having specific enum for rarity. It looks like 0-8 are
         * just a coincedence... */
        public const int BOSS = 0;
        public const int LUNAR = 1;
        public const int COMMON = 2;
        public const int UNCOMMON = 3;
        public const int RARE = 4;
        public const int VOID_BOSS = 5;
        public const int VOID_COMMON = 6;
        public const int VOID_UNCOMMON = 7;
        public const int VOID_RARE = 8;
        public const int EQUIPMENT = 225;

        public static ConfigEntry<bool> Enabled { get; set; }
        public static ConfigEntry<float> VoidChance { get; set; }
        public static ConfigEntry<int> ItemCount { get; set; }
        public static ConfigEntry<bool> ItemCountRandomize { get; set; }
        public static ConfigEntry<bool> Equipment { get; set; }

        [ConCommand(commandName = "reload_cios_config", flags = ConVarFlags.None, helpText = "Reload CommandItemOnStage's config.")]
        private static void CCReloadConfig(ConCommandArgs args)
        {
            instance.Config.Reload();

            PrintConfig();
        }

        private static void PrintConfig()
        {
            Log.LogInfo(
                "Enabled: " + Enabled.Value + ", " + 
                "Void item chance: " + VoidChance.Value + ", " + 
                "Item count: " + ItemCount.Value + ", " + 
                "Randomize item count: " + ItemCountRandomize.Value + ", " + 
                "Give equipment: " + Equipment.Value
            );
        }

        public void Awake()
        {
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();

            instance = this;

            Enabled = Config.Bind<bool>(PluginName, "enabled", true, "Enables giving a command item on every stage");
            VoidChance = Config.Bind<float>(PluginName, "void_chance", 0f, "Allows to specify the chance to get a void variant (0..100)");
            ItemCount = Config.Bind<int>(PluginName, "item_count", 1, "Allows to specify how many command items will be dropped at every stage");
            ItemCountRandomize = Config.Bind<bool>(PluginName, "item_count_randomize", true, "Enables random range of command items spawned specified by item_count option");
            Equipment = Config.Bind<bool>(PluginName, "equipment", false, "Whether an equipment command item should be dropped (first stage only)");
            
            On.RoR2.Stage.RespawnCharacter += (org, self, character) =>
            {
                org(self, character);

                if (!Enabled.Value)
                {
                    return;
                }

                Transform transform = character.GetBodyObject().transform;
                SceneDef scene = SceneCatalog.currentSceneDef;
                int count = ItemCount.Value;

                if (ItemCountRandomize.Value)
                {
                    count = Random.Range(1, count + 1);
                }

                for (int i = 0; i < count; i++)
                {
                    DropCommandItem(transform, GetIndex(scene, character, i));
                }

                if (Equipment.Value && scene.stageOrder == 1)
                {
                    DropCommandItem(transform, EQUIPMENT);
                }

                Log.LogInfo("Spawned " + count + " command items on " + scene.baseSceneName);
            };

            Log.Init(Logger);
            Log.LogInfo(PluginName + " is done.");

            PrintConfig();
        }

        private void DropCommandItem(Transform transform, int index)
        {
            Vector3 position = transform.position + transform.forward * 10f;
            PickupIndex pickupIndex = new PickupIndex(index);

            position.x += Random.Range(-2, 2);
            position.z += Random.Range(-2, 2);
            position.y += 10;

            GameObject commandCube = UnityEngine.Object.Instantiate<GameObject>(CommandArtifactManager.commandCubePrefab, position, transform.rotation);
            commandCube.GetComponent<PickupIndexNetworker>().NetworkpickupIndex = pickupIndex;
            commandCube.GetComponent<PickupPickerController>().SetOptionsFromPickupForCommandArtifact(pickupIndex);
            NetworkServer.Spawn(commandCube);
        }

        private int GetIndex(SceneDef stage, CharacterMaster character, int i)
        {
            int index = GetIndex(stage.stageOrder % 5 == 0 && i == 0 ? UNCOMMON : COMMON, character);

            if (stage.isFinalStage)
            {
                index = GetIndex(i == 0 ? RARE : UNCOMMON, character);
            }

            return index;
        }

        private int GetIndex(int index, CharacterMaster character)
        {
            if (Util.CheckRoll(VoidChance.Value, character))
            {
                switch (index)
                {
                    case COMMON: return VOID_COMMON;
                    case UNCOMMON: return VOID_UNCOMMON;
                    case RARE: return VOID_RARE;
                    case BOSS: return VOID_BOSS;
                }
            }

            return index;
        }
    }
}