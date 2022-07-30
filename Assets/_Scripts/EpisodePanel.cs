using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EpisodePanel : MonoBehaviour {

	[SerializeField] TextMeshProUGUI episodeText;
	
	public void SetEpisode(string episode, string title)
	{
		episodeText.text = episode + ". " + title;
	}
}
