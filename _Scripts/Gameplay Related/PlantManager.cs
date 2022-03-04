using System.Collections.Generic;
using tzdevil.DatabaseRelated;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace tzdevil.GameplayRelated
{
    [System.Serializable]
    public class Plants
    {
        public PlantSO Plant;
        public int RemainingCount { get; set; }
        public GameObject PlantGO;
    }

    public class PlantManager : MonoBehaviour
    {
        public List<Plants> Plants;

        private void Start() => Setup();

        private void Setup()
        {
            // Set up the plant selection panel.
            for (int i = 0; i < Plants.Count; i++)
            {
                Plants[i].PlantGO.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = $"x{Plants[i].RemainingCount} - {Plants[i].Plant.PlantCost} G";
                Plants[i].PlantGO.transform.Find("Image").GetComponent<Image>().sprite = Plants[i].Plant.PlantSprite;
                Plants[i].RemainingCount = 9;
            }
        }

        private void Update() => UpdatePlants();

        private void UpdatePlants()
        {
            // Update the information of remaining plants.
            for (int i = 0; i < Plants.Count; i++) // remaningcount < 0 ise siyah yap. kullanýlan ise yeþil.
                Plants[i].PlantGO.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = $"x{Plants[i].RemainingCount} - {Plants[i].Plant.PlantCost} G";
        }

        public void SelectNewPlant(PlantSO plant)
        {
            // If conditions are met, then select a plant. Recolor the boxes.
            if (Plants.FirstOrDefault(a => a.Plant == plant).RemainingCount > 0)
            {
                Plants.ForEach(delegate (Plants p) { p.PlantGO.GetComponent<Image>().color = p.RemainingCount > 0 ? new Color32(255, 255, 255, 220) : new Color32(97, 97, 97, 220); });
                Plants.FirstOrDefault(a => a.Plant == plant).PlantGO.GetComponent<Image>().color = new Color32(96, 183, 133, 220);
                GameManager.Plant = plant;
                UpdatePlants();
            }
        }
    }
}