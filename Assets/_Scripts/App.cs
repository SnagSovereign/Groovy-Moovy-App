using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class App : MonoBehaviour {

	// Create a static link to this class so that other
    // classes can access its public methods & variables
    public static App app;

	//unity linking variables
	[SerializeField] TMP_InputField searchInput;
	[SerializeField] GameObject scrollContent;
	[SerializeField] GameObject resultPanel;

	// class level variables
	string search;   //will hold the input from the search input field. 
	string source;
	JResults[] jSearchResults;
	JMediaDetails jMediaDetails;
	List<GameObject> resultsPanels = new List<GameObject>();

	void Start () 
	{
		app = this;

		// Add an onSubmit event listener to search Input Field
		searchInput.onSubmit.AddListener( delegate { OnSearchSubmit(); });

		//put the cursor on the input field ready to type
		searchInput.Select();
	}

	public void SendAPIRequest(string[] requestParameters, Type responseType)
	{
		// begin the API request
		source = "https://www.omdbapi.com/?apikey=4d928b0c&";

		// Complete the API request based on the expected response type:

		// if the request is a search expecting results
		if(responseType == typeof(JResults))
		{
			source += "s=" + requestParameters[0];
		}
		// if the request uses an imdbID to get details of one piece of media
		else if(responseType == typeof(JMediaDetails))
		{
			source += "i=" + requestParameters[0];
		}
		// if an invalid responseType was passed into the method
		else 
		{
			Debug.LogError("Unknown response type: " + responseType);
		}

		StartCoroutine(CheckAPIResponseFinished(responseType));
	}

	IEnumerator CheckAPIResponseFinished(Type responseType)
	{
		Debug.Log(source);

		//generate a web request
		WWW webRequest = new WWW(source);

		//wait for the response (yield method running)
		yield return webRequest;

		//check that nothing went wrong with data
		if (webRequest != null && webRequest.isDone)
		{
			// handle the response depending on the type:

			// if the response is search results
			if(responseType == typeof(JResults))
			{
				//deserialise JSON search data
				jSearchResults = JSearch.FromJson<JResults>(webRequest.text);

				// display the search results on the screen
				AddResultsPanel(jSearchResults);
			}
			else if(responseType == typeof(JMediaDetails))
			{
				//deserialise JSON details data
				jMediaDetails = JMediaDetails.CreateFromJSON(webRequest.text);

			}
		}
	}

	void AddResultsPanel(JResults[] searchResults)
	{
		//loop through the array creating a panel for each object
		for (int i = 0; i < searchResults.Length; i++)
		{
			//instantiate a new results prefab
			GameObject newRow = Instantiate(resultPanel, scrollContent.transform);

			//add the panel to the tracking list
			resultsPanels.Add(newRow);

			//put the details on the panel (fill data)
			ResultPanel panel = newRow.GetComponent<ResultPanel>();
			panel.SetID(searchResults[i].imdbID);
			panel.FillDetails(searchResults[i].Title, searchResults[i].Year);

			//call another web request for the image
			GetBookImage(panel, searchResults[i]);
		}
	}

	void GetBookImage(ResultPanel panel, JResults searchResult)
	{
		source = searchResult.Poster; 
		StartCoroutine(JDetailsImageCoroutine(panel));
	}

	IEnumerator JDetailsImageCoroutine(ResultPanel panel)
	{
		WWW imageReq = new WWW(source);
		yield return imageReq;

		//check it worked and place the image
		if (imageReq != null && imageReq.isDone && imageReq.error == null)
		{
			panel.posterImage.texture = imageReq.texture;
		}
	}

	void ClearScreenData()
	{
		foreach (GameObject panel in resultsPanels)
		{
			//destroy object
			DestroyObject(panel);
		}
	}

	public void OnSearchSubmit()
	{
		//get the input
		search = searchInput.text;
		//clear the old data
		ClearScreenData();
		// Send a search API request
		SendAPIRequest(new string[1] { search }, typeof(JResults));
	}
}
