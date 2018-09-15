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
	[System.Obsolete]
	public class ManualManager : MonoBehaviour
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

        //List of scripts that are derived from RobotControl
        private System.Type[] tankListAI;
        //List of spawnpoints
        private SpawnPoint[] spawnPoints;

        private System.Type[] clickedAI = new System.Type[2];
        private List<System.Type> selectedAI = new List<System.Type>();

        private Image rightSideButton;

        [SerializeField]
        private GameObject baseTank;

        [SerializeField]
        private GameObject buttonPrefab;

        [SerializeField]
        private Color selectedColor;

        [SerializeField]
        private Transform leftAIContainer;

        [SerializeField]
        private Transform rightAIContainer;

        [SerializeField]
        private Text victoryText;

        private List<Image> leftButtons = new List<Image>();
        private List<Image> rightButtons = new List<Image>();

        private GameObject matchTrue;
        private GameObject matchFalse;
        private bool matchInProgress = false;
        private List<GameObject> tanks = new List<GameObject>();

        [SerializeField]
        private GameObject playerUILeft;
        [SerializeField]
        private GameObject playerUIRight;
        private Transform healthbarUI;


        void Start()
        {
            matchTrue = transform.GetChild(1).GetChild(0).gameObject;
            matchFalse = transform.GetChild(1).GetChild(1).gameObject;
            healthbarUI = transform.GetChild(1).GetChild(2);
            FindRobots();
            FindSpawnLocations();
            FillRightContainer();
        }

        private void Update()
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
            foreach (Transform child in healthbarUI)
            {
                Destroy(child.gameObject);
            }
            matchInProgress = false;
            matchTrue.SetActive(true);
            matchFalse.SetActive(false);
            string[] endMessages = new string[4] { " was victorious!", " pwned!", " destroyed the others!", " was the best!" };
            victoryText.text = tanks[index].name + endMessages[Random.Range(0, endMessages.Length)];

            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i] != null)
                {
                    tanks[i].SendMessage("OnVictory");
                    Destroy(tanks[i], 2);
                }
            }
            tanks = new List<GameObject>();
        }

        //Manually end the match
        public void EndMatch()
        {
            foreach (Transform child in healthbarUI)
            {
                Destroy(child.gameObject);
            }
            matchInProgress = false;
            matchTrue.SetActive(true);
            matchFalse.SetActive(false);
            victoryText.text = "Manually stopped!";

            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i] != null)
                {
                    Destroy(tanks[i].gameObject);
                }
            }
            tanks = new List<GameObject>();
        }

        //Fill the option selection boxes
        private void FillRightContainer()
        {
            for (int i = 0; i < tankListAI.Length; i++)
            {
                GameObject newButton = Instantiate(buttonPrefab, leftAIContainer);
                newButton.transform.GetChild(0).GetComponent<Text>().text = tankListAI[i].Name;
                newButton.name = tankListAI[i].Name;
                System.Type sendType = tankListAI[i];
                Image newImage = newButton.GetComponent<Image>();
                leftButtons.Add(newImage);
                newButton.GetComponent<Button>().onClick.AddListener(delegate { LeftSideButtonPress(sendType, newImage); });
            }
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

        private void FindSpawnLocations()
        {
            //Find the spawnpoint locations
            spawnPoints = new SpawnPoint[transform.GetChild(0).childCount];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Transform child = transform.GetChild(0).GetChild(i);
                spawnPoints[i] = new SpawnPoint(child.position, child.eulerAngles.y);
            }
        }

        //Gathers all derived types from a given baseType
        private IEnumerable<System.Type> FindDerivedTypes(System.Type baseType)
        {
            Assembly assembly = Assembly.GetAssembly(baseType);
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
        }

        //called when a button on the left side is pressed
        public void LeftSideButtonPress(System.Type AI, Image button)
        {
            clickedAI[0] = AI;
            for (int i = 0; i < leftButtons.Count; i++)
            {
                leftButtons[i].color = Color.white;
            }

            button.color = selectedColor;
        }

        // when a button on the right side is pressed
        public void RightSideButtonPressed(System.Type AI, Image button)
        {
            clickedAI[1] = AI;
            for (int i = 0; i < rightButtons.Count; i++)
            {
                rightButtons[i].color = Color.white;
            }
            button.color = selectedColor;
            rightSideButton = button;
        }

        //starts the match
        public void StartMatch()
        {
            for (int i = 0; i < selectedAI.Count; i++)
            {
                // spawn tank
                GameObject newTank = Instantiate(baseTank);
                newTank.transform.position = spawnPoints[i].position;
                newTank.transform.eulerAngles = new Vector3(0f, spawnPoints[i].angle, 0f);
                newTank.AddComponent(selectedAI[i]);
                newTank.name = selectedAI[i].Name;
                tanks.Add(newTank);

                // add the health UI for the tank
                GameObject NewUI = (Instantiate((i % 2 == 0) ? playerUILeft : playerUIRight));
                NewUI.transform.SetParent(transform.GetChild(1));
                NewUI.GetComponent<RectTransform>().offsetMin = new Vector2(0, (i / 2) * -115);
                NewUI.GetComponent<RectTransform>().offsetMax = new Vector2(0, (i / 2) * -115);
                NewUI.transform.SetParent(healthbarUI);
                newTank.GetComponent<RobotMotor>().playerUI = NewUI.transform;
                newTank.GetComponent<RobotMotor>().hasHealthUI = true;
            }
            matchInProgress = true;
            matchTrue.SetActive(false);
            matchFalse.SetActive(true);
        }

        public void AddAI()
        {
            if (selectedAI.Count < 8 && clickedAI[0] != null)
            {
                GameObject newButton = Instantiate(buttonPrefab, rightAIContainer);
                newButton.transform.GetChild(0).GetComponent<Text>().text = clickedAI[0].Name;
                newButton.name = clickedAI[0].Name;
                System.Type sendType = clickedAI[0];
                Image newImage = newButton.GetComponent<Image>();
                rightButtons.Add(newImage);
                newButton.GetComponent<Button>().onClick.AddListener(delegate { RightSideButtonPressed(sendType, newImage); });
                selectedAI.Add(clickedAI[0]);
            }
        }

        public void RemoveAI()
        {
            // remove the button from the list, and the AI out of the array for selected AIs
            selectedAI.Remove(clickedAI[1]);
            rightButtons.Remove(rightSideButton);
            Destroy(rightSideButton.gameObject);
            clickedAI[1] = null;
            rightSideButton = null;
        }

        public void RemoveAllAI()
        {
            Debug.Log(selectedAI.Count);
            selectedAI.Clear();
            Debug.Log(selectedAI.Count);
            rightButtons.Clear();
            for (int i = 0; i < rightAIContainer.childCount; i++)
            {
                Destroy(rightAIContainer.GetChild(i).gameObject);
            }
            clickedAI[1] = null;
            rightSideButton = null;
        }
    }
}
