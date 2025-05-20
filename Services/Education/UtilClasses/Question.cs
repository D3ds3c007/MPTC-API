
namespace MPTC_API.Services.Education.UtilClasses;
public class Question
{
    public int Number { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
    public double Score { get; set; }
    public double Scale { get; set; }
    public int correcType { get; set; }
    public Dictionary<string, string> Items { get; set; }

    public Question(){
        this.Number = 0;
        this.Text = "";
        this.Description = "";
        this.Score = 0;
        this.Scale = 0;
        this.correcType = 0;
        this.Items = new Dictionary<string, string>();
    }

}
