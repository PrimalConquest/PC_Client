using UnityEngine;

public class MatchmakingSetup : MonoBehaviour
{

    [SerializeField] MatchmakingService _matchmakingService;
    [SerializeField] MatchmakingScreenController _matchmakingScreenController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _matchmakingService.Init();
        _matchmakingScreenController.Init();
    }

}
