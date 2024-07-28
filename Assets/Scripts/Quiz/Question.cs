using System.Collections.Generic;

[System.Serializable]
public class Question
{
    public string Description { get; set; }
    public string CorrectAnswer { get; set; }
    public List<string> Options { get; set; }

    public string Category { get; set; }
}
