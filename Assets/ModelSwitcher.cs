using UnityEngine;

public class ModelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] models;

    private int currentIndex;

    private void Start()
    {
        UpdateVisibility();
    }

    public void NextModel()
    {
        if (models == null || models.Length == 0)
            return;

        currentIndex =
            (currentIndex + 1) % models.Length;

        UpdateVisibility();
    }

    public void PreviousModel()
    {
        if (models == null || models.Length == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = models.Length - 1;
        }

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (models == null)
            return;

        for (int i = 0; i < models.Length; i++)
        {
            if (models[i] != null)
            {
                models[i].SetActive(
                    i == currentIndex
                );
            }
        }
    }
}