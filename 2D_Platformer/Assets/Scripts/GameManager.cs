using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player player;
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDuration;

    [SerializeField] private int fruitCollected;
    [SerializeField] private bool randomFruit;
    [SerializeField] private int totalFruits;

    [SerializeField] private Fruit[] fruitList;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        Player.OnAnyPlayerDeath += Player_OnAnyPlayerDeath;
        UpdateFruitInfo();
    }

    private void UpdateFruitInfo()
    {
        fruitList = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = fruitList.Length;
    }

    private void Player_OnAnyPlayerDeath(object sender, System.EventArgs e)
    {
        RespawnPlayer();
    }

    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }
    public bool IsFruitRandom() { return randomFruit; }

    public int GetFruit() { return fruitCollected; }
    public void AddFruit() => fruitCollected++;


    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDuration);

        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

        player = newPlayer.GetComponent<Player>();
    }

    public void RespawnObject(GameObject prefab, Transform target, float delay =0,bool isToggleActive = false)
    {
        StartCoroutine(RespawnObjectCoroutine(prefab, target, delay, isToggleActive));
    }

    private IEnumerator RespawnObjectCoroutine(GameObject prefab, Transform target, float delay, bool isToggleActive)
    {
        if (!isToggleActive)
        {
            Vector3 spawnPos = target.position;

            yield return new WaitForSeconds(delay);

            GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.identity);
            newObject.SetActive(true);
            Destroy(prefab);
        }
        else
        {
            prefab.SetActive(false);
            yield return new WaitForSeconds(delay);
            prefab.SetActive(true);
        }
    }
}
