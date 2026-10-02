using UnityEngine;

public class ModelSwitcher : MonoBehaviour
{
    public GameObject[] models;

    private int currentIndex = 0;

    void Start()
    {
        if (models.Length > 0)
        {
            UpdateVisibility();
        }
    }

    public void NextModel()
    {
        if (models.Length == 0) return;

        currentIndex++;
        if (currentIndex >= models.Length)
        {
            currentIndex = 0;
        }
        UpdateVisibility();
    }

    public void PreviousModel()
    {
        if (models.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = models.Length - 1;
        }
        UpdateVisibility();
    }


    private void UpdateVisibility()
    {
        for (int i = 0; i < models.Length; i++)
        {
            if (models[i] != null)
            {
                models[i].SetActive(i == currentIndex);
            }
        }
    }
}