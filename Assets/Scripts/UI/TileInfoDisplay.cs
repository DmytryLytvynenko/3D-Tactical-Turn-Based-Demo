using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileInfoDisplay : MonoBehaviour, IInfoShowable
{
    [SerializeField] private TileInfo tileInfo;
    [SerializeField] private Canvas infoCanvas;
    [SerializeField] private TextMeshProUGUI testTextBox;
    public void ShowInfo()
    {
        infoCanvas.gameObject.SetActive(true);
        testTextBox.text = "";
        testTextBox.text += "Biom: " + "\t" + tileInfo.Biom.ToString() + "\n";
        testTextBox.text += "VisibilityFine: " + "\t"+ tileInfo.VisibilityFine.ToString() + "\n";
        testTextBox.text += "SpeedMultiplier: " + "\t" + tileInfo.SpeedMultiplier.ToString() + "\n";
        testTextBox.text += "AttackRangeFine: " + "\t" + tileInfo.AttackRangeFine.ToString() + "\n";
    }
    public void HideInfo()
    {
        infoCanvas.gameObject.SetActive(false);
    }
}
