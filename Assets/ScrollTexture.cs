using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField] Vector2 scroll;
    private Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    Vector2 offset;
    // Update is called once per frame
    void Update()
    {
        offset += Time.deltaTime * scroll;
        renderer.material.SetTextureOffset("_MainTex", offset);
    }
}
