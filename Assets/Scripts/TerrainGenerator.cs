using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Layer
{
  public Tilemap grid;
  public Tile white;
  public Color layerColor;

  public void setTileIntensity(int x, int y, float intensity)
  {
    Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
    start.x += x;
    start.y += y;
    grid.SetTile(grid.WorldToCell(start), white);
    grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
    grid.SetColor(grid.WorldToCell(start), layerColor * intensity);
  }
}

public class TerrainGenerator : MonoBehaviour
{
  public Tilemap grid;
  public Tile white;
  public int pixWidth = 10;
  public int pixHeight = 10;

  public float xOrg;
  public float yOrg;

  public float scale = 1.0f;
  public float waterLevel = 0.5f;
  public float rockLevel = 0.9f;
  public float power = 1.0f;

  public Color rockColor;
  public Color landColor;
  public Color waterColor;

  private float[] pix;

  public Layer populaceLayer = new Layer();
  public Layer buildingsLayer = new Layer();

  void Start()
  {
    pix = new float[pixWidth * pixHeight];
    //populaceLayer.setTileIntensity(10, 10, 1.0f);
  }

  bool isWater(int x, int y)
  {
    float r = pix[x * pixWidth + y];
    if (r < waterLevel)
    {
      return true;
    }
    return false;
  }

  bool isLand(int x, int y)
  {
    float r = pix[x * pixWidth + y];
    if (r >= waterLevel && r < rockLevel)
    {
      return true;
    }
    return false;
  }

  bool isRock(int x, int y)
  {
    float r = pix[x * pixWidth + y];
    if (r >= rockLevel)
    {
      return true;
    }
    return false;
  }

  void CalcNoise()
  {
    // For each pixel in the texture...

    for(int y = 0; y < pixHeight; ++y)
    {
      for(int x = 0; x < pixWidth; ++x)
      {
        float xCoord = ((float)(x) / (float)(pixWidth)) * scale;
        float yCoord = ((float)(y) / (float)(pixHeight)) * scale;

        float final = Mathf.PerlinNoise(xCoord, yCoord) 
                    + 0.5f * Mathf.PerlinNoise(2.0f * xCoord, 2.0f * yCoord)
                    + 0.25f * Mathf.PerlinNoise(4.0f * xCoord, 4.0f * yCoord);
        pix[x * pixWidth + y] = Mathf.Pow(final, power);
      }
    }

    // Copy the pixel data to the texture and load it into the GPU.
    //noiseTex.SetPixels(pix);
    //noiseTex.Apply();

    for (int i = 0; i < pixWidth; ++i)
    {
      for (int j = 0; j < pixHeight; ++j)
      {
        Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
        start.x += i;
        start.y += j;

        if (isWater(i, j))
        {
          float val = pix[i * pixWidth + j];
          Color finalColor = Color.Lerp(waterColor, landColor, (val - 0.0f) / (waterLevel - 0.0f));
          grid.SetTile(grid.WorldToCell(start), white);
          grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
          grid.SetColor(grid.WorldToCell(start), finalColor);
        }
        else if (isLand(i, j))
        {
          float val = pix[i * pixWidth + j];
          Color finalColor = Color.Lerp(landColor, rockColor, (val - waterLevel) / (rockLevel - waterLevel));
          grid.SetTile(grid.WorldToCell(start), white);
          grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
          grid.SetColor(grid.WorldToCell(start), finalColor);
        }
        else if(isRock(i, j))
        {
          grid.SetTile(grid.WorldToCell(start), white);
          grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
          grid.SetColor(grid.WorldToCell(start), rockColor);
        }
      }
    }
  }

  void Update()
  {
    CalcNoise();
  }

  public int dropletDensity = 10; // every step in a direction knocks one off this list until the rain drop settles
  public float maxSedimentPickup = 0.5f;
  public float minSedimentPickup = 0.01f;

  System.Random prng;
  void Erode()
  {
    for(uint i = 0; i < 100; ++i)
    {
      int x = prng.Next(0, pixWidth - 1);
      int y = prng.Next(0, pixHeight - 1);
      int currentLife = dropletDensity;
      float currentSedimentPickup = 0.0f;

      while(currentLife > 0)
      {
        //from neighbors determine the lowest level to travel to
        int nextX = x;
        int nextY = y;
        //move x and y to that level and verify it is valid position
        if(nextX >= pixWidth || nextY >= pixHeight)
        {
          break;
        }
        //if sediment max is reached or did not move from previous position
        if(currentSedimentPickup >= maxSedimentPickup)
        {
          pix[x * pixWidth + y] += currentSedimentPickup; // distributed directly to the tile
        }
        //else gather sediment affecting the height of the map from previous position
        else
        {
          float sedimentPickup = Random.Range(minSedimentPickup, maxSedimentPickup);
          float delta = pix[x * pixWidth * y] - sedimentPickup;
          currentSedimentPickup += sedimentPickup;
          pix[x * pixWidth * y] = delta;
        }
        --currentLife;
      }
    }
  }
}
