using UnityEngine;
using System.Collections;

public class Plasma : MonoBehaviour {

    Color32[] colour;
    int width = 128;
    int length = 128;
    float size;
    int GRAIN = 8;
    Texture2D texture;

	// Use this for initialization
	void Start () 
    {
        size = (float)width + length;

        texture = new Texture2D(width, length);
        renderer.material.mainTexture = texture;

        colour = new Color32[width * length];

        drawPlasma(width, length);
        texture.SetPixels32(colour);
        texture.Apply();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(Input.GetKeyDown("n"))
        {
            drawPlasma(width, length);
            texture.SetPixels32(colour);
            texture.Apply();
        }
	}

    float displace(float num)
    {
        float max = num / size * GRAIN;
        return Random.Range(-0.5f, 0.5f) * max;
    }

    Color getColour(float c)
    {
        float r = 0, g = 0, b = 0;

        if(c < 0.5f)
        {
            r = c * 2;
        }
        
        else
        {
            r = (1.0f - c) * 2;
        }

        if(c>=0.3f && c < 0.8f)
        {
            g = (c - 0.3f) * 2;
        }

        else if(c < 0.3f)
        {
            g = (0.3f - c) * 2;
        }

        else
        {
            g = (1.3f - c) * 2;
        }

        if(c>=0.5f)
        {
            b = (c - 0.5f) * 2;
        }

        else
        {
            b = (0.5f - c) * 2;
        }

        return new Color(r, g, b);
    }

    void divideGrid(float x, float y, float w, float l, float c1, float c2, float c3, float c4)
    {
        float newWidth = w * 0.5f;
        float newLength = l * 0.5f;

        if(w < 1.0f || l < 1.0f)
        {
            float c = (c1 + c2 + c3 + c4) * 0.25f;
            colour[(int)x + (int)y * width] = getColour(c);
        }

        else
        {
            float middle = (c1 + c2 + c3 + c4) * 0.25f + displace(newWidth + newLength);
            float edge1 = (c1 + c2) * 0.5f;
            float edge2 = (c2 + c3) * 0.5f;
            float edge3 = (c3 + c4) * 0.5f;
            float edge4 = (c4 + c1) * 0.5f;

            if(middle <= 0)
            {
                middle = 0;
            }

            else if(middle > 1.0f)
            {
                middle = 1.0f;
            }

            divideGrid(x, y, newWidth, newLength, c1, edge1, middle, edge4);
            divideGrid(x + newWidth, y, newWidth, newLength, edge1, c2, edge2, middle);
            divideGrid(x + newWidth, y + newLength, newWidth, newLength, middle, edge2, c3, edge3);
            divideGrid(x, y + newLength, newWidth, newLength, edge4, middle, edge3, c4);
        }
    }

    void drawPlasma(float w, float l)
    {
        float c1, c2, c3, c4;
        c1 = Random.value;
        c2 = Random.value;
        c3 = Random.value;
        c4 = Random.value;

        divideGrid(0.0f, 0.0f, w, l, c1, c2, c3, c4);
    }

}
