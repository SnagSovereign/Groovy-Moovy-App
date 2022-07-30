using UnityEngine;

public static class ParseJson {

    public static JResults[] ResultsFromJson<JResults>(string json)
    {
            //create each array object from the string supplied
            ResultsWrapper<JResults> wrapper = JsonUtility.FromJson<ResultsWrapper<JResults>>(json);
            //return the array of Jresults objects
            return wrapper.Search;
    }

    public static JEpisode[] EpisodesFromJson<JEpisode>(string json)
    {
            //create each array object from the string supplied
            EpisodesWrapper<JEpisode> wrapper = JsonUtility.FromJson<EpisodesWrapper<JEpisode>>(json);
            //return the array of Jresults objects
            return wrapper.Episodes;
    }

    

    //serializable object to hold our array of results objects
	[System.Serializable]
    private class ResultsWrapper<JResults>
    {
        //array of json objects formated like JResults. 
        public JResults[] Search;
    }

    //serializable object to hold our array of episodes objects
    [System.Serializable]
    private class EpisodesWrapper<JEpisode>
    {
        //array of json objects formated like JEpisodes. 
        public JEpisode[] Episodes;
    }
}
