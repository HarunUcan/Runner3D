using UnityEngine;

[CreateAssetMenu(fileName = "New Obstacle Type", menuName = "Obstacle Type")]
public class ObstacleType : ScriptableObject
{
    public GameObject obstaclePrefab;
    public Obstacles obstacleType;
    public int obstacleSize;
    public bool canBeGoldOnTop;
    public bool canBeGoldOnBottom;
}
