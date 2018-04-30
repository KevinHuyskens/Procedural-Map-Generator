using UnityEngine;
using System.Collections;

public class ColorPickerTester : MonoBehaviour
{
    public MapGenerator colorMap;
    public ColorPicker picker;
    public ColorPicker picker2;
    public ColorPicker picker3;

    void Start()
    {
        picker.onValueChanged.AddListener(color =>
        {
            colorMap.maps[colorMap.mapIndex].backGroundColor = color;
        });
        picker2.onValueChanged.AddListener(color =>
        {
            colorMap.maps[colorMap.mapIndex].foreGroundColor = color;
        });
        picker3.onValueChanged.AddListener(color =>
        {
            colorMap.maps[colorMap.mapIndex].tileColor = color;
        });
        colorMap.maps[colorMap.mapIndex].backGroundColor = picker.CurrentColor;
        colorMap.maps[colorMap.mapIndex].foreGroundColor = picker2.CurrentColor;
        colorMap.maps[colorMap.mapIndex].tileColor = picker3.CurrentColor;

     }
}
