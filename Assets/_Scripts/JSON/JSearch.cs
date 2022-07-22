using UnityEngine;


public static class JSearch {

    public static JResults[] FromJson<JResults>(string json)
    {
        //create each array object from the string supplied
        Wrapper<JResults> wrapper = JsonUtility.FromJson<Wrapper<JResults>>(json);
        //return the array of Jresults objects
        return wrapper.Search;
    }

    //serializable object to hold our array of json objects
	[System.Serializable]
    private class Wrapper<JResults>
    {
        //array of json objects formated like JResults. 
        public JResults[] Search;
    }
}
