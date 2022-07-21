﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPanel : MonoBehaviour {

	private string imdbID;

	public RawImage posterImage;
	[SerializeField] TextMeshProUGUI titleText;
	[SerializeField] TextMeshProUGUI yearText;

	public void SetID(string id)
	{
		imdbID = id;
	}


	public void FillDetails(string title, string year)
	{
		titleText.text = title;
		yearText.text = year;
	}
}
