using UnityEngine;
using System.Collections;
using System.IO;


public class Plasma : MonoBehaviour {

    Color[] colour;
    int width = 1024;
    int length = 1024;
    float size;
    float grey;
    float col1, col2, col3, col4;
    int GRAIN = 8;
    float r, g, b;
    Texture2D texture, heightmap;
    //Terrain terrain;
    float[,] heights;
    GameObject terrain;
    TerrainData tData;

	// Use this for initialization
	void Start () 
    {
        size = (float)width + length;

        texture = new Texture2D(width, length);
        renderer.material.mainTexture = texture;

        colour = new Color[width * length];

        //sets a random height between 0 and 1 for the 4 corners of the grid
        col1 = Random.value;
        col2 = Random.value;
        col3 = Random.value;
        col4 = Random.value;

        //terrain = (Terrain)gameObject.GetComponent("Terrain");
        heights = new float[width, length];

        drawPlasma(width, length);
        texture.SetPixels(colour);
        texture.Apply();
        //byte[] img = texture.EncodeToPNG();
        //File.WriteAllBytes(Application.dataPath + "/Plasma" + ".png", img);
        setHeightMap();
	}
	
	// Update is called once per frame
	void Update () 
    {
       if(Input.GetKeyDown("n"))
       {
            col1 = Random.value;
            col2 = Random.value;
            col3 = Random.value;
            col4 = Random.value;
            drawPlasma(width, length);
            texture.SetPixels(colour);
            texture.Apply();
            setHeightMap();
            //byte[] img = texture.EncodeToPNG();
            //File.WriteAllBytes(Application.dataPath + "/Plasma" + ".png", img);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


	}

    float displace(float num)
    {
        float max = num / size * GRAIN;
        return Random.Range(-0.5f, 0.5f) * max;
    }

    //get a r, g, b value for each point on the grid
    Color getColour(float c)
    {
        r = 0;
        g = 0;
        b = 0;

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

        //grey = (0.299f * r) + (0.587f * g) + (0.114f * b);

        return new Color(r,g,b);
    }

    //divide the grid down
    void divideGrid(float x, float y, float w, float l, float c1, float c2, float c3, float c4)
    {
        float newWidth = w * 0.5f;
        float newLength = l * 0.5f;

        //get the average of the grid piece and draw as asingle pixel
        if(w < 1.0f || l < 1.0f)
        {
            float c = (c1 + c2 + c3 + c4) * 0.25f;
            colour[(int)x + (int)y * width] = getColour(c);
        }

        else
        {
            float middle = (c1 + c2 + c3 + c4) * 0.25f + displace(newWidth + newLength); //mid point displacement
            float edge1 = (c1 + c2) * 0.5f; //top edge average
            float edge2 = (c2 + c3) * 0.5f; //right edge average
            float edge3 = (c3 + c4) * 0.5f; // bottom edge average
            float edge4 = (c4 + c1) * 0.5f; //left edge average

            //check to see if the mid point is between 0 and 1
            if(middle <= 0)
            {
                middle = 0;
            }

            else if(middle > 1.0f)
            {
                middle = 1.0f;
            }

            //redo the operation for each of the new grids
            divideGrid(x, y, newWidth, newLength, c1, edge1, middle, edge4);
            divideGrid(x + newWidth, y, newWidth, newLength, edge1, c2, edge2, middle);
            divideGrid(x + newWidth, y + newLength, newWidth, newLength, middle, edge2, c3, edge3);
            divideGrid(x, y + newLength, newWidth, newLength, edge4, middle, edge3, c4);
        }
    }
    
    void drawPlasma(float w, float l)
    {
        //call operation to calculate averages and drawing
        divideGrid(0.0f, 0.0f, w, l, col1, col2, col3, col4);
    }

    //
    //For terrain
    void setHeightMap()
    {
        heightmap = new Texture2D(width, length);
        Color color = new Color();
        for(int i = 0; i < heightmap.height; i++)
        {
            for(int j = 0; j < heightmap.width; j++)
            {
                color = texture.GetPixel(i,j);
                //grey =  (0.299f * color.r) + (0.587f * color.g) + (0.114f * color.b);
                //color = new Color(grey,grey,grey);
                heightmap.SetPixel(i, j, color);
            }
        }
        loadHeightMap();
    }

    void loadHeightMap()
    {
        Color[] values = heightmap.GetPixels();
        int index = 0;
        for (int z = 0; z < heightmap.height; z++)
        {
            for (int x = 0; x < heightmap.width; x++)
            {
                heights[z, x] = values[index].grayscale;
                index++;
            }
        }
        //for (int i = 0; i != heights.Length; i++)
        //{
        //    for (int j = 0; j != heights.Length; j++)
        //    {
        //        Debug.Log(heights[i, j]);
        //    }
        //}
        applyTerrain();
    }

    void applyTerrain()
    {
        tData = new TerrainData();
        tData.heightmapResolution = width;
        tData.SetHeights(0, 0, heights);
        SplatPrototype[] terrainTexture = new SplatPrototype[1];
        terrainTexture[0] = new SplatPrototype();
        terrainTexture[0].texture = texture;
        terrainTexture[0].tileSize = new Vector2(width,length);
        tData.splatPrototypes = terrainTexture;
        terrain = Terrain.CreateTerrainGameObject(tData);
        terrain.name = "Terrain";
    }
}