using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWLPXL.ActionCharacter
{


    public class AppQuit : MonoBehaviour
    {
        public int TargetFrameRate;
        public bool EnableDebug;
        public KeyCode QuitKey = KeyCode.Escape;
        public KeyCode ResetScene = KeyCode.F1;
        // Start is called before the first frame update

       
        void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
        }

        // Update is called once per frame
        void Update()
        {

            if (DebugHelpers.UseDebug != EnableDebug)
            {
                DebugHelpers.UseDebug = EnableDebug;
            }

            if (Input.GetKeyDown(ResetScene))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(QuitKey))
            {
                AppQuitter();
            }

        }


        public void AppQuitter()
        {
            Application.Quit();
        }
    }
}