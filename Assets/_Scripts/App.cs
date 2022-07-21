using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class App : MonoBehaviour {

	//unity linking variables
	[SerializeField] TMP_InputField searchInput;
	[SerializeField] GameObject scrollContent;
	[SerializeField] GameObject resultPanel;

	// class level variables
	string search;   //will hold the input from the search input field. 
	string source;
	JResults[] jSearch;
	List<GameObject> resultsPanels = new List<GameObject>();

	void Start () 
	{
		// Add an onSubmit event listener to search Input Field
		searchInput.onSubmit.AddListener( delegate { OnSearchSubmit(); });

		//put the cursor on the input field ready to type
		searchInput.Select();
	}

	void SendAPIRequest()
	{
		//create an API request
		source = "https://www.omdbapi.com/?apikey=4d928b0c&s=" + search;

		StartCoroutine(CheckAPIResponseFinished());
	}

	IEnumerator CheckAPIResponseFinished()
	{
		Debug.Log(source);
		//generate a web request
		WWW webRequest = new WWW(source);
		//wait for the response (yield method running)
		yield return webRequest;

		print(webRequest.text);

		Debug.Log("first yield finished");
		//check that nothing went wrong with data
		if (webRequest != null && webRequest.isDone)
		{
			//deserialise JSON search data
			jSearch = JSearch.FromJson<JResults>(webRequest.text);

			foreach(var result in jSearch)
			{
				print("title: " + result.Title + "\n" +
					  "year: " + result.Year + "\n" +
					  "imdbID: " + result.imdbID + "\n" +
					  "type: " + result.Type + "\n" +
					  "poster: " + result.Poster + "\n");
			}

			//put the data on the screen with a prefab
			AddResultsPanel(jSearch);
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
		SendAPIRequest();
	}
}
