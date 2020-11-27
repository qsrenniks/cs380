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

  public Color rockColor;
  public Color landColor;
  public Color waterColor;

  private Color[] pix;

  public Layer populace = new Layer();
  public Layer buildings = new Layer();

  void Start()
  {
    pix = new Color[pixWidth * pixHeight];
    //populace.setTileIntensity(10, 10, 1.0f);
  }

  bool isWater(int x, int y)
  {
    float r = pix[x * pixWidth + y].r;
    if (r < waterLevel)
    {
      return true;
    }
    return false;
  }

  bool isLand(int x, int y)
  {
    float r = pix[x * pixWidth + y].r;
    if (r >= waterLevel && r < rockLevel)
    {
      return true;
    }
    return false;
  }

  bool isRock(int x, int y)
  {
    float r = pix[x * pixWidth + y].r;
    if (r >= rockLevel)
    {
      return true;
    }
    return false;
  }

  void CalcNoise()
  {
    // For each pixel in the texture...
    float y = 0.0F;

    while (y < pixWidth)
    {
      float x = 0.0F;
      while (x < pixHeight)
      {
        float xCoord = xOrg + x / pixWidth * scale;
        float yCoord = yOrg + y / pixHeight * scale;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        pix[(int)y * pixWidth + (int)x] = new Color(sample, sample, sample);
        x++;
      }
      y++;
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
          float val = pix[i * pixWidth + j].r;
          Color finalColor = Color.Lerp(waterColor, landColor, (val - 0.0f) / (waterLevel - 0.0f));
          grid.SetTile(grid.WorldToCell(start), white);
          grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
          grid.SetColor(grid.WorldToCell(start), finalColor);
        }
        else if (isLand(i, j))
        {
          float val = pix[i * pixWidth + j].r;
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
}
