using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ColorManager : NetworkBehaviour
{
    public static ColorManager Instance;

    private static readonly Color[] AvailableColors = {
        Color.red, Color.blue, Color.green,
        Color.yellow, new Color(1f, 0.5f, 0f), new Color(0.5f, 0f, 1f)
    };

    private List<Color> usedColors = new List<Color>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Color GetRandomAvailableColor()
    {
        List<Color> remaining = new List<Color>();
        foreach (Color c in AvailableColors)
        {
            if (!usedColors.Contains(c))
                remaining.Add(c);
        }

        if (remaining.Count == 0)
        {
            Debug.LogWarning("No hay colores disponibles");
            return Color.white;
        }

        Color chosen = remaining[Random.Range(0, remaining.Count)];
        usedColors.Add(chosen);
        return chosen;
    }

    public Color GetNewColorExcluding(Color currentColor)
    {
        List<Color> remaining = new List<Color>();
        foreach (Color c in AvailableColors)
        {
            if (!usedColors.Contains(c) && c != currentColor)
                remaining.Add(c);
        }

        if (remaining.Count == 0)
            return currentColor;

        usedColors.Remove(currentColor);
        Color newColor = remaining[Random.Range(0, remaining.Count)];
        usedColors.Add(newColor);
        return newColor;
    }

    public void ReleaseColor(Color color)
    {
        if (usedColors.Contains(color))
            usedColors.Remove(color);
    }
}
