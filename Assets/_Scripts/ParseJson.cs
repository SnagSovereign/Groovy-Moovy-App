using UnityEngine;

public static class ParseJson {

    public static T[] FromJson<T>(string json)
    {
        //create each array object from the string supplied
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        //return the array of objects
        if(typeof(T) == typeof(JResult)) { return wrapper.Search; }
        else if(typeof(T) == typeof(JEpisode)) { return wrapper.Episodes; }
        else { return null; }
    }

    //serializable object to hold our array of objects
	[System.Serializable]
    private class Wrapper<T>
    {
        public T[] Search, Episodes;
    }
}