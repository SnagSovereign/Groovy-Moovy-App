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

	[SerializeField] Texture2D placeholderPoster;

	//unity linking variables
	[Header("Search Screen Objects")]
	[SerializeField] TMP_InputField searchInput;
	[SerializeField] GameObject searchScrollContent;
	[SerializeField] GameObject resultPanel;

	[Header("Media Details Objects")]
	[SerializeField] RawImage mediaDetailsPoster;
	[SerializeField] TextMeshProUGUI topBarTitleText;
	[SerializeField] TextMeshProUGUI mediaDetailsTitleText;
	[SerializeField] TextMeshProUGUI yearRatingRuntimeText;
	[SerializeField] GameObject genreLayoutGroup;
	[SerializeField] GameObject genreButton;
	[SerializeField] TextMeshProUGUI mediaDetailsPlotText;
	[SerializeField] SeasonsButton seasonsButton;
	[SerializeField] TextMeshProUGUI imdbRatingText;
	[SerializeField] TextMeshProUGUI creditsText;

	[Header("Seasons Screen Objects")]
	[SerializeField] GameObject seasonsScrollContent;
	[SerializeField] GameObject seasonPanel;

	[Header("Episodes Screen Objects")]
	[SerializeField] GameObject episodesScrollContent;
	public TextMeshProUGUI seasonText;
	[SerializeField] GameObject episodePanel;



	[Header("Canvases")]
	[SerializeField] GameObject topBar;
	[SerializeField] GameObject[] screens;

	// class level variables
	int currentScreenIndex = -1;
	string search;   //will hold the input from the search input field. 
	string source;
	JResults[] jSearchResults;
	[HideInInspector] public JMediaDetails jMediaDetails;
	JEpisode[] jEpisodes;

	List<GameObject> resultsPanels = new List<GameObject>();
	List<GameObject> seasonPanels = new List<GameObject>();
	List<GameObject> episodePanels = new List<GameObject>();


	void Start () 
	{
		app = this;

		MenuGoTo(0);

		// Add an onSubmit event listener to search Input Field
		searchInput.onSubmit.AddListener( delegate { OnSearchSubmit(); });

		//put the cursor on the input field ready to type
		searchInput.Select();
	}

	public void PrepareAPIRequest(string[] requestParameters, Type responseType)
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
		else if(responseType == typeof(JEpisode))
		{
			source += "i=" + requestParameters[0] + "&" + "season=" + requestParameters[1];
		}
		// if an invalid responseType was passed into the method
		else 
		{
			Debug.LogError("Unknown response type: " + responseType);
		}

		StartCoroutine(SendAPIRequest(responseType));
	}

	IEnumerator SendAPIRequest(Type responseType)
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
				// jSearchResults = ParseJson.ResultsFromJson<JResults>(webRequest.text);
				jSearchResults = ParseJson.FromJson<JResults>(webRequest.text);


				// display the search results on the search screen
				GenerateResultsList(jSearchResults);
			}
			// if the response is media details
			else if(responseType == typeof(JMediaDetails))
			{
				//deserialise JSON details data
				jMediaDetails = JMediaDetails.CreateFromJSON(webRequest.text);

				// display the media details on the media details screen
				DisplayMediaDetails(jMediaDetails);
			}
			else if (responseType == typeof(JEpisode))
			{
				jEpisodes = ParseJson.FromJson<JEpisode>(webRequest.text);
				GenerateEpisodeList(jEpisodes);
			}
		}
	}

	void GenerateResultsList(JResults[] searchResults)
	{
		//loop through the array creating a panel for each object
		for (int i = 0; i < searchResults.Length; i++)
		{
			//instantiate a new results prefab
			GameObject newRow = Instantiate(resultPanel, searchScrollContent.transform);

			//add the panel to the tracking list
			resultsPanels.Add(newRow);

			//put the details on the panel (fill data)
			ResultPanel panel = newRow.GetComponent<ResultPanel>();
			panel.SetID(searchResults[i].imdbID);
			panel.FillDetails(searchResults[i].Title, searchResults[i].Year);

			//call another web request for the image
			GetPosterImage(panel.posterImage, searchResults[i].Poster);
		}
	}

	void DisplayMediaDetails(JMediaDetails mediaDetails)
	{
		topBarTitleText.text = mediaDetailsTitleText.text = mediaDetails.Title;
		yearRatingRuntimeText.text = mediaDetails.Year + "   " + 
									 mediaDetails.Rated + "   " + 
									 mediaDetails.Runtime;
		mediaDetailsPlotText.text = mediaDetails.Plot;
		imdbRatingText.text = mediaDetails.imdbRating + " / 10";
		
		creditsText.text = "Actors:<color=#AEAEAEFF> " + mediaDetails.Actors + "<color=#FFFFFFFF>\n"
						 + "Directors:<color=#AEAEAEFF> " + mediaDetails.Director + "<color=#FFFFFFFF>\n"
						 + "Writers:<color=#AEAEAEFF> " + mediaDetails.Writer;

		string[] genres = mediaDetails.Genre.Split(new[] { ", " }, StringSplitOptions.None);

		if(mediaDetails.Type == "series")
		{
			seasonsButton.gameObject.SetActive(true);
			seasonsButton.seasonsText.text = mediaDetails.totalSeasons + " Seasons";
		}

		foreach(string genre in genres)
		{
			//instantiate a new genre button
			GameObject newGenre = Instantiate(genreButton, genreLayoutGroup.transform);
			newGenre.GetComponent<GenreButton>().genreText.text = genre;
		}

		GetPosterImage(mediaDetailsPoster, mediaDetails.Poster);
	}

	public void GenerateSeasonList()
	{
		for (int season = 1; season <= int.Parse(jMediaDetails.totalSeasons); season++)
		{
			// Instantiate season panels
			GameObject newRow = Instantiate(seasonPanel, seasonsScrollContent.transform);
			seasonPanels.Add(newRow);

			SeasonPanel panel = newRow.GetComponent<SeasonPanel>();
			panel.SetSeason(season);
		}

	}

	public void GenerateEpisodeList(JEpisode[] episodes)
	{
		foreach(JEpisode episode in episodes)
		{
			// Instantiate episode panels
			GameObject newRow = Instantiate(episodePanel, episodesScrollContent.transform);
			episodePanels.Add(newRow);

			EpisodePanel panel = newRow.GetComponent<EpisodePanel>();
			panel.SetEpisode(episode.Episode, episode.Title);
		}
	}

	void GetPosterImage(RawImage image, string imageURL)
	{
		source = imageURL; 
		StartCoroutine(FetchImage(image));
	}

	IEnumerator FetchImage(RawImage image)
	{
		WWW imageReq = new WWW(source);
		yield return imageReq;

		//check it worked and place the image
		if (imageReq != null && imageReq.isDone && imageReq.error == null)
		{
			image.texture = imageReq.texture;
		}
	}

	void ClearSearchScreen()
	{
		foreach (GameObject panel in resultsPanels)
		{
			//destroy object
			DestroyObject(panel);
		}
	}

	public void ClearMediaDetailsScreen()
	{
		topBarTitleText.text = "";
		mediaDetailsTitleText.text = "";
		yearRatingRuntimeText.text = "";
		mediaDetailsPlotText.text = "";
		imdbRatingText.text = "";
		mediaDetailsPoster.texture = null;
		creditsText.text = "";
		mediaDetailsPoster.texture = placeholderPoster;
		seasonsButton.seasonsText.text = "";
		seasonsButton.gameObject.SetActive(false);

		// destory all of the genres
		 foreach (Transform genreButton in genreLayoutGroup.transform) 
		 {
     		GameObject.Destroy(genreButton.gameObject);
		 }
	}

	void ClearSeasonsScreen()
	{
		foreach (GameObject panel in seasonPanels)
		{
			//destroy object
			DestroyObject(panel);
		}
	}

	void ClearEpisodesScreen()
	{
		seasonText.text = "";
		foreach (GameObject panel in episodePanels)
		{
			//destroy object
			DestroyObject(panel);
		}
	}

	public void MenuGoTo(int screenIndex)
	{
		// changes the UI canvas
		// The corresponding index for each screen are as follows:
        // 		0 = Search
        // 		1 = Media Details

		// Exit this method if the currentScreenIndex is already the screenIndex to switch to
        if (currentScreenIndex == screenIndex) return;

		// Disable all of the screens
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }

		// Disable the top bar if the new screenIndex is 0 (search screen)
		// Enable the top bar for any other screen
		topBar.SetActive(screenIndex == 0 ? false : true);

		// Enable the new screen that is being switched to
        screens[screenIndex].SetActive(true);

		// Update the currentScreenIndex to the new one
        currentScreenIndex = screenIndex;
	}

	public void BackButtonClicked()
	{
		switch (currentScreenIndex - 1)
		{
			// going back to search
			case 0: 
				ClearMediaDetailsScreen();
				break;
			// going back to media details screen
			case 1:
				ClearSeasonsScreen();
				break;
			// going back to seasons screen
			case 2:
				ClearEpisodesScreen();
				break;
			default:
				Debug.LogWarning("Cannot go back to that screen");
				break;

		}

		MenuGoTo(currentScreenIndex - 1);
	}

	public void OnSearchSubmit()
	{
		//get the input
		search = searchInput.text;
		//clear the old data
		ClearSearchScreen();
		// Send a search API request
		PrepareAPIRequest(new string[1] { search }, typeof(JResults));
	}
}
