using UnityEngine;

public class CeilingAndWallsManager : MonoBehaviour
{
    // This script is used to document and identify the ceiling and wall
    // objects placed manually in the Unity scene.
    //
    // Contribution: Responsible for placing, aligning, and setting up
    // ceiling objects and wall geometry.

    public GameObject[] walls;
    public GameObject[] ceilings;

    void Start()
    {
        Debug.Log("CeilingAndWallsManager active");

        Debug.Log("Number of walls found: " + walls.Length);
        Debug.Log("Number of ceilings found: " + ceilings.Length);

        for (int i = 0; i < walls.Length; i++)
            Debug.Log("Wall " + i + ": " + walls[i].name);

        for (int i = 0; i < ceilings.Length; i++)
            Debug.Log("Ceiling " + i + ": " + ceilings[i].name);
    }
}
