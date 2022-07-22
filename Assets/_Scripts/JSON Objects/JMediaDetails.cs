using UnityEngine;

[System.Serializable]
public class JMediaDetails 
{
    //list of variables by title returned by the API
    public string Title;
    public string Year;
    public string Rated;
    public string Released;
    public string Runtime;
    public string Genre;
    public string Writer;
    public string Actors;
    public string Plot;
    public string Poster;
    public string imdbRating;
    public string imdbID;
    public string Type;

    public static JMediaDetails CreateFromJSON(string jString)
    {
        //create a return a Jdetails object using jsonUtility
        return JsonUtility.FromJson<JMediaDetails>(jString);
    }

}

