using System.Collections.Generic;
using System.Linq;
using TMPro;
using tzdevil.DatabaseRelated;
using tzdevil.PlantRelated;
using tzdevil.ZombieRelated;
using UnityEngine;
using UnityEngine.UI;

namespace tzdevil.GameplayRelated
{
    public static class DevilUtils
    {
        public static Vector3 GetScreenEquivalentOfCanvas(Canvas canvas, Vector3 pos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Camera.main.WorldToScreenPoint(pos), canvas.worldCamera, out Vector2 dataPos);

            return canvas.transform.TransformPoint(dataPos);
        }
    }

    public enum GameState { Pause, Play }
   
    public class GameManager : MonoBehaviour
    {
        private static GameState gameState;

        [Header("Player Related")]
        public static float Gold;

        [Header("Plant Related")]
        public GameObject PlantPrefab;
        public static PlantSO Plant;

        // Startup References.
        private Canvas Canvas => GameObject.Find("Canvas").GetComponent<Canvas>();
        private Transform PlantsRoot => GameObject.Find("Plants").transform;
        private Transform ZombiesRoot => GameObject.Find("Zombies").transform;
        private GameObject Column1 => GameObject.Find("Column_1");
        private PlantManager PlantManager => GameObject.Find("PlantManager").GetComponent<PlantManager>();
        private TextMeshProUGUI GoldText => GameObject.Find("GoldText").GetComponent<TextMeshProUGUI>();
        private TextMeshProUGUI FreeGoldText => GameObject.Find("FreeGoldText").GetComponent<TextMeshProUGUI>();

        [Header("Zombie Related")]
        public List<ZombieSO> ZombieList;
        public GameObject ZombiePrefab;
        private ZombieSO SelectedZombie;
        public float ZombieSpawnRate;
        public float ZombieTimer;
        private const int columnCount = 5; // from how many columns can a zombie come?
        [HideInInspector] public static int totalZombieThisRound; // total zombie LEFT this round.

        [Header("Free Gold Related")]
        private float freeGoldRate;
        private float freeGoldRateTimer;
        private float freeGoldValue;

        private void Start() => PreSetup();

        private void PreSetup()
        {
            // Set up the starting scene.
            gameState = GameState.Play;
            ZombieSpawnRate = 6f;
            ZombieTimer = ZombieSpawnRate;
            totalZombieThisRound = 50;
            Gold = 0;
            freeGoldRate = 10;
            freeGoldValue = 100;
            ChooseNewZombie();
        }

        private void Update()
        {
            // IF the game's in Play Mode, then spawn a zombie every zombieSpawnRate seconds.
            if (gameState == GameState.Play && totalZombieThisRound > 0)
            {
                if (ZombieTimer > 0) ZombieTimer -= Time.deltaTime;
                else
                {
                    SpawnNewZombie();

                    // Reset zombie timers. 
                    totalZombieThisRound--;
                    ZombieTimer = (ZombieSpawnRate > 2f ) ? ZombieSpawnRate - 0.25f : ZombieSpawnRate;
                }
            }

            // Update gold text every frame.
            GoldText.text = $"{Gold} G";
            GainGoldEveryXSecond();
        }

        private void GainGoldEveryXSecond()
        {
            FreeGoldText.text = $"Free {freeGoldValue} gold in {freeGoldRateTimer:n1} seconds...";

            if (freeGoldRateTimer > 0) { freeGoldRateTimer -= Time.deltaTime; }
            else
            {
                Gold += freeGoldValue;
                freeGoldRateTimer = freeGoldRate;
            }
        }

        #region Plant Scripts
        public void SpawnNewPlant(Image area)
        {
            // IF there is no area, or IF there is no Plant, or IF you can't spawn an another plant, or IF there isn't enough gold to spawn a new plant; then do nothing.
            if (!area.gameObject) return;
            if (!Plant) return;
            if (PlantManager.Plants.FirstOrDefault(a => a.Plant == Plant).RemainingCount <= 0) return;
            if (Plant.PlantCost > GameManager.Gold) return;

            // Spawn new Plant.
            Vector3 spawnPos = DevilUtils.GetScreenEquivalentOfCanvas(Canvas, area.transform.position);
            GameObject plant = Instantiate(PlantPrefab, spawnPos, Quaternion.identity, PlantsRoot);
            plant.name = Plant.name;
            plant.GetComponent<PlantBehaviour>().Plant = Plant;
            plant.GetComponent<PlantBehaviour>().Area = area.gameObject;

            // Reduce from the remaining count and recolor the box if the RemainingCount is 0.
            PlantManager.Plants.FirstOrDefault(a => a.Plant == Plant).RemainingCount--;
            PlantManager.Plants.ForEach(delegate (Plants p) { if (p.RemainingCount <= 0) p.PlantGO.GetComponent<Image>().color = new Color32(97, 97, 97, 220); });

            // Deactive the Raycast so you can't summon another plant there.
            area.raycastTarget = false;

            GameManager.Gold -= Plant.PlantCost;
        }

        // Check if the mouse is hovering over areas or not.
        public void OnHoverEnterArea(Image area) => area.color = new Color32(68, 80, 74, 220);
        public void OnHoverExitArea(Image area)
        {
            // Check what equation to get color of the square returns.
            bool equation = (area.transform.parent.name[^1] % 2 != 0 && int.Parse(area.name) % 2 == 0) 
                            || (area.transform.parent.name[^1] % 2 == 0 && int.Parse(area.name) % 2 != 0);

            // Recolor the square depending on the equation.
            area.color = equation
                         ? new Color32(15, 15, 15, 220)
                         : new Color32(25, 27, 32, 220);
        }
        #endregion

        #region Zombie Scripts 
        private void SpawnNewZombie()
        {
            // Spawn a zombie with x equal to any object in Column_1 and y on 9.35f.
            var spawnPos = Column1.transform.GetChild(Random.Range(0, columnCount)).transform.position;
            spawnPos.x = 9.35f;
            spawnPos.z = 0;
            GameObject zombie = Instantiate(ZombiePrefab, spawnPos, Quaternion.identity, ZombiesRoot);
            zombie.GetComponent<ZombieBehaviour>().Zombie = SelectedZombie;
            ChooseNewZombie();
        }
        #endregion

        private void ChooseNewZombie() => SelectedZombie = ZombieList[Random.Range(0, ZombieList.Count)];

        // Switch to Pause Mode to end the round.
        public static void EndRound() => gameState = GameState.Pause;
    }
}
