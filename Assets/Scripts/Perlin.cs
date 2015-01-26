﻿using UnityEngine;
using System.Collections;

public class Perlin : MonoBehaviour
{
    int x, y, z;
    float r, g, b;
    int size = 256;
    float TWO_PI = Mathf.PI * 2;
    float noiseScale;
    int[] perlin = new int[512];
    Color32[] colour;
    Texture2D texture;
    GameObject camera;

    // Use this for initialization
    void Start()
    {
        texture = new Texture2D(size, size);
        renderer.material.mainTexture = texture;
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.transform.position = new Vector3(0,12,0);
        colour = new Color32[size * size];
        noiseScale = Random.Range(0.01f, 0.1f);
        permutationSetup();
        z = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("n"))
        {
            noiseScale = Random.Range(0.01f, 0.1f);
        }

        for(y = 0; y < size; y++)
        {
            for(x = 0; x < size; x++)
            {
                float c = improvedNoise(x * noiseScale, y * noiseScale, z * noiseScale) * TWO_PI;
                colour[x + y * size] = getColour(c);
            }
        }
        z++;
        texture.SetPixels32(colour);
        texture.Apply();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    Color getColour(float c)
    {
        r = 0;
        g = 0;
        b = 0;

        if (c < 0.5f)
        {
            r = c * 2;
        }

        else
        {
            r = (1.0f - c) * 2;
        }

        if (c >= 0.3f && c < 0.8f)
        {
            g = (c - 0.3f) * 2;
        }

        else if (c < 0.3f)
        {
            g = (0.3f - c) * 2;
        }

        else
        {
            g = (1.3f - c) * 2;
        }

        if (c >= 0.5f)
        {
            b = (c - 0.5f) * 2;
        }

        else
        {
            b = (0.5f - c) * 2;
        }

        //grey = (0.299f * r) + (0.587f * g) + (0.114f * b);

        return new Color(r, g, b);
    }

    float improvedNoise(float x, float y, float z)
    {
        int newX = (int)Mathf.Floor(x) & 255;
        int newY = (int)Mathf.Floor(y) & 255;
        int newZ = (int)Mathf.Floor(z) & 255;
        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);
        z -= Mathf.Floor(z);
        float u = fade(x);
        float v = fade(y);
        float w = fade(z);
        int A = perlin[newX] + newY;
        int AA = perlin[A] + newZ;
        int AB = perlin[A + 1] + newZ;
        int B = perlin[newX + 1] + newY;
        int BA = perlin[B] + newZ;
        int BB = perlin[B + 1] + newZ;

        return lerp(w, lerp(v, lerp(u, grad(perlin[AA], x, y, z), 
            grad(perlin[BA], x - 1, y, z)), 
            lerp(u, grad(perlin[AB], x, y - 1, z), 
            grad(perlin[BB], x - 1, y - 1, z))), 
            lerp(v, lerp(u, grad(perlin[AA + 1], x, y, z - 1), 
            grad(perlin[BA + 1], x - 1, y, z - 1)), 
            lerp(u, grad(perlin[AB + 1], x, y - 1, z - 1), 
            grad(perlin[BB + 1], x - 1, y - 1, z - 1))));
    }

    float fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    float lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    float grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    void permutationSetup()
    {
        int[] permutationTable = new int[] {151,160,137,91,90,15,
   131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
   190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
   88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
   77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
   102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
   135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
   5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
   223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
   129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
   251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
   49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
   138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};
        for (int i = 0; i < 256; i++)
        {
            perlin[i] = perlin[i + 256] = permutationTable[i];
        }
    }
}