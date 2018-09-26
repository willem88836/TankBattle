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

        [SerializeField] private GameObject _baseTank;
        [SerializeField] private GameObject _behaviourButtonPrefab;
        [SerializeField] private GameObject _sensorCamera;
		[SerializeField] private Text _victoryText;

		[SerializeField] private Color _selectedBehaviourColor;
		[SerializeField] private Color _unselectedBehaviourColor;

		[SerializeField] private Transform _leftBehaviourParent;
        [SerializeField] private Transform _rightBehaviourParent;

        //List of scripts that are derived from RobotControl
        private System.Type[] tankListAI;

        //List of spawnpoints
        private SpawnPoint[] spawnPoints;
        private System.Type[] selectedAI = new System.Type[2];

        private List<Button> leftButtons = new List<Button>();
        private List<Button> rightButtons = new List<Button>();

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
            _sensorCamera.SetActive(false);
            matchFalse.SetActive(true);
            matchTrue.SetActive(false);

            FindBehaviours();
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
            _victoryText.text = tanks[index].name + endMessages[Random.Range(0, endMessages.Length)];

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
                GameObject newTank = Instantiate(_baseTank);
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
            _victoryText.text = "Manually stopped, no outcome!";

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
        private void FindBehaviours()
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
                GameObject newButtonObject = Instantiate(_behaviourButtonPrefab, _leftBehaviourParent);
                newButtonObject.transform.GetChild(0).GetComponent<Text>().text = tankListAI[i].Name;
                newButtonObject.name = tankListAI[i].Name;
                System.Type sendType = tankListAI[i];

                Button newButton = newButtonObject.GetComponent<Button>();
                leftButtons.Add(newButton);
                newButton.onClick.AddListener(delegate { LeftButtonPress(sendType, newButton); });
            }

            for (int i = 0; i < tankListAI.Length; i++)
            {
                GameObject newButtonObject = Instantiate(_behaviourButtonPrefab, _rightBehaviourParent);
                newButtonObject.transform.GetChild(0).GetComponent<Text>().text = tankListAI[i].Name;
                newButtonObject.name = tankListAI[i].Name;
                System.Type sendType = tankListAI[i];

                Button newButton = newButtonObject.GetComponent<Button>();
                rightButtons.Add(newButton);
                newButtonObject.GetComponent<Button>().onClick.AddListener(delegate { RightButtonPress(sendType, newButton); });
            }

			// Already select the first behaviours
			if (leftButtons[0].onClick != null)
				leftButtons[0].onClick.Invoke();

			if (rightButtons[0].onClick != null)
				rightButtons[0].onClick.Invoke();
        }

        //A button in the left container was pressed
        public void LeftButtonPress(System.Type AI, Button button)
        {
            selectedAI[0] = AI;
            for (int i = 0; i < leftButtons.Count; i++)
            {
                leftButtons[i].GetComponent<Image>().color = _unselectedBehaviourColor;
            }
            button.GetComponent<Image>().color = _selectedBehaviourColor;

			Debug.Log("Selected Left: " + AI.ToString());
		}

        //A button in the right container was pressed
        public void RightButtonPress(System.Type AI, Button button)
        {
            selectedAI[1] = AI;
            for (int i = 0; i < rightButtons.Count; i++)
            {
                rightButtons[i].GetComponent<Image>().color = _unselectedBehaviourColor;
            }
            button.GetComponent<Image>().color = _selectedBehaviourColor;

			Debug.Log("Selected Right: " + AI.ToString());
		}

        public void ToggleSensors()
        {
            showSensors = !showSensors;
            _sensorCamera.SetActive(showSensors);
        }
    }
}
