using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public float minX = 151.0f, maxX = 12.0f;

    public Color minColor = Color.red, maxColor = Color.green;

    private RectTransform _rt;
    private Image _image;


    private void Start() {
        _rt = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    public void setProportion(float prop) {
        _rt.offsetMax = new Vector2(-Mathf.Lerp(minX, maxX, prop), _rt.offsetMax.y);
        _image.color = Color.Lerp(minColor, maxColor, prop);
    }
}
