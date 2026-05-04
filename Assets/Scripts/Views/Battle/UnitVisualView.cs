using UnityEngine;
using UnityEngine.UI;

// Placed in the visual overlay layer of BoardView.
// Sized to span the unit's full shape footprint.
// Does NOT handle input — the input-catcher tiles in the grid do that.
[RequireComponent(typeof(Image))]
public class UnitVisualView : MonoBehaviour
{
    [SerializeField] Image _bg;
    [SerializeField] Text  _label;

    public void Set(Color color, string labelText)
    {
        _bg.color   = color;
        _label.text = labelText;
    }

    void Reset()
    {
        _bg = GetComponent<Image>();
    }
}
