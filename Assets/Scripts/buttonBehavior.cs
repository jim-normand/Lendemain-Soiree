using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    public bool isSelected;
    //public Shader outlineShader;
    private Renderer rend;
    private Shader defaultShader;
    private Shader transparent;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        transparent = Shader.Find("Unlit/Transparent");
        defaultShader = Shader.Find("Standard");
    }

    // Update is called once per frame
    void Update()
    {
        /*if (isSelected)
        {
            rend.material.shader = defaultShader;
        }
        else
        {
            rend.material.shader = transparent;
        }*/

    }

    /// <summary>
    /// Change la variable qui vérifie si le bouton est sélectionné en true
    /// </summary>
    public void Select()
    {
        isSelected = true;
        rend.material.shader = defaultShader;
    }

    /// <summary>
    /// Change la variable qui vérifie si le bouton est sélectionné en false
    /// </summary>
    public void Deselect()
    {
        isSelected = false;
        rend.material.shader = transparent;
    }
}
