using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WaveController : MonoBehaviour
{
    RawImage image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        var rect = image.uvRect;
        rect.x += Time.deltaTime/3;
        image.uvRect = rect;
    }
}
