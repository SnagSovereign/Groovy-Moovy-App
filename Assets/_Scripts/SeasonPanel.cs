using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeasonPanel : MonoBehaviour {

	private int seasonNumber;

	[SerializeField] TextMeshProUGUI seasonText;

	public void SetSeason(int season)
	{
		seasonNumber = season;
		seasonText.text = "Season " + seasonNumber;
	}

	public void SeasonPanelClicked()
	{
		App.app.PrepareAPIRequest(new string[2] { App.app.jMediaDetails.imdbID, seasonNumber.ToString() }, 
								  typeof(JEpisode));
		App.app.seasonText.text = "Season " + seasonNumber;
		//App.app.GenerateEpisodeList(seasonNumber);
		App.app.MenuGoTo(3);
	}

}
