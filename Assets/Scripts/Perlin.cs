﻿using UnityEngine;
using System.Collections;

public class Perlin : MonoBehaviour
{

    int size = 1024;
    float noiseScale;
    Texture2D texture;

    // Use this for initialization
    void Start()
    {
        texture = new Texture2D(size, size);
        renderer.material.mainTexture = texture;
        noiseScale = Random.Range(0.01f, 0.1f);
        setHeights();
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("n"))
        {
            noiseScale = Random.Range(0.01f, 0.1f);
            setHeights();
            texture.Apply();
        }
    }

    void setHeights()
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float value = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                texture.SetPixel(x, y, new Color(value, value, value, 1));
            }
        }
    }
}