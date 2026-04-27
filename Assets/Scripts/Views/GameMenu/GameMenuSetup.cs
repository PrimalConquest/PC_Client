using UnityEngine;

public class GameMenuSetup : MonoBehaviour
{

    [SerializeField] LoadoutViewModel _loadoutViewModel;
    [SerializeField] LoadoutScreenController _loadoutScreenController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _loadoutViewModel.Init();
        _loadoutScreenController.Init();
    }

}
