using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll_UI : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Time.deltaTime * scrollSpeed;

        image.uvRect = new Rect(new Vector2(image.uvRect.x + offset, image.uvRect.y), new Vector2(image.uvRect.size.x, image.uvRect.size.y));
    }
}
