using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonBehavior : MonoBehaviour
{
    public bool isSelected;
    public Shader outlineShader;
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
        if(isSelected)
        {
            rend.material.shader = defaultShader;
        }

        else
        {
            rend.material.shader = transparent;
        }

    }

    public void Select()
    {
        this.isSelected = true;
    }

    public void Deselect()
    {
        this.isSelected = false;
    }
}
