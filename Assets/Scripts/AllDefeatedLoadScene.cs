using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AllDefeatedLoadScene : MonoBehaviour
{
    public string nextSceneName; // The name of the scene to load when no more enemies are present
    public string enemyTag = "Enemy"; // The tag assigned to enemy objects

    private void Update()
    {
        // Check if there are any remaining enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
