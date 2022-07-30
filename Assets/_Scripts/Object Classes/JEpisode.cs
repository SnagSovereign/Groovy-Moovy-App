using UnityEngine;

[System.Serializable]
public class JEpisode 
{
    //list of variables by title returned by the API
    public string Title;
    public string Episode;


    public static JEpisode CreateFromJSON(string jString)
    {
        //create a return a Jdetails object using jsonUtility
        return JsonUtility.FromJson<JEpisode>(jString);
    }

}

