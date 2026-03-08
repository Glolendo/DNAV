using UnityEngine;

public class HospitalWallColors : MonoBehaviour
{
    [Header("Lobby Walls")]
    public Renderer[] lobbyWalls;

    [Header("Radiology Walls")]
    public Renderer[] radiologyWalls;

    [Header("General Hallway Walls")]
    public Renderer[] hallwayWalls;

    [Header("Materials")]
    public Material lobbyMaterial;      // black
    public Material radiologyMaterial;  // green
    public Material hallwayMaterial;    // white

    void Start()
    {
        SetWallColors();
    }

    void SetWallColors()
    {
        // Lobby
        foreach (Renderer wall in lobbyWalls)
        {
            wall.material = lobbyMaterial;
        }

        // Radiology
        foreach (Renderer wall in radiologyWalls)
        {
            wall.material = radiologyMaterial;
        }

        // Hallways
        foreach (Renderer wall in hallwayWalls)
        {
            wall.material = hallwayMaterial;
        }
    }
}

    {
        wallRenderer.material = redMaterial;
    }
}
