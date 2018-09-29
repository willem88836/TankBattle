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
    public class VersusManager : BattleManager
    {

		[Header("Versus")]
        [SerializeField] GameObject _baseTank;
        [SerializeField] GameObject _behaviourButtonPrefab;
        [SerializeField] GameObject _sensorCamera;
		[SerializeField] Transform _spawnpointParent;

		[Header("UI")]
        [SerializeField] GameObject matchFalse;
        [SerializeField] GameObject matchTrue;
		[SerializeField] Text _victoryText;

		[Header("BehaviourList")]
		[SerializeField] Color _selectedBehaviourColor;
		[SerializeField] Color _unselectedBehaviourColor;
		[SerializeField] Transform _leftBehaviourParent;
        [SerializeField] Transform _rightBehaviourParent;

		//List of scripts that are derived from RobotControl
		System.Type[] tankListAI;

		//List of spawnpoints
		SpawnPoint[] _spawnPoints;
		System.Type[] _selectedAI = new System.Type[2];

		List<Button> leftButtons = new List<Button>();
		List<Button> rightButtons = new List<Button>();

		private bool matchInProgress = false;
        private bool showSensors = false;
        private List<GameObject> tanks = new List<GameObject>();

		protected override void Awake()
		{
			base.Awake();

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
            for (int i = 0; i < _selectedAI.Length; i++)
            {
                // spawn tank
                GameObject newTank = Instantiate(_baseTank);
                newTank.transform.position = _spawnPoints[i].position;
                newTank.transform.eulerAngles = new Vector3(0f, _spawnPoints[i].angle, 0f);
                if (_selectedAI[i] != null)
                {
                    newTank.AddComponent(_selectedAI[i]);
                    newTank.name = _selectedAI[i].Name;
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
            _spawnPoints = new SpawnPoint[_spawnpointParent.childCount];
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                Transform child = _spawnpointParent.GetChild(i);
                _spawnPoints[i] = new SpawnPoint(child.position, child.eulerAngles.y);
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
            _selectedAI[0] = AI;
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
            _selectedAI[1] = AI;
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
