using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Framework;
using UnityEngine.UI;

namespace Framework
{
    public class VictoryManager : MonoBehaviour
    {
        [SerializeField]
        private float spawnField;
        [SerializeField]
        private float winnerScale;
        [SerializeField]
        private GameObject emptyTank;
        [SerializeField]
        private Text text;
        
        System.Type[] tankListAI;
	    // Use this for initialization
	    void Start ()
        {
            Debug.Log(CompetitionManager.winner);
            FindRobots();
            
            foreach(System.Type bot in tankListAI)
            {
                GameObject newTank = Instantiate(emptyTank);
                newTank.AddComponent(bot);
                newTank.name = bot.Name;
                newTank.GetComponent<RobotMotor>().invinsible = true;
                if (bot == CompetitionManager.winner)
                {
                    newTank.transform.localScale = new Vector3(winnerScale, winnerScale, winnerScale);
                    newTank.transform.position = new Vector3(0, 0, 0);
                    text.text += bot;
                }
                else
                {
                    newTank.transform.position = new Vector3(Random.Range(-spawnField, spawnField), 0, Random.Range(-spawnField, spawnField));
                }
            }
	    }


        private void FindRobots()
        {
            //Find all AI scripts
            tankListAI = FindDerivedTypes(typeof(RobotControl)).ToArray();
            string debugMessage = "Found " + tankListAI.Length + " AI scripts: ";
            foreach (System.Type type in tankListAI)
            {
                debugMessage += type.Name + ", ";
            }
            Debug.Log(debugMessage);
        }
        /// <summary>
        /// Gathers all derived types from a given baseType
        /// </summary>
        private IEnumerable<System.Type> FindDerivedTypes(System.Type baseType)
        {
            Assembly assembly = Assembly.GetAssembly(baseType);
            return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
        }

        public void BackToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }

    }
}
