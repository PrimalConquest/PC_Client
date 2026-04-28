using UnityEngine;
using UnityEngine.UI;

public class StatScreenController : MonoBehaviour
{
    [SerializeField] Text _rpText;

    public async void Init()
    {
        var (dto, err) = await StatService.GetStats();
        if (err != null || dto == null) return;
        _rpText.text = dto.RankPoints.ToString();
    }
}
