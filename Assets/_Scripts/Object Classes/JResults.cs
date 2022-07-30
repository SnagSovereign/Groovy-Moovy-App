using UnityEngine;

[System.Serializable]
public class JResults 
{
    //list of variables by title returned by the API
    public string Title;
    public string Year;
    public string imdbID;
    public string Type;
    public string Poster;


    public static JResults CreateFromJSON(string jString)
    {
        //create a return a Jdetails object using jsonUtility
        return JsonUtility.FromJson<JResults>(jString);
    }

}

