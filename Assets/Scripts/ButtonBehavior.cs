using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    private Renderer rend;
    private Shader defaultShader;
    private Shader transparent;

    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<Renderer>();
        transparent = Shader.Find("Unlit/Transparent");
        defaultShader = Shader.Find("Standard");
        Deselect();
    }

    /// <summary>
    /// Change la variable qui vérifie si le bouton est sélectionné en true
    /// </summary>
    public void Select()
    {
        rend.material.shader = defaultShader;
    }

    /// <summary>
    /// Change la variable qui vérifie si le bouton est sélectionné en false
    /// </summary>
    public void Deselect()
    {
        rend.material.shader = transparent;
    }
}
