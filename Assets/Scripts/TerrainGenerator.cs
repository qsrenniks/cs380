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
    Color newColor = layerColor * intensity;
    grid.SetColor(grid.WorldToCell(start), newColor);
  }

  public float getTileIntensity(int x, int y)
  {
    Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);

    start.x += x;
    start.y += y;
    grid.SetTile(grid.WorldToCell(start), white);
    grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
    return grid.GetColor(grid.WorldToCell(start)).r;
  }
}

public class TerrainGenerator : MonoBehaviour
{
  public Tilemap grid;
  public Tile white;
  public int mapSize = 100;

  public float xOrg;
  public float yOrg;

  public float scale = 1.0f;
  public float waterLevel = 0.5f;
  private float currentWaterLevel = 0.5f;
  public float waterLevelOffsetMax = 0.1f;
  public float rockLevel = 0.9f;
  public float power = 1.0f;

  public void onScaleChanged(float newScale)
  {
    scale = newScale;
  }

  public void onRockLevelChanged(float newRockLevel)
  {
    rockLevel = newRockLevel;
  }

  public void onWaterLevelChanged(float newWaterLevel)
  {
    waterLevel = newWaterLevel;
  }

  public void onMaxSedimentLoadChanged(float newSedimentLoad)
  {
    maxSedimentPickup = newSedimentLoad;
  }

  public void onMinSedimentLoadChanged(float newSedimentLoad)
  {
    minSedimentPickup = newSedimentLoad;
  }

  public void onErosionRadius(float erosion)
  {
    erosionRadius = (int)erosion;
  }

  public void onDropletLifetime(float dropletLifetime)
  {
    maxDropletLifetime = (int)dropletLifetime;
  }

  public void onDropletSpeed(float speed)
  {
    startSpeed = speed;
  }

  public void onEvaporationSpeed(float evapSpeed)
  {
    evaporationSpeed = evapSpeed;
  }

  public Color rockColor;
  public Color landColor;
  public Color waterColor;

  public GameObject weatherManager;
  public GameObject cameraObject;

  private float[] pix;

  public Layer populaceLayer = new Layer();
  public Layer buildingsLayer = new Layer();
  public Layer foodLayer = new Layer();
  public Layer foliageLayer = new Layer();
  public Layer snowLayer = new Layer();

    public int dropletDensity = 10; // every step in a direction knocks one off this list until the rain drop settles
  public float maxSedimentPickup = 0.5f;
  public float minSedimentPickup = 0.01f;
  public int seed;
  public int erosionRadius = 3;
  public float startSpeed = 1.0f;
  public float startDropletVolume = 1.0f;
  public int maxDropletLifetime = 30;
  public float inertia = 0.05f;
  public float sedimentCapacityCoeff = 4.0f;
  public float minSedimentCapacity = 0.01f;
  public float gravity = 4.0f;
  public float evaporationSpeed = 0.01f;
  public float depositSpeed = 0.3f;
  public float erodeSpeed = 0.3f;
  public int rainPerDay = 10;
  public float treeR = 50.0f;
  public float snowElevationLimit = 0.9f;

  private float currentSnowAmount = 0.0f;

  //this is the erosion layer of the map that helps tell each node how they should evenly pull and place sediment
  int [][] erosionIndices;
  float [][] erosionWeights;

  System.Random rand;
  int currentSeed;
  int currentErosionRadius;
  int currentMapSize;

  void Awake()
  {
      pix = new float[mapSize * mapSize];
      cameraObject.transform.position = new Vector3((float)mapSize / 2.0f, (float)mapSize / 2.0f, -10.0f);
      Camera a = cameraObject.GetComponent(typeof(Camera)) as Camera;
      a.orthographicSize = ((float)mapSize / 2.0f) + 15.0f;
      currentWaterLevel = waterLevel;
      //populaceLayer.setTileIntensity(10, 10, 1.0f);
      CalcNoise();
      Initialize(false);
  }
  void Start()
  {
      
  }

  public bool IsWater(int x, int y)
  {
    float r = pix[x * mapSize + y];
    if (r < currentWaterLevel)
    {
      return true;
    }
    return false;
  }

  public bool IsLand(int x, int y)
  {
    float r = pix[x * mapSize + y];
    if (r >= currentWaterLevel && r < rockLevel)
    {
      return true;
    }
    return false;
  }

  public bool IsRock(int x, int y)
  {
    float r = pix[x * mapSize + y];
    if (r >= rockLevel)
    {
      return true;
    }
    return false;
  }

  void CalcNoise()
  {
    for(int y = 0; y < mapSize; ++y)
    {
      for(int x = 0; x < mapSize; ++x)
      {
        float xCoord = (((float)(x) / (float)(mapSize))) * scale;
        float yCoord = (((float)(y) / (float)(mapSize))) * scale;

        float final = Mathf.PerlinNoise(xCoord, yCoord) 
        + 0.5f * Mathf.PerlinNoise(2.0f * xCoord, 2.0f * yCoord)
        + 0.25f * Mathf.PerlinNoise(4.0f * xCoord, 4.0f * yCoord);
        pix[x * mapSize + y] = 1.0f - Mathf.Pow(final, power);
      }
    }
  }

  public bool IsInBounds(int x, int y)
  {
     if (x < 0 || x >= mapSize)
       return false;
     if (y < 0 || y >= mapSize)
       return false;
     return true;
  }

  void Update()
  {
    WeatherController weatherController = weatherManager.GetComponent(typeof(WeatherController)) as WeatherController;
    WeatherController.weather cW = weatherController.getCurrentWeather();

    if(cW == WeatherController.weather.rain)
    {
      Erode(rainPerDay);

      currentWaterLevel -= Time.deltaTime * TimeManager.Instance.realSecondsToGameDay;
      currentWaterLevel = Mathf.Clamp(currentWaterLevel, waterLevel - waterLevelOffsetMax, waterLevel + waterLevelOffsetMax);
    }
    else if(cW == WeatherController.weather.clear)
    {
      float snowDelta = Time.deltaTime * TimeManager.Instance.realSecondsToGameDay;
      currentSnowAmount -= snowDelta;
      currentSnowAmount = Mathf.Clamp(currentSnowAmount, 0.0f, 1.0f);

      currentWaterLevel += snowDelta;
      currentWaterLevel = Mathf.Clamp(currentWaterLevel, waterLevel - waterLevelOffsetMax, waterLevel + waterLevelOffsetMax);
    }
    else if(cW == WeatherController.weather.snow)
    {
      currentSnowAmount += Time.deltaTime * TimeManager.Instance.realSecondsToGameDay;
      currentSnowAmount = Mathf.Clamp(currentSnowAmount, 0.0f, 1.0f);
    }

    for(int y = 0; y < mapSize; ++y)
    {
      for(int x = 0; x < mapSize; ++x)
      {
        float xCoord = (((float)(x) / (float)(mapSize))) * treeR;
        float yCoord = (((float)(y) / (float)(mapSize))) * treeR;

        float final = Mathf.PerlinNoise(xCoord, yCoord);

        if(IsLand(x, y) && final >= 0.8f)
        {
          foliageLayer.setTileIntensity(x, y, 1.0f);
        }
        else 
        {
          foliageLayer.setTileIntensity(x, y, 0.0f);
        }

        float xCoordFood = (((float)(x) / (float)(mapSize))) * treeR;
        float yCoordFood = (((float)(y) / (float)(mapSize))) * treeR;
        float foodFinal = Mathf.PerlinNoise(xCoordFood, yCoordFood);
        if (IsLand(x, y) && foodFinal >= 0.65f)
        {
          foodLayer.setTileIntensity(x, y, 1.0f);
        }
        else
        {
          foodLayer.setTileIntensity(x, y, 0.0f);
        }

        if (IsLand(x, y) || IsRock(x, y))
        {
          float height = pix[x * mapSize + y];
          float snow = (currentSnowAmount) * height;
          if(snow >= snowElevationLimit)
          {
            snowLayer.setTileIntensity(x, y, 1.0f);
          }
          else
          {
            snowLayer.setTileIntensity(x, y, 0.0f);
          }
        }
        else
        {
          snowLayer.setTileIntensity(x, y, 0.0f);
        }
      }
    }

    for (int i = 0; i < mapSize; ++i)
    {
      for (int j = 0; j < mapSize; ++j)
      {
        Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
        start.x += i;
        start.y += j;

        if (IsWater(i, j))
        {
          float val = pix[i * mapSize + j];
          //Color finalColor = Color.Lerp(waterColor, landColor, (val - 0.0f) / (currentWaterLevel - 0.0f));
          Color finalColor = waterColor;
          grid.SetTile(grid.WorldToCell(start), white);
          grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
          grid.SetColor(grid.WorldToCell(start), finalColor);
        }
        else if (IsLand(i, j))
        {
          float val = pix[i * mapSize + j];
          //Color finalColor = Color.Lerp(landColor, rockColor, (val - currentWaterLevel) / (rockLevel - currentWaterLevel));
          Color finalColor = landColor;
          grid.SetTile(grid.WorldToCell(start), white);
          grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
          grid.SetColor(grid.WorldToCell(start), finalColor);
        }
        else if(IsRock(i, j))
        {
          grid.SetTile(grid.WorldToCell(start), white);
          grid.SetTileFlags(grid.WorldToCell(start), TileFlags.None);
          grid.SetColor(grid.WorldToCell(start), rockColor);
        }
      }
    }
  }

  void Initialize(bool setSeed)
  {
    //initializes the seed for the raindrop generator, if its different it resets the seed
    if(setSeed || rand == null || currentSeed != seed)
    {
      rand = new System.Random(seed);
      currentSeed = seed;
    }

    //if the map has changed or the indices arent setup we need to set them up
    // this method of erosion distributes the sediment collection evenly around the rain drop
    // this prevents the raindrop from digging craters where it travels
    if(erosionIndices == null || currentErosionRadius != erosionRadius || currentMapSize != mapSize)
    {
      InitializeIndices(erosionRadius);
      currentErosionRadius = erosionRadius;
      currentMapSize = mapSize;
    }
  }

  void Erode(int numberOfRaindrops, bool resetSeed = false)
  {


    for(uint i = 0; i < numberOfRaindrops; ++i)
    {
      float x = rand.Next(0, mapSize - 1);
      float y = rand.Next(0, mapSize - 1);
      float dirX = 0.0f;
      float dirY = 0.0f;
      float speed = startSpeed;
      float water = startDropletVolume;
      float sediment = 0.0f;

      for(int lifetime = 0; lifetime < maxDropletLifetime; ++lifetime)
      {
        int xGridIndex = (int)x;
        int yGridIndex = (int)y;
        int dropletIndex = yGridIndex * mapSize + xGridIndex;

        float cellOffsetX = x - xGridIndex;
        float cellOffsetY = y - yGridIndex;

        Gradient grad = CalculateGradient(x, y);
        dirX = (dirX * inertia - grad.gradX * (1.0f - inertia));
        dirY = (dirY * inertia - grad.gradY * (1.0f - inertia));

        float len = Mathf.Sqrt(dirX * dirX + dirY * dirY); // this is a much easier way of determining where to go next then creating neighbor lists and determining heights and then returning the next index to go down

        if (len != 0)
        {
          dirX /= len;
          dirY /= len;
        }
        x += dirX;
        y += dirY;

        if((dirX == 0 && dirY == 0) || x < 0 || x >= mapSize -1 || y < 0 || y >= mapSize - 1) //makes sure the pos doesnt fall off the map
        {
          break; // if the raindrop does fall off we break out of that droplets sim
        }

        float newHeight = CalculateGradient(x, y).height; // calculate height at new location to pull sediment from
        float delta = newHeight - grad.height; // difference from current to new node

        float sedimentCapacity = Mathf.Max(-delta * speed * water * sedimentCapacityCoeff, minSedimentCapacity);

        if(sediment > sedimentCapacity || delta > 0.0f)
        {
          float depositAmount = (delta > 0) ? Mathf.Min(delta, sediment) : (sediment - sedimentCapacity) * depositSpeed;
          sediment -= depositAmount;

          pix[dropletIndex] += depositAmount * (1.0f - cellOffsetX) * (1.0f - cellOffsetY);
          pix[dropletIndex + 1] += depositAmount * cellOffsetX * (1.0f - cellOffsetY);
          pix[dropletIndex + mapSize] += depositAmount * (1.0f - cellOffsetX) * cellOffsetY;
          pix[dropletIndex + mapSize + 1] += depositAmount * cellOffsetX * cellOffsetY;
        }
        else 
        {
          float erodeAmount = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed);

          for (int j = 0; j < erosionIndices[dropletIndex].Length; ++j) //run through indices eroding from the radius from the droplet
          {
            int index = erosionIndices[dropletIndex][j];
            float erode = erodeAmount * erosionWeights[dropletIndex][j];
            float erodeDelta = (pix[index] < erode) ? pix[index] : erode;
            pix[index] -= erodeDelta;
            sediment += erodeDelta;
          }
        }

        speed = Mathf.Sqrt(speed * speed + delta * gravity);
        water *= (1.0f - evaporationSpeed); // evaporate water volume from droplet
      }
    }
  }

  Gradient CalculateGradient(float posX, float posY)
  {
    int coordX = (int)posX;
    int coordY = (int)posY;

    float x = posX - coordX;
    float y = posY - coordY;

    int index = coordY * mapSize + coordX;
    float heightNW = pix[index];
    float heightNE = pix[index + 1];
    float heightSW = pix[index + mapSize];
    float heightSE = pix[index + mapSize + 1];

    float gradientX = (heightNE - heightNW) * (1-y) + (heightSE - heightSW) * y;
    float gradientY = (heightSW - heightNW) * (1-x) + (heightSE - heightNE) * x;

    //uses bilinear interpolation on the cell and the surrounding cell
    float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

    Gradient grad = new Gradient();
    grad.height = height;
    grad.gradX = gradientX;
    grad.gradY = gradientY;
    return grad;
  }

  void InitializeIndices(int eRadius)
  {
    erosionIndices = new int[mapSize * mapSize][];
    erosionWeights = new float[mapSize * mapSize][];

    //4 offsets around a cell
    int[] xOffset = new int[eRadius * eRadius * 4];
    int[] yOffset = new int[eRadius * eRadius * 4];

    float[] weights = new float[eRadius * eRadius * 4];

    float weightSum = 0;
    int addIndex = 0;

    for(int i = 0; i < erosionIndices.GetLength(0); ++i)
    {
      int xIndex = i % mapSize;
      int yIndex = i / mapSize;

      if(yIndex <= eRadius || yIndex >= mapSize - eRadius || xIndex <= eRadius + 1 || xIndex >= mapSize - eRadius)
      {
        weightSum = 0;
        addIndex = 0;

        for (int y = -eRadius; y <= eRadius; ++y)
        {
          for(int x = -eRadius; x <= eRadius; ++x)
          {
            float sqrDist = x * x + y * y;
            if(sqrDist < eRadius * eRadius)
            {
              int xc = xIndex + x;
              int yc = yIndex + y;

              if(xc >= 0 && xc < mapSize && yc >= 0 && yc < mapSize)
              {
                float weight = 1.0f - Mathf.Sqrt(sqrDist) / eRadius;
                weightSum += weight;
                weights[addIndex] = weight;
                xOffset[addIndex] = x;
                yOffset[addIndex] = y;
                ++addIndex;
              }
            }
          }
        }
      }

      int numEntries = addIndex; //num of neighbors for each node
      erosionIndices[i] = new int[numEntries];
      erosionWeights[i] = new float[numEntries];

      for(int j = 0; j < numEntries; ++j)
      {
        erosionIndices[i][j] = (yOffset[j] + yIndex) * mapSize + xOffset[j] + xIndex;
        erosionWeights[i][j] = weights[j] / weightSum;
      }
    }
  }

  struct Gradient
  {
    public float height;
    public float gradX;
    public float gradY;
  }
}
