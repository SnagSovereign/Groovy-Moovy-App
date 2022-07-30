using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeasonsButton : MonoBehaviour {

	public TextMeshProUGUI seasonsText;

	public void SeasonsButtonClicked() 
	{
		App.app.GenerateSeasonList();
		App.app.MenuGoTo(2);
	}
}
