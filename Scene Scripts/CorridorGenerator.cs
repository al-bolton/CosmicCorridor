using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    // Classes used here for prefabs used in corridor generation
    [Serializable]
    class Wall
    {
        public GameObject wallPrefab;
        public Vector3 wallRotation;
        public Vector3 wallPosition;
        public int wallProbability;
    }

    [Serializable]
    class Floor
    {
        public GameObject floorPrefab;
        public Vector3 floorRotation;
        public Vector3 floorPosition;
    }

    [Serializable]
    class Obstacle
    {
        public GameObject obstaclePrefab;
        public Vector3 obstacleRotation;
        public Vector3 obstaclePosition;
        public int obstacleProbability;
    }


    [Header("Generation Prefabs")]
    // Total used to track overall probability counts (they don't have to add up
    // to 100, so it's not a true probability)
    public int wallProbabilityTotal = 0;
    public int obstacleProbabilityTotal = 0;
    // Arrays used in generation
    [Tooltip("Array used for wall prefabs used in corridor generation")][SerializeField] Wall[] walls = null;
    [Tooltip("Array used for floor prefabs used in corridor generation")] [SerializeField] Floor[] floors = null;
    [Tooltip("Array used for obstacle prefabs used in corridor generation")] [SerializeField] Obstacle[] obstacles = null;

    [Header("Generation parameters")]
    [Tooltip("The chance of an obstacle being generated on a tile")] [SerializeField] float obstacleChance = 0.05f;
    [Tooltip("The max no. of obstacles on a corridor section")] [SerializeField] int maxObstacles = 2;
    [Tooltip("The max no. of obstacles on a corridor section")] [SerializeField] int obstacleOffset = 5;

    [Header("Location parameters")]
    [Tooltip("The movement to be made by the player before a new corridor section is generated")] [SerializeField] float movementPerGeneration = 2f;
    [Tooltip("Tile width")] [SerializeField] float widthOfTile = 2f;
    [Tooltip("The amount of tile wide the corridor is")] [SerializeField] int amountOfTiles = 5;
    [Tooltip("How far away the corridor geenrator starts from the player")] [SerializeField] float startingDistanceFromPlayer = 50f;

    [Header("Related Game Objects")]
    [SerializeField] GameObject sceneMovingObjectHolder = null;
    [SerializeField] GameObject corridorSectionParent = null;

    // Vectors used in setting the corridor generator
    Vector3 secondWallCorrection;
    Vector3 scenePosition;

    int corridorsGenerated = 0;

    // Array to be used in obstacle detection
    public List<List<int>> obstacleListTracker = new List<List<int>>();
    

    // Start is called before the first frame update
    void Start()
    {
        // Find where the sceneMovingObject is located in the scene
        scenePosition = sceneMovingObjectHolder.transform.localPosition;

        // Set the correction used to place the second wall (required as wall
        // needs to be rotated and moved to be opposite first wall)
        secondWallCorrection = new Vector3 (widthOfTile * amountOfTiles, 0 , movementPerGeneration);

        // Find the total 'probability' for all the walls
        for (int i = 0; i < walls.Length; i++)
        {
            wallProbabilityTotal += walls[i].wallProbability;
        }

        // Find the total 'probability' for all the obstacles
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacleProbabilityTotal += obstacles[i].obstacleProbability;
        }

        // Initial array population for obstacles as they're not generated until offset
        InitialObstacleArrayPopulation();

        // Generate the initial corridor before the game starts
        InitialCorridorGeneration();
    }

    private void InitialObstacleArrayPopulation()
    {
        for (int i = 0; i < obstacleOffset; i++)
        {
            List<int> sectionObstacles = new List<int>();

            for (int tile = 0; tile < amountOfTiles; tile++)
            {
                sectionObstacles.Add(0);
            }

            obstacleListTracker.Add(sectionObstacles);
        }
    }

    private void InitialCorridorGeneration()
    {
        for (float i = 0; i <= startingDistanceFromPlayer; i += movementPerGeneration)
        {
            // Generate a corridor section until the generator hits the set distance
            GenerateCorridorSection();

            // Move the Corridor Generator along, unless it's the last generation
            if (i != startingDistanceFromPlayer)
            {
                transform.position = new Vector3(transform.position.x,
                transform.position.y,
                transform.position.z + movementPerGeneration);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Determine if the scene has moved enough to generate a new corridor section
        SceneMovementTracker();
    }

    private void SceneMovementTracker()
    {
        // Find how far the scene has moved
        float sceneAmountMoved = Vector3.Distance(scenePosition, sceneMovingObjectHolder.transform.localPosition);

        // If it's moved enough, we generate a corridor section and reset the
        // scene movement tracker
        if (sceneAmountMoved >= movementPerGeneration)
        {
            GenerateCorridorSection();
            scenePosition = sceneMovingObjectHolder.transform.localPosition;
        }
    }

    private void GenerateCorridorSection()
    {
        // Instantiate the first wall
        int firstWallGenerated = GenerateRandomWall(walls.Length);
        Instantiate(walls[firstWallGenerated].wallPrefab, transform.position + walls[firstWallGenerated].wallPosition, Quaternion.Euler(walls[firstWallGenerated].wallRotation), corridorSectionParent.transform);

        // Instantiate the floor. Only one tile type currently so built for that

        for (int i = 0; i < amountOfTiles; i++)
        {
            Vector3 floorLocalPosition = new Vector3(transform.position.x + i * widthOfTile, transform.position.y, transform.position.z);

            Instantiate(floors[0].floorPrefab, floorLocalPosition + floors[0].floorPosition, Quaternion.Euler(floors[0].floorRotation), corridorSectionParent.transform);
        }

        // Instantiate the second wall
        int secondWallGenerated = GenerateRandomWall(walls.Length);

        Vector3 secondWallTransform = new Vector3(transform.position.x + secondWallCorrection.x - walls[secondWallGenerated].wallPosition.x,
            transform.position.y + secondWallCorrection.y + walls[secondWallGenerated].wallPosition.y,
            transform.position.z + secondWallCorrection.z + walls[secondWallGenerated].wallPosition.z);

        Instantiate(walls[secondWallGenerated].wallPrefab, secondWallTransform, Quaternion.Inverse(Quaternion.Euler(walls[secondWallGenerated].wallRotation)), corridorSectionParent.transform);

        // Only start to set obstacles after initial offset by changing chance to true chance
        if (corridorsGenerated >= obstacleOffset)
        {
            // Instantiate obstacles
            GenerateObstacles();
        }

        corridorsGenerated++;
    }

    private int GenerateRandomWall(int wallArrayLength)
    {
        // Finds a random value within the sum total of the wall probabilities
        int randomWallValue = UnityEngine.Random.Range(0, wallProbabilityTotal);

        // Generic integer needed for checking if the value generated is in
        // that wall's probability range
        int sumWallProbability = 0;

        // Move through all of the walls and probabilities, and return the value
        // if random value hit this wall's probability
        for (int i = 0; i < wallArrayLength; i++)
        {
            sumWallProbability += walls[i].wallProbability;

            if (randomWallValue <= sumWallProbability)
            {
                return i;
            }
        }

        // If this fails (which I can't see how) - famous last words, then it returns wall[0], which should be the generic wall
        return 0;
    }

    private void GenerateObstacles()
    {
        // First we want to work out how many obstacles there will be
        int obstaclesGeneratedThisSection = 0;

        // Set up our temporary array which we'll randomise
        int[] tempObstaclesArray = new int[amountOfTiles];

        for (int i = 0; i < amountOfTiles; i++)
        {
            tempObstaclesArray[i] = i;
        }

        // Shuffle the array
        ShuffleIntArray(tempObstaclesArray);

        // Before we set the obstacles, we create a list which we'll add to our
        // obstacle tracker

        List<int> sectionObstacles = new List<int>();

        for (int tile = 0; tile < amountOfTiles; tile++)
        {
            sectionObstacles.Add(0);
        }

        // For each tile, we find if we instantiate an obstacle on it using
        // separate function
        for (int i = 0; i < amountOfTiles; i++)
        {
            if (obstaclesGeneratedThisSection >= maxObstacles)
            {
                break;
            }

            if (ObstacleGenerationAttempt())
            {
                GenerateRandomObstacle(tempObstaclesArray, i);

                sectionObstacles[tempObstaclesArray[i]] = 1;

                obstaclesGeneratedThisSection++;
            }
        }

        // We add this to our overall obstacle tracker so it knows which tiles
        // are occupied by an obstacle
        obstacleListTracker.Add(sectionObstacles);
    }

    private void ShuffleIntArray(int[] arrayToBeShuffled)
    {
        // Function to shuffle an int array, so that we try to put obstacles on
        // random tiles and aren't bised to left of corridor

        for (int i = 0; i < arrayToBeShuffled.Length; i++)
        {
            int temp = arrayToBeShuffled[i];
            int randomIndex = UnityEngine.Random.Range(i, arrayToBeShuffled.Length);
            arrayToBeShuffled[i] = arrayToBeShuffled[randomIndex];
            arrayToBeShuffled[randomIndex] = temp;
        }
    }

    private void GenerateRandomObstacle(int[] tempObstaclesArray, int i)
    {
        // Finds a random value within the sum total of the obstacle probabilities
        int randomObstacleValue = UnityEngine.Random.Range(0, obstacleProbabilityTotal);

        // Generic integer needed for checking if the value generated is in
        // that obstacle's probability range
        int sumObstacleProbability = 0;

        int obstacleIndexToUse = 0;

        // Move through all of the obstacles and probabilities, and return the value
        // if random value hit this obstacle's probability. Works similar to how
        // functionality used in walls
        for (int j = 0; j < obstacles.Length; j++)
        {
            sumObstacleProbability += obstacles[j].obstacleProbability;

            if (randomObstacleValue <= sumObstacleProbability)
            {
                obstacleIndexToUse = j;
                break;
            }
        }

        // Set the position based on which tile we want to generate the obstacle on top of
        Vector3 obstaclePosition = new Vector3(transform.position.x + (1 + tempObstaclesArray[i] * 2), transform.position.y, transform.position.z + 1);

        // Finally, we instantiate the obstacle
        Instantiate(obstacles[obstacleIndexToUse].obstaclePrefab, obstaclePosition, Quaternion.Euler(obstacles[obstacleIndexToUse].obstacleRotation), corridorSectionParent.transform);
    }

    private bool ObstacleGenerationAttempt()
    {
        // Function to determine if we will generate an obstacle on this tile
        
        float randomRoll = UnityEngine.Random.Range(0f, 1f);

        if (randomRoll <= obstacleChance)
        {
            return true;
        }

        return false;
    }
}