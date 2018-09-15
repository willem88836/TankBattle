using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Reflection;
using Framework;
using Random = UnityEngine.Random;

namespace Framework
{
    public class VersusManager : MonoBehaviour
    {

        //Class to store a spawnpoints position and angle in a single variable
        public class SpawnPoint
        {
            public Vector3 position;
            public float angle;

            public SpawnPoint(Vector3 newPos, float newAngle)
            {
                position = newPos;
                angle = newAngle;
            }
        }

        [SerializeField]
        private GameObject baseTank;

        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        private GameObject sensorCamera;

        [SerializeField]
        private Color selectedColor;

        [SerializeField]
        private Transform leftAIContainer;

        [SerializeField]
        private Transform rightAIContainer;

        [SerializeField]
        private Text victoryText;

        //List of scripts that are derived from RobotControl
        private System.Type[] tankListAI;
        //List of spawnpoints
        private SpawnPoint[] spawnPoints;
        private System.Type[] selectedAI = new System.Type[2];

        private List<Image> leftButtons = new List<Image>();
        private List<Image> rightButtons = new List<Image>();

        private GameObject matchFalse;
        private GameObject matchTrue;
        private bool matchInProgress = false;
        private bool showSensors = false;
        private List<GameObject> tanks = new List<GameObject>();

        // Use this for initialization
        void Start()
        {
            matchFalse = transform.GetChild(1).GetChild(0).gameObject;
            matchTrue = transform.GetChild(1).GetChild(1).gameObject;

            showSensors = false;
            sensorCamera.SetActive(false);
            matchFalse.SetActive(true);
            matchTrue.SetActive(false);

            FindRobots();
            FindSpawnLocations();
            FillContainers();
        }

        // Update is called once per frame
        void Update()
        {
            if (matchInProgress)
            {
                for (int i = 0; i < tanks.Count; i++)
                {
                    if (tanks[i] == null)
                    {
                        tanks.Remove(tanks[i]);
                        return;
                    }
                }
                if (tanks.Count == 1)
                {
                    EndMatch(0);
                }
            }
        }

        //End the match with a victor
        private void EndMatch(int index)
        {
            matchInProgress = false;
            matchTrue.SetActive(false);
            matchFalse.SetActive(true);
            string[] endMessages = new string[4] { " is victorious!", " pwned!", " destroyed the other!", " is the best!" };
            victoryText.text = tanks[index].name + endMessages[Random.Range(0, endMessages.Length)];

            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i] != null)
                {
                    Destroy(tanks[i]);
                }
            }
            tanks = new List<GameObject>();
        }

        //Start the match
        public void StartMatch()
        {
            for (int i = 0; i < selectedAI.Length; i++)
            {
                // spawn tank
                GameObject newTank = Instantiate(baseTank);
                newTank.transform.position = spawnPoints[i].position;
                newTank.transform.eulerAngles = new Vector3(0f, spawnPoints[i].angle, 0f);
                if (selectedAI[i] != null)
                {
                    newTank.AddComponent(selectedAI[i]);
                    newTank.name = selectedAI[i].Name;
                }
                tanks.Add(newTank);
            }
            matchInProgress = true;
            matchTrue.SetActive(true);
            matchFalse.SetActive(false);
        }

        //Manually end the match
        public void EndMatch()
        {
            matchInProgress = false;
            matchTrue.SetActive(false);
            matchFalse.SetActive(true);
            victoryText.text = "Manually stopped!";

            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i] != null)
                {
                    Destroy(tanks[i]);
                }
            }
            tanks = new List<GameObject>();
        }

        //Find all AI scripts
        private void FindRobots()
        {
            tankListAI = FindDerivedTypes(typeof(RobotControl)).ToArray();
            string debugMessage = "Found " + tankListAI.Length + " AI scripts: ";
            foreach (System.Type type in tankListAI)
            {
                debugMessage += type.Name + ", ";
            }
            Debug.Log(debugMessage);
        }

        //Find derived types of a base type
        private IEnumerable<System.Type> FindDerivedTypes(System.Type baseType)
        {
            Assembly assembly = Assembly.GetAssembly(baseType);
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
        }

        //Find all the spawn locations
        private void FindSpawnLocations()
        {
            spawnPoints = new SpawnPoint[transform.GetChild(0).childCount];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Transform child = transform.GetChild(0).GetChild(i);
                spawnPoints[i] = new SpawnPoint(child.position, child.eulerAngles.y);
            }
        }

        //Fill both containers with all the AI scripts found
        private void FillContainers()
        {
            for (int i = 0; i < tankListAI.Length; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, leftAIContainer);
                newButton.transform.GetChild(0).GetComponent<Text>().text = tankListAI[i].Name;
                newButton.name = tankListAI[i].Name;
                System.Type sendType = tankListAI[i];
                Image newImage = newButton.GetComponent<Image>();
                leftButtons.Add(newImage);
                newButton.GetComponent<Button>().onClick.AddListener(delegate { LeftButtonPress(sendType, newImage); });
            }

            for (int i = 0; i < tankListAI.Length; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, rightAIContainer);
                newButton.transform.GetChild(0).GetComponent<Text>().text = tankListAI[i].Name;
                newButton.name = tankListAI[i].Name;
                System.Type sendType = tankListAI[i];
                Image newImage = newButton.GetComponent<Image>();
                rightButtons.Add(newImage);
                newButton.GetComponent<Button>().onClick.AddListener(delegate { RightButtonPress(sendType, newImage); });
            }
        }

        //A button in the left container was pressed
        public void LeftButtonPress(System.Type AI, Image button)
        {
            selectedAI[0] = AI;
            for (int i = 0; i < leftButtons.Count; i++)
            {
                leftButtons[i].color = Color.white;
            }
            button.color = selectedColor;
        }

        //A button in the right container was pressed
        public void RightButtonPress(System.Type AI, Image button)
        {
            selectedAI[1] = AI;
            for (int i = 0; i < rightButtons.Count; i++)
            {
                rightButtons[i].color = Color.white;
            }
            button.color = selectedColor;
        }

        public void ToggleSensors()
        {
            showSensors = !showSensors;
            sensorCamera.SetActive(showSensors);
        }
    }
}
