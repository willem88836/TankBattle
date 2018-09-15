using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Framework;

namespace Framework
{
    public class MenuManager : MonoBehaviour
    {

        private void Awake()
        {
            Screen.SetResolution(820, 820, false);
        }

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}