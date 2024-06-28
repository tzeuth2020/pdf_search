using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace pdf_search.Data;

public class Submissions {
    public required string batch {get; set;}
    public required string name {get; set;}
    public string? q1 {get; set;} 
    public string? q2 {get; set;} 
    public string? q3 {get; set;} 
    public string? q4 {get; set;} 
    public string? q5 {get; set;} 
    public string? q6 {get; set;} 
    public string? q7 {get; set;} 
    public string? q8 {get; set;} 
    public string? q9 {get; set;} 
    public string? q10 {get; set;} 
    public string? wholeDoc {get; set;}
    public System.DateOnly? date {get; set;}

    public void setQuestion (int i, string value) {
        string propertyName = "q" + i;
        PropertyInfo? propertyInfo = typeof(Submissions).GetProperty(propertyName);
        if (propertyInfo != null && propertyInfo.CanWrite) {
            propertyInfo.SetValue(this, value);
        } else {
            throw new ArgumentException("invalid question number");
        }
    }

}

