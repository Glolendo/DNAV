using System.Collections.Generic;
using UnityEngine;

public class GazeCategoryTracker : MonoBehaviour
{
    public float maxDistance = 25f;

    private GazeCategory currentCategory = GazeCategory.Other;
    private float categoryStartTime;

    private Dictionary<GazeCategory, float> categoryTimes =
        new Dictionary<GazeCategory, float>();

    void Start()
    {
        foreach (GazeCategory category in System.Enum.GetValues(typeof(GazeCategory)))
        {
            categoryTimes[category] = 0f;
        }

        categoryStartTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        GazeCategory detectedCategory = DetectGazeCategory();

        if (detectedCategory != currentCategory)
        {
            AddTimeToCurrentCategory();

            currentCategory = detectedCategory;
            categoryStartTime = Time.realtimeSinceStartup;

            if (TelemetryManager.Instance != null)
            {
                TelemetryManager.Instance.LogEvent(
                    "GazeCategoryChanged",
                    currentCategory.ToString(),
                    "Head-gaze category changed"
                );
            }
        }
    }

    GazeCategory DetectGazeCategory()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            GazeCategoryTag tag = hit.collider.GetComponent<GazeCategoryTag>();

            if (tag != null)
            {
                return tag.category;
            }
        }

        return GazeCategory.Other;
    }

    void AddTimeToCurrentCategory()
    {
        float duration = Time.realtimeSinceStartup - categoryStartTime;
        categoryTimes[currentCategory] += duration;
    }

    public float GetCategoryPercent(GazeCategory category)
    {
        AddTimeToCurrentCategory();

        float total = 0f;

        foreach (var item in categoryTimes)
        {
            total += item.Value;
        }

        if (total <= 0f) return 0f;

        return categoryTimes[category] / total;
    }
}