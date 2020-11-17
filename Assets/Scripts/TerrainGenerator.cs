using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
  public Tilemap grid;
  public Tile water;
  public Tile land;
  public Tile rock;
  public int pixWidth = 10;
  public int pixHeight = 10;

  public float xOrg;
  public float yOrg;

  public float scale = 1.0f;
  public float waterLevel = 0.5f;
  public float rockLevel = 0.9f;

  private Color[] pix;

  void Start()
  {
    pix = new Color[pixWidth * pixHeight];
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
        float r = pix[i * pixWidth + j].r;
        if (r < waterLevel)
        {
          grid.SetTile(grid.WorldToCell(start), water);
        }
        else if (r >= waterLevel && r < rockLevel)
        {
          grid.SetTile(grid.WorldToCell(start), land);
        }
        else
        { 
          grid.SetTile(grid.WorldToCell(start), rock);
        }
      }
    }
  }

  void Update()
  {
    CalcNoise();
  }
}
