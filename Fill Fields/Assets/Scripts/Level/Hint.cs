using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Hint : ScriptableObject {
    private MovementDirection[] directions;
    public bool isEnable;

    public void Initialize(XmlNode lvlNode, bool isEnable = false) {
        this.isEnable = isEnable;

        LoadHints();

        void LoadHints() {
            XmlNode hintNode = lvlNode.SelectSingleNode("hint");
            int hintMovementsAmount = hintNode.InnerText.ToCharArray().Length;

            directions = new MovementDirection[hintMovementsAmount];

            for(int i = 0;i < hintMovementsAmount;i++) {
                int.TryParse(hintNode.InnerText[i].ToString(), out int value);

                value--;
                directions[i] = (MovementDirection)value;
            }
        }
    }

    public MovementDirection GetDirection(int hintIndex) {
        if(directions.Length <= hintIndex) {
            return GetDirection(directions.Length - 1);
        }

        return directions[hintIndex];
    }
}
